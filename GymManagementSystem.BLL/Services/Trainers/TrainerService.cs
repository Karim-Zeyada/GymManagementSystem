using GymManagementSystem.BLL.ViewModels.Trainers;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Models.ValueObjects;
using GymManagementSystem.DAL.Repositories.Trainers;

namespace GymManagementSystem.BLL.Services.Trainers;

public class TrainerService : ITrainerService
{
    private readonly ITrainerRepository _trainerRepository;

    public TrainerService(ITrainerRepository trainerRepository)
    {
        _trainerRepository = trainerRepository;
    }

    public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync()
    {
        var trainers = await _trainerRepository.GetAllAsync();
        return trainers.Select(t => new TrainerViewModel
        {
            Id = t.Id,
            Name = t.Name,
            Email = t.Email,
            Phone = t.Phone,
            Speciality = t.Speciality
        }).ToList();
    }

    public async Task<TrainerDetailsViewModel?> GetTrainerDetailsAsync(int id)
    {
        var trainer = await _trainerRepository.GetByIdAsync(id);
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
        var trainer = await _trainerRepository.GetByIdAsync(id);
        if (trainer == null) return null;

        return new EditTrainerViewModel
        {
            Id = trainer.Id,
            Name = trainer.Name,
            DateOfBirth = trainer.DateOfBirth,
            Gender = trainer.Gender,
            Email = trainer.Email,
            Phone = trainer.Phone,
            BuildingNumber = trainer.Address?.BuildingNumber ?? 0,
            Street = trainer.Address?.Street ?? string.Empty,
            City = trainer.Address?.City ?? string.Empty,
            Speciality = trainer.Speciality
        };
    }

    public async Task<DeleteTrainerViewModel?> GetTrainerForDeleteAsync(int id)
    {
        var trainer = await _trainerRepository.GetByIdAsync(id);
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
        if (await _trainerRepository.IsEmailExistsAsync(model.Email))
        {
            return (false, "A user with this email address already exists.");
        }

        // 2. Phone uniqueness
        if (await _trainerRepository.IsPhoneExistsAsync(model.Phone))
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

        _trainerRepository.Add(trainer);
        await _trainerRepository.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool Success, string? ErrorMessage)> UpdateTrainerAsync(EditTrainerViewModel model)
    {
        var trainer = await _trainerRepository.GetByIdAsync(model.Id);
        if (trainer == null)
        {
            return (false, "Trainer not found.");
        }

        // Email uniqueness
        if (await _trainerRepository.IsEmailExistsAsync(model.Email, model.Id))
        {
            return (false, "This email address is already in use by another user.");
        }

        // Phone uniqueness
        if (await _trainerRepository.IsPhoneExistsAsync(model.Phone, model.Id))
        {
            return (false, "This phone number is already in use by another user.");
        }

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

        _trainerRepository.Update(trainer);
        await _trainerRepository.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool Success, string? ErrorMessage)> DeleteTrainerAsync(int id)
    {
        var trainer = await _trainerRepository.GetByIdAsync(id);
        if (trainer == null)
        {
            return (false, "Trainer not found.");
        }

        // Business rule: Cannot delete trainer with scheduled sessions
        if (await _trainerRepository.HasActiveSessionsAsync(id))
        {
            return (false, "Cannot delete trainer with assigned active sessions.");
        }

        _trainerRepository.Delete(trainer);
        await _trainerRepository.SaveChangesAsync();

        return (true, null);
    }
}
