using Dapper;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UTNApiTalleres.Data.Repositorio.Interfaz;
using UTNApiTalleres.Model;

namespace UTNApiTalleres.Data.Repositorio
{
    public class ConfiguracionDao : IConfiguracionDao
    {
        private PostgresqlConfiguration _connectionString;


        public ConfiguracionDao(PostgresqlConfiguration connectionString)
        {
            _connectionString = connectionString;
        }

        protected NpgsqlConnection dbConnection()
        {
            return new NpgsqlConnection(_connectionString.ConnectionString);
        }
        public async Task<object> getDatos(string nombreVistaPopUp)
        {
            try
            {
                var db = dbConnection();
                // Obtener detalles de columnas desde la tabla ConfiguracionPopup
                var config = db.QueryFirstOrDefault<vwConfigPopUp>(@"SELECT ""columnas"",  
                                                                         ""consultaSelect"",  
                                                                         ""clase""  
                                                                    FROM  public.""ConfigVistaPopUp""  
                                                                    WHERE ""nombrevista"" = @ViewName",
                                                                      new { ViewName = nombreVistaPopUp });

                if (config == null)
                {
                    // Manejar el caso en el que la vista no se encuentre en la tabla de configuración
                    return null;
                }

                // Ejecutar la consulta select obtenida
                var tableData = await db.QueryAsync(config.ConsultaSelect);

                // Deserializar el JSON almacenado en la columna 'columnas'
                var columnDetails = JsonConvert.DeserializeObject<object>(config.Columnas);

                //var selectDatos = JsonConvert.DeserializeObject<object>(tableData);
                var selectDatos = tableData.Select(row => JsonConvert.DeserializeObject<object>(JsonConvert.SerializeObject(row)));

                return new
                {
                    Columnas = JsonConvert.DeserializeObject<object>(config.Columnas),
                    ConsultaSelect = selectDatos.ToList(),
                    Clase = config.Clase
                };
            }

            catch (System.Exception ex)
            {
                //log error
                //return (IEnumerable<object>)StatusCode(500, ex.Message);
                return  ex.Data;
            }
            //return Ok(await _personaDao.findAll());
        }
    }


     
}
