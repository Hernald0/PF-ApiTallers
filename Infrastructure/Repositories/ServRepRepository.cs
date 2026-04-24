using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UTNApiTalleres.Data;
using UTNApiTalleres.Infrastructure.Repositories.Interface;
using UTNApiTalleres.Model;
using WebApiTalleres.Models;
using System.Security.Claims;
using UTNApiTalleres.Application.Interfaces;

namespace UTNApiTalleres.Infrastructure.Repositories
{
    public class ServRepRepository : IServRepRepository
    {
        private PostgresqlConfiguration _connectionString;
        private ICurrentUserService _currentUser;

        public ServRepRepository(PostgresqlConfiguration connectionString,
                                 ICurrentUserService currentUser)
        {
            this._connectionString = connectionString;
            this._currentUser = currentUser;
        }

        protected NpgsqlConnection dbConnection()
        {
            return new NpgsqlConnection(this._connectionString.ConnectionString);
        }

        public async Task<Servicio> CreateServicio(Servicio servicio)
        {
            var sql_insert = @"INSERT INTO public.""Servicios"" (""Nombre"",
                                                                 ""Descripcion"",                                                                 
                                                                 ""UsuarioAlta"",
                                                                 ""FechaAlta"")
                               VALUES ( @Nombre, @Descripcion, @UsuarioAlta, @FechaAlta  )
                             returning *;";

            using (var db = dbConnection())
            {

                var newServicio = await db.QuerySingleAsync<Servicio>(
                                            sql_insert,
                                            new
                                            {
                                                Nombre = servicio.Nombre,
                                                Descripcion = servicio.Descripcion,
                                                FechaAlta = DateTime.Now,
                                                UsuarioAlta = servicio.UsuarioAlta
                                            });

                return newServicio;
            }
        }





        public Task<Servicio> FindServicio(int IdServicio)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Servicio>> FindAllServicio()
        {

            var sql_query = @"
                            SELECT  ""Id"", ""Nombre"", ""Descripcion"", 
                                    ""FechaAlta"", ""UsuarioAlta"", ""FechaBaja"", ""UsuarioBaja"", 
                                    ""DuracionAproximada"", ""Tipo"", ""PrecioCosto"", ""PrecioVenta""
                            FROM public.""Servicios""  ";

            using (var db = dbConnection())
            {
                var oServicio = await db.QueryAsync<Servicio>(sql_query);

                return oServicio.ToList();

            }
        }

        public async Task<int> DeleteServicio(int IdServicio)
        {
            int affectedRows = 0;
            var db = dbConnection();

            {

                var sql_delete = @"
                delete from public.""Servicios"" as Servicio
                 WHERE Servicio.""Id"" = @IdServicio
                     ;";


                affectedRows = await db.ExecuteAsync(
                                                           sql_delete,
                                                           new
                                                           {
                                                               IdServicio = IdServicio
                                                           }
                                                     );


            }

            return affectedRows;
        }

        public async Task<Servicio> UpdateServicio(Servicio servicio)
        {
            var db = dbConnection();


            var sql_update = @"
                update public.""Servicios"" as Servicios
                    set ""Nombre"" = @NombreServicio,
                        ""Descripcion"" = @DescripcionServicio, 
                        ""DuracionAproximada"" = @DuracionAproximada,
                        ""PrecioCosto"" = @PrecioCosto,
                        ""PrecioVenta"" = @PrecioVenta
                    WHERE  ""Id"" = @IdServicio
                     returning  Servicios.""Id"" as id,
                                Servicios.""Nombre"" as nombre,
                                Servicios.""Descripcion"" as descripcion,
                                Servicios.""DuracionAproximada"" as duracionAproximada,
                                Servicios.""PrecioCosto"" as precioCosto,
                                Servicios.""PrecioVenta"" as precioVenta,
                                Servicios.""DuracionAproximada"" as duracionAproximada,
                                Servicios.""FechaAlta"" as fechaAlta,
                                Servicios.""UsuarioAlta"" as usuarioAlta,
                                Servicios.""FechaBaja"" as fechaBaja,
                                Servicios.""UsuarioBaja"" as usuarioBaja;";


            var updatedServicio = await db.QuerySingleAsync<Servicio>(
                                         sql_update,
                                         new
                                         {
                                             IdServicio = servicio.Id,
                                             NombreServicio = servicio.Nombre,
                                             DescripcionServicio = servicio.Descripcion,
                                             DuracionAproximada = servicio.DuracionAproximada,  
                                             PrecioCosto = servicio.PrecioCosto,
                                             PrecioVenta = servicio.PrecioVenta

                                         });



            return updatedServicio;
        }

