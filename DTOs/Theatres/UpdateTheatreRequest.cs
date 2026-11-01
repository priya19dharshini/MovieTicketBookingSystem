using System.ComponentModel.DataAnnotations;

namespace MovieTicketBookingSystem.DTOs.Theatres
{
    public class UpdateTheatreRequest
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;
    }
}
