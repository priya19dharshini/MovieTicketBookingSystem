using System.ComponentModel.DataAnnotations;

namespace MovieTicketBookingSystem.DTOs.Movies;

public class CreateMovieRequest : IValidatableObject
{
    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Range(1, int.MaxValue)]
    public int DurationMinutes { get; set; }

    public DateOnly ReleaseDate { get; set; }

    [Required]
    [StringLength(50)]
    public string Language { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Genre { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (ReleaseDate == default)
        {
            yield return new ValidationResult(
                "Release date is required.",
                new[] { nameof(ReleaseDate) });
        }
    }
}