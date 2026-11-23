namespace GymManagementSystem.BLL.ViewModels.Memberships;

public class MembershipViewModel
{
    public int Id { get; set; }
    public int MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public string MemberEmail { get; set; } = string.Empty;
    public string MemberPhone { get; set; } = string.Empty;

    public int PlanId { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public decimal PlanPrice { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public string Status
    {
        get
        {
            var now = DateTime.UtcNow;
            if (StartDate > now) return "Upcoming";
            if (EndDate < now) return "Expired";
            return "Active";
        }
    }

    public string StatusBadgeClass => Status switch
    {
        "Active" => "bg-success text-white",
        "Upcoming" => "bg-info text-white",
        "Expired" => "bg-secondary text-white",
        _ => "bg-light text-dark"
    };
}
