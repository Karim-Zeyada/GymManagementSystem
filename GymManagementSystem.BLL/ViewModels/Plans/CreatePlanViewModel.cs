using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.BLL.ViewModels.Plans;

public class CreatePlanViewModel
{
    [Required(ErrorMessage = "Plan name is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Plan name must be between 3 and 50 characters.")]
    [Display(Name = "Plan Name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(250, MinimumLength = 5, ErrorMessage = "Description must be between 5 and 250 characters.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Duration in days is required.")]
    [Range(1, 365, ErrorMessage = "Duration must be between 1 and 365 days.")]
    [Display(Name = "Duration (Days)")]
    public int DurationDays { get; set; } = 30;

    [Required(ErrorMessage = "Price is required.")]
    [Range(1, 100000, ErrorMessage = "Price must be greater than 0.")]
    [DataType(DataType.Currency)]
    public decimal Price { get; set; }
}
