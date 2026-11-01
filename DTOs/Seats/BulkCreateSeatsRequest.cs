using System.ComponentModel.DataAnnotations;

namespace MovieTicketBookingSystem.DTOs.Seats
{
    public class BulkCreateSeatsRequest
    {
        [Range(1, int.MaxValue)]
        public int ScreenId { get; set; }

        [Required]
        [MinLength(1)]
        public List<CreateSeatItemRequest> Seats { get; set; } = new();
    }
}
