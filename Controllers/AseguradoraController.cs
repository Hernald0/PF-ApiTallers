using Microsoft.AspNetCore.Mvc;
using System;
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

        [HttpGet("Init")]
        public async Task<IActionResult> Init()
        {
          
                
                return StatusCode(200, "Hola Mundo!");
           

        }

        [HttpGet]
        public async Task<IActionResult> GetAllAseguradora()
        {
            try
            {                
                var aseguradoras = await _aseguradoraDao.findAll();
               
                return Ok(aseguradoras);
            
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
          
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAseguradora(int id)
        {
            try
            {
                var aseguradora =  await _aseguradoraDao.find(id);
                if (aseguradora == null)
                    return NotFound();
                return Ok(aseguradora);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAseguradora([FromBody] Aseguradora aseguradora)
        {
            if (aseguradora == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var oCreated = await _aseguradoraDao.create(aseguradora);
            
            return Created("Se creo exitosamente.", oCreated);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAseguradora([FromBody] Aseguradora Aseguradora)
        {                     

            try
            {
                //validamos que exista la persona
                var aseguradora = await _aseguradoraDao.find(Aseguradora.Id);

                //en caso de no existir retornamos Not Found
                if (aseguradora == null)
                    return NotFound("La Aseguradora no existe.");

                //En caso de existir avanzamos con la actualización
                var actualizado = await _aseguradoraDao.update(Aseguradora);

                if (actualizado)
                    return Ok(new { response = "Actualización exitosa." });
                else
                    return NoContent();
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAseguradora(int id)
        {

            try
            {

                var aseguradora = await _aseguradoraDao.find(id);

                if (aseguradora == null)
                    return NotFound("Aseguradora no encontrada.");

                await _aseguradoraDao.delete(id);

                return NoContent();

            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
     
        }

    }
}
