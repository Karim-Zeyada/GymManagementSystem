using GymManagementSystem.BLL.Services.Analytics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GymManagementSystem.PL.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{
    private readonly IAnalyticsService _analyticsService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IAnalyticsService analyticsService, ILogger<HomeController> logger)
    {
        _analyticsService = analyticsService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("Loading home dashboard analytics metrics.");
        var analytics = await _analyticsService.GetDashboardAnalyticsAsync();
        return View(analytics);
    }
}
