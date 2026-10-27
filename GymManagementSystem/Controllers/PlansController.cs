using GymManagementSystem.DAL.Data.Contexts;
using GymManagementSystem.DAL.Repositories.Plans;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.PL.Controllers
{
    public class PlansController : Controller
    {
        public IPlanRepository planRepo = new PlanRepository();
        public async Task<IActionResult> Index()
        {
            var plans = await planRepo.GetAllAsync();
            return View(plans);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return RedirectToAction(nameof(Index));
            }

            var plan = await planRepo.GetByIdAsync(id);
            
            if (plan is null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }
    }
}
