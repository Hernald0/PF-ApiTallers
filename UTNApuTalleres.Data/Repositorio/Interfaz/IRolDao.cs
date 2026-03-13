using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiTalleres.Models;
 

namespace UTNApiTalleres.Data.Repositorio.Interfaz
{
    public interface IRolDao
    {
        Task<IEnumerable<Rol>> GetAllAsync();

        Task<int> InsertAsync(Rol rol);

        Task UpdateAsync(Rol rol);

        Task DeleteAsync(int id);

      

    }
}