        public async Task<IEnumerable<ItemVentaDTO>> FindFilterServRep(string pBusqueda, string? pTipo)
        {

            var db = dbConnection();

            const string sql = @"
                select * from (
                                SELECT ""Id"", ""Nombre"" as nombre, 'servicio' as tipo, ""Descripcion"", ""PrecioCosto"", ""PrecioVenta"", ""DuracionAproximada"", ""Tipo"" as clase, null as stock
	                                FROM public.""Servicios"" where ""FechaBaja"" is null 
                                union
                                SELECT ""Id"", ""Nombre"", 'repuesto' as tipo, ""Descripcion"", ""PrecioCosto"", ""PrecioVenta"", null as ""DuracionAproximada"", null as ""Tipo"", ""Stock""
	                                FROM public.""Repuestos"" where ""FechaBaja"" is null 
                            ) as tabla
                WHERE lower(nombre) LIKE '%' || lower(@Cadena) || '%' and ( @Tipo IS NULL OR tipo = @Tipo);";



            var result = await db.QueryAsync<ItemVentaDTO>(sql, new { Cadena = pBusqueda, Tipo = pTipo });

            return result.ToList();


        }
        #region REPUESTOS

        public async Task<IEnumerable<Repuesto>> FindAllRepuestos()
        {

            var sql_query = @"
                            SELECT  ""Id"", ""Nombre"", ""Descripcion"", ""Stock"", ""PrecioCosto"", ""PrecioVenta"", ""FechaBaja"", ""UsuarioBaja""
                            FROM public.""Repuestos""  ";

            using (var db = dbConnection())
            {
                var oRepuestos = await db.QueryAsync<Repuesto>(sql_query);

                return oRepuestos.ToList();

            }
        }

        public async Task<Repuesto> CreateRepuesto(Repuesto repuesto)
        {
            var sql_insert = @"INSERT INTO public.""Repuestos"" ( ""Nombre"",
                                                                 ""Descripcion"",                                                                 
                                                                 ""Stock"",
                                                                 ""PrecioCosto"",
                                                                 ""PrecioVenta"")
                               VALUES ( @Nombre, @Descripcion, @Stock, @PrecioCosto, @PrecioVenta  )
                             returning *;";

