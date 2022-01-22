using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UTNApiTalleres.Data.Repositorio.Interfaz;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AseguradoraController : ControllerBase
    {

        private readonly IAseguradoraDao _aseguradoraDao;

        public AseguradoraController(IAseguradoraDao aseguradoraDao)
        {
            _aseguradoraDao = aseguradoraDao;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAseguradora()
        {
            return Ok(await _aseguradoraDao.findAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAseguradora(int id)
        {
            return Ok(await _aseguradoraDao.find(id));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAseguradora([FromBody] Aseguradora aseguradora)
        {
            if (aseguradora == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var oCreated = await _aseguradoraDao.create(aseguradora);
            
            return Created("created", oCreated);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAseguradora(int id)
        {
        
             await _aseguradoraDao.delete(id);

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAseguradora(int id)
        {
         

            await _aseguradoraDao.delete(id);

            return NoContent();
        }

    }
}
