using GymManagementSystem.BLL.Services.Analytics;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.PL.Controllers;

public class HomeController : Controller
{
    private readonly IAnalyticsService _analyticsService;

    public HomeController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var analytics = await _analyticsService.GetDashboardAnalyticsAsync();
        return View(analytics);
    }
}
