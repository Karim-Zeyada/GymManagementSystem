namespace GymManagementSystem.BLL.ViewModels.Sessions;

public class SessionDetailsViewModel
{
    public int Id { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TrainerName { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int BookedSlots { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public string TimeString => $"{StartDate:hh:mm tt} – {EndDate:hh:mm tt}";

    public string DurationString
    {
        get
        {
            var span = EndDate - StartDate;
            int hours = (int)span.TotalHours;
            int minutes = span.Minutes;
            if (hours > 0 && minutes > 0)
                return $"{hours} Hours {minutes} Minutes";
            if (hours > 0)
                return $"{hours} Hours";
            return $"{minutes} Minutes";
        }
    }

    public string CapacityString => $"{BookedSlots}/{Capacity}";

    public SessionStatus Status
    {
        get
        {
            var now = DateTime.Now;
            if (StartDate > now) return SessionStatus.Upcoming;
            if (EndDate < now) return SessionStatus.Completed;
            return SessionStatus.Ongoing;
        }
    }

    public string HeaderClass => Status switch
    {
        SessionStatus.Upcoming => "bg-primary text-white",
        SessionStatus.Ongoing => "bg-success text-white",
        SessionStatus.Completed => "bg-secondary text-white",
        _ => "bg-secondary text-white"
    };

    public string StatusBadgeClass => Status switch
    {
        SessionStatus.Upcoming => "bg-primary text-white",
        SessionStatus.Ongoing => "bg-success text-white",
        SessionStatus.Completed => "bg-secondary text-white",
        _ => "bg-secondary text-white"
    };
}
