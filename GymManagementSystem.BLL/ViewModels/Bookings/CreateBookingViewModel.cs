using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.BLL.ViewModels.Bookings;

public class CreateBookingViewModel
{
    [Required(ErrorMessage = "Please select a workout session.")]
    [Display(Name = "Workout Session")]
    public int SessionId { get; set; }

    [Required(ErrorMessage = "Please select a member.")]
    [Display(Name = "Member")]
    public int MemberId { get; set; }

    public IEnumerable<SelectListItem> Sessions { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Members { get; set; } = new List<SelectListItem>();
}
