using GymManagementSystem.Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.Controllers
{
    public class PlansController : Controller
    {
        public GymDbContext Context = new GymDbContext();
        public async Task<IActionResult> Index()
        {
            var plans = await Context.Plans.ToListAsync();
            return View(plans);
        }
    }
}
