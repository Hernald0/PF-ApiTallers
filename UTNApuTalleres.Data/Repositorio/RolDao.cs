using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTNApiTalleres.Data.Repositorio.Interfaz;
using UTNApiTalleres.Model;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Data.Repositorio
{
    public class RolDao : IRolDao
    {

        private PostgresqlConfiguration _connectionString;

        public RolDao(PostgresqlConfiguration connectionString)
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
	                               p.""PermisoId"" as pid, p.""PermisoId"" ,p.""Codigo"", p.""Descripcion""
                            FROM public.""Roles"" r
                            inner join public.""RolAccesos"" ra
	                            on r.""RolId"" = ra.""RolId""
                            inner join public.""Accesos"" a
	                            on ra.""AccesoId"" = a.""Id""
                            left join public.""RolPermisos"" rp
	                            on (rp.""RolId"" =  r.""RolId""  and 
		                            rp.""AccesoId"" = ra.""AccesoId"" ) 
                            left join public.""Permisos"" p
	                            on (rp.""PermisoId"" = p.""PermisoId"")
                            where r.""RolId"" = @id
                            and a.""Activo"" is true";

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

        public async Task<int> InsertAsync(Rol rol)
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
        /*
        public async Task InsertRolAccesoAsync(int id)
        {
            using var db = dbConnection();
            await db.ExecuteAsync(@"DELETE FROM public.""Roles"" WHERE ""RolId""=@id", new { id });
        }
        
        public async Task InsertRolAccesoPermisoAsync(int id)
        {
            using var db = dbConnection();
            await db.ExecuteAsync(@"DELETE FROM public.""Roles"" WHERE ""RolId""=@id", new { id });
        }

        public async Task deleteRolAccesoAsync(int id)
        {
            using var db = dbConnection();
            await db.ExecuteAsync(@"DELETE FROM public.""Roles"" WHERE ""RolId""=@id", new { id });
        }

        public async Task deleteRolAccesoPermisoAsync(int id)
        {
            using var db = dbConnection();
            await db.ExecuteAsync(@"DELETE FROM public.""Roles"" WHERE ""RolId""=@id", new { id });
        }*/

    }
}
