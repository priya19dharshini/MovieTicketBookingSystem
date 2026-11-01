using MovieTicketBookingSystem.Models;

namespace MovieTicketBookingSystem.Repositories.Interfaces;

public interface IScreenRepository
{
    Task<List<Screen>> GetAllAsync(int? theatreId);

    Task<Screen?> GetByIdAsync(int id);

    Task<List<Screen>> GetByTheatreIdAsync(int theatreId);

    Task AddAsync(Screen screen);

    Task UpdateAsync(Screen screen);

    Task DeleteAsync(Screen screen);

    Task<bool> ExistsByNameAsync(
        int theatreId,
        string screenName,
        int? excludeScreenId = null);
}