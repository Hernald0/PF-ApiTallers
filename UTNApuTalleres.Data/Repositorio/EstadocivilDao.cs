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
    public class EstadocivilDao : IEstadoCivilDao
    {

        private PostgresqlConfiguration _connectionString;


        public EstadocivilDao(PostgresqlConfiguration connectionString)
        {
            this._connectionString = connectionString;
        }

        protected NpgsqlConnection dbConnection()
        {
            return new NpgsqlConnection(this._connectionString.ConnectionString);
        }

        public async Task<bool> create(Estadocivil estadocivil)
        {
            var sql_insert = @" INSERT INTO public.""Estadociviles""(""Descripcion"", ""FechaAlta"", ""UsuarioAlta"")
	                            VALUES ( @Descripcion, @FechaAlta, @UsuarioAlta);
                             ";

            var parameters = new DynamicParameters();

            
            parameters.Add("Descripcion", estadocivil.Descripcion, DbType.String);
            parameters.Add("FechaAlta", DateTime.Now, DbType.DateTime);
            parameters.Add("UsuarioAlta", "HCELAYA", DbType.String);


            using (var connection = dbConnection())
            {
                var result = await connection.ExecuteAsync(sql_insert, parameters);

                return result > 0;
            }
        }

        public async Task<bool> update(Estadocivil estadocivil)
        {
            var query = @"UPDATE public.""Estadociviles""                            
                          SET  
                            ""Descripcion"" = @Descripcion
                          WHERE ""Id"" = @Id";

            var parameters = new DynamicParameters();

            parameters.Add("Id", estadocivil.Id, DbType.Int32);
            parameters.Add("Descripcion", estadocivil.Descripcion, DbType.String);

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
                                FROM  public.""Estadociviles""
                                WHERE ""Id"" = @Id  
                            ";

            var result = await db.ExecuteAsync(sql_script, new { Id = id });

            return result > 0;
        }

        public async Task<Estadocivil> find(int id)
        {
            var db = dbConnection();

            var command = @" select ""Id"", ""Descripcion"", ""FechaAlta"", ""UsuarioAlta"", ""FechaBaja"", ""UsuarioBaja""
                             from public.""Estadociviles""
                             where ""Id"" = @Id ";

            return await db.QueryFirstOrDefaultAsync<Estadocivil>(command, new { Id = id });
        }

        public async Task<IEnumerable<Estadocivil>> findAll()
        {
            var query = @" select ""Id"", ""Descripcion"", ""FechaAlta"", ""UsuarioAlta"", ""FechaBaja"", ""UsuarioBaja""
                             from public.""Estadociviles""
                            ";
            using (var db = dbConnection())
            {
                var oEstadosCiviles = await db.QueryAsync<Estadocivil>(query);

                return oEstadosCiviles.ToList();

            }
        }

       
    }
}
