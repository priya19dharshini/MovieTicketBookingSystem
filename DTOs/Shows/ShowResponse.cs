namespace MovieTicketBookingSystem.DTOs.Shows;

public class ShowResponse
{
    public int ShowId { get; set; }

    public int MovieId { get; set; }

    public int ScreenId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public decimal TicketPrice { get; set; }
}