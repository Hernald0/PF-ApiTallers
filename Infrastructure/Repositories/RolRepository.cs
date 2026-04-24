using Dapper;
using Npgsql;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UTNApiTalleres.Application.DTOs;
using UTNApiTalleres.Data;
using UTNApiTalleres.Infrastructure.Repositories.Interface;
using UTNApiTalleres.Model;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Infrastructure.Repositories
{
    public class RolRepository : IRolRepository
    {
        private PostgresqlConfiguration _connectionString;

        public RolRepository(PostgresqlConfiguration connectionString)
        {
            this._connectionString = connectionString;
        }

        protected NpgsqlConnection dbConnection()
        {
            return new NpgsqlConnection(this._connectionString.ConnectionString);
        }

        public async Task<IEnumerable<Rol>> GetAllAsync()
        {
            using var db = dbConnection();
            return await db.QueryAsync<Rol>(@"SELECT * FROM public.""Roles""");
        }

        public async Task<Rol> GetByIdAsync(int id)
        {
            using var db = dbConnection();

            var sql_query = @"SELECT r.""RolId"", r.""Nombre"", r.""Descripcion"", r.""Activo"",
	                               a.""Id"" as aid, a.""Id"", a.""Nombre"", a.""Ruta"", a.""Activo"",
	                               p.""PermisoId"" as pid, p.""PermisoId"" ,p.""Etiqueta"", p.""Descripcion"", 
                                   CASE
									    WHEN rp.""PermisoId"" IS NOT NULL THEN true
                                        ELSE false
                                    END AS Activo
                            FROM public.""Roles"" r
                            left join public.""RolAccesos"" ra
	                            on r.""RolId"" = ra.""RolId""
                            left join public.""Accesos"" a
	                            on ra.""AccesoId"" = a.""Id""
                            left join public.""RolPermisos"" rp
	                            on (rp.""RolId"" =  r.""RolId""  and 
		                            rp.""AccesoId"" = ra.""AccesoId"" ) 
                            left join public.""Permisos"" p
	                            on (rp.""PermisoId"" = p.""PermisoId"")
                            where r.""RolId"" = @id
                            and r.""Activo"" is true";

            var rolDictionary = new Dictionary<int, Rol>();

            var result = await db.QueryAsync<Rol, Acceso, Permiso, Rol>(
                                sql_query,
                                (rol, acceso, permiso) =>
                                {
                                    // 1️⃣ Consolidar Rol
                                    if (!rolDictionary.TryGetValue(rol.RolId, out var rolEntry))
                                    {
                                        rolEntry = rol;
                                        rolEntry.Accesos = new List<Acceso>();
                                        rolDictionary.Add(rol.RolId, rolEntry);
                                    }

                                    // 2️⃣ Consolidar Acceso
                                    if (acceso != null && acceso.Id > 0)
                                    {
                                        var accesoEntry = rolEntry.Accesos
                                            .FirstOrDefault(a => a.Id == acceso.Id);

                                        if (accesoEntry == null)
                                        {
                                            accesoEntry = acceso;
                                            accesoEntry.Permisos = new List<Permiso>();
                                            rolEntry.Accesos.Add(accesoEntry);
                                        }

                                        // 3️⃣ Consolidar Permisos
                                        if (permiso != null && permiso.PermisoId > 0)
                                        {
                                            if (!accesoEntry.Permisos.Any(p => p.PermisoId == permiso.PermisoId))
                                            {
                                                accesoEntry.Permisos.Add(permiso);
                                            }
                                        }
                                    }

                                    return rolEntry;
                                },
                                new { id },
                                splitOn: "aid,pid"
                            );


            return rolDictionary.Values.FirstOrDefault();
        }

        public async Task<int> AddAsync(Rol rol)
        {
            using var db = dbConnection();
            var sql = @"INSERT INTO public.""Roles"" (""Nombre"", ""Descripcion"", ""Activo"") VALUES (@Nombre, @Descripcion, @Activo) RETURNING ""RolId"";";
            return await db.ExecuteScalarAsync<int>(sql, rol);
        }

        public async Task UpdateAsync(Rol rol)
        {
            using var db = dbConnection();
            await db.ExecuteAsync(@"UPDATE public.""Roles"" SET ""Nombre""=@Nombre, ""Activo""=@Activo, ""Descripcion""=@Descripcion WHERE ""RolId""=@RolId", rol);
        }

        public async Task DeleteAsync(int id)
        {
            using var db = dbConnection();
            await db.ExecuteAsync(@"DELETE FROM public.""Roles"" WHERE ""RolId""=@id", new { id });
        }

        public async Task<bool> ExistsAsync(int id)
        {
            /*
            const string sql = @"
                                SELECT EXISTS (
                                    SELECT 1
                                    FROM public.""Roles""
                                    WHERE ""RolId"" = @id
                                );
                            ";
            using var db = dbConnection();
           
            var valor = await db.ExecuteScalarAsync<bool>(sql, new { id });

            return valor;
            */
            using var conn = dbConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                                SELECT EXISTS (
                                    SELECT 1
                                    FROM public.""Roles""
                                    WHERE ""RolId"" = @id
                                );
                            ";
            cmd.Parameters.AddWithValue("id", id);
            bool existe = (bool) await cmd.ExecuteScalarAsync();
            await conn.CloseAsync();
            return existe;
        }

        public async Task<bool> existeNombreAsync(string nombre)
        {
            var db = dbConnection();

            string sql = @"select count(*) from public.""Roles""
                            where upper(""Nombre"") = @Nombre";

            int cant = await db.QuerySingleAsync<int>(sql, new { Nombre = nombre.ToUpper() });

            if (cant > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
