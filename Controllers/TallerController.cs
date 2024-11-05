using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UTNApiTalleres.Data.Repositorio.Interfaz;
using UTNApiTalleres.Model;
using WebApiTalleres.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UTNApiTalleres.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TallerController : ControllerBase
    {
        private readonly ITallerDao _tallerDao;


        public TallerController(ITallerDao TallerDao)
        {
            _tallerDao = TallerDao;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTaller()
        {
            try
            {
                var talleres = await _tallerDao.findAll();
                return Ok(talleres);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }

        }


        // GET api/<TallerController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaller(int id)
        {
            //return Ok( _personaDao.find(id));
            try
            {
                var taller = await _tallerDao.find(id);
                if (taller == null)
                    return NotFound();
                return Ok(taller);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTaller([FromBody] Taller Taller)
        {
            if (Taller == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var oCreated = await _tallerDao.create(Taller);

            return Created("Se creo exitosamente.", oCreated);
        }

        // PUT api/<TallerController>/5
        [HttpPut]
        public async Task<IActionResult> UpdateTaller([FromBody] Taller Taller)
        {

            try
            {
                //validamos que exista la persona
                var taller = await _tallerDao.find(Taller.Id);

                //en caso de no existir retornamos Not Found
                if (taller == null)
                    return NotFound("El taller no existe.");

                //En caso de existir avanzamos con la actualización
                var regActualizados = await _tallerDao.update(Taller);

                if (regActualizados)
                    return Content("Actualización exitosa");
                else
                    return NoContent();
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }

        }

        // DELETE api/<TallerController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaller(int id)
        {
            try
            {

                var persona = await _tallerDao.find(id);

                if (persona == null)
                    return NotFound("Taller no encontrado.");

                await _tallerDao.delete(id);

                return NoContent();

            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet()]
        [Route("empleados/{id:int}")]
        public async Task<IActionResult> findEmpleadoAll(int id)
        {

            try
            {
                var empleados = await _tallerDao.findEmpleadoAll(id);
                return Ok(empleados);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }


        }

        [HttpGet()]
        [Route("Vehiculo")]
        public async Task<IActionResult> getVehiculo(int idVehiculo)
        {

            try
            {
                var oVehiculo = await _tallerDao.getVehiculo(idVehiculo);
                return Ok(oVehiculo);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }


        }

        [HttpPost()]
        [Route("modelovehiculo")]
        public async Task<IActionResult> postMarcaModelo(mvvmModelovehiculo mvvmModelovehiculo)
        {
            try
            {
                var nuevoModeloVehiculo = await _tallerDao.createMarcaModelo(mvvmModelovehiculo);
                return Ok(nuevoModeloVehiculo);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPut()]
        [Route("empleados")]
        public async Task<IActionResult> putEmpleado(Empleado empleado)
        {

            try
            {
                var empleados = await _tallerDao.createEmpleado(empleado);
                return Ok(empleados);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }

        }
        /*
        [HttpDelete()]
        [Route("empleados")]
        public async Task<IActionResult> deleteEmpleado(Empleado empleado)
        {

            try
            {
                var empleados = await _tallerDao.deleteEmpleado(empleado);
                return Ok(empleados);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }


        }*/

        [HttpGet()]
        [Route("clientes/{id:int}")]
        public async Task<IActionResult> findClienteAll(int id)
        {

            try
            {
                var clientes = await _tallerDao.findClienteAll(id);
                return Ok(clientes);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }


        }


        [HttpGet()]
        [Route("marcaVehiculo")]
        public async Task<IActionResult> findMarcaVehiculoAll()
        {

            try
            {
                var oMarcas = await _tallerDao.findMarcaVehiculoAll();
                return Ok(oMarcas);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }


        }



        [HttpGet()]
        [Route("marcaModelosVehiculo/{id:int}")]
        public async Task<IActionResult> findModeloVehiculo(int id)
        {

            try
            {
                var oMarcaModelo = await _tallerDao.findMarcaModeloVehiculo(id);
                return Ok(oMarcaModelo);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }


        }

        [HttpGet()]
        [Route("modelosVehiculo")]
        public async Task<IActionResult> findModeloVehiculoAll()
        {

            try
            {
                var oMarcaModelo = await _tallerDao.findMarcaModeloVehiculoAll();
                return Ok(oMarcaModelo);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }


        }

        [HttpPut()]
        [Route("updModeloVehiculo")]
        public async Task<IActionResult> updateModeloVehiculo(mvvmModelovehiculo modelo)
        {

            try
            {
                var oMarcaModelo = await _tallerDao.updateMarcaModelo(modelo);
                return Ok(oMarcaModelo);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }


        }

        [HttpDelete()]
        [Route("delModeloVehiculo/{id:int}")]
        public async Task<IActionResult> deleteModeloVehiculo(int id) //(mvvmModelovehiculo modelo)
        {

            try
            {
                var oMarcaModelo = await _tallerDao.deleteMarcaModelo(id);
                return Ok(oMarcaModelo);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }


        }


       

        
    }
}
