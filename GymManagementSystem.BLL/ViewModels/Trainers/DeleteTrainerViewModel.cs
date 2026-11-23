using GymManagementSystem.DAL.Enums;

namespace GymManagementSystem.BLL.ViewModels.Trainers;

public class DeleteTrainerViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Speciality Speciality { get; set; }
}
