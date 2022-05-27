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
    public class TipoidentificadorDao : ITipoidentificadorDao
    {

        private PostgresqlConfiguration _connectionString;


        public TipoidentificadorDao(PostgresqlConfiguration connectionString)
        {
            this._connectionString = connectionString;
        }

        protected NpgsqlConnection dbConnection()
        {
            return new NpgsqlConnection(this._connectionString.ConnectionString);
        }

        public async Task<bool> create(TipoIdentificador tipoidentificador)
        {
            var sql_insert = @" INSERT INTO public.""Tipoidentificadores""(
	                        ""Identificador"", ""DescripcionIdentificador"", ""FechaAlta"", ""UsuarioAlta"")
	                        VALUES (@Identificador, @DescripcionIdentificador, @FechaAlta, @UsuarioAlta);
                             ";

            var parameters = new DynamicParameters();


            parameters.Add("Identificador", tipoidentificador.Identificador, DbType.String);
            parameters.Add("DescripcionIdentificador", tipoidentificador.DescripcionIdentificador, DbType.String);
            parameters.Add("FechaAlta", DateTime.Now, DbType.DateTime);
            parameters.Add("UsuarioAlta", "HCELAYA", DbType.String);

            using (var connection = dbConnection())
            {
                var result = await connection.ExecuteAsync(sql_insert, parameters);

                return result > 0;
            }
        }

        public async Task<bool> update(TipoIdentificador tipoidentificador)
        {
            var query = @"UPDATE public.""Tipoidentificadores""                            
                          SET  
                            ""Identificador"" = @Identificador,
                            ""DescripcionIdentificador"" = @DescripcionIdentificador
                          WHERE ""Id"" = @Id";

            var parameters = new DynamicParameters();

            parameters.Add("Id", tipoidentificador.Id, DbType.Int32);
            parameters.Add("Identificador", tipoidentificador.Identificador, DbType.String);
            parameters.Add("DescripcionIdentificador", tipoidentificador.DescripcionIdentificador, DbType.String);

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
                                FROM  public.""Tipoidentificadores""
                                WHERE ""Id"" = @Id  
                            ";

            var result = await db.ExecuteAsync(sql_script, new { Id = id });

            return result > 0;
        }

        public async Task<TipoIdentificador> find(int id)
        {
            var db = dbConnection();

            var command = @" select ""Id"", ""Identificador"", ""DescripcionIdentificador"", ""FechaAlta"", ""UsuarioAlta"", ""FechaBaja"", ""UsuarioBaja""
                             from public.""Tipoidentificadores""
                             where ""Id"" = @Id ";

            return await db.QueryFirstOrDefaultAsync<TipoIdentificador>(command, new { Id = id });
        }

        public async Task<IEnumerable<TipoIdentificador>> findAll()
        {
            var query = @"  select ""Id"", ""Identificador"", ""DescripcionIdentificador"", ""FechaAlta"", ""UsuarioAlta"", ""FechaBaja"", ""UsuarioBaja""
                             from public.""Tipoidentificadores""
                            ";
            using (var db = dbConnection())
            {
                var oPaises = await db.QueryAsync<TipoIdentificador>(query);

                return oPaises.ToList();

            }
        }

      
    }
}
