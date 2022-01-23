using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTNApiTalleres.Data.Repositorio.Interfaz;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Data.Repositorio
{
    public class PaisDao : IPaisDao
    {

        private PostgresqlConfiguration _connectionString;


        public PaisDao(PostgresqlConfiguration connectionString)
        {
            this._connectionString = connectionString;
        }

        protected NpgsqlConnection dbConnection()
        {
            return new NpgsqlConnection(this._connectionString.ConnectionString);
        }

        public async Task<bool> create(Pais pais)
        {
            var sql_insert = @" INSERT INTO public.""Pais""(""Nombre"", ""CodigoArea"")
	                            VALUES ( @Nombre, @CodigoArea);
                             ";

            var parameters = new DynamicParameters();


            parameters.Add("Nombre", pais.Nombre, DbType.String);
            parameters.Add("CodigoArea", pais.CodigoArea, DbType.Int16);
            


            using (var connection = dbConnection())
            {
                var result = await connection.ExecuteAsync(sql_insert, parameters);

                return result > 0;
            }
        }

        public async Task<bool> update(Pais pais)
        {
            var query = @"UPDATE public.""Pais""                            
                          SET  
                            ""Nombre"" = @Nombre,
                            ""CodigoArea"" = @CodigoArea 
                          WHERE ""Id"" = @Id";

            var parameters = new DynamicParameters();

            parameters.Add("Id", pais.Id, DbType.Int32);
            parameters.Add("Nombre", pais.Nombre, DbType.String);
            parameters.Add("CodigoArea", pais.CodigoArea, DbType.Int16);

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
                                FROM  public.""Pais""
                                WHERE ""Id"" = @Id  
                            ";

            var result = await db.ExecuteAsync(sql_script, new { Id = id });

            return result > 0;
        }

        public async Task<Pais> find(int id)
        {
            var db = dbConnection();

            var command = @" select ""Id"", ""Nombre"", ""CodigoArea""
                             from public.""Pais""
                             where ""Id"" = @Id ";

            return await db.QueryFirstOrDefaultAsync<Pais>(command, new { Id = id });
        }

        public async Task<IEnumerable<Pais>> findAll()
        {
            var query = @"  select ""Id"", ""Nombre"", ""CodigoArea""
                             from public.""Pais""
                            ";
            using (var db = dbConnection())
            {
                var oPaises = await db.QueryAsync<Pais>(query);

                return oPaises.ToList();

            }
        }

       
    }
}
