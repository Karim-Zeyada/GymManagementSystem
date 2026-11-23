using GymManagementSystem.BLL.Services.Sessions;
using GymManagementSystem.BLL.ViewModels.Sessions;
using GymManagementSystem.DAL.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GymManagementSystem.PL.Controllers;

[Authorize]
public class SessionController : Controller
{
    private readonly ISessionService _sessionService;
    private readonly ILogger<SessionController> _logger;

    public SessionController(ISessionService sessionService, ILogger<SessionController> logger)
    {
        _sessionService = sessionService;
        _logger = logger;
    }

    // 1. Index (GET: /Session)
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("Navigating to Sessions Index view.");
        var sessions = await _sessionService.GetAllSessionsAsync();
        return View(sessions);
    }

    // 2. Details (GET: /Session/Details/5)
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Invalid session ID {SessionId} requested in Details.", id);
            return RedirectToAction(nameof(Index));
        }

        _logger.LogInformation("Retrieving details for session ID {SessionId}.", id);
        var session = await _sessionService.GetSessionDetailsAsync(id);
        if (session == null)
        {
            _logger.LogWarning("Session with ID {SessionId} was not found.", id);
            TempData["ErrorMessage"] = "Session not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(session);
    }

    // 3. Create (GET: /Session/Create)
    [HttpGet]
    [Authorize(Roles = "Admin,Trainer")]
    public async Task<IActionResult> Create()
    {
        _logger.LogInformation("Opening Session Create page.");
        var model = await _sessionService.GetCreateSessionViewModelAsync();
        await PopulateLookupsAsync(model);
        return View(model);
    }

    // 3. Create (POST: /Session/Create)
    [HttpPost]
    [Authorize(Roles = "Admin,Trainer")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSessionViewModel model)
    {
        _logger.LogInformation("Submitting Session Create request for Category {CategoryId} and Trainer {TrainerId}.", model.CategoryId, model.TrainerId);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Model state invalid during Session Create.");
            await PopulateLookupsAsync(model);
            return View(model);
        }

        var result = await _sessionService.CreateSessionAsync(model);
        if (!result.IsSuccess)
        {
            _logger.LogWarning("Session creation failed: {Error}", result.Error);
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to create session.");
            await PopulateLookupsAsync(model);
            return View(model);
        }

        _logger.LogInformation("Session created successfully with ID {SessionId}.", result.Value);
        TempData["SuccessMessage"] = "Session created successfully.";
        return RedirectToAction(nameof(Index));
    }

    // 4. Edit (GET: /Session/Edit/5)
    [HttpGet]
    [Authorize(Roles = "Admin,Trainer")]
    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Invalid session ID {SessionId} for Edit GET.", id);
            return RedirectToAction(nameof(Index));
        }

        _logger.LogInformation("Opening Session Edit page for session ID {SessionId}.", id);
        var model = await _sessionService.GetSessionForEditAsync(id);
        if (model == null)
        {
            _logger.LogWarning("Session with ID {SessionId} was not found for editing.", id);
            TempData["ErrorMessage"] = "Session not found.";
            return RedirectToAction(nameof(Index));
        }

        if (model.StartDate <= DateTime.Now)
        {
            _logger.LogWarning("Attempted to edit session ID {SessionId} that is ongoing or completed.", id);
            TempData["ErrorMessage"] = "Only upcoming sessions can be edited. Ongoing or completed sessions cannot be changed.";
            return RedirectToAction(nameof(Index));
        }

        await PopulateEditLookupsAsync(model);
        return View(model);
    }

    // 4. Edit (POST: /Session/Edit/5)
    [HttpPost]
    [Authorize(Roles = "Admin,Trainer")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditSessionViewModel model)
    {
        if (id != model.Id)
        {
            _logger.LogWarning("Session ID mismatch in Edit POST: route ID {RouteId} vs model ID {ModelId}.", id, model.Id);
            return BadRequest();
        }

        _logger.LogInformation("Submitting Session Edit request for session ID {SessionId}.", id);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Model state invalid during Session Edit for ID {SessionId}.", id);
            await PopulateEditLookupsAsync(model);
            return View(model);
        }

        var result = await _sessionService.UpdateSessionAsync(model);
        if (!result.IsSuccess)
        {
            _logger.LogWarning("Session update failed for ID {SessionId}: {Error}", id, result.Error);
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to update session.");
            await PopulateEditLookupsAsync(model);
            return View(model);
        }

        _logger.LogInformation("Session ID {SessionId} updated successfully.", id);
        TempData["SuccessMessage"] = "Session updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    // 5. Delete (GET: /Session/Delete/5)
    [HttpGet]
    [Authorize(Roles = "Admin,Trainer")]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Invalid session ID {SessionId} for Delete GET.", id);
            return RedirectToAction(nameof(Index));
        }

        _logger.LogInformation("Opening Session Delete confirmation for session ID {SessionId}.", id);
        var model = await _sessionService.GetSessionForDeleteAsync(id);
        if (model == null)
        {
            _logger.LogWarning("Session with ID {SessionId} was not found for deletion.", id);
            TempData["ErrorMessage"] = "Session not found.";
            return RedirectToAction(nameof(Index));
        }

        if (model.Status == SessionStatus.Ongoing)
        {
            _logger.LogWarning("Attempted to open Delete for ongoing session ID {SessionId}.", id);
            TempData["ErrorMessage"] = "Ongoing sessions cannot be deleted while training is in progress.";
            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    // 5. DeleteConfirmed (POST: /Session/Delete/5)
    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin,Trainer")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Invalid session ID {SessionId} for Delete POST.", id);
            return RedirectToAction(nameof(Index));
        }

        _logger.LogInformation("Submitting Session Delete for session ID {SessionId}.", id);
        var result = await _sessionService.DeleteSessionAsync(id);
        if (!result.IsSuccess)
        {
            _logger.LogWarning("Session deletion failed for ID {SessionId}: {Error}", id, result.Error);
            TempData["ErrorMessage"] = result.Error ?? "Failed to delete session.";
            return RedirectToAction(nameof(Index));
        }

        _logger.LogInformation("Session ID {SessionId} deleted successfully.", id);
        TempData["SuccessMessage"] = "Session deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    #region Helper Methods (Lookups)
    private async Task PopulateLookupsAsync(CreateSessionViewModel? model = null)
    {
        var categories = await _sessionService.GetCategoriesSelectListAsync();
        var trainers = await _sessionService.GetTrainersSelectListAsync();

        ViewBag.Categories = categories;
        ViewBag.Trainers = trainers;

        if (model != null)
        {
            model.Categories = categories;
            model.Trainers = trainers;
        }
    }

    private async Task PopulateEditLookupsAsync(EditSessionViewModel? model = null)
    {
        var trainers = await _sessionService.GetTrainersSelectListAsync();
        ViewBag.Trainers = trainers;

        if (model != null)
        {
            model.Trainers = trainers;
            if (string.IsNullOrEmpty(model.CategoryName) && model.Id > 0)
            {
                var existing = await _sessionService.GetSessionForEditAsync(model.Id);
                if (existing != null)
                {
                    model.CategoryName = existing.CategoryName;
                    model.Capacity = existing.Capacity;
                }
            }
        }
    }
    #endregion
}
