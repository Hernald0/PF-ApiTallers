using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Data.Repositorio.Interfaz
{
    public interface IUsuarioDao
    {
         Task<IEnumerable<WebApiTalleres.Models.Usuario>> GetAllAsync();

        Task<WebApiTalleres.Models.Usuario> GetUsuario(int id);

        Task<int> InsertAsync(Usuario usuario);

        Task UpdateAsync(Usuario usuario);

        Task DeleteAsync(int id);
    }
}