            using (var db = dbConnection())
            {

                var newRepuesto = await db.QuerySingleAsync<Repuesto>(
                                            sql_insert,
                                            new
                                            {
                                                Nombre = repuesto.Nombre,
                                                Descripcion = repuesto.Descripcion,
                                                Stock = repuesto.Stock,
                                                PrecioCosto = repuesto.PrecioCosto,
                                                PrecioVenta = repuesto.PrecioVenta
                                            });

                return newRepuesto;
            }
        }

        public async Task<int> DeleteRepuesto(int IdRepuesto)
        {
            int affectedRows = 0;
            var db = dbConnection();

            {

                var sql_delete = @"
                delete from public.""Repuestos""  
                 WHERE  ""Id"" = @IdRepuesto ;";


                affectedRows = await db.ExecuteAsync(
                                                           sql_delete,
                                                           new
                                                           {
                                                               IdRepuesto = IdRepuesto
                                                           }
                                                     );


            }

            return affectedRows;
        }

        public async Task<Repuesto> UpdateRepuesto(Repuesto repuesto)
        {
            var db = dbConnection();


            var sql_update = @"
                update public.""Repuestos""  
                    set ""Nombre"" = @Nombre,
                        ""Descripcion"" = @Descripcion,
                        ""Stock"" = @Stock,
                        ""PrecioCosto"" = @PrecioCosto,
                        ""PrecioVenta"" = @PrecioVenta
                    WHERE  ""Id"" = @IdRepuesto
                     returning   ""Id"" as id,
                                 ""Nombre"" as nombre,
                                 ""Descripcion"" as descripcion,
                                 ""Stock"" as stock,
                                 ""PrecioCosto"" as PrecioCosto,
                                 ""PrecioVenta"" as PrecioVenta;";


            var updatedRepuesto = await db.QuerySingleAsync<Repuesto>(
                                         sql_update,
                                         new
                                         {
                                             IdRepuesto = repuesto.Id,
                                             Nombre = repuesto.Nombre,
                                             Descripcion = repuesto.Descripcion,
                                             Stock = repuesto.Stock,
                                             PrecioCosto = repuesto.PrecioCosto,
                                             PrecioVenta = repuesto.PrecioVenta
                                         });



            return updatedRepuesto;
        }

        public Task<Repuesto> FindRepuesto(int IdRepuesto)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Repuesto>> FindAllRepuesto()
        {
            throw new NotImplementedException();
        }

        #endregion
        public async Task<int> BajaLogicaServicio(int IdServicio)
        {
            using (var db = dbConnection())
            {

                const string sql = @"update public.""Servicios""
                                    set ""FechaBaja"" = @FechaBaja,
	                                    ""UsuarioBaja"" = @Usuario
                                    where ""Id"" = @Id ; ";


            var   user = _currentUser.UserName;

                var rows = await db.ExecuteAsync(sql, new { Id = IdServicio, FechaBaja = DateTime.Now, Usuario = user });
            return rows;

            }
        }

        public async Task<int> BajaLogicaRepuesto(int IdRepuesto)
        {
            using (var db = dbConnection())
            {

               
            const string sql = @"update public.""Repuestos""
                                    set ""FechaBaja"" = @FechaBaja,
	                                    ""UsuarioBaja"" = @Usuario
                                    where ""Id"" = @Id ; ";

            var user = this._currentUser.UserName;

            //return await db.QuerySingleAsync<int>(sql, new { Id = IdRepuesto, FechaBaja = DateTime.Now, Usuario = user });


            var rows = await db.ExecuteAsync(sql, new
            {
                Id = IdRepuesto,
                FechaBaja = DateTime.Now,
                Usuario = user
            });

            return rows;

            }
        }

        public async Task<bool> TieneItems(int id, string tipo)
        {
            var db = dbConnection();

            const string sql = @"
                                SELECT EXISTS (
                                    SELECT 1
                                    FROM public.""VentaDetalles""
                                    WHERE (""ServicioId"" = @Id AND @Tipo = 'servicio')
                                       OR (""RepuestoId"" = @Id AND @Tipo = 'repuesto')

                                    UNION ALL

                                    SELECT 1
                                    FROM public.""RecepcionTareas""
                                    WHERE ""IdServicio"" = @Id
                                      AND ""Tipo"" = @Tipo

                                    UNION ALL

                                    SELECT 1
                                    FROM public.""OrdenDetalles""
                                    WHERE (""ServicioId"" = @Id AND @Tipo = 'servicio')
                                       OR (""RepuestoId"" = @Id AND @Tipo = 'repuesto')
                                )";

             
           
            return await db.QuerySingleAsync<bool>(sql, new { Id = id, Tipo = tipo });
       
        }

        public async Task<bool> ValidarNombreRepuesto(string nombre)
        {
            var db = dbConnection();

            string sql = @"select count(*) from public.""Repuestos""
                            where upper(""Nombre"") = @Nombre";

            int cant = await db.QuerySingleAsync<int>(sql, new { Nombre = nombre.ToUpper() });  
            
            if ( cant > 0 )
            { 
               return true;
            }
            else
            {
                return false;
            }

        }

        public async Task<bool> ValidarNombreServicio(string nombre)
        {
            var db = dbConnection();

            string sql = @"select count(*) from public.""Servicios""
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
