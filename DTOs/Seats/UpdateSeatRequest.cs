using System.ComponentModel.DataAnnotations;

namespace MovieTicketBookingSystem.DTOs.Seats
{
    public class UpdateSeatRequest
    {
        [Range(1, int.MaxValue)]
        public int ScreenId { get; set; }

        [Required]
        [StringLength(5)]
        public string RowName { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int SeatNumber { get; set; }

        [Required]
        [StringLength(30)]
        public string SeatType { get; set; } = string.Empty;
    }
}
