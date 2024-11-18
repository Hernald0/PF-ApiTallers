using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebApiTalleres.Models;
using Dapper;
using UTNApiTalleres.Data.Repositorio.Interfaz;
using System.Linq;
using System.Data;

namespace UTNApiTalleres.Data.Repositorio
{
    public class AseguradoraDao : IAseguradoraDao
    {

        private PostgresqlConfiguration _connectionString;


        public AseguradoraDao(PostgresqlConfiguration connectionString)
        {
            this._connectionString = connectionString;
        }

        protected NpgsqlConnection dbConnection()
        {
            return new NpgsqlConnection(this._connectionString.ConnectionString);
        }

        public async Task<bool> create(Aseguradora aseguradora)
        {
            
            var sql_insert = @"INSERT INTO public.""Aseguradoras"" (""Nombre"", 
                                                                    ""UsuarioAlta"",
                                                                    ""FechaAlta"")
                               VALUES ( @Nombre, @UsuarioAlta, @FechaAlta  )
                             ";
            
            var parameters = new DynamicParameters();
            
            parameters.Add("Nombre", aseguradora.Nombre, DbType.String);
            parameters.Add("UsuarioAlta", "HCELAYA", DbType.String);
            parameters.Add("FechaAlta", DateTime.Now, DbType.DateTime);

            using (var connection = dbConnection())
            {
                var result = await connection.ExecuteAsync(sql_insert, parameters);

                return result > 0;
            }

            

        }

        public async Task<bool> update(Aseguradora aseguradora)
        {
            var query = @"UPDATE ""Aseguradoras""                             
                          SET  
                            ""Nombre"" = @Nombre
                          WHERE ""Id"" = @Id";

            var parameters = new DynamicParameters();

            parameters.Add("Id", aseguradora.Id, DbType.Int32);
            parameters.Add("Nombre", aseguradora.Nombre, DbType.String);

            using (var connection = dbConnection())
            {
                var result = await connection.ExecuteAsync(query, parameters);

                return result > 0;
            }

        }

        public async Task<bool> delete(int id)
        {
            var db = dbConnection();

            var sql_script = @"
                                DELETE 
                                FROM  public.""Aseguradoras""
                                WHERE ""Id"" = @Id  
                            ";

            var result = await db.ExecuteAsync(sql_script, new { Id = id });

            return result > 0;
        }

        public async Task<Aseguradora> find(int id)
        {
            var db = dbConnection();

            var command = @" select ""Id"", 
                                    ""Nombre"",
                                    ""UsuarioAlta"",
                                    ""FechaAlta"",
                                    ""UsuarioBaja"",
                                    ""FechaBaja""
                             from public.""Aseguradoras""
                             where ""Id"" = @Id ";

            return await db.QueryFirstOrDefaultAsync<Aseguradora>(command, new {Id = id });
        }

        public async Task<IEnumerable<Aseguradora>> findAll()
        {

            try
            {
                var query = @" select ""Id"", 
                                    ""Nombre"",
                                    ""UsuarioAlta"",
                                    ""FechaAlta"",
                                    ""UsuarioBaja"",
                                    ""FechaBaja""
                             from public.""Aseguradoras""
                            ";
            using (var db = dbConnection())
            {
                var oAseguradoras = await db.QueryAsync<Aseguradora>(query);
                
                return oAseguradoras.ToList();

            }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error de conexión: {ex.Message}");
                throw;
            }


        }

      
    }
}
