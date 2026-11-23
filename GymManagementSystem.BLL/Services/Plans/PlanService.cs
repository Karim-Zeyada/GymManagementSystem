using GymManagementSystem.BLL.ViewModels.Plans;
using GymManagementSystem.DAL.Repositories.Plans;

namespace GymManagementSystem.BLL.Services.Plans;

public class PlanService : IPlanService
{
    private readonly IPlanRepository _planRepository;

    public PlanService(IPlanRepository planRepository)
    {
        _planRepository = planRepository;
    }

    public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync()
    {
        var plans = await _planRepository.GetAllAsync();
        return plans.Select(p => new PlanViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            DurationDays = p.DurationDays,
            Description = p.Description,
            IsActive = p.IsActive
        }).ToList();
    }

    public async Task<PlanDetailsViewModel?> GetPlanDetailsAsync(int id)
    {
        var plan = await _planRepository.GetByIdAsync(id);
        if (plan == null) return null;

        return new PlanDetailsViewModel
        {
            Id = plan.Id,
            Name = plan.Name,
            Price = plan.Price,
            DurationDays = plan.DurationDays,
            Description = plan.Description,
            IsActive = plan.IsActive
        };
    }

    public async Task<EditPlanViewModel?> GetPlanForEditAsync(int id)
    {
        var plan = await _planRepository.GetByIdAsync(id);
        if (plan == null) return null;

        return new EditPlanViewModel
        {
            Id = plan.Id,
            Name = plan.Name,
            Price = plan.Price,
            DurationDays = plan.DurationDays,
            Description = plan.Description
        };
    }

    public async Task<(bool Success, string? ErrorMessage)> UpdatePlanAsync(EditPlanViewModel model)
    {
        var plan = await _planRepository.GetByIdAsync(model.Id);
        if (plan == null)
        {
            return (false, "Plan not found.");
        }

        // Business rule: Cannot update a plan with active memberships
        if (await _planRepository.HasActiveMembershipsAsync(model.Id))
        {
            return (false, "Cannot update a plan with active memberships to preserve existing contracts.");
        }

        // Name cannot be modified (preserves historical references)
        plan.Price = model.Price;
        plan.DurationDays = model.DurationDays;
        plan.Description = model.Description.Trim();
        plan.UpdatedAt = DateTime.UtcNow;

        _planRepository.Update(plan);
        await _planRepository.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool Success, string? ErrorMessage)> ToggleStatusAsync(int id)
    {
        var plan = await _planRepository.GetByIdAsync(id);
        if (plan == null)
        {
            return (false, "Plan not found.");
        }

        // Business rule: Cannot deactivate a plan with active memberships
        if (plan.IsActive && await _planRepository.HasActiveMembershipsAsync(id))
        {
            return (false, "Cannot deactivate a plan with active memberships.");
        }

        plan.IsActive = !plan.IsActive;
        plan.UpdatedAt = DateTime.UtcNow;

        _planRepository.Update(plan);
        await _planRepository.SaveChangesAsync();

        return (true, null);
    }
}
