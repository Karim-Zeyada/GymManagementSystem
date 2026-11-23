using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementSystem.BLL.ViewModels.Sessions;

public class CreateSessionViewModel
{
    [Required(ErrorMessage = "Category is required.")]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Trainer is required.")]
    [Display(Name = "Trainer")]
    public int TrainerId { get; set; }

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(500, MinimumLength = 3, ErrorMessage = "Description must be between 3 and 500 characters.")]
    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Capacity is required.")]
    [Range(1, 500, ErrorMessage = "Capacity must be at least 1.")]
    [Display(Name = "Capacity")]
    public int Capacity { get; set; } = 10;

    [Required(ErrorMessage = "Start Date & Time is required.")]
    [Display(Name = "Start Date & Time")]
    public DateTime StartDate { get; set; } = DateTime.Now.AddHours(2);

    [Required(ErrorMessage = "End Date & Time is required.")]
    [Display(Name = "End Date & Time")]
    public DateTime EndDate { get; set; } = DateTime.Now.AddHours(3);

    public IEnumerable<SelectListItem>? Categories { get; set; }
    public IEnumerable<SelectListItem>? Trainers { get; set; }
}
