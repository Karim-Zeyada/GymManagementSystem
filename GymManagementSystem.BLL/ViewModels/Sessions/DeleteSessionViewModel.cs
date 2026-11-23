namespace GymManagementSystem.BLL.ViewModels.Sessions;

public class DeleteSessionViewModel
{
    public int Id { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string TrainerName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int BookedSlots { get; set; }

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
}
