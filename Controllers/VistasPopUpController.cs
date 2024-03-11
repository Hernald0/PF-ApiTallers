using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Npgsql;
using Dapper;
using System.Threading.Tasks;
using UTNApiTalleres.Data.Repositorio.Interfaz;
using System;

namespace UTNApiTalleres.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class VistasPopUpController : ControllerBase
    {


        private readonly IConfiguracionDao _configuracionDao;

        public VistasPopUpController(IConfiguracionDao configuracionDao)
        {
            _configuracionDao = configuracionDao;
        }

        [HttpGet("{vistaPopUp}")]
        public async Task<object> GetVistaPopUp(string vistaPopUp)
        {

            try
            {
                var datosVista = await _configuracionDao.getDatos(vistaPopUp);

                if (datosVista == null)
                    //var clase =  new { null };
                        return  new
                        {
                              mensaje = "No se encontraron datos para la consulta.",
                              columnas = new { },
                              consultaSelect = new object[] { },
                              clase = new { }
                        };
                else
                        return Ok(datosVista);


            }
            catch (Exception ex)
            {
                //log error
                //return StatusCode(500, ex.Message);
                // Loguea la excepción
                Console.WriteLine(ex.ToString());
                // Retornar un error HTTP 500
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}
