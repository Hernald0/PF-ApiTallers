using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTNApiTalleres.Data.Repositorio.Interfaz;
using UTNApiTalleres.Model;

namespace UTNApiTalleres.Data.Repositorio
{
    public class ServRepDao : IServRepDao
    {

        private PostgresqlConfiguration _connectionString;
        private readonly IPersonaDao _personaDao;

        public ServRepDao(PostgresqlConfiguration connectionString)
        {
            this._connectionString = connectionString;
        }

        protected NpgsqlConnection dbConnection()
        {
            return new NpgsqlConnection(this._connectionString.ConnectionString);
        }

        public async Task<Servicio> CreateServicio(Servicio servicio)
        {
            var sql_insert = @"INSERT INTO public.""Servicios"" (""Nombre"",
                                                                 ""Descripcion"",                                                                 
                                                                 ""UsuarioAlta"",
                                                                 ""FechaAlta"")
                               VALUES ( @Nombre, @Descripcion, @UsuarioAlta, @FechaAlta  )
                             returning *;";
            /*
            var parameters = new DynamicParameters();

            parameters.Add("Nombre", servicio.Nombre, DbType.String);
            parameters.Add("Descripcion", servicio.Descripcion, DbType.String);
            parameters.Add("UsuarioAlta", "HCELAYA", DbType.String);
            parameters.Add("FechaAlta", DateTime.Now, DbType.DateTime);
            */
            using (var db = dbConnection())
            {
                //var result = await connection.ExecuteAsync(sql_insert, parameters);

                //return result > 0;
                var newServicio = await db.QuerySingleAsync<Servicio>(
                                            sql_insert,
                                            new
                                            {
                                                Nombre = servicio.Nombre,
                                                Descripcion = servicio.Descripcion,
                                                FechaAlta = DateTime.Now,
                                                UsuarioAlta = servicio.UsuarioAlta
                                            });

                return newServicio;
            }
        }

       
     
     

        public Task<Servicio> FindServicio(int IdServicio)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Servicio>> FindAllServicio()
        {

            var sql_query = @"
                            SELECT ""Id"", 
                                    ""Nombre"",
                                    ""Descripcion"",
                                    ""UsuarioAlta"",
                                    ""FechaAlta"",
                                    ""UsuarioBaja"",
                                    ""FechaBaja""
                            FROM public.""Servicios"" 
                           ";

            using (var db = dbConnection())
            {
                var oServicio = await db.QueryAsync<Servicio>(sql_query);

                return oServicio.ToList();

            }
        }

        public async Task<int> DeleteServicio(int IdServicio)
        {
            int affectedRows = 0;
            var db = dbConnection();

            {

                var sql_delete = @"
                delete from public.""Servicios"" as Servicio
                 WHERE Servicio.""Id"" = @IdServicio
                     ;";


                affectedRows = await db.ExecuteAsync(
                                                           sql_delete,
                                                           new
                                                           {
                                                               IdServicio = IdServicio
                                                           }
                                                     );


            }

            return affectedRows;
        }

        public async Task<Servicio> UpdateServicio(Servicio servicio)
        {
            var db = dbConnection();


            var sql_update = @"
                update public.""Servicios"" as Servicios
                    set ""Nombre"" = @NombreServicio,
                        ""Descripcion"" = @DescripcionServicio      
                    WHERE Servicios.""Id"" = @IdServicio
                     returning  Servicios.""Id"" as id,
                                Servicios.""Nombre"" as nombre,
                                Servicios.""FechaAlta"" as fechaAlta,
                                Servicios.""UsuarioAlta"" as usuarioAlta,
                                Servicios.""FechaBaja"" as fechaBaja,
                                Servicios.""UsuarioBaja"" as usuarioBaja;";


            var updatedServicio = await db.QuerySingleAsync<Servicio>(
                                         sql_update,
                                         new
                                         {
                                             IdServicio = servicio.Id,
                                             NombreServicio = servicio.Nombre,
                                             DescripcionServicio = servicio.Descripcion
                                         });



            return updatedServicio;
        }
        
        public async Task<IEnumerable<ItemVentaDTO>> FindFilterServRep(string pBusqueda)
        {

            var db = dbConnection();

            const string sql = @"
                select * from (
                                SELECT ""Id"", ""Nombre"" as nombre, 'servicio' as tipo, ""Descripcion"", ""precioCosto"", ""precioVenta"", ""Duracion_aproximada"", ""Tipo"" as clase, null as stock
	                                FROM public.""Servicios""
                                union
                                SELECT ""Id"", ""Nombre"", 'repuesto' as tipo, ""Descripcion"", ""precioCosto"", ""precioVenta"", null as ""Duracion_aproximada"", null as ""Tipo"", ""Stock""
	                                FROM public.""Repuestos"") as tabla
                WHERE lower(nombre) LIKE '%' || lower(@Parametro) || '%';";

             
              
            var result = await db.QueryAsync<ItemVentaDTO>(sql, new { Parametro = pBusqueda });
               
            return  result.ToList();
             

        }


    }
}
