using AutoMapper;
using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.Memberships;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.UnitOfWork;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace GymManagementSystem.BLL.Services.Memberships;

public class MembershipService : IMembershipService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<MembershipService> _logger;

    public MembershipService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<MembershipService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<MembershipViewModel>> GetAllMembershipsAsync(string? filter = null)
    {
        var memberships = await _unitOfWork.Memberships.GetAllWithDetailsAsync();
        var viewModels = _mapper.Map<IEnumerable<MembershipViewModel>>(memberships);

        var now = DateTime.UtcNow;
        return filter?.ToLowerInvariant() switch
        {
            "active" => viewModels.Where(m => m.StartDate <= now && m.EndDate >= now),
            "expired" => viewModels.Where(m => m.EndDate < now),
            "upcoming" => viewModels.Where(m => m.StartDate > now),
            _ => viewModels
        };
    }

    public async Task<MembershipDetailsViewModel?> GetMembershipDetailsAsync(int id)
    {
        var membership = await _unitOfWork.Memberships.GetWithDetailsAsync(id);
        if (membership == null) return null;

        return _mapper.Map<MembershipDetailsViewModel>(membership);
    }

    public async Task<CreateMembershipViewModel> GetCreateMembershipViewModelAsync(int? memberId = null)
    {
        var members = await _unitOfWork.Members.GetAllAsync();
        var plans = (await _unitOfWork.Plans.GetAllAsync()).Where(p => p.IsActive);

        return new CreateMembershipViewModel
        {
            MemberId = memberId ?? 0,
            StartDate = DateTime.Today,
            Members = members.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = $"{m.Name} ({m.Phone})",
                Selected = memberId.HasValue && m.Id == memberId.Value
            }),
            Plans = plans.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Name} — {p.DurationDays} Days — {p.Price:C0}"
            })
        };
    }

    public async Task<Result> CreateMembershipAsync(CreateMembershipViewModel model)
    {
        var member = await _unitOfWork.Members.GetByIdAsync(model.MemberId);
        if (member == null)
        {
            _logger.LogWarning("Failed to assign membership: Member with ID {MemberId} not found.", model.MemberId);
            return Result.Fail("Member not found.");
        }

        var plan = await _unitOfWork.Plans.GetByIdAsync(model.PlanId);
        if (plan == null || !plan.IsActive)
        {
            _logger.LogWarning("Failed to assign membership: Plan with ID {PlanId} not found or inactive.", model.PlanId);
            return Result.Fail("Selected plan not found or is currently inactive.");
        }

        // Prevent overlapping active memberships
        var hasActive = await _unitOfWork.Memberships.HasActiveMembershipAsync(model.MemberId, model.StartDate);
        if (hasActive)
        {
            _logger.LogWarning("Member {MemberId} already has an active membership covering start date {StartDate}.", model.MemberId, model.StartDate);
            return Result.Fail("Member already has an active membership covering this start date. Please choose a date after the current membership expires.");
        }

        var membership = new Membership
        {
            MemberId = model.MemberId,
            PlanId = model.PlanId,
            StartDate = model.StartDate,
            EndDate = model.StartDate.AddDays(plan.DurationDays),
            CreatedAt = DateTime.UtcNow
        };

        _unitOfWork.Memberships.Add(membership);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Successfully assigned membership #{MembershipId} to Member {MemberId} under Plan {PlanId}.", membership.Id, model.MemberId, model.PlanId);
        return Result.Ok();
    }

    public async Task<Result> CancelMembershipAsync(int id)
    {
        var membership = await _unitOfWork.Memberships.GetByIdAsync(id);
        if (membership == null)
        {
            return Result.Fail("Membership not found.");
        }

        _unitOfWork.Memberships.Delete(membership);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Cancelled membership #{MembershipId}.", id);
        return Result.Ok();
    }
}
