using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTNApiTalleres.Model;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Data.Repositorio.Interfaz
{
    public interface IAccesoDao
    {
        Task<IEnumerable<Acceso>> GetAllAsync();

        Task<int> InsertAsync(Acceso acceso);

        Task UpdateAsync(Acceso acceso);

        Task DeleteAsync(int id);
        Task DeletePermisoAsync(int id);

        Task<IEnumerable<Acceso>> GetByRolAsync(int rolId);

        Task SetAccesosPorRolAsync(int rolId, List<int> accesosIds);
        Task<Acceso>  GetByIdAsync(int id);

 

        
    }
}
