using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using UTNApiTalleres.Data.Repositorio.Interfaz;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlgoController : Controller
    {  //ServiciosRepuestos
        private readonly IServRepDao _servRepDao;
        public AlgoController(IServRepDao ServRepDao)
        {
            _servRepDao = ServRepDao;
        }

        [HttpPost()]
        [Route("insServicio")]
        public async Task<IActionResult> postServicio(Servicio servicio)
        {
            try
            {
                var nuevoServicio = await _servRepDao.CreateServicio(servicio);
                return Ok(nuevoServicio);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPut()]
        [Route("updServicio")]
        public async Task<IActionResult> updateServicio(Servicio servicio)
        {
            try
            {
                var modificacionServicio = await _servRepDao.UpdateServicio(servicio);
                return Ok(modificacionServicio);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }

        }

        [HttpDelete()]
        [Route("delServicio/{id:int}")]
        public async Task<IActionResult> deleteServicio(int IdServicio)
        {
            try
            {
                var eliminadoServicio = await _servRepDao.DeleteServicio(IdServicio);
                return Ok(eliminadoServicio);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet()]
        [Route("findAllServicio")]
        public async Task<IActionResult> findAllServicio()
        {

            try
            {
                var oServicios = await _servRepDao.FindAllServicio();
                return Ok(oServicios);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }


        }

        [HttpGet()]
        [Route("findFilterServReps")]
        public async Task<IActionResult> findFilterServicioRepuesto([FromQuery] string pBusqueda,
                                                                    [FromQuery] string tipo)
        {

            if (string.IsNullOrWhiteSpace(pBusqueda) || pBusqueda.Length < 3)
            {
                return BadRequest("El parámetro de búsqueda debe contener al menos 3 caracteres.");
            }

            try
            {
                var oServRepsFiltrados = await _servRepDao.FindFilterServRep (pBusqueda, tipo);
                return Ok(oServRepsFiltrados);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }


        }


    


    }
}
