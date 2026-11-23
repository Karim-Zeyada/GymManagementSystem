using GymManagementSystem.DAL.Enums;

namespace GymManagementSystem.BLL.ViewModels.Members;

public class MemberViewModel
{
    public int Id { get; set; }
    public string? Photo { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public string Phone { get; set; } = string.Empty;
}
