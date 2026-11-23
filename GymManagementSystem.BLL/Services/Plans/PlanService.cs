using AutoMapper;
using GymManagementSystem.BLL.ViewModels.Plans;
using GymManagementSystem.DAL.Repositories.UnitOfWork;

namespace GymManagementSystem.BLL.Services.Plans;

public class PlanService : IPlanService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PlanService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync()
    {
        var plans = await _unitOfWork.Plans.GetAllAsync();
        return _mapper.Map<IEnumerable<PlanViewModel>>(plans);
    }

    public async Task<PlanDetailsViewModel?> GetPlanDetailsAsync(int id)
    {
        var plan = await _unitOfWork.Plans.GetByIdAsync(id);
        if (plan == null) return null;

        return _mapper.Map<PlanDetailsViewModel>(plan);
    }

    public async Task<EditPlanViewModel?> GetPlanForEditAsync(int id)
    {
        var plan = await _unitOfWork.Plans.GetByIdAsync(id);
        if (plan == null) return null;

        return _mapper.Map<EditPlanViewModel>(plan);
    }

    public async Task<(bool Success, string? ErrorMessage)> UpdatePlanAsync(EditPlanViewModel model)
    {
        var plan = await _unitOfWork.Plans.GetByIdAsync(model.Id);
        if (plan == null)
        {
            return (false, "Plan not found.");
        }

        // Business rule: Cannot update a plan with active memberships
        if (await _unitOfWork.Plans.HasActiveMembershipsAsync(model.Id))
        {
            return (false, "Cannot update a plan with active memberships to preserve existing contracts.");
        }

        // Name cannot be modified (preserves historical references)
        plan.Price = model.Price;
        plan.DurationDays = model.DurationDays;
        plan.Description = model.Description.Trim();
        plan.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Plans.Update(plan);
        await _unitOfWork.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool Success, string? ErrorMessage)> ToggleStatusAsync(int id)
    {
        var plan = await _unitOfWork.Plans.GetByIdAsync(id);
        if (plan == null)
        {
            return (false, "Plan not found.");
        }

        // Business rule: Cannot deactivate a plan with active memberships
        if (plan.IsActive && await _unitOfWork.Plans.HasActiveMembershipsAsync(id))
        {
            return (false, "Cannot deactivate a plan with active memberships.");
        }

        plan.IsActive = !plan.IsActive;
        plan.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Plans.Update(plan);
        await _unitOfWork.SaveChangesAsync();

        return (true, null);
    }
}
