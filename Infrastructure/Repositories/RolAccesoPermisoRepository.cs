using Dapper;
using Npgsql;
using System.Threading.Tasks;
using UTNApiTalleres.Data;
using UTNApiTalleres.Infrastructure.Repositories.Interface;

namespace UTNApiTalleres.Infrastructure.Repositories
{
    public class RolAccesoPermisoRepository : IRolAccesoPermisoRepository
    {

        private PostgresqlConfiguration _connectionString;
        protected NpgsqlConnection dbConnection()
        {
            return new NpgsqlConnection(this._connectionString.ConnectionString);
        }
        public RolAccesoPermisoRepository(PostgresqlConfiguration connectionString)
        {
            _connectionString = connectionString;
        }
        public Task<int> deleteRolAccesoPermisoAsync(int rolId, int accesoId, int permisoId)
        {
            using var db = dbConnection();
            return db.ExecuteAsync(@"DELETE FROM public.""RolPermisos"" 
                                   WHERE ""RolId""=@RolId and 
                                          ""PermisoId"" = @PermisoId and 
                                          ""AccesoId"" = @AccesoId", new { RolId = rolId, PermisoId = permisoId, AccesoId = accesoId });
        }

        public async Task<int> AddRolAccesoPermisoAsync(int rolId, int accesoId, int permisoId)
        {
            using var conn = dbConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO public.""RolPermisos""(
                                            ""RolId"", ""PermisoId"", ""AccesoId"")
                                            VALUES(@RolId, @PermisoId, @AccesoId)";

            cmd.Parameters.AddWithValue("RolId", rolId);
            cmd.Parameters.AddWithValue("AccesoId", accesoId);
            cmd.Parameters.AddWithValue("PermisoId", permisoId);

            int insertoOk = (int)await cmd.ExecuteNonQueryAsync();

           // await conn.CloseAsync();
            return insertoOk;
        }

        public async Task<bool> ExistsAsync(int rolId, int accesoId, int permisoId)
        {
            

            using var conn = dbConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
                                SELECT EXISTS (
                                    SELECT 1
                                    FROM public.""RolPermisos""
                                    WHERE ""RolId"" = @RolId
                                       and ""AccesoId"" = @AccesoId
                                       and ""PermisoId"" = @PermisoId
                                );
                            ";

            cmd.Parameters.AddWithValue("RolId", rolId);
            cmd.Parameters.AddWithValue("AccesoId", accesoId);
            cmd.Parameters.AddWithValue("PermisoId", permisoId);

            bool insertoOk = (bool) await cmd.ExecuteScalarAsync();

            await conn.CloseAsync();
            return insertoOk;

             

        }
    }
}
