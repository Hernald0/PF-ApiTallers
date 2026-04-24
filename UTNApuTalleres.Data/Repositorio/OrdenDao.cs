
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTNApiTalleres.Data.Repositorio.Interfaz;

using WebApiTalleres.Models;
using Npgsql;
using Dapper;
using WebApiTalleres.Models.Enum;
using UTNApiTalleres.Model;

namespace UTNApiTalleres.Data.Repositorio
{
    public class OrdenDao : IOrdenDao

    {
        private PostgresqlConfiguration _connectionString;

        public OrdenDao(PostgresqlConfiguration connectionString)
        {
            this._connectionString = connectionString;
        }

        protected NpgsqlConnection dbConnection()
        {
            return new NpgsqlConnection(this._connectionString.ConnectionString);
        }

        public async Task<int>  AgregarOrder(int? RecepcionId, RecepcionTurnoDTO orden)
        {

            var db = dbConnection();

            var sql = @"
                    INSERT INTO public.""Ordenes""(""IdTurno"", ""IdCliente"", ""IdVehiculo"",""IdRecepcion"", ""Estado"",""IdEmpleadoAsignado"",""Usuario"")
	                VALUES(@IdTurno, @IdCliente, @IdVehiculo, @IdRecepcion, @Estado, @IdEmpleadoAsignado, @Usuario)  
                    returning  ""Id""";

            Empleado oEmpleado = getAsignarEmpleado();

            var orderId = await  db.QuerySingleAsync<int>(sql, new
            {
                @IdTurno = orden.IdTurno,
                @IdCliente = orden.IdCliente,
                @IdVehiculo = orden.IdVehiculo,
                @IdRecepcion = RecepcionId,
                @Estado = EstadoOrden.ADiagnosticar,
                @IdEmpleadoAsignado = oEmpleado.Id,
                @Usuario = orden.Usuario

            });

            AgregarDetalles(orderId, orden.Servicios);

            return orderId;
        }

        private void AgregarDetalles(int? orderId, List<ItemVentaCreateDTO> Servicios)
        {
            var db = dbConnection();

            var sqlDetalle = @"INSERT INTO public.""OrdenDetalles""
                                    (""OrdenId"", 
                                     ""ServicioId"", 
                                     ""RepuestoId"",  
                                     ""PrecioUnitario"", 
                                     ""Bonificacion"", 
                                     ""Cantidad"", 
                                     ""SubTotal"")
                                   values(
                                     @OrdenId,
                                     @ServicioId, 
                                     @RepuestoId, 
                                     @PrecioUnitario, 
                                     @Bonificacion, 
                                     @Cantidad, 
                                     @SubTotal
                                   );
                ";

            foreach (ItemVentaCreateDTO s in Servicios)
            {


                var orderDetalleId = db.Execute(sqlDetalle, new
                {

                    @OrdenId = orderId,
                    //@ServicioId = (s.Tipo == "servicio" ? s.ItemId : null),
                    //@RepuestoId = (s.Tipo == "respuesto" ? s.ItemId : null),
                    @ServicioId = s.ServicioId,
                    @RepuestoId = s.RepuestoId,
                    @PrecioUnitario = s.PrecioUnitario,
                    @Bonificacion = s.Bonificacion,
                    @Cantidad = s.Cantidad,
                    @SubTotal = s.Subtotal

                });

            }
        }

        public int CancelarOrder(int id)
        {
            throw new NotImplementedException();
        }

        public void DeleteOrder(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Orden>> GetOrdenes(string rol, int? idEmpleado)
        {
            var sql_query = @" select      o.""Id"" as ido, o.*, rv.""FechaRecepcion"",
			                                c.""Id"" as idc, c.*, 
			                                p.""Id"" as idp, p.* 
	                                from public.""Ordenes"" as o 
		                                inner join public.""RecepcionVehiculo"" rv
			                                on o.""IdRecepcion"" = rv.""Id""
		                                inner join public.""Clientes"" as c
	                                on o.""IdCliente"" = c.""Id""
		                                inner join public.""Personas"" as p
	                                on c.""PersonaId"" = p.""Id""
                                   where (@Rol IN ('Administrador','Jefe de Taller'))
                                           OR
                                         (@Rol NOT IN ('Administrador','Jefe de Taller','Administrativo') and o.""IdEmpleadoAsignado"" = @IdEmpleado)
                                           or 
                                         (@Rol IN ('Administrativo') and (o.""Estado"" IN ( @Estado, @Estado1)) )
                                   order by o.""Id"" desc";



            using (var connection = dbConnection())
            {

                var oOrders = await connection.QueryAsync<Orden, Cliente, Persona, Orden>(
                                sql_query,
                                (orden, cliente, persona) =>
                                {
                                    orden.Cliente = cliente;
                                    orden.Cliente.Persona = persona;

                                    return orden;
                                },
                                param: new { Rol = rol,
                                             IdEmpleado = idEmpleado,
                                             Estado = EstadoOrden.EsperaConfirmacion,
                                             Estado1 = EstadoOrden.Finalizado
                                },
                                splitOn: "idc, idp"
                            );



                return oOrders;

            }
        }
        public int UpdateVentaId(int? OrdenId, int? VentaId)
        {
            using (var db = dbConnection())
            {

                {
                    var query = @"
                    UPDATE public.""Ordenes"" 
                    SET   ""VentaId"" = @VentaId                      
                    WHERE ""Id"" = @Id;
                ";
                 

                 return   db.Execute(query, new
                    {
                        @Id = OrdenId,
                        @VentaId = VentaId,
                         
                    });

                   
                }
            }
        }
        public  void ModificarOrder(OrdenDTO orden)
        {
            using ( var db = dbConnection())
            {
                 
                {
                    var query = @"
                    UPDATE public.""Ordenes"" 
                    SET   
                        
	                    ""Usuario"" = @Usuario,
                        ""FechaInicio"" = @FechaInicio, 
	                    ""FechaFin"" = @FechaFin, 	                
                        ""Estado"" = @Estado,
                        ""IdEmpleadoAsignado"" = @IdEmpleadoAsignado,                                            
		                ""ObservacionTecnico"" = @ObservacionTecnico
                    WHERE ""Id"" = @Id;
                ";
                    /*, 
	                    
                    */

                    db.Execute(query, new
                    {   
                        @Id = orden.IdOrden,
                        @FechaInicio = orden.FechaInicio,
                        @FechaFin = orden.FechaFin,
                        @IdEmpleadoAsignado = orden.IdEmpleadoAsignado,
                        @Estado = orden.Estado,
                        @Usuario = orden.Usuario,
                        @ObservacionTecnico = orden.ObservacionTecnico
                     });

                    var deleteDetallesQuery = "DELETE FROM public.\"OrdenDetalles\" WHERE \"OrdenId\" = @Id;";
                    db.Execute(deleteDetallesQuery, new { Id = orden.IdOrden });

                    if (orden.Servicios.Count>0 ) AgregarDetalles(orden.IdOrden, orden.Servicios);

                    /*
                    foreach (var detalle in orden.Items)
                    {
                        detalle.VentaId = (int)orden.Id;
                        var detalleQuery = @"
                        INSERT INTO public.""OrdenDetalles"" 
                        (""VentaId"", ""ServicioId"", ""RepuestoId"", ""Cantidad"", ""PrecioUnitario"", ""Descuento"", ""Subtotal"") 
                        VALUES (@VentaId, @ServicioId, @RepuestoId, @Cantidad, @PrecioUnitario, @Descuento, @Subtotal);
                    ";
                        connection.Execute(detalleQuery, detalle, transaction);
                    }*/



                }
            }
        }

        public async Task<int> deficionClienteOrder(int orderId, int estado)
        {
            var db = dbConnection();

            var sql = @"
                    update public.""Ordenes""
                    set ""Estado"" = @Estado
                    where ""Id"" = @OrderId
                    ";



            return await db.ExecuteAsync(sql, new
            {

                Estado = estado,
                OrderId = orderId


            });
             
        }

        public async Task<bool> ModificarEmpleadoAsignado(EmpleadoAsignadoDTO empleado)
        {
            var db = dbConnection();

            var sql = @"
                    update public.""Ordenes""
                    set ""IdEmpleadoAsignado"" = @IdEmpleadoAsignado
                    where ""Id"" = @OrderId
                    ";

       

            var orderId = await db.ExecuteAsync(sql, new
            {
                
                @IdEmpleadoAsignado = empleado.IdEmpleado,
                @OrderId = empleado.IdOrder


            });

            return orderId > 0;
        }
        public async Task<List<EmpleadosComboDTO>> getEmpleadosMecanicos()
        {

            var sql_query = @"select e.""Id"" as ""IdEmpleado"",
	                       e.""User""
                    from public.""Empleados"" e
                    inner join public.""Usuarios"" as u
                                on e.""User"" = u.""User""
		                    inner join public.""UsuarioRol"" as ur
                            on u.""Id"" = ur.""UserId""
		                    inner join public.""Roles"" as r
                            on ur.""RolId"" = r.""RolId""
                    where r.""RolId"" in (7, 9)
                        and r.""Activo"" = true;";

            using (var connection = dbConnection())
            {

                var empleadosMecanicos = (await connection.QueryAsync<EmpleadosComboDTO>(sql_query)).ToList();

                return empleadosMecanicos;
            }
        }

        private Empleado getAsignarEmpleado()
        {

            var sql_query = @"	Select e.""Id"", u.""Id"" uId, u.*
                                from public.""Empleados"" as e 
	                                inner join public.""Usuarios"" as u
		                                on e.""User"" = u.""User""
	                                inner join public.""UsuarioRol"" as ur
	                                on u.""Id"" = ur.""UserId""
                                    inner join public.""Roles"" as r
	                                on ur.""RolId"" = r.""RolId""
                                where r.""RolId"" = 7
                                and r.""Activo"" = true;";

            using (var connection = dbConnection())
            {
                 var oEmpleado = connection.Query < Empleado, Usuario, Empleado>(
                        sql_query,
                        map: (empleado, usuario) =>
                        {
                            if (usuario != null)
                            {
                                empleado.Usuario = usuario;
                                
                            }

                            return empleado;
                        },
                         splitOn: "uId"
                         );
                return oEmpleado.FirstOrDefault();
            };



        }

        public async Task<Orden> GetOrden(int id)
        {

            

            var sql_query1 = @" select       o.""Id"" as ido, o.*,
	  	                                    c.""Id"" as idc, c.*, 
	  	                                    p.""Id"" as idp, p.* 
                                        from public.""Ordenes"" as o 

                                            inner join public.""Clientes"" as c
                                        on o.""IdCliente"" = c.""Id""

                                            inner join public.""Personas"" as p
                                        on c.""PersonaId"" = p.""Id""
                                where o.""Id"" = @Id"
                                        ;
           // , clId, peId, veId, mvId, maId, ,  
    var sql_query = @"	select  o.""Id"" as oId, o.""Id"",  o.""Estado"", o.""ObservacionTecnico"", o.""FechaInicio"", o.""FechaFin"", o.""Usuario"", o.""IdEmpleadoAsignado"",
		                                 rv.""FechaRecepcion"", rv.""HoraRecepcion"", rv.""Combustible"", rv.""Kilometraje"", rv.""IdAseguradora"", rv.""Inspector"", rv.""NroSiniestro"", rv.""Franquicia"", rv.""MotivoConsulta"",		
		                                od.""Id"" as itemid, od.""ServicioId"", od.""RepuestoId"", od.""PrecioUnitario"", od.""Bonificacion"", od.""Cantidad"", od.""SubTotal"",
                                        r.""Id"" as repId, r.""Nombre"" as ""NombreRepuesto"", r.""Descripcion"" as ""DescripcionRepuesto"", 
		                                r.""PrecioCosto"" as ""CostoRepuesto"", r.""PrecioVenta"" as ""VentaRepuesto"",
		                                s.""Id"" as serId, s.""Nombre"" as ""NombreServicio"", s.""Descripcion"" as ""DescripcionServicio"", 
		                                s.""PrecioCosto"" as ""CostoServicio"", s.""PrecioVenta"" as ""VentaServicio"",
		                                c.""Id"" as clId, c.""Id"", c.""PersonaId"", c.""TallerId"",
		                                p.""Id"" as peId, p.""Id"", p.""Nombre"", p.""RazonSocial"", p.""Apellido"", p.""FecNacimiento"", p.""IdLocalidad"", p.""Barrio"", p.""Direccion"", p.""NroDireccion"", p.""Dpto"", p.""Piso"", p.""Telcelular"", p.""Telfijo"", p.""Email"", p.""IdTipoIdentificador"", p.""NroIdentificacion"", p.""TipoPersona"", p.""IdGenero"", p.""Ocupacion"", p.""IdEstadoCivil"", p.""FechaAlta"", p.""UsrAlta"", p.""FechaBaja"", p.""UsrBaja"", p.""FechaMod"", p.""UsrMod"",
		                                v.""Id"" as veId, v.""Id"", v.""IdModelo"", v.""Patente"", v.""Color"", v.""NumeroSerie"", anio, v.""IdCliente"", v.""FechaAlta"", v.""UsrAlta"", v.""FechaMod"", v.""UsrMod"", v.""FechaBaja"", v.""UsrBaja"",
		                                mv.""Id"" as mvId, mv.""Id"", mv.""IdMarca"", mv.""NombreModelo"",
		                                ma.""Id"" as maId, ma.""Id"", ma.""Nombre""  
                                from public.""Ordenes"" as o
	                                inner join public.""RecepcionVehiculo"" rv
			                                on rv.""Id"" = o.""IdRecepcion""
	                                left join public.""OrdenDetalles""  od
			                                on o.""Id"" = od.""OrdenId"" 
	                                LEFT JOIN public.""Repuestos"" r 
			                                ON od.""RepuestoId"" = r.""Id""
	                                LEFT JOIN public.""Servicios"" s 
			                                ON od.""ServicioId"" = s.""Id""
	                                inner join public.""Clientes"" as c
			                                on o.""IdCliente"" = c.""Id""
	                                inner join public.""Personas"" as p
			                                on c.""PersonaId"" = p.""Id""
	                                inner join public.""Vehiculos"" as v
			                                on o.""IdVehiculo"" = v.""Id""
	                                inner join public.""Modelovehiculos"" as mv
			                                on v.""IdModelo"" = mv.""Id""
	                                inner join public.""Marcavehiculos"" as ma
			                                on mv.""IdMarca"" = ma.""Id"" 
                                where o.""Id"" = @Id";


            using (var connection = dbConnection())
            {
                var ordenDict = new Dictionary<int, Orden>();

                var oOrders = await connection.QueryAsync<Orden>(
                    sql_query,
                    types: new[]
                    {
                        typeof(Orden),             // columnas de orden (o.*)
                        typeof(ItemVentaCreateDTO),// columnas de detalle (od.*)
                        typeof(RepuestoDTO),       // r.*
                        typeof(ServicioDTO),       // s.*
                        typeof(Cliente),           // c.*
                        typeof(Persona),           // p.*
                        typeof(Vehiculo),          // v.*
                        typeof(Modelovehiculo),    // mv.*
                        typeof(Marcavehiculo)      // ma.*
                    },
                    map: objects =>
                    {
                        var orden = (Orden)objects[0];
                        var item = (ItemVentaCreateDTO)objects[1];
                        var repuesto = (RepuestoDTO)objects[2];
                        var servicio = (ServicioDTO)objects[3];
                        var cliente = (Cliente)objects[4];
                        var persona = (Persona)objects[5];
                        var vehiculo = (Vehiculo)objects[6];
                        var modelovehiculo = (Modelovehiculo)objects[7];
                        var marcavehiculo = (Marcavehiculo)objects[8];

            // deduplicar por Id de orden (para filas múltiples por la misma orden)
                        if (!ordenDict.TryGetValue(orden.Id, out var currentOrden))
                        {
                            currentOrden = orden;

                // asignaciones anidadas (solo la primera vez que aparece la orden)
                            if (cliente != null)
                            {
                                cliente.Persona = persona;
                                currentOrden.Cliente = cliente;
                            }

                            if (vehiculo != null)
                            {
                                vehiculo.Modelovehiculo = modelovehiculo;
                                vehiculo.Modelovehiculo.Marcavehiculo = marcavehiculo;
                                currentOrden.Vehiculo = vehiculo;
                            }

                // inicializar lista
                            currentOrden.Items = currentOrden.Items ?? new List<ItemVentaCreateDTO>();

                            ordenDict.Add(currentOrden.Id, currentOrden);
                        }

            // agregar item si viene
                        if (item != null && (item.RepuestoId.HasValue || item.ServicioId.HasValue))
                        {
                            var itemFinal = new ItemVentaCreateDTO
                            {
                                ServicioId = item.ServicioId,
                                RepuestoId = item.RepuestoId,
                                Cantidad = item.Cantidad,
                                PrecioUnitario = item.PrecioUnitario,
                                Bonificacion = item.Bonificacion,
                                Subtotal = item.Subtotal,
                                Tipo = item.RepuestoId.HasValue ? "repuesto" : "servicio",
                                Nombre = item.RepuestoId.HasValue ? repuesto?.NombreRepuesto : servicio?.NombreServicio,
                                Descripcion = item.RepuestoId.HasValue ? repuesto?.DescripcionRepuesto : servicio?.DescripcionServicio
                            };

                            ordenDict[currentOrden.Id].Items.Add(itemFinal);
                        }

            // siempre devolver la instancia consolidada
                        return ordenDict[currentOrden.Id];
                    },
                    param: new { Id = id },
                    // splitOn: debe contener los alias donde comienza cada nuevo objeto (en el mismo orden).
                    splitOn: "itemid,repId,serId,clId,peId,veId,mvId,maId"
                );   /*
                var oOrders = await connection.QueryAsync<Orden, ItemVentaCreateDTO, Cliente, Persona, Vehiculo, Modelovehiculo, Marcavehiculo, dynamic, dynamic, Orden>(
                                sql_query,
                                map: (orden, item, cliente, persona, vehiculo, modelovehiculo, marcavehiculo, repuesto, servicio) =>
                                {
                                    //Evalua si el turno existe, lo reutiliza sin crear unos nuevo
                                    if (!ordenDict.TryGetValue(orden.Id, out var currentOrden))
                                    {
                                        currentOrden = orden;

                                        // Detectar si el ítem es un Repuesto o Servicio
                                        var itemFinal = new ItemVentaCreateDTO
                                        {
                                            ServicioId = item.ServicioId,
                                            RepuestoId = item.RepuestoId,
                                            Cantidad = item.Cantidad,
                                            PrecioUnitario = item.PrecioUnitario,
                                            Bonificacion = item.Bonificacion,
                                            Subtotal = item.Subtotal
                                        };

                                        if (item.RepuestoId.HasValue)
                                        {
                                            itemFinal.Tipo = "Repuesto";
                                            itemFinal.Nombre = repuesto?.NombreRepuesto;
                                            itemFinal.Descripcion = repuesto?.DescripcionRepuesto;

                                        }
                                        else if (item.ServicioId.HasValue)
                                        {
                                            itemFinal.Tipo = "Servicio";
                                            itemFinal.Nombre = servicio?.NombreServicio;
                                            itemFinal.Descripcion = servicio?.DescripcionServicio;
                                        }

                                        orden.Items ??= new List<ItemVentaCreateDTO>();
                                        orden.Items.Add(itemFinal);



                                        if (cliente != null)
                                        {
                                            cliente.Persona = persona;
                                            orden.Cliente = cliente;
                                        }

                                        if (vehiculo != null)
                                        {
                                            orden.Vehiculo = vehiculo;
                                        }

                                        if (modelovehiculo != null)
                                        {
                                            orden.Vehiculo.Modelovehiculo = modelovehiculo;
                                        }

                                        if (marcavehiculo != null)
                                        {
                                            orden.Vehiculo.Modelovehiculo.Marcavehiculo = marcavehiculo;
                                        }




                                        ordenDict.Add(orden.Id, currentOrden);


                                    }
                                    else
                                    {
                                        // si ya existe y viene otro item de detalle, lo agregamos
                                        if (item != null && item.ItemId != 0)
                                        {
                                            currentOrden.Items.Add(item);
                                        }
                                    }

                                    // siempre devolvemos algo
                                    return ordenDict[orden.Id];

                                },

                                new { Id = id },

                                splitOn: "odid, clId, peId, veId, mvId, maId"
                            );*/

                var orden = oOrders.FirstOrDefault();
                return orden;

            }
        }
    }
}
