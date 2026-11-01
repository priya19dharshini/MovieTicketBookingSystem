namespace MovieTicketBookingSystem.Models
{
    public class Seat
    {
        public int SeatId { get; set; }

        public int ScreenId { get; set; }

        public string RowName { get; set; } = string.Empty;

        public int SeatNumber { get; set; }

        public string SeatType { get; set; } = string.Empty;

        // Navigation property
        public Screen Screen { get; set; } = null!;
    }
}
