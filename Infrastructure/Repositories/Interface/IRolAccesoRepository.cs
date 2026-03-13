 
using System.Threading.Tasks;
using UTNApiTalleres.Application.DTOs;
 

namespace UTNApiTalleres.Infrastructure.Repositories.Interface
{
    public interface IRolAccesoRepository
    {
        Task<int> AddRolAccesoAsync(int rolId, int accesoId);
        Task<int>  DeleteRolAccesoAsync(int rolId, int accesoId);

        Task<bool> ExistsAsync(int rolId, int accesoId);
    }
}
