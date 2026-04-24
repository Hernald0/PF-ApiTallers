using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using UTNApiTalleres.Application.Interfaces;
using UTNApiTalleres.Model;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Controllers
{
    

        [ApiController]
        [Route("api/[controller]")]
        public class ServiciosRepuestosController : Controller
        {
            private readonly IServRepService _servRepServicio;
            public ServiciosRepuestosController(IServRepService ServRepDao)
            {
                _servRepServicio = ServRepDao;
            }

            [HttpPost()]
            [Route("insServicio")]
            public async Task<IActionResult> postServicio(Servicio servicio)
            {
                try
                {
                    var nuevoServicio = await _servRepServicio.CrearServicio(servicio);
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
                    var modificacionServicio = await _servRepServicio.ActualizarServicio(servicio);
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
            public async Task<IActionResult> deleteServicio(int id)
            {
                if (id == null || id <= 0)
                    return BadRequest();
        
                try
                {
                    var eliminadoServicio = await _servRepServicio.EliminarServicio(id);
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
                    var oServicios = await _servRepServicio.GetAllServicios();
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
                    var oServRepsFiltrados = await _servRepServicio.RecuperarFiltradoServRep(pBusqueda, tipo);
                    return Ok(oServRepsFiltrados);
                }
                catch (Exception ex)
                {
                    //log error
                    return StatusCode(500, ex.Message);
                }


            }

        #region REPUESTOS

        [HttpGet()]
        [Route("validarNombreRepuesto/{nombreRepuesto}")]
        public async Task<IActionResult> validarNombreRepuesto(string nombreRepuesto)
        {

            try
            {
                var oRepuestos = await _servRepServicio.ExisteNombreRepuesto(nombreRepuesto);
                return Ok(oRepuestos);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }


        }

        [HttpGet()]
        [Route("validarNombreServicio/{nombreServicio}")]
        public async Task<IActionResult> validarNombreServicio(string nombreServicio)
        {

            try
            {
                var oServicios = await _servRepServicio.ExisteNombreServicio(nombreServicio);
                return Ok(oServicios);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }


        }

        [HttpGet()]
        [Route("findAllRepuestos")]
        public async Task<IActionResult> findAllRepuesto()
        {

            try
            {
                var oRepuestos = await _servRepServicio.GetAllRepuestos();
                return Ok(oRepuestos);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }


        }


        [HttpPost()]
        [Route("insRepuesto")]
        public async Task<IActionResult> postRepuesto(Repuesto repuesto)
        {
            try
            {
                var nuevoRepuesto = await _servRepServicio.CrearRepuesto(repuesto);
                return Ok(nuevoRepuesto);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPut()]
        [Route("updRepuesto")]
        public async Task<IActionResult> updateServicio(Repuesto repuesto)
        {
            try
            {
                var modificacionRepuesto = await _servRepServicio.ActualizarRepuesto(repuesto);
                return Ok(modificacionRepuesto);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }

        }

        [HttpDelete()]
        [Route("delRepuesto/{id:int}")]
        [Authorize]
        public async Task<IActionResult> deleteRepuesto(int id)
        {


            var user = User.Identity?.Name;
            var isAuth = User.Identity?.IsAuthenticated;
            try
            {
                if (id == null || id <= 0)
                    return BadRequest();

                var eliminadoRepuesto = await _servRepServicio.EliminarRepuesto(id);
                return Ok(eliminadoRepuesto);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }

        }

        #endregion

    }

}
   
