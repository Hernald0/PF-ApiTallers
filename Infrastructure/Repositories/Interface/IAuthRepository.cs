using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTNApiTalleres.Application.DTOs;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Infrastructure.Repositories.Interface
{
    public interface IAuthRepository
    {
        Usuario ValidarUsuario(string usuario, string password);
        string ObtenerRol(int usuarioId);
        Task<List<MenuGrupoDTO>> ObtenerAccesosAsync(int usuarioId);


    }

}
