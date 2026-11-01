namespace MovieTicketBookingSystem.Models;

public class Show
{
    public int ShowId { get; set; }

    public int MovieId { get; set; }

    public int ScreenId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public decimal TicketPrice { get; set; }

    // Navigation properties
    public Movie Movie { get; set; } = null!;

    public Screen Screen { get; set; } = null!;
}