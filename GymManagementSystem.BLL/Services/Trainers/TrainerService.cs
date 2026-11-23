using AutoMapper;
using GymManagementSystem.BLL.ViewModels.Trainers;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Models.ValueObjects;
using GymManagementSystem.DAL.Repositories.UnitOfWork;

namespace GymManagementSystem.BLL.Services.Trainers;

public class TrainerService : ITrainerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TrainerService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync()
    {
        var trainers = await _unitOfWork.Trainers.GetAllAsync();
        return _mapper.Map<IEnumerable<TrainerViewModel>>(trainers);
    }

    public async Task<TrainerDetailsViewModel?> GetTrainerDetailsAsync(int id)
    {
        var trainer = await _unitOfWork.Trainers.GetByIdAsync(id);
        if (trainer == null) return null;

        var addressString = trainer.Address != null
            ? $"{trainer.Address.BuildingNumber} · {trainer.Address.Street} · {trainer.Address.City}"
            : string.Empty;

        return new TrainerDetailsViewModel
        {
            Id = trainer.Id,
            Name = trainer.Name,
            Speciality = trainer.Speciality,
            Email = trainer.Email,
            Phone = trainer.Phone,
            DateOfBirth = trainer.DateOfBirth,
            Address = addressString
        };
    }

    public async Task<EditTrainerViewModel?> GetTrainerForEditAsync(int id)
    {
        var trainer = await _unitOfWork.Trainers.GetByIdAsync(id);
        if (trainer == null) return null;

        return _mapper.Map<EditTrainerViewModel>(trainer);
    }

    public async Task<DeleteTrainerViewModel?> GetTrainerForDeleteAsync(int id)
    {
        var trainer = await _unitOfWork.Trainers.GetByIdAsync(id);
        if (trainer == null) return null;

        return new DeleteTrainerViewModel
        {
            Id = trainer.Id,
            Name = trainer.Name,
            Email = trainer.Email,
            Speciality = trainer.Speciality
        };
    }

    public async Task<(bool Success, string? ErrorMessage)> CreateTrainerAsync(CreateTrainerViewModel model)
    {
        // 1. Email uniqueness
        if (await _unitOfWork.Trainers.IsEmailExistsAsync(model.Email))
        {
            return (false, "A user with this email address already exists.");
        }

        // 2. Phone uniqueness
        if (await _unitOfWork.Trainers.IsPhoneExistsAsync(model.Phone))
        {
            return (false, "A user with this phone number already exists.");
        }

        var trainer = new Trainer
        {
            Name = model.Name.Trim(),
            Email = model.Email.Trim(),
            Phone = model.Phone.Trim(),
            DateOfBirth = model.DateOfBirth,
            Gender = model.Gender,
            Speciality = model.Speciality,
            CreatedAt = DateTime.UtcNow,
            Address = new Address
            {
                BuildingNumber = model.BuildingNumber,
                Street = model.Street.Trim(),
                City = model.City.Trim()
            }
        };

        _unitOfWork.Trainers.Add(trainer);
        await _unitOfWork.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool Success, string? ErrorMessage)> UpdateTrainerAsync(EditTrainerViewModel model)
    {
        var trainer = await _unitOfWork.Trainers.GetByIdAsync(model.Id);
        if (trainer == null)
        {
            return (false, "Trainer not found.");
        }

        // Email uniqueness check excluding current trainer
        if (await _unitOfWork.Trainers.IsEmailExistsAsync(model.Email, model.Id))
        {
            return (false, "A user with this email address already exists.");
        }

        // Phone uniqueness check excluding current trainer
        if (await _unitOfWork.Trainers.IsPhoneExistsAsync(model.Phone, model.Id))
        {
            return (false, "A user with this phone number already exists.");
        }

        trainer.Name = model.Name.Trim();
        trainer.Email = model.Email.Trim();
        trainer.Phone = model.Phone.Trim();
        trainer.Speciality = model.Speciality;
        trainer.UpdatedAt = DateTime.UtcNow;

        if (trainer.Address == null)
        {
            trainer.Address = new Address();
        }
        trainer.Address.BuildingNumber = model.BuildingNumber;
        trainer.Address.Street = model.Street.Trim();
        trainer.Address.City = model.City.Trim();

        _unitOfWork.Trainers.Update(trainer);
        await _unitOfWork.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool Success, string? ErrorMessage)> DeleteTrainerAsync(int id)
    {
        var trainer = await _unitOfWork.Trainers.GetByIdAsync(id);
        if (trainer == null)
        {
            return (false, "Trainer not found.");
        }

        // Check active sessions
        if (await _unitOfWork.Trainers.HasActiveSessionsAsync(id))
        {
            return (false, "Cannot delete a trainer who has active or upcoming sessions assigned.");
        }

        _unitOfWork.Trainers.Delete(trainer);
        await _unitOfWork.SaveChangesAsync();

        return (true, null);
    }
}
