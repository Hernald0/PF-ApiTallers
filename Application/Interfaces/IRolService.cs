using System.Collections.Generic;
using System.Threading.Tasks;
using UTNApiTalleres.Application.DTOs;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Application.Interfaces
{
    public interface IRolService
    {
        Task<IEnumerable<Rol>> GetAllAsync();
        Task<Rol?> GetByIdAsync(int id);

        Task<int> CreateAsync(CreateRolDto dto);
        Task UpdateAsync(Rol rol);
        Task DeleteAsync(int id);

        Task<int> AssignAccesoAsync(int rolId, int accesoId);
        Task<int> RemoveRolAccesoAsync(int rolId, int accesoId);

        Task<int> AssignPermisoAsync(int rolId, int accesoId, int permisoId);
        Task<int> RemovePermisoAsync(int rolId, int accesoId, int permisoId);
    }
}
