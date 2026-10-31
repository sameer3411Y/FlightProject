using Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Repositories
{
    public interface IFlightRepository
    {
        Task<IEnumerable<Flight>> GetAllAsync();
        Task<Flight?> GetByIdAsync(int id);
        Task<IEnumerable<Flight>> SearchAsync(string origin, string destination, DateTime date);
        Task AddAsync(Flight flight);
        Task UpdateAsync(Flight flight);
        Task DeleteAsync(int id);
    }
}
