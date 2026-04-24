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
    public class AccesoDao : IAccesoDao
    {
        private PostgresqlConfiguration _connectionString;

        public AccesoDao(PostgresqlConfiguration connectionString)
        {
            this._connectionString = connectionString;
        }

        protected NpgsqlConnection dbConnection()
        {
            return new NpgsqlConnection(this._connectionString.ConnectionString);
        }

        public async Task<IEnumerable<Acceso>> GetAllAsync()
        {
            using var db = dbConnection();

            var sql_select = @"SELECT  a.""Id"", a.""Nombre"", a.""Ruta"",a.""Agrupador"", a.""Activo"",
	                                   p.""PermisoId"", p.""Etiqueta"", p.""Descripcion""
                                FROM public.""Accesos"" a
	                                left join 
	                                public.""Permisos"" p
	                                on ( a.""Id"" = p.""AccesoId"")";

            var accesoDict = new Dictionary<int, Acceso>();

            await db.QueryAsync<Acceso, Permiso, Acceso>(
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
                splitOn: "PermisoId"
            );

            return accesoDict.Values;
        }

        public async Task<int> InsertAsync(Acceso acceso)
        {
            using var db = dbConnection();
            var sql_acceso = @"INSERT INTO public.""Accesos"" (""Nombre"", ""Ruta"",""Agrupador"", ""Activo"")
                        VALUES (@Nombre, @Ruta, @Agrupador, @Activo)
                        RETURNING ""Id"";";
            var AccesoId =  await db.ExecuteScalarAsync<int>(sql_acceso, acceso);

            /*
            var sql_permiso = @"INSERT INTO public.""Permisos"" (""AccesoId"", ""PermisoId"", ""Codigo"", ""Descripcion"")
                        VALUES (@AccesoId, @PermisoId, @Codigo, @Descripcion)
                        RETURNING ""Id"";";

            foreach (var Permiso in acceso.Permisos)
            {
                

                var PermisoId = await db.ExecuteScalarAsync<int>(sql_permiso, new { AccesoId = AccesoId, PermisoId = Permiso.PermisoId, Codigo = Permiso.Codigo, Descripcion = Permiso.Descripcion });
            }*/

            await InsUpdAsync(AccesoId, acceso.Permisos);

            return AccesoId;
        }

        public async Task UpdateAsync(Acceso acceso)
        {
            using var db = dbConnection();
            
            var sql = @"UPDATE public.""Accesos"" SET ""Nombre""=@Nombre, ""Ruta""=@Ruta, ""Agrupador""=@Agrupador, ""Activo""=@Activo WHERE ""Id""=@Id";
           
            await db.ExecuteAsync(sql, acceso);

            await InsUpdAsync(acceso.Id,  acceso.Permisos);

        }

        public async Task InsUpdAsync(int AccesoId, List<Permiso> permisos)
        {
            using var db = dbConnection();
    

            int PermisoId = 0;


            var sql_permiso_ins = @"INSERT INTO public.""Permisos"" (""AccesoId"", ""Etiqueta"", ""Descripcion"")
                                    VALUES (@AccesoId, @Etiqueta, @Descripcion)
                                    RETURNING ""PermisoId"";";

            var sql_permiso_upd = @"UPDATE public.""Permisos""
	                                SET    ""Etiqueta""=@Etiqueta, ""Descripcion""=@Descripcion, ""AccesoId""=@AccesoId
                                    WHERE ""PermisoId"" = @PermisoId
	                                Returning ""PermisoId""; ";

            foreach (var Permiso in permisos)
            {

                if (Permiso.PermisoId != 0)
                {

                      PermisoId = await db.ExecuteScalarAsync<int>(sql_permiso_upd, new { AccesoId = AccesoId, PermisoId = Permiso.PermisoId, Etiqueta = Permiso.Etiqueta, Descripcion = Permiso.Descripcion });
                }
                else
                {
                      PermisoId = await db.ExecuteScalarAsync<int>(sql_permiso_ins, new { AccesoId = AccesoId, Etiqueta = Permiso.Etiqueta, Descripcion = Permiso.Descripcion });
                }

                
            }

        }

        public async Task DeletePermisoAsync(int id)
        {
            using var db = dbConnection();
            await db.ExecuteAsync(@"DELETE FROM public.""Permisos"" WHERE ""PermisoId""=@id", new { id });
        }

        public async Task DeleteAsync(int id)
        {
            using var db = dbConnection();
            await db.ExecuteAsync(@"DELETE FROM public.""Accesos"" WHERE ""Id""=@id", new { id });
        }

        public async Task<IEnumerable<Acceso>> GetByRolAsync(int rolId)
        {
            using var db = dbConnection();
            var sql = @"SELECT a.* FROM public.""Accesos"" a
                        INNER JOIN rol_acceso ra ON ra.accesoid = a.id
                        WHERE ra.rolid = @rolId";
            return await db.QueryAsync<Acceso>(sql, new { rolId });
        }

      

        public async Task<Acceso> GetByIdAsync(int id)
        {
            using var db = dbConnection();

            var sql_select = @"SELECT  a.""Id"", a.""Nombre"", a.""Ruta"", a.""Activo"",
	                                   p.""PermisoId"", p.""Etiqueta"", p.""Descripcion""
                                FROM public.""Accesos"" a
	                                left join 
	                                public.""Permisos"" p
	                                on ( a.""Id"" = p.""AccesoId"")
                                WHERE a.""Id""= @id";

            var accesoDict = new Dictionary<int, Acceso>();

            await db.QueryAsync<Acceso, Permiso, Acceso>(
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
                new { id },
                splitOn: "PermisoId"
            );

            return accesoDict.Values.FirstOrDefault();
        }


        public async Task SetAccesosPorRolAsync(int rolId, List<int> accesosIds)
        {
            using var db = dbConnection();
            using var tran = db.BeginTransaction();

            await db.ExecuteAsync(@"DELETE FROM public.""RolAccesos"" WHERE ""RolId""=@rolId", new { rolId }, tran);

            foreach (var id in accesosIds)
            {
                await db.ExecuteAsync(
                    @"INSERT INTO public.""RolAccesos"" (""RolId"", ""AccesoId"") VALUES (@rolId, @id)",
                    new { rolId, id }, tran);
            }

            tran.Commit();
        }

        public async Task<bool> ExisteNombreAcceso(string nombre)
        {
            var db = dbConnection();

            string sql = @"select count(*) from public.""Accesos""
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

        public async Task<bool> ExisteRutaAcceso(string ruta)
        {
            var db = dbConnection();

            string sql = @"select count(*) from public.""Accesos""
                            where REPLACE(upper(""Ruta""),'/','') = @Ruta";

            int cant = await db.QuerySingleAsync<int>(sql, new { Ruta = ruta.ToUpper() });

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
