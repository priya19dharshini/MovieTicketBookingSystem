
using Microsoft.EntityFrameworkCore;
using MovieTicketBookingSystem.Models;

namespace MovieTicketBookingSystem.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Movie> Movies { get; set; }
    public DbSet<Theatre> Theatres { get; set; }

    public DbSet<Screen> Screens { get; set; }

    public DbSet<Seat> Seats { get; set; }

    public DbSet<Show> Shows { get; set; }

    public DbSet<ShowSeat> ShowSeats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(m => m.MovieId);

            entity.Property(m => m.Title)
                  .HasMaxLength(150)
                  .IsRequired();

            entity.Property(m => m.Description)
                  .HasMaxLength(500);

            entity.Property(m => m.DurationMinutes)
                  .IsRequired();

            entity.ToTable(t =>
                t.HasCheckConstraint(
                    "CK_Movies_DurationMinutes",
                    "[DurationMinutes] > 0"));

            entity.Property(m => m.ReleaseDate)
                  .HasColumnType("date")
                  .IsRequired();

            entity.Property(m => m.Language)
                  .HasMaxLength(50)
                  .IsRequired();

            entity.Property(m => m.Genre)
                  .HasMaxLength(50)
                  .IsRequired();
        });

        modelBuilder.Entity<Theatre>(entity =>
        {
            entity.HasKey(t => t.TheatreId);

            entity.Property(t => t.Name)
                  .HasMaxLength(150)
                  .IsRequired();

            entity.Property(t => t.Location)
                  .HasMaxLength(200)
                  .IsRequired();
        });

        modelBuilder.Entity<Screen>(entity =>
        {
            entity.HasKey(s => s.ScreenId);

            entity.Property(s => s.ScreenName)
                .HasMaxLength(50)
                .IsRequired();

            entity.HasOne(s => s.Theatre)
                .WithMany()
                .HasForeignKey(s => s.TheatreId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(s => new { s.TheatreId, s.ScreenName })
                .IsUnique();
        });

        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasKey(s => s.SeatId);

            entity.Property(s => s.RowName)
                .HasMaxLength(5)
                .IsRequired();

            entity.Property(s => s.SeatNumber)
                .IsRequired();

            entity.Property(s => s.SeatType)
                .HasMaxLength(30)
                .IsRequired();

            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Seats_SeatNumber",
                "[SeatNumber] > 0"));

            entity.HasOne(s => s.Screen)
                .WithMany()
                .HasForeignKey(s => s.ScreenId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(s => new
            {
                s.ScreenId,
                s.RowName,
                s.SeatNumber
            }).IsUnique();
        });

        modelBuilder.Entity<Show>(entity =>
        {
            entity.HasKey(s => s.ShowId);

            entity.Property(s => s.StartTime)
                .HasColumnType("datetime2")
                .IsRequired();

            entity.Property(s => s.EndTime)
                .HasColumnType("datetime2")
                .IsRequired();

            entity.Property(s => s.TicketPrice)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            entity.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_Shows_EndTime",
                    "[EndTime] > [StartTime]");

                t.HasCheckConstraint(
                    "CK_Shows_TicketPrice",
                    "[TicketPrice] >= 0");
            });

            entity.HasOne(s => s.Movie)
                .WithMany()
                .HasForeignKey(s => s.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(s => s.Screen)
                .WithMany()
                .HasForeignKey(s => s.ScreenId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ShowSeat>(entity =>
        {
            entity.HasKey(ss => ss.ShowSeatId);

            entity.Property(ss => ss.Status)
                .HasMaxLength(20)
                .IsRequired();

            entity.HasOne(ss => ss.Show)
                .WithMany()
                .HasForeignKey(ss => ss.ShowId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(ss => ss.Seat)
                .WithMany()
                .HasForeignKey(ss => ss.SeatId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(ss => new
            {
                ss.ShowId,
                ss.SeatId
            }).IsUnique();
        });
    }
}
