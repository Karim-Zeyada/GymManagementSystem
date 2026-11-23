namespace GymManagementSystem.BLL.ViewModels.Plans;

public class PlanDetailsViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int DurationDays { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
