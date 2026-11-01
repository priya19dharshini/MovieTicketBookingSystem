using System.ComponentModel.DataAnnotations;

namespace MovieTicketBookingSystem.DTOs.Screens
{
    public class CreateScreenRequest
    {
        [Range(1, int.MaxValue)]
        public int TheatreId { get; set; }

        [Required]
        [StringLength(50)]
        public string ScreenName { get; set; } = string.Empty;
    }
}
