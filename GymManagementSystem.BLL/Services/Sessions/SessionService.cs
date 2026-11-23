using AutoMapper;
using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.Sessions;
using GymManagementSystem.DAL.Enums;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.UnitOfWork;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace GymManagementSystem.BLL.Services.Sessions;

public class SessionService : ISessionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<SessionService> _logger;

    public SessionService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<SessionService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync()
    {
        _logger.LogInformation("Retrieving all training sessions with category, trainer, and booking details.");
        var sessions = await _unitOfWork.Sessions.GetAllWithDetailsAsync();
        return _mapper.Map<IEnumerable<SessionViewModel>>(sessions);
    }

    public async Task<SessionDetailsViewModel?> GetSessionDetailsAsync(int id)
    {
        _logger.LogInformation("Retrieving details for session ID {SessionId}.", id);
        var session = await _unitOfWork.Sessions.GetWithDetailsAsync(id);
        if (session == null)
        {
            _logger.LogWarning("Session with ID {SessionId} was not found.", id);
            return null;
        }

        return _mapper.Map<SessionDetailsViewModel>(session);
    }

    public async Task<CreateSessionViewModel> GetCreateSessionViewModelAsync()
    {
        var model = new CreateSessionViewModel
        {
            StartDate = DateTime.Now.AddHours(2),
            EndDate = DateTime.Now.AddHours(3),
            Capacity = 10
        };

        model.Categories = await GetCategoriesSelectListAsync();
        model.Trainers = await GetTrainersSelectListAsync();
        return model;
    }

    public async Task<IEnumerable<SelectListItem>> GetCategoriesSelectListAsync()
    {
        var categories = await _unitOfWork.Categories.GetAllAsync();
        return categories.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name
        }).ToList();
    }

    public async Task<IEnumerable<SelectListItem>> GetTrainersSelectListAsync()
    {
        var trainers = await _unitOfWork.Trainers.GetAllAsync();
        return trainers.Select(t => new SelectListItem
        {
            Value = t.Id.ToString(),
            Text = $"{t.Name} ({t.Speciality})"
        }).ToList();
    }

    public async Task<Result<int>> CreateSessionAsync(CreateSessionViewModel model)
    {
        _logger.LogInformation("Attempting to create session for category ID {CategoryId} with trainer ID {TrainerId}.", model.CategoryId, model.TrainerId);

        // 1. Validate Category exists
        var category = await _unitOfWork.Categories.GetByIdAsync(model.CategoryId);
        if (category == null)
        {
            _logger.LogWarning("Category validation failed: Category {CategoryId} does not exist.", model.CategoryId);
            return Result.Fail<int>("The selected Category does not exist.");
        }

        // 2. Validate Trainer exists
        var trainer = await _unitOfWork.Trainers.GetByIdAsync(model.TrainerId);
        if (trainer == null)
        {
            _logger.LogWarning("Trainer validation failed: Trainer {TrainerId} does not exist.", model.TrainerId);
            return Result.Fail<int>("The selected Trainer does not exist.");
        }

        // 3. Business rule: Start must be in the future
        var now = DateTime.Now;
        if (model.StartDate <= now)
        {
            _logger.LogWarning("Time validation failed: Start time {StartDate} is in the past.", model.StartDate);
            return Result.Fail<int>("Session start time must be in the future.");
        }

        // 4. Business rule: Start < End
        if (model.StartDate >= model.EndDate)
        {
            _logger.LogWarning("Time validation failed: End time {EndDate} must be after start time {StartDate}.", model.EndDate, model.StartDate);
            return Result.Fail<int>("Session end time must be after start time.");
        }

        // 5. Business rule: Capacity must be at least 1
        if (model.Capacity < 1)
        {
            return Result.Fail<int>("Session capacity must be at least 1.");
        }

        // 6. Business rule: Trainer specialty must match Category
        if (!DoesTrainerMatchCategory(trainer.Speciality, category.Name))
        {
            _logger.LogWarning("Specialty mismatch: Trainer {TrainerName} ({Speciality}) does not match category '{CategoryName}'.", trainer.Name, trainer.Speciality, category.Name);
            return Result.Fail<int>($"Trainer {trainer.Name} specializes in {trainer.Speciality}, which does not match the '{category.Name}' category.");
        }

        // 7. Business rule: Trainer must be free (no double-booking)
        if (await _unitOfWork.Sessions.HasTrainerConflictAsync(model.TrainerId, model.StartDate, model.EndDate))
        {
            _logger.LogWarning("Schedule conflict: Trainer {TrainerName} is already scheduled between {StartDate} and {EndDate}.", trainer.Name, model.StartDate, model.EndDate);
            return Result.Fail<int>($"Trainer {trainer.Name} is already scheduled for another session during this time slot.");
        }

        // 8. Create and persist entity
        var session = new Session
        {
            CategoryId = model.CategoryId,
            TrainerId = model.TrainerId,
            Description = model.Description.Trim(),
            Capacity = model.Capacity,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            CreatedAt = DateTime.UtcNow
        };

        _unitOfWork.Sessions.Add(session);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Session ID {SessionId} created successfully.", session.Id);
        return Result.Ok(session.Id);
    }

    public async Task<EditSessionViewModel?> GetSessionForEditAsync(int id)
    {
        _logger.LogInformation("Retrieving session ID {SessionId} for editing.", id);
        var session = await _unitOfWork.Sessions.GetWithDetailsAsync(id);
        if (session == null)
        {
            _logger.LogWarning("Session ID {SessionId} not found for edit.", id);
            return null;
        }

        var model = _mapper.Map<EditSessionViewModel>(session);
        model.Trainers = await GetTrainersSelectListAsync();
        return model;
    }

    public async Task<Result> UpdateSessionAsync(EditSessionViewModel model)
    {
        _logger.LogInformation("Attempting to update session ID {SessionId}.", model.Id);

        var session = await _unitOfWork.Sessions.GetWithDetailsAsync(model.Id);
        if (session == null)
        {
            _logger.LogWarning("Session ID {SessionId} not found for update.", model.Id);
            return Result.Fail("Session not found.", "NotFound");
        }

        // 1. Business rule: Cannot edit an Ongoing or Completed session
        var now = DateTime.Now;
        if (session.StartDate <= now)
        {
            _logger.LogWarning("Edit rejected: Session ID {SessionId} has already started or completed.", model.Id);
            return Result.Fail("Only upcoming sessions can be edited. Ongoing or completed sessions are locked.");
        }

        // 2. Business rule: Start must be in the future
        if (model.StartDate <= now)
        {
            return Result.Fail("Session start time must be in the future.");
        }

        // 3. Business rule: Start < End
        if (model.StartDate >= model.EndDate)
        {
            return Result.Fail("Session end time must be after start time.");
        }

        // 4. Validate Trainer exists
        var trainer = await _unitOfWork.Trainers.GetByIdAsync(model.TrainerId);
        if (trainer == null)
        {
            _logger.LogWarning("Trainer validation failed: Trainer {TrainerId} does not exist.", model.TrainerId);
            return Result.Fail("The selected Trainer does not exist.");
        }

        // 5. Business rule: Trainer specialty must match the (locked) Category
        var category = await _unitOfWork.Categories.GetByIdAsync(session.CategoryId);
        if (category != null && !DoesTrainerMatchCategory(trainer.Speciality, category.Name))
        {
            _logger.LogWarning("Specialty mismatch: Trainer {TrainerName} ({Speciality}) does not match locked category '{CategoryName}'.", trainer.Name, trainer.Speciality, category.Name);
            return Result.Fail($"Trainer {trainer.Name} specializes in {trainer.Speciality}, which does not match the '{category.Name}' category.");
        }

        // 6. Business rule: New trainer must be free in the chosen time slot
        if (await _unitOfWork.Sessions.HasTrainerConflictAsync(model.TrainerId, model.StartDate, model.EndDate, model.Id))
        {
            _logger.LogWarning("Schedule conflict: Trainer {TrainerName} is already scheduled during updated time slot.", trainer.Name);
            return Result.Fail($"Trainer {trainer.Name} is already scheduled for another session during this time slot.");
        }

        // 7. Apply updates to editable fields (Category and Capacity remain locked!)
        session.TrainerId = model.TrainerId;
        session.Description = model.Description.Trim();
        session.StartDate = model.StartDate;
        session.EndDate = model.EndDate;
        session.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Sessions.Update(session);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Session ID {SessionId} updated successfully.", session.Id);
        return Result.Ok();
    }

    public async Task<DeleteSessionViewModel?> GetSessionForDeleteAsync(int id)
    {
        _logger.LogInformation("Retrieving session ID {SessionId} for deletion confirmation.", id);
        var session = await _unitOfWork.Sessions.GetWithDetailsAsync(id);
        if (session == null)
        {
            _logger.LogWarning("Session ID {SessionId} not found for deletion.", id);
            return null;
        }

        return _mapper.Map<DeleteSessionViewModel>(session);
    }

    public async Task<Result> DeleteSessionAsync(int id)
    {
        _logger.LogInformation("Attempting to delete session ID {SessionId}.", id);

        var session = await _unitOfWork.Sessions.GetWithDetailsAsync(id);
        if (session == null)
        {
            _logger.LogWarning("Session ID {SessionId} not found for deletion.", id);
            return Result.Fail("Session not found.", "NotFound");
        }

        // 1. Business rule: Cannot delete an Ongoing session
        var now = DateTime.Now;
        if (session.StartDate <= now && session.EndDate >= now)
        {
            _logger.LogWarning("Delete rejected: Session ID {SessionId} is currently ongoing.", id);
            return Result.Fail("Ongoing sessions cannot be deleted while in progress.");
        }

        // 2. Cascade delete bookings associated with this session
        if (session.Bookings != null && session.Bookings.Any())
        {
            _logger.LogInformation("Cascading deletion of {BookingCount} booking(s) associated with session ID {SessionId}.", session.Bookings.Count, id);
            foreach (var booking in session.Bookings.ToList())
            {
                _unitOfWork.Bookings.Delete(booking);
            }
        }

        _unitOfWork.Sessions.Delete(session);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Session ID {SessionId} deleted successfully.", id);
        return Result.Ok();
    }

    private static bool DoesTrainerMatchCategory(Speciality speciality, string categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryName)) return false;

        var s = speciality.ToString().ToLowerInvariant();
        var c = categoryName.ToLowerInvariant().Replace(" ", "").Replace("-", "").Replace("_", "");

        if (s.Contains(c) || c.Contains(s)) return true;
        if (c.Contains("strength") && (s.Contains("strength") || s.Contains("bodybuilding"))) return true;
        if (c.Contains("cardio") && s.Contains("cardio")) return true;
        if (c.Contains("flexibility") && (s.Contains("yoga") || s.Contains("fitness"))) return true;
        if (c.Contains("balance") && (s.Contains("yoga") || s.Contains("fitness"))) return true;
        if (c.Contains("endurance") && (s.Contains("crossfit") || s.Contains("cardio"))) return true;
        if (s.Contains("generalfitness")) return true;

        return false;
    }
}
