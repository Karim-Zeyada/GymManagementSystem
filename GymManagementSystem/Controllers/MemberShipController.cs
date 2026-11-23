using GymManagementSystem.BLL.Services.Memberships;
using GymManagementSystem.BLL.ViewModels.Memberships;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GymManagementSystem.PL.Controllers;

[Authorize]
public class MemberShipController : Controller
{
    private readonly IMembershipService _membershipService;
    private readonly ILogger<MemberShipController> _logger;

    public MemberShipController(
        IMembershipService membershipService,
        ILogger<MemberShipController> logger)
    {
        _membershipService = membershipService;
        _logger = logger;
    }

    // 1. Index (GET: /MemberShip?status=active)
    [HttpGet]
    public async Task<IActionResult> Index(string? status)
    {
        _logger.LogInformation("Loading memberships list with filter '{Filter}'.", status ?? "all");
        var memberships = await _membershipService.GetAllMembershipsAsync(status);
        ViewBag.CurrentFilter = status?.ToLowerInvariant() ?? "all";
        return View(memberships);
    }

    // 2. Create (GET: /MemberShip/Create?memberId=5)
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(int? memberId)
    {
        _logger.LogInformation("Opening Create Membership view with memberId {MemberId}.", memberId);
        var model = await _membershipService.GetCreateMembershipViewModelAsync(memberId);
        return View(model);
    }

    // 3. Create (POST: /MemberShip/Create)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateMembershipViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var refreshed = await _membershipService.GetCreateMembershipViewModelAsync(model.MemberId);
            model.Members = refreshed.Members;
            model.Plans = refreshed.Plans;
            return View(model);
        }

        var result = await _membershipService.CreateMembershipAsync(model);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to assign membership.");
            var refreshed = await _membershipService.GetCreateMembershipViewModelAsync(model.MemberId);
            model.Members = refreshed.Members;
            model.Plans = refreshed.Plans;
            return View(model);
        }

        TempData["SuccessMessage"] = "Membership successfully assigned.";
        return RedirectToAction(nameof(Index));
    }

    // 4. Cancel (POST: /MemberShip/Cancel/5)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        if (id <= 0)
        {
            return RedirectToAction(nameof(Index));
        }

        var result = await _membershipService.CancelMembershipAsync(id);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error ?? "Failed to cancel membership.";
        }
        else
        {
            TempData["SuccessMessage"] = "Membership successfully cancelled.";
        }

        return RedirectToAction(nameof(Index));
    }
}
