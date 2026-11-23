using GymManagementSystem.DAL.Enums;

namespace GymManagementSystem.BLL.ViewModels.Trainers;

public class TrainerDetailsViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Speciality Speciality { get; set; }
    public string SpecializationLabel => $"{Speciality} Trainer";
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string Address { get; set; } = string.Empty;
}
