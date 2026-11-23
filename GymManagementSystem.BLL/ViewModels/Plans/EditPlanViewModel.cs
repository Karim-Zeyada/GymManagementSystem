using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.BLL.ViewModels.Plans;

public class EditPlanViewModel
{
    public int Id { get; set; }

    // Locked / Display Only
    public string Name { get; set; } = string.Empty;

    // Editable fields
    [Required(ErrorMessage = "Price is required.")]
    [Range(1, 100000, ErrorMessage = "Price must be greater than 0.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Duration in days is required.")]
    [Range(1, 365, ErrorMessage = "Duration must be between 1 and 365 days.")]
    [Display(Name = "Duration (Days)")]
    public int DurationDays { get; set; }

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters.")]
    public string Description { get; set; } = string.Empty;
}
