using GymManagementSystem.BLL.Services.Plans;
using GymManagementSystem.BLL.ViewModels.Plans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GymManagementSystem.PL.Controllers
{
    [Authorize]
    public class PlansController : Controller
    {
        private readonly IPlanService _planService;
        private readonly ILogger<PlansController> _logger;

        public PlansController(IPlanService planService, ILogger<PlansController> logger)
        {
            _planService = planService;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var plans = await _planService.GetAllPlansAsync();
            return View(plans);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return RedirectToAction(nameof(Index));
            }

            var plan = await _planService.GetPlanDetailsAsync(id);
            if (plan is null)
            {
                TempData["ErrorMessage"] = "Plan not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(plan);
        }

        // Create (GET: /Plans/Create)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View(new CreatePlanViewModel());
        }

        // Create (POST: /Plans/Create)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePlanViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _planService.CreatePlanAsync(model);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Error ?? "Failed to create plan.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Plan created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                return RedirectToAction(nameof(Index));
            }

            var plan = await _planService.GetPlanForEditAsync(id);
            if (plan is null)
            {
                TempData["ErrorMessage"] = "Plan not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(plan);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditPlanViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _planService.UpdatePlanAsync(model);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Error ?? "Failed to update plan.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Plan updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id)
        {
            return await ToggleStatus(id);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            if (id <= 0)
            {
                return RedirectToAction(nameof(Index));
            }

            var result = await _planService.ToggleStatusAsync(id);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
            }
            else
            {
                TempData["SuccessMessage"] = "Plan Status Changed";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
