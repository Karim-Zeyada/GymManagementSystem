using GymManagementSystem.BLL.ViewModels.Trainers;

namespace GymManagementSystem.BLL.Services.Trainers;

public interface ITrainerService
{
    Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync();
    Task<TrainerDetailsViewModel?> GetTrainerDetailsAsync(int id);
    Task<EditTrainerViewModel?> GetTrainerForEditAsync(int id);
    Task<DeleteTrainerViewModel?> GetTrainerForDeleteAsync(int id);
    Task<(bool Success, string? ErrorMessage)> CreateTrainerAsync(CreateTrainerViewModel model);
    Task<(bool Success, string? ErrorMessage)> UpdateTrainerAsync(EditTrainerViewModel model);
    Task<(bool Success, string? ErrorMessage)> DeleteTrainerAsync(int id);
}
