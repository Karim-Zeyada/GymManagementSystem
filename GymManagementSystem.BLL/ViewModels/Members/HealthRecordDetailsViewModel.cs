namespace GymManagementSystem.BLL.ViewModels.Members;

public class HealthRecordDetailsViewModel
{
    public int MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public decimal Height { get; set; }
    public decimal Weight { get; set; }
    public string BloodType { get; set; } = string.Empty;
    public string? Note { get; set; }
}
