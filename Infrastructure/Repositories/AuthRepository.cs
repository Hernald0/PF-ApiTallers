using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using UTNApiTalleres.Application.DTOs;
using UTNApiTalleres.Infrastructure.Repositories.Interface;
using UTNApiTalleres.Model;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Data.Repositorio
{
   
    public class AuthRepository : IAuthRepository
    {
        private PostgresqlConfiguration _connectionString;
        protected NpgsqlConnection dbConnection()
        {
            return new NpgsqlConnection(this._connectionString.ConnectionString);
        }
        public AuthRepository(PostgresqlConfiguration connectionString)
        {
            _connectionString = connectionString;
        }

        public Usuario ValidarUsuario(string usuario, string password)
       {
            var db = dbConnection();

            string sql = @"
            SELECT u.""Id"",
                   u.""User"",
                   concat(p.""Nombre"", ' ', p.""Apellido"") AS NombreCompleto,
                   e.""Id"" as ""IdEmpleado"" 
                FROM public.""Usuarios"" u 
                        inner join public.""Personas"" p
                            on u.""IdPersona"" = p.""Id""
                        inner join public.""Empleados"" e
                            on p.""Id"" = e.""IdPersona""
                WHERE (u.""User"" = @usuario OR u.""Email"" = @usuario)
              AND ""Password"" = @password
              AND ""Activo"" = true";

            return db.QueryFirstOrDefault<Usuario>(sql, new { usuario, password });
        }

        /*
        public List<string> ObtenerRoles(int usuarioId)
        {
            var db = dbConnection();

            string sql = @"
                        SELECT r.""Nombre"" 
                        FROM public.""Roles"" r
                        inner join public.""UsuarioRol"" ur
                            on r.""RolId"" = ur.""RolId""   
                        WHERE ur.""UserId"" = @usuarioId";

            return db.Query<string>(sql, new { usuarioId }).ToList();
        }*/

        public  string  ObtenerRol(int usuarioId)
        {
            var db = dbConnection();

            string sql = @"
                        SELECT r.""Nombre"" 
                        FROM public.""Roles"" r
                        inner join public.""UsuarioRol"" ur
                            on r.""RolId"" = ur.""RolId""   
                        WHERE ur.""UserId"" = @usuarioId";

            return db.QueryFirstOrDefault<string>(sql, new { usuarioId }) ;
        }
        public async Task<List<MenuGrupoDTO>> ObtenerAccesosAsync(int usuarioId)
        {
            var db = dbConnection();
            /*
            string sql_select = @"select a.""Id"", 
                                         a.""Nombre"", 
                                         a.""Ruta"", 
                                         a.""Agrupador"",
                                         pe.""PermisoId"", 
                                         pe.""Etiqueta"", pe.""Descripcion""
                                    from public.""Accesos"" as a 
				                        inner join public.""RolAccesos"" ra
		                                    on a.""Id"" = ra.""AccesoId""
	                                    inner join public.""UsuarioRol"" ur
		                                    on ra.""RolId"" = ur.""RolId""
				                        LEFT JOIN public.""RolPermisos"" rp
					                        on (ra.""AccesoId"" = rp.""AccesoId"" and					    
						                        ra.""RolId"" = rp.""RolId"")
				                        LEFT JOIN public.""Permisos"" pe
					                        on (rp.""PermisoId"" = pe.""PermisoId"")
			                        WHERE ur.""UserId"" = @Id
                                      AND a.""Activo"" = true ";*/

            string sql_select = @"select a.""Id"", 
                                 a.""Nombre"", 
                                 a.""Ruta"", 
                                 a.""Agrupador"",
                                 pe.""PermisoId"", 
                                 pe.""Etiqueta"", 
                                 pe.""Descripcion""
                            from public.""Accesos"" as a 
                                inner join public.""RolAccesos"" ra
                                    on a.""Id"" = ra.""AccesoId""
                                inner join public.""UsuarioRol"" ur
                                    on ra.""RolId"" = ur.""RolId""
                                LEFT JOIN public.""RolPermisos"" rp
                                    on (ra.""AccesoId"" = rp.""AccesoId"" and					    
                                        ra.""RolId"" = rp.""RolId"")
                                LEFT JOIN public.""Permisos"" pe
                                    on (rp.""PermisoId"" = pe.""PermisoId"")
                        WHERE ur.""UserId"" = @Id
                          AND a.""Activo"" = true
                        ORDER BY a.""Agrupador"", a.""Nombre""";

            var accesoDict = new Dictionary<int, AccesoDTO>();

            await db.QueryAsync<AccesoDTO, Permiso, AccesoDTO>(
                sql_select,
                (acceso, permiso) =>
                {
                    if (!accesoDict.TryGetValue(acceso.Id, out var accesoEntry))
                    {
                        accesoEntry = acceso;
                        accesoEntry.Permisos = new List<Permiso>();
                        accesoDict.Add(acceso.Id, accesoEntry);
                    }

                    if (permiso != null && permiso.PermisoId > 0)
                    {
                        accesoEntry.Permisos.Add(permiso);
                    }

                    return accesoEntry;
                },
                new { Id = usuarioId },
                splitOn: "PermisoId"
            );

            // Agrupar acá antes de devolver
            var menu = accesoDict.Values
                .GroupBy(a => a.Agrupador ?? "General")   // si no tiene agrupador, va a "General"
                .Select(g => new MenuGrupoDTO
                {
                    Agrupador = g.Key,
                    Accesos = g.ToList()
                })
                .ToList();

            return menu;
        }
    }
}
