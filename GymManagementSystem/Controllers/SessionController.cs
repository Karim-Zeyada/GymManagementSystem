using GymManagementSystem.BLL.Services.Sessions;
using GymManagementSystem.BLL.ViewModels.Sessions;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.PL.Controllers;

public class SessionController : Controller
{
    private readonly ISessionService _sessionService;

    public SessionController(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    // 1. Index (GET: /Session)
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var sessions = await _sessionService.GetAllSessionsAsync();
        return View(sessions);
    }

    // 2. Details (GET: /Session/Details/5)
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        if (id <= 0)
        {
            return RedirectToAction(nameof(Index));
        }

        var session = await _sessionService.GetSessionDetailsAsync(id);
        if (session == null)
        {
            TempData["ErrorMessage"] = "Session not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(session);
    }

    // 3. Create (GET: /Session/Create)
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = await _sessionService.GetCreateSessionViewModelAsync();
        ViewBag.Categories = model.Categories;
        ViewBag.Trainers = model.Trainers;
        return View(model);
    }

    // 3. Create (POST: /Session/Create)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSessionViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var reloadModel = await _sessionService.GetCreateSessionViewModelAsync();
            model.Categories = reloadModel.Categories;
            model.Trainers = reloadModel.Trainers;
            ViewBag.Categories = model.Categories;
            ViewBag.Trainers = model.Trainers;
            return View(model);
        }

        var result = await _sessionService.CreateSessionAsync(model);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to create session.");
            var reloadModel = await _sessionService.GetCreateSessionViewModelAsync();
            model.Categories = reloadModel.Categories;
            model.Trainers = reloadModel.Trainers;
            ViewBag.Categories = model.Categories;
            ViewBag.Trainers = model.Trainers;
            return View(model);
        }

        TempData["SuccessMessage"] = "Session created successfully.";
        return RedirectToAction(nameof(Index));
    }

    // 4. Edit (GET: /Session/Edit/5)
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0)
        {
            return RedirectToAction(nameof(Index));
        }

        var model = await _sessionService.GetSessionForEditAsync(id);
        if (model == null)
        {
            TempData["ErrorMessage"] = "Session not found.";
            return RedirectToAction(nameof(Index));
        }

        if (model.StartDate <= DateTime.Now)
        {
            TempData["ErrorMessage"] = "Only upcoming sessions can be edited. Ongoing or completed sessions cannot be changed.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Trainers = model.Trainers;
        return View(model);
    }

    // 4. Edit (POST: /Session/Edit/5)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditSessionViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            var freshModel = await _sessionService.GetSessionForEditAsync(id);
            if (freshModel != null)
            {
                model.Trainers = freshModel.Trainers;
                model.CategoryName = freshModel.CategoryName;
            }
            ViewBag.Trainers = model.Trainers;
            return View(model);
        }

        var result = await _sessionService.UpdateSessionAsync(model);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to update session.");
            var freshModel = await _sessionService.GetSessionForEditAsync(id);
            if (freshModel != null)
            {
                model.Trainers = freshModel.Trainers;
                model.CategoryName = freshModel.CategoryName;
            }
            ViewBag.Trainers = model.Trainers;
            return View(model);
        }

        TempData["SuccessMessage"] = "Session updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    // 5. Delete (GET: /Session/Delete/5)
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
        {
            return RedirectToAction(nameof(Index));
        }

        var model = await _sessionService.GetSessionForDeleteAsync(id);
        if (model == null)
        {
            TempData["ErrorMessage"] = "Session not found.";
            return RedirectToAction(nameof(Index));
        }

        if (model.Status == SessionStatus.Ongoing)
        {
            TempData["ErrorMessage"] = "Ongoing sessions cannot be deleted while training is in progress.";
            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    // 5. DeleteConfirmed (POST: /Session/Delete/5)
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var result = await _sessionService.DeleteSessionAsync(id);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error ?? "Failed to delete session.";
            return RedirectToAction(nameof(Index));
        }

        TempData["SuccessMessage"] = "Session deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
