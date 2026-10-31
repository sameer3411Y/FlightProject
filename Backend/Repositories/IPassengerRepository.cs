using Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Repositories
{
    public interface IPassengerRepository
    {
        Task<IEnumerable<Passenger>> GetAllAsync();
        Task<Passenger?> GetByIdAsync(int id);
        Task AddAsync(Passenger passenger);
        Task UpdateAsync(Passenger passenger);
        Task DeleteAsync(int id);
    }
}
