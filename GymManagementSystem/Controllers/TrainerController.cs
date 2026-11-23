using GymManagementSystem.BLL.Services.Trainers;
using GymManagementSystem.BLL.ViewModels.Trainers;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.PL.Controllers
{
    public class TrainerController : Controller
    {
        private readonly ITrainerService _trainerService;

        public TrainerController(ITrainerService trainerService)
        {
            _trainerService = trainerService;
        }

        // 1. Index (GET: /Trainer)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var trainers = await _trainerService.GetAllTrainersAsync();
            return View(trainers);
        }

        // 2. Details (GET: /Trainer/Details/5)
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return RedirectToAction(nameof(Index));
            }

            var trainer = await _trainerService.GetTrainerDetailsAsync(id);
            if (trainer == null)
            {
                TempData["ErrorMessage"] = "Trainer not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(trainer);
        }

        // 3. Create (GET: /Trainer/Create)
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateTrainerViewModel
            {
                DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-25))
            });
        }

        // 4. Create (POST: /Trainer/Create)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTrainerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _trainerService.CreateTrainerAsync(model);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Failed to create trainer.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Trainer Created Successfully";
            return RedirectToAction(nameof(Index));
        }

        // 5. Edit (GET: /Trainer/Edit/5)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                return RedirectToAction(nameof(Index));
            }

            var trainer = await _trainerService.GetTrainerForEditAsync(id);
            if (trainer == null)
            {
                TempData["ErrorMessage"] = "Trainer not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(trainer);
        }

        // 6. Edit (POST: /Trainer/Edit/5)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditTrainerViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _trainerService.UpdateTrainerAsync(model);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Failed to update trainer.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Trainer Updated Successfully";
            return RedirectToAction(nameof(Index));
        }

        // 7. Delete (GET: /Trainer/Delete/5 - Confirmation Page)
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return RedirectToAction(nameof(Index));
            }

            var trainer = await _trainerService.GetTrainerForDeleteAsync(id);
            if (trainer == null)
            {
                TempData["ErrorMessage"] = "Trainer not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(trainer);
        }

        // 8. Delete (POST: /Trainer/Delete/5 - Commit Deletion)
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0)
            {
                return RedirectToAction(nameof(Index));
            }

            var result = await _trainerService.DeleteTrainerAsync(id);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return RedirectToAction(nameof(Delete), new { id });
            }

            TempData["SuccessMessage"] = "Trainer Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
