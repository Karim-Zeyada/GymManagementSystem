namespace GymManagementSystem.BLL.ViewModels.Bookings;

public class BookingViewModel
{
    public int Id { get; set; }
    public int MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public string MemberEmail { get; set; } = string.Empty;
    public string MemberPhone { get; set; } = string.Empty;

    public int SessionId { get; set; }
    public string SessionCategory { get; set; } = string.Empty;
    public string TrainerName { get; set; } = string.Empty;
    public DateTime SessionStartDate { get; set; }
    public DateTime SessionEndDate { get; set; }

    public DateTime BookingDate { get; set; }
    public bool IsAttended { get; set; }

    public string TimeString => $"{SessionStartDate:hh:mm tt} – {SessionEndDate:hh:mm tt}";
    public string DateString => SessionStartDate.ToString("MMM dd, yyyy");
}
