using Dapper;
using Npgsql;
using System;
using System.Threading.Tasks;
using UTNApiTalleres.Data;
using UTNApiTalleres.Infrastructure.Repositories.Interface;

namespace UTNApiTalleres.Infrastructure.Repositories
{
    public class RolAccesoRepository : IRolAccesoRepository
    {
        private PostgresqlConfiguration _connectionString;
        protected NpgsqlConnection dbConnection()
        {
            return new NpgsqlConnection(this._connectionString.ConnectionString);
        }
        public RolAccesoRepository(PostgresqlConfiguration connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<int> DeleteRolAccesoAsync(int rolId, int accesoId)
        {
            /*using var db = dbConnection();

            //elimino los permisos relacionados al acceso

            var row_delete = db.ExecuteAsync(@"DELETE FROM public.""RolPermisos"" 
                                    WHERE ""RolId""=@RolId and                                             
                                          ""AccesoId"" = @AccesoId ", new { RolId = rolId, AccesoId = accesoId });

            //elimino el acceso

            return await db.ExecuteAsync(@"DELETE FROM public.""RolAccesos"" 
                                    WHERE ""RolId""=@RolId and                                             
                                          ""AccesoId"" = @AccesoId ", new { RolId = rolId, AccesoId = accesoId });
            */
            using var conn = dbConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"DELETE FROM public.""RolPermisos"" 
                                    WHERE ""RolId"" = @RolId and
                                          ""AccesoId"" = @AccesoId ";

            cmd.Parameters.AddWithValue("RolId", rolId);
            cmd.Parameters.AddWithValue("AccesoId", accesoId);

            int rowsAffected = await cmd.ExecuteNonQueryAsync();

            await conn.CloseAsync();

            ////////////////////////////////////
          
            using var conn1 = dbConnection();
            await conn.OpenAsync();

            using var cmd1 = conn.CreateCommand();
            cmd1.CommandText = @"DELETE FROM public.""RolAccesos"" 
                                    WHERE ""RolId""=@RolId and                                             
                                          ""AccesoId"" = @AccesoId ";

            cmd1.Parameters.AddWithValue("RolId", rolId);
            cmd1.Parameters.AddWithValue("AccesoId", accesoId);

            //int existe1 = (int)await cmd.ExecuteNonQueryAsync();
             rowsAffected = await cmd1.ExecuteNonQueryAsync();

            await conn1.CloseAsync();

            return rowsAffected;


        }
        
         

        public async Task<int> AddRolAccesoAsync(int rolId, int accesoId)
        {

            using var db = dbConnection();
            var retorno = await db.ExecuteAsync(@"INSERT INTO public.""RolAccesos""(
                                                ""RolId"", ""AccesoId"")
                                                VALUES(@RolId, @AccesoId)", new { RolId = rolId, AccesoId = accesoId });

            Console.WriteLine("Valor a retornar" + retorno);

            return retorno; 


        }

        public async Task<bool> ExistsAsync(int rolId, int accesoId)
        {
            /*
            const string sql = @"
                                SELECT EXISTS (
                                    SELECT 1
                                    FROM public.""RolAccesos""
                                    WHERE ""RolId"" = @RolId
                                       and ""AccesoId"" = @AccesoId
                                );
                            ";
            using var db = dbConnection();

            var valor =   db.ExecuteScalarAsync<bool>(sql, new { RolId = rolId, AccesoId = accesoId });

            return valor;*/

            using var conn = dbConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                                SELECT EXISTS (
                                    SELECT 1
                                    FROM public.""RolAccesos""
                                    WHERE ""RolId"" = @RolId
                                       and ""AccesoId"" = @AccesoId
                                );
                            ";
            
            cmd.Parameters.AddWithValue("RolId", rolId);
            cmd.Parameters.AddWithValue("AccesoId", accesoId);

            bool existe = (bool)await cmd.ExecuteScalarAsync();

            await conn.CloseAsync();

            return existe;

        }

    }
}
