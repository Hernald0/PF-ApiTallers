using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UTNApiTalleres.Application.DTOs;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Infrastructure.Repositories.Interface
{
    public interface IRolRepository
    {
        Task<IEnumerable<Rol>> GetAllAsync();

        Task<Rol> GetByIdAsync(int id);

        Task<int> AddAsync(Rol rol);

        Task UpdateAsync(Rol rol);

        Task DeleteAsync(int id);

        Task<bool> ExistsAsync(int id);

        Task<bool> existeNombreAsync(string nombre);


    }
}
