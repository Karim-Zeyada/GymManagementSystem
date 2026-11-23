using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.Trainers;

namespace GymManagementSystem.BLL.Services.Trainers;

public interface ITrainerService
{
    Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync();
    Task<TrainerDetailsViewModel?> GetTrainerDetailsAsync(int id);
    Task<EditTrainerViewModel?> GetTrainerForEditAsync(int id);
    Task<DeleteTrainerViewModel?> GetTrainerForDeleteAsync(int id);
    Task<Result> CreateTrainerAsync(CreateTrainerViewModel model);
    Task<Result> UpdateTrainerAsync(EditTrainerViewModel model);
    Task<Result> DeleteTrainerAsync(int id);
}
