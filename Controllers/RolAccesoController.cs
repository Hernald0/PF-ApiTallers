using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UTNApiTalleres.Application.DTOs;
using UTNApiTalleres.Data.Repositorio.Interfaz;

namespace UTNApiTalleres.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class RolAccesoController : Controller
    {

        private readonly IRolDao _RolDao;

        public RolAccesoController(IRolDao dao)
        {
            _RolDao = dao;
        }
        /*
        [HttpPost]
        public async Task<IActionResult> CreateRolAcceso(AccesoRolDTO accesoRol)
        {
            var id = await _RolDao.InsertRolAccesoAsync(accesoRol);
            return Ok(new { id });
        }

        [HttpPost]
        public async Task<IActionResult> CreateRolAccesoPermiso(PermisoRolDTO permisoRol)
        {
            var id = await _RolDao.InsertRolAccesoPermisoAsync(permisoRol);
            return Ok(new { id });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRolAccesoAsync(AccesoRolDTO accesoRol)
        {
            var id = await _RolDao.deleteRolAccesoAsync(accesoRol);
            return Ok(new { id });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRolAccesoPermiso(PermisoRolDTO permisoRol)
        {
            var id = await _RolDao.deleteRolAccesoPermisoAsync(permisoRol);
            return Ok(new { id });
        }*/
    }
}
