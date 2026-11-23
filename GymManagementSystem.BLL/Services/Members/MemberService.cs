using AutoMapper;
using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.Services.Attachments;
using GymManagementSystem.BLL.ViewModels.Members;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Models.ValueObjects;
using GymManagementSystem.DAL.Repositories.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace GymManagementSystem.BLL.Services.Members;

public class MemberService : IMemberService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IAttachmentService _attachmentService;
    private readonly ILogger<MemberService> _logger;

    public MemberService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IAttachmentService attachmentService,
        ILogger<MemberService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _attachmentService = attachmentService;
        _logger = logger;
    }

    public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync()
    {
        var members = await _unitOfWork.Members.GetAllAsync();
        return _mapper.Map<IEnumerable<MemberViewModel>>(members);
    }

    public async Task<MemberDetailsViewModel?> GetMemberDetailsAsync(int id)
    {
        var member = await _unitOfWork.Members.GetWithDetailsAsync(id);
        if (member == null) return null;

        var activeMembership = member.Memberships
            .OrderByDescending(ms => ms.EndDate)
            .FirstOrDefault();

        var addressString = member.Address != null
            ? $"{member.Address.BuildingNumber} · {member.Address.Street} · {member.Address.City}"
            : string.Empty;

        return new MemberDetailsViewModel
        {
            Id = member.Id,
            Photo = member.Photo,
            Name = member.Name,
            Email = member.Email,
            Phone = member.Phone,
            Gender = member.Gender,
            DateOfBirth = member.DateOfBirth,
            Address = addressString,
            PlanName = activeMembership?.Plan?.Name,
            MembershipStartDate = activeMembership?.StartDate,
            MembershipEndDate = activeMembership?.EndDate,
            HasHealthRecord = member.HealthRecord != null,
            HealthRecordId = member.HealthRecord?.Id
        };
    }

    public async Task<HealthRecordDetailsViewModel?> GetHealthRecordDetailsAsync(int memberId)
    {
        var member = await _unitOfWork.Members.GetWithHealthRecordAsync(memberId);
        if (member == null || member.HealthRecord == null) return null;

        var hr = member.HealthRecord;
        return new HealthRecordDetailsViewModel
        {
            MemberId = member.Id,
            MemberName = member.Name,
            Height = hr.Height,
            Weight = hr.Weight,
            BloodType = hr.BloodType.ToString(),
            Note = hr.MedicalConditions
        };
    }

    public async Task<EditMemberViewModel?> GetMemberForEditAsync(int id)
    {
        var member = await _unitOfWork.Members.GetByIdAsync(id);
        if (member == null) return null;

        return _mapper.Map<EditMemberViewModel>(member);
    }

    public async Task<DeleteMemberViewModel?> GetMemberForDeleteAsync(int id)
    {
        var member = await _unitOfWork.Members.GetByIdAsync(id);
        if (member == null) return null;

        return new DeleteMemberViewModel
        {
            Id = member.Id,
            Name = member.Name,
            Email = member.Email,
            Phone = member.Phone
        };
    }

    public async Task<Result> CreateMemberAsync(CreateMemberViewModel model)
    {
        // 1. Email uniqueness check
        if (await _unitOfWork.Members.IsEmailExistsAsync(model.Email))
        {
            return Result.Fail("This email address is already in use by another user.");
        }

        // 2. Phone uniqueness check
        if (await _unitOfWork.Members.IsPhoneExistsAsync(model.Phone))
        {
            return Result.Fail("This phone number is already registered.");
        }

        // 3. Minimum age validation (16 years)
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - model.DateOfBirth.Year;
        if (model.DateOfBirth > today.AddYears(-age)) age--;

        if (age < 16)
        {
            return Result.Fail("Member must be at least 16 years old to register.");
        }

        // 4. Map Entity via AutoMapper
        var member = _mapper.Map<Member>(model);

        // 5. Handle profile photo attachment
        if (model.Photo != null)
        {
            var uploadResult = await _attachmentService.SaveAsync(model.Photo, "members");
            if (!uploadResult.IsSuccess)
            {
                return Result.Fail(uploadResult.Error ?? "Failed to save profile photo.");
            }
            member.Photo = uploadResult.Value;
        }

        _unitOfWork.Members.Add(member);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Member registered successfully with ID {MemberId}.", member.Id);
        return Result.Ok();
    }

    public async Task<Result> UpdateMemberAsync(EditMemberViewModel model)
    {
        var member = await _unitOfWork.Members.GetByIdAsync(model.Id);
        if (member == null)
        {
            return Result.Fail("Member not found.", "NotFound");
        }

        // Email uniqueness check excluding current member
        if (await _unitOfWork.Members.IsEmailExistsAsync(model.Email, model.Id))
        {
            return Result.Fail("This email address is already in use by another user.");
        }

        // Phone uniqueness check excluding current member
        if (await _unitOfWork.Members.IsPhoneExistsAsync(model.Phone, model.Id))
        {
            return Result.Fail("This phone number is already in use by another user.");
        }

        // Handle profile photo update
        if (model.PhotoFile != null)
        {
            var uploadResult = await _attachmentService.SaveAsync(model.PhotoFile, "members");
            if (!uploadResult.IsSuccess)
            {
                return Result.Fail(uploadResult.Error ?? "Failed to save profile photo.");
            }

            if (!string.IsNullOrWhiteSpace(member.Photo))
            {
                _attachmentService.Delete(member.Photo, "members");
            }
            member.Photo = uploadResult.Value;
        }

        // Update fields
        member.Name = model.Name.Trim();
        member.Email = model.Email.Trim();
        member.Phone = model.Phone.Trim();
        member.UpdatedAt = DateTime.UtcNow;

        if (member.Address == null)
        {
            member.Address = new Address();
        }
        member.Address.BuildingNumber = model.BuildingNumber;
        member.Address.Street = model.Street.Trim();
        member.Address.City = model.City.Trim();

        _unitOfWork.Members.Update(member);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Member ID {MemberId} updated successfully.", member.Id);
        return Result.Ok();
    }

    public async Task<Result> DeleteMemberAsync(int id)
    {
        var member = await _unitOfWork.Members.GetByIdAsync(id);
        if (member == null)
        {
            return Result.Fail("Member not found.", "NotFound");
        }

        // Check active bookings
        if (await _unitOfWork.Members.HasActiveBookingsAsync(id))
        {
            return Result.Fail("Cannot delete a member with active or ongoing session bookings.");
        }

        var healthRecord = await _unitOfWork.HealthRecords.GetByIdAsync(id);
        if (healthRecord != null)
        {
            _unitOfWork.HealthRecords.Delete(healthRecord);
        }

        if (!string.IsNullOrWhiteSpace(member.Photo))
        {
            _attachmentService.Delete(member.Photo, "members");
        }

        _unitOfWork.Members.Delete(member);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Member ID {MemberId} deleted successfully.", id);
        return Result.Ok();
    }
}
