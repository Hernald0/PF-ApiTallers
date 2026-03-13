
using System.Threading.Tasks;
using UTNApiTalleres.Application.DTOs;

namespace UTNApiTalleres.Infrastructure.Repositories.Interface
{
    public interface IRolAccesoPermisoRepository
    {
        Task<int> AddRolAccesoPermisoAsync(int rolId, int accesoId, int permisoId);

        Task<int> deleteRolAccesoPermisoAsync(int rolId, int accesoId, int permisoId);

        Task<bool> ExistsAsync(int rolId, int accesoId, int permisoId);
    }
}
