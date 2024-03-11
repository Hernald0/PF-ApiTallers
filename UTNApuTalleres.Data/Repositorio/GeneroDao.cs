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
    public class GeneroDao : IGeneroDao
    {

        private PostgresqlConfiguration _connectionString;


        public GeneroDao(PostgresqlConfiguration connectionString)
        {
            this._connectionString = connectionString;
        }

        protected NpgsqlConnection dbConnection()
        {
            return new NpgsqlConnection(this._connectionString.ConnectionString);
        }

        public async Task<bool> create(Genero genero)
        {

            try
            {
                var sql_insert = @"INSERT INTO public.""Generos"" (""Descripcion"",  ""FechaAlta"", ""UsuarioAlta"", ""FechaBaja"", ""UsuarioBaja"")
                               VALUES ( @Descripcion, @FechaAlta, @UsuarioAlta, @FechaBaja, @UsuarioBaja)
                             ";

                var parameters = new DynamicParameters();

                parameters.Add("Descripcion", genero.Descripcion, DbType.String);
                parameters.Add("FechaAlta", DateTime.Now, DbType.DateTime);
                parameters.Add("UsuarioAlta", "HCELAYA", DbType.String);
                parameters.Add("FechaBaja", genero.FechaBaja, DbType.DateTime);
                parameters.Add("UsuarioBaja", genero.UsuarioBaja, DbType.String);

                using (var connection = dbConnection())
                {
                    var result = await connection.ExecuteAsync(sql_insert, parameters);

                    return result > 0;
                }

            }
            catch (Exception ex)
            {
                //log error
                
                return false;
            }

            
        }

        public async Task<bool> update(Genero genero)
        {

            var query = @"UPDATE ""Generos""                             
                          SET                              
                            ""Descripcion""=@Descripcion, 
                            ""FechaAlta""=@FechaAlta, 
                            ""UsuarioAlta""=@UsuarioAlta, 
                            ""FechaBaja""=@FechaBaja, 
                            ""UsuarioBaja""=@UsuarioBaja
                          WHERE ""Id"" = @Id";

            var parameters = new DynamicParameters();

            parameters.Add("Id", genero.Id, DbType.Int32);
            parameters.Add("Descripcion", genero.Descripcion, DbType.String);
            parameters.Add("FechaAlta", DateTime.Now, DbType.DateTime);
            parameters.Add("UsuarioAlta", "HCELAYA", DbType.String);
            parameters.Add("FechaBaja", genero.FechaBaja, DbType.DateTime);
            parameters.Add("UsuarioBaja", genero.UsuarioBaja, DbType.String);


            using (var connection = dbConnection())
            {
                var result = await connection.ExecuteAsync(query, parameters);

                return result > 0;
            }
        }

        public async Task<bool> delete(int id)
        {
            var db = dbConnection();

            try
            { 
            var sql_script = @"
                                DELETE 
                                FROM  public.""Generos""
                                WHERE ""Id"" = @Id  
                            ";

            var result = await db.ExecuteAsync(sql_script, new { Id = id });

            return result > 0;
            }
            catch (PostgresException e)
            {

                var parameters = new DynamicParameters();

                parameters.Add("Id", id, DbType.Int32);
                parameters.Add("FechaBaja", DateTime.Now, DbType.DateTime);
                parameters.Add("Usuario", "HCELAYA", DbType.String);

                var sql_script = "";

                //Controla la FK
                //if (e.SqlState.Equals("23503"))
                if (e.ConstraintName.Equals("fk_personas_genero"))
                {
                    sql_script = @"
                                        UPDATE  public.""Generos""
                                        SET ""FechaBaja"" = @FechaBaja,
                                            ""UsuarioBaja"" = @Usuario                               
                                         WHERE ""Id"" = @Id  
                                    ";

                    var result = await db.ExecuteAsync(sql_script, parameters);

                    return result > 0;
                }

                else

                    return false;
            }
        }

        public async Task<Genero> find(int id)
        {
            var db = dbConnection();

            var command = @" select ""Id"", ""Descripcion"",  ""FechaAlta"", ""UsuarioAlta"", ""FechaBaja"", ""UsuarioBaja""
                             from public.""Generos""
                             where ""Id"" = @Id ";

            return await db.QueryFirstOrDefaultAsync<Genero>(command, new { Id = id });
        }

        public async Task<IEnumerable<Genero>> findAll()
        {
            var query = @" select ""Id"", ""Descripcion"",  ""FechaAlta"", ""UsuarioAlta"", ""FechaBaja"", ""UsuarioBaja""
                             from public.""Generos""
                            ";
            using (var db = dbConnection())
            {
                var oGeneros = await db.QueryAsync<Genero>(query);

                return oGeneros.ToList();

            }
        }

        
    }
}
