using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementSystem.BLL.ViewModels.Sessions;

public class EditSessionViewModel : IValidatableObject
{
    public int Id { get; set; }

    [Display(Name = "Category")]
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;

    [Display(Name = "Capacity")]
    public int Capacity { get; set; }

    [Required(ErrorMessage = "Trainer is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a trainer.")]
    [Display(Name = "Trainer")]
    public int TrainerId { get; set; }

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(500, MinimumLength = 3, ErrorMessage = "Description must be between 3 and 500 characters.")]
    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Start Date & Time is required.")]
    [Display(Name = "Start Date & Time")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "End Date & Time is required.")]
    [Display(Name = "End Date & Time")]
    public DateTime EndDate { get; set; }

    public IEnumerable<SelectListItem>? Trainers { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartDate <= DateTime.Now)
        {
            yield return new ValidationResult("Session start time must be in the future.", new[] { nameof(StartDate) });
        }

        if (EndDate <= StartDate)
        {
            yield return new ValidationResult("Session end time must be after the start time.", new[] { nameof(EndDate) });
        }
    }
}
