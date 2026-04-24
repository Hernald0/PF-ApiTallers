using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UTNApiTalleres.Data.Repositorio;
using UTNApiTalleres.Data.Repositorio.Interfaz;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Controllers
{
   
        [ApiController]
        [Route("api/[controller]")]
        public class UsuariosController : ControllerBase
        {
            private readonly IUsuarioDao _UsuarioDao;
            public UsuariosController(IUsuarioDao dao) => _UsuarioDao = dao;

            [HttpGet]
            public async Task<IActionResult> GetAll() => Ok(await _UsuarioDao.GetAllAsync());

            [HttpGet("{id}")]
            public async Task<IActionResult> GetPersona(int id)
            {
                //return Ok( _personaDao.find(id));
                try
                {
                    var persona = await _UsuarioDao.GetUsuario(id);
                    if (persona == null)
                        return NotFound();
                    return Ok(persona);
                }
                catch (System.Exception ex)
                {
                    //log error
                    return StatusCode(500, ex.Message);
                }
            }

        [HttpPost]
            public async Task<IActionResult> Create(Usuario usuario)
            {
                var id = await _UsuarioDao.InsertAsync(usuario);
                return Ok(new { id });
            }

            [HttpPut("{id}")]
            public async Task<IActionResult> Update(int id, Usuario usuario)
            {
                usuario.Id = id;
                await _UsuarioDao.UpdateAsync(usuario);
                return NoContent();
            }

            [HttpDelete("{id}")]
            public async Task<IActionResult> Delete(int id)
            {
                await _UsuarioDao.DeleteAsync(id);
                return NoContent();
            }


            [HttpGet("validarNombreUsuario/{nombre}")]
            public async Task<IActionResult> validarNombreUsuario(string nombre)
            {
                var accesos = await _UsuarioDao.ExisteNombreUsuario(nombre);
                return Ok(accesos);
            }

            [HttpGet("validarEmailUsuario/{ruta}")]
            public async Task<IActionResult> validarEmailUsuario(string ruta)
            {
                var accesos = await _UsuarioDao.ExisteEmailUsuario(ruta);
                return Ok(accesos);
            }
    }
     
}
