using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;
using System.Threading.Tasks;
using UTNApiTalleres.Data.Repositorio.Interfaz;
using UTNApiTalleres.Model;
using Dapper;
using System.Linq;
using System.Data;

namespace UTNApiTalleres.Data.Repositorio
{
    public class ServicioDao : IServicioDao
    {

        private PostgresqlConfiguration _connectionString;

        public ServicioDao(PostgresqlConfiguration connectionString)
        {
            this._connectionString = connectionString;
        }

        protected NpgsqlConnection dbConnection()
        {
            return new NpgsqlConnection(this._connectionString.ConnectionString);
        }

        public async Task<bool> create(Servicio servicio)
        {
            var sql_insert = @"INSERT INTO public.""Servicios"" (""Nombre"",
                                                                 ""Descripcion"",                                                                 
                                                                    ""UsuarioAlta"",
                                                                    ""FechaAlta"")
                               VALUES ( @Nombre, @UsuarioAlta, @FechaAlta  )
                             ";

            var parameters = new DynamicParameters();

            parameters.Add("Nombre", servicio.Nombre, DbType.String);
            parameters.Add("Descripción", servicio.Nombre, DbType.String);
            parameters.Add("UsuarioAlta", "HCELAYA", DbType.String);
            parameters.Add("FechaAlta", DateTime.Now, DbType.DateTime);

            using (var connection = dbConnection())
            {
                var result = await connection.ExecuteAsync(sql_insert, parameters);

                return result > 0;
            }
        }

        public Task<bool> delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Servicio> find(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Servicio>> findAll()
        {

            var query = @" select   ""Id"", 
                                    ""Nombre"",
                                    ""Descripcion"",
                                    ""UsuarioAlta"",
                                    ""FechaAlta"",
                                    ""UsuarioBaja"",
                                    ""FechaBaja""
                             from public.""Servicios""
                            ";
            using (var db = dbConnection())
            {
                var oServicios = await db.QueryAsync<Servicio>(query);

                return oServicios.ToList();

            }


        }

        public Task<bool> update(Servicio Servicio)
        {
            throw new NotImplementedException();
        }
    }
}
