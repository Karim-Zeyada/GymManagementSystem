using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.BLL.ViewModels.Memberships;

public class CreateMembershipViewModel
{
    [Required(ErrorMessage = "Please select a member.")]
    [Display(Name = "Member")]
    public int MemberId { get; set; }

    [Required(ErrorMessage = "Please select a plan.")]
    [Display(Name = "Gym Plan")]
    public int PlanId { get; set; }

    [Required(ErrorMessage = "Start date is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Start Date")]
    public DateTime StartDate { get; set; } = DateTime.Today;

    public IEnumerable<SelectListItem> Members { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Plans { get; set; } = new List<SelectListItem>();
}
