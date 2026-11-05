
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MovieTicketBookingSystem.Constants;
using MovieTicketBookingSystem.DTOs.Bookings;

namespace MovieTicketBookingSystem.Authorization;

public class BookingOwnerHandler
    : AuthorizationHandler<BookingOwnerRequirement, BookingResponse>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        BookingOwnerRequirement requirement,
        BookingResponse booking)
    {
        // Admin can access any booking.
        if (context.User.IsInRole(UserRoles.Admin))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // Get logged-in user's ID from JWT.
        var userIdClaim = context.User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        // Validate and convert user ID.
        if (!int.TryParse(userIdClaim, out int currentUserId))
        {
            return Task.CompletedTask;
        }

        // Check booking ownership.
        if (booking.UserId == currentUserId)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
