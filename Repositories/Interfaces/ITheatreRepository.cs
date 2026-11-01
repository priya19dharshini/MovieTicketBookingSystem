using MovieTicketBookingSystem.Models;

namespace MovieTicketBookingSystem.Repositories.Interfaces
{
    public interface ITheatreRepository
    {
        Task<List<Theatre>> GetAllAsync(int pageNumber, int pageSize);

        Task<Theatre?> GetByIdAsync(int id);

        Task AddAsync(Theatre theatre);

        Task UpdateAsync(Theatre theatre);

        Task DeleteAsync(Theatre theatre);
    }
}
