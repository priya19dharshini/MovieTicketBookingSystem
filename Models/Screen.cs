namespace MovieTicketBookingSystem.Models
{
    public class Screen
    {
        public int ScreenId { get; set; }

        public int TheatreId { get; set; }

        public string ScreenName { get; set; } = string.Empty;

        // Navigation property
        public Theatre Theatre { get; set; } = null!;
    }
}
