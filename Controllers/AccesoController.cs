using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using UTNApiTalleres.Data.Repositorio;
using UTNApiTalleres.Data.Repositorio.Interfaz;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccesoController : ControllerBase
    {
        private readonly IAccesoDao _AccesoDao;

        public AccesoController(IAccesoDao dao)
        {
            _AccesoDao = dao;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var accesos = await _AccesoDao.GetAllAsync();
            return Ok(accesos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var acceso = await _AccesoDao.GetByIdAsync(id);
            return Ok(acceso);
        }

        [HttpGet("por-rol/{rolId}")]    
        public async Task<IActionResult> GetByRol(int rolId)
        {
            var accesos = await _AccesoDao.GetByRolAsync(rolId);
            return Ok(accesos);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Acceso acceso)
        {
            var id = await _AccesoDao.InsertAsync(acceso);
            return Ok(new { id });
        }

        [HttpPut ]
        public async Task<IActionResult> Update([FromBody] Acceso acceso)
        {
             
            await _AccesoDao.UpdateAsync(acceso);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _AccesoDao.DeleteAsync(id);
            return NoContent();
        }

        [HttpDelete("permiso/{id}")]
        public async Task<IActionResult> DeletePermiso(int id)
        {
            await _AccesoDao.DeletePermisoAsync(id);
            return NoContent();
        }

        
        // 🔗 Asignar accesos a un rol
        [HttpPost("asignar/{rolId}")]
        public async Task<IActionResult> AsignarAccesosARol(int rolId, [FromBody] List<int> accesosIds)
        {
            await _AccesoDao.SetAccesosPorRolAsync(rolId, accesosIds);
            return Ok();
        }

        [HttpGet("validarNombreAcceso/{nombre}")]
        public async Task<IActionResult> validarNombreAcceso(string nombre)
        {
            var accesos = await _AccesoDao.ExisteNombreAcceso(nombre);
            return Ok(accesos);
        }

        [HttpGet("validarRutaAcceso/{ruta}")]
        public async Task<IActionResult> validarRutaAcceso(string ruta)
        {
            var accesos = await _AccesoDao.ExisteRutaAcceso(ruta);
            return Ok(accesos);
        }
    }
}
