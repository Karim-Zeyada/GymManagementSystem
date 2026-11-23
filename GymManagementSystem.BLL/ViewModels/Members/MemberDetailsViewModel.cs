using GymManagementSystem.DAL.Enums;

namespace GymManagementSystem.BLL.ViewModels.Members;

public class MemberDetailsViewModel
{
    public int Id { get; set; }
    public string? Photo { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string Address { get; set; } = string.Empty;

    public string? PlanName { get; set; }
    public DateTime? MembershipStartDate { get; set; }
    public DateTime? MembershipEndDate { get; set; }

    public bool HasHealthRecord { get; set; }
    public int? HealthRecordId { get; set; }
}
