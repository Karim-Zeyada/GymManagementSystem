using AutoMapper;
using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.Bookings;
using GymManagementSystem.BLL.ViewModels.Sessions;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.UnitOfWork;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace GymManagementSystem.BLL.Services.Bookings;

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<BookingService> _logger;

    public BookingService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<BookingService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<SessionScheduleViewModel> GetScheduleAndBookingsAsync()
    {
        var now = DateTime.UtcNow;
        var allSessions = await _unitOfWork.Sessions.GetAllWithDetailsAsync();
        var upcomingSessions = allSessions.Where(s => s.EndDate >= now);

        var allBookings = await _unitOfWork.Bookings.GetAllWithDetailsAsync();

        return new SessionScheduleViewModel
        {
            UpcomingSessions = _mapper.Map<IEnumerable<SessionViewModel>>(upcomingSessions),
            RecentBookings = _mapper.Map<IEnumerable<BookingViewModel>>(allBookings)
        };
    }

    public async Task<CreateBookingViewModel> GetCreateBookingViewModelAsync(int? sessionId = null)
    {
        var now = DateTime.UtcNow;
        var availableSessions = (await _unitOfWork.Sessions.GetAllWithDetailsAsync())
            .Where(s => s.EndDate >= now && s.Bookings.Count < s.Capacity);

        var allMembers = await _unitOfWork.Members.GetAllAsync();

        return new CreateBookingViewModel
        {
            SessionId = sessionId ?? 0,
            Sessions = availableSessions.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = $"{s.Category?.Name ?? "Session"} with {s.Trainer?.Name ?? "Trainer"} ({s.StartDate:MMM dd, hh:mm tt}) — {s.Bookings.Count}/{s.Capacity} Slots",
                Selected = sessionId.HasValue && s.Id == sessionId.Value
            }),
            Members = allMembers.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = $"{m.Name} ({m.Phone})"
            })
        };
    }

    public async Task<Result> CreateBookingAsync(CreateBookingViewModel model)
    {
        var session = await _unitOfWork.Sessions.GetWithDetailsAsync(model.SessionId);
        if (session == null)
        {
            _logger.LogWarning("Booking failed: Session with ID {SessionId} not found.", model.SessionId);
            return Result.Fail("Workout session not found.");
        }

        if (session.EndDate < DateTime.UtcNow)
        {
            _logger.LogWarning("Booking failed: Session ID {SessionId} has already ended.", model.SessionId);
            return Result.Fail("Cannot book a session that has already finished.");
        }

        var currentBookingsCount = await _unitOfWork.Bookings.GetSessionBookingsCountAsync(model.SessionId);
        if (currentBookingsCount >= session.Capacity)
        {
            _logger.LogWarning("Booking failed: Session ID {SessionId} is fully booked ({Count}/{Capacity}).", model.SessionId, currentBookingsCount, session.Capacity);
            return Result.Fail("This workout session has already reached maximum capacity.");
        }

        var member = await _unitOfWork.Members.GetByIdAsync(model.MemberId);
        if (member == null)
        {
            _logger.LogWarning("Booking failed: Member with ID {MemberId} not found.", model.MemberId);
            return Result.Fail("Member not found.");
        }

        var isAlreadyBooked = await _unitOfWork.Bookings.IsMemberBookedInSessionAsync(model.MemberId, model.SessionId);
        if (isAlreadyBooked)
        {
            _logger.LogWarning("Booking failed: Member {MemberId} is already booked in session {SessionId}.", model.MemberId, model.SessionId);
            return Result.Fail("This member is already registered for this session.");
        }

        var hasActiveMembership = await _unitOfWork.Memberships.HasActiveMembershipAsync(model.MemberId, session.StartDate);
        if (!hasActiveMembership)
        {
            _logger.LogWarning("Booking failed: Member {MemberId} does not have an active membership on {StartDate}.", model.MemberId, session.StartDate);
            return Result.Fail("Member does not have an active membership covering the date of this session. Please assign or renew their membership first.");
        }

        var booking = new Booking
        {
            MemberId = model.MemberId,
            SessionId = model.SessionId,
            Date = DateTime.Now,
            IsAttended = false
        };

        _unitOfWork.Bookings.Add(booking);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Successfully booked member {MemberId} into session {SessionId} (Booking #{BookingId}).", model.MemberId, model.SessionId, booking.Id);
        return Result.Ok();
    }

    public async Task<Result> ToggleAttendanceAsync(int bookingId)
    {
        var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
        if (booking == null)
        {
            _logger.LogWarning("Attendance toggle failed: Booking {BookingId} not found.", bookingId);
            return Result.Fail("Booking not found.");
        }

        booking.IsAttended = !booking.IsAttended;
        booking.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Bookings.Update(booking);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Toggled attendance for booking #{BookingId} to {IsAttended}.", bookingId, booking.IsAttended);
        return Result.Ok();
    }

    public async Task<Result> CancelBookingAsync(int bookingId)
    {
        var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
        if (booking == null)
        {
            return Result.Fail("Booking not found.");
        }

        _unitOfWork.Bookings.Delete(booking);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Cancelled booking #{BookingId}.", bookingId);
        return Result.Ok();
    }

    public async Task<IEnumerable<BookingViewModel>> GetBookingsForSessionAsync(int sessionId)
    {
        var bookings = await _unitOfWork.Bookings.GetBookingsBySessionIdAsync(sessionId);
        return _mapper.Map<IEnumerable<BookingViewModel>>(bookings);
    }
}
