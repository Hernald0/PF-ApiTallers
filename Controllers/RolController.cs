using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using UTNApiTalleres.Application.DTOs;
using UTNApiTalleres.Application.Interfaces;
using UTNApiTalleres.Data.Repositorio;
using UTNApiTalleres.Data.Repositorio.Interfaz;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolController : ControllerBase
    {

        private readonly IRolService _rolService;

        public RolController(IRolService rolService)
        {
            _rolService = rolService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _rolService.GetAllAsync());


        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdl(int id)
        {

            var rol = await _rolService.GetByIdAsync(id);
            return rol == null ? NotFound() : Ok(rol);

        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRolDto dto)
        {
            var id = await _rolService.CreateAsync(dto);
            return CreatedAtAction(nameof(Create), new { id }, null);
        }

        [HttpPut]
        public async Task<IActionResult> Update(Rol rol)
        {

            await _rolService.UpdateAsync(rol);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _rolService.DeleteAsync(id);
            return NoContent();
        }
        // -----------------------------
        // Relaciones
        // -----------------------------

        [HttpPost("{rolId:int}/accesos/{accesoId:int}")]
        public async Task<IActionResult> CreateRolAcceso(int rolId, int accesoId)
        {
          
            try
            {
                var result = await _rolService.AssignAccesoAsync(rolId, accesoId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno del servidor");
            }

        }

        [HttpPost("insertRolPermiso")]
        public async Task<IActionResult> CreateRolAcceso(PermisoRolDTO rolPermiso)
        {

            var result = await _rolService.AssignPermisoAsync(rolPermiso.RolId, rolPermiso.AccesoId, rolPermiso.PermisoId);

            if (result == null)
                return Ok("Error en el insert del Rol-Acceso");

            return Ok(result);

        }

        [HttpDelete("{rolId}/accesos/{accesoId}")]
        public async Task<IActionResult> DeleteRolAcceso(int rolId, int accesoId) 
        {

            var result = await _rolService.RemoveRolAccesoAsync(rolId, accesoId);

            if (result == null)
                return Ok("Error en el delete del Rol-Acceso");

            return Ok(result);

        }

        [HttpDelete("{rolId}/accesos/{accesoId}/permisos/{permisoId}")]
        public   IActionResult DeleteRolAcceso( [FromRoute] int rolId,
                                                [FromRoute] int accesoId,
                                                [FromRoute] int permisoId) 
         
        {

            var result =   _rolService.RemovePermisoAsync(rolId,
                                                              accesoId,
                                                              permisoId);

            if (result == null)
                return Ok("Error en el delete del Rol-Acceso");

            return Ok(result);

        }
        /*private readonly RolDao _RolDao;
      

        public RolController( IRolDao RolDao )
        {
            _RolDao = (RolDao)RolDao;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _RolDao.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdl(int id)
        {
            
            return Ok(await _RolDao.GetByIdAsync(id));

        }

        [HttpPost]
        public async Task<IActionResult> Create(Rol rol)
        {
            var id = await _RolDao.InsertAsync(rol);
            return Ok(new { id });
        }

        [HttpPut]
        public async Task<IActionResult> Update( Rol rol)
        {
           
            await _RolDao.UpdateAsync(rol);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _RolDao.DeleteAsync(id);
            return NoContent();
        }*/
    }
}
