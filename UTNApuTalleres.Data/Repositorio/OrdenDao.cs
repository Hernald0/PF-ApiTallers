
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTNApiTalleres.Data.Repositorio.Interfaz;
using UTNApiTalleres.Model;
using WebApiTalleres.Models;
using Npgsql;
using Dapper;

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
                    INSERT INTO public.""Ordenes""(""IdTurno"", ""IdCliente"", ""IdVehiculo"",""IdRecepcion"")
	                VALUES(@IdTurno, @IdCliente, @IdVehiculo, @IdRecepcion)  
                    returning  ""Id""";

            var orderId = await  db.QuerySingleAsync<int>(sql, new
            {
                @IdTurno = orden.IdTurno,
                @IdCliente = orden.IdCliente,
                @IdVehiculo = orden.IdVehiculo,
                @IdRecepcion = RecepcionId

            });

            foreach (ItemVentaCreateDTO s in orden.Servicios)
            {
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

                var orderDetalleId = db.Execute(sqlDetalle, new
                {
                                  
                    @OrdenId = orderId,
                    @ServicioId = (s.Tipo == "servicio" ? s.ItemId : null), 
                    @RepuestoId = (s.Tipo == "respuesto" ? s.ItemId : null),
                    @PrecioUnitario = s.PrecioUnitario,
                    @Bonificacion = s.Bonificacion,
                    @Cantidad = s.Cantidad,
                    @SubTotal = s.Subtotal

                });


            }

            return orderId;
        }

        public int CancelarOrder(int id)
        {
            throw new NotImplementedException();
        }

        public void DeleteOrder(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Orden>> GetOrders()
        {
            var sql_query = @" select      o.""Id"" as ido, o.*,
	  	                                        c.""Id"" as idc, c.*, 
	  	                                        p.""Id"" as idp, p.* 
                                        from public.""Ordenes"" as o 

                                            inner join public.""Clientes"" as c
                                        on o.""IdCliente"" = c.""Id""

                                            inner join public.""Personas"" as p
                                        on c.""PersonaId"" = p.""Id""";



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
                                splitOn: "idc, idp"
                            );



                return oOrders;

            }
        }

        public void ModificarOrder(Orden orden)
        {
            throw new NotImplementedException();
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
    var sql_query = @"	select  o.""Id"" as oId, o.""Id"", 
		                                 rv.""FechaRecepcion"", rv.""HoraRecepcion"", rv.""Combustible"", rv.""Kilometraje"", rv.""IdAseguradora"", rv.""Inspector"", rv.""NroSiniestro"", rv.""Franquicia"", rv.""MotivoConsulta"",		
		                                od.""Id"" as itemid, od.""ServicioId"", od.""RepuestoId"", od.""PrecioUnitario"", od.""Bonificacion"", od.""Cantidad"", od.""SubTotal"",
                                        r.""Id"" as repId, r.""Nombre"" as ""NombreRepuesto"", r.""Descripcion"" as ""DescripcionRepuesto"", 
		                                r.""precioCosto"" as ""CostoRepuesto"", r.""precioVenta"" as ""VentaRepuesto"",
		                                s.""Id"" as serId, s.""Nombre"" as ""NombreServicio"", s.""Descripcion"" as ""DescripcionServicio"", 
		                                s.""precioCosto"" as ""CostoServicio"", s.""precioVenta"" as ""VentaServicio"",
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
                                Tipo = item.RepuestoId.HasValue ? "Repuesto" : "Servicio",
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
