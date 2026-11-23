using GymManagementSystem.BLL.Services.Bookings;
using GymManagementSystem.BLL.ViewModels.Bookings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GymManagementSystem.PL.Controllers;

[Authorize]
public class BookingController : Controller
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<BookingController> _logger;

    public BookingController(
        IBookingService bookingService,
        ILogger<BookingController> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    // 1. Index (GET: /Booking) - Sessions Schedule & Bookings Hub
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("Loading Sessions Schedule and Bookings Hub.");
        var model = await _bookingService.GetScheduleAndBookingsAsync();
        return View(model);
    }

    // 2. Create (GET: /Booking/Create?sessionId=3)
    [HttpGet]
    [Authorize(Roles = "Admin,Trainer")]
    public async Task<IActionResult> Create(int? sessionId)
    {
        _logger.LogInformation("Opening Book Member page with sessionId {SessionId}.", sessionId);
        var model = await _bookingService.GetCreateBookingViewModelAsync(sessionId);
        return View(model);
    }

    // 3. Create (POST: /Booking/Create)
    [HttpPost]
    [Authorize(Roles = "Admin,Trainer")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBookingViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var refreshed = await _bookingService.GetCreateBookingViewModelAsync(model.SessionId);
            model.Sessions = refreshed.Sessions;
            model.Members = refreshed.Members;
            return View(model);
        }

        var result = await _bookingService.CreateBookingAsync(model);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to book session.");
            var refreshed = await _bookingService.GetCreateBookingViewModelAsync(model.SessionId);
            model.Sessions = refreshed.Sessions;
            model.Members = refreshed.Members;
            return View(model);
        }

        TempData["SuccessMessage"] = "Member booked into workout session successfully.";
        return RedirectToAction(nameof(Index));
    }

    // 4. ToggleAttendance (POST: /Booking/ToggleAttendance/5)
    [HttpPost]
    [Authorize(Roles = "Admin,Trainer")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleAttendance(int id, string? returnUrl = null)
    {
        if (id <= 0)
        {
            return RedirectToAction(nameof(Index));
        }

        var result = await _bookingService.ToggleAttendanceAsync(id);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error ?? "Failed to update attendance.";
        }
        else
        {
            TempData["SuccessMessage"] = "Attendance status updated successfully.";
        }

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(nameof(Index));
    }

    // 5. Cancel (POST: /Booking/Cancel/5)
    [HttpPost]
    [Authorize(Roles = "Admin,Trainer")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        if (id <= 0)
        {
            return RedirectToAction(nameof(Index));
        }

        var result = await _bookingService.CancelBookingAsync(id);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error ?? "Failed to cancel booking.";
        }
        else
        {
            TempData["SuccessMessage"] = "Booking cancelled successfully.";
        }

        return RedirectToAction(nameof(Index));
    }
}
