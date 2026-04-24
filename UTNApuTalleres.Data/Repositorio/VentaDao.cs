using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTNApiTalleres.Data.Repositorio.Interfaz;
using Npgsql;
using Dapper;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Data.Repositorio
{
    public class VentaDao : IVentaDao
    {
        private PostgresqlConfiguration _connectionString;

        public VentaDao(PostgresqlConfiguration connectionString)
        {
            this._connectionString = connectionString;
        }

        protected NpgsqlConnection dbConnection()
        {
            return new NpgsqlConnection(this._connectionString.ConnectionString);
        }

        public async  Task<Venta>   ObtenerPorId(int id)
        {
            /* anda OK
            var sql_query = @" select       v.""Id"" as idv, v.*,
			                                vd.""Id"" as iddv, vd.*,
			                                c.""Id"" as idc, c.*, 
			                                p.""Id"" as idp, p.* 
	                                from public.""Ventas"" as v 
		                                left join public.""VentaDetalles"" as vd
			                                on v.""Id"" = vd.""VentaId""
		                                left join public.""Clientes"" as c
			                                on v.""ClienteId"" = c.""Id""
		                                left join public.""Personas"" as p
			                                on c.""PersonaId"" = p.""Id""
                             where    v.""Id"" = @Id
                             ";



            using (var connection = dbConnection())
            {
                var result = await connection.QueryAsync<Venta, ItemVentaCreateDTO, Cliente, Persona, Venta>(
                    sql_query,
                    (venta, itemDetalle, cliente, persona) =>
                    {
                        venta.Cliente = cliente;
                        venta.Cliente.Persona = persona;

                        if (itemDetalle?.VentaId != null)
                        {
                            venta.Items ??= new List<ItemVentaCreateDTO>();
                            venta.Items.Add(itemDetalle);
                        }

                        return venta;
                    },
                    new { Id = id },
                    splitOn: "iddv,idc,idp"
                );
            */
            var sql_query = @"
                                SELECT 
                                    v.""Id"" as idv, v.*,
    
                                    vd.""Id"" as iddv, vd.""VentaId"", vd.""ServicioId"", vd.""RepuestoId"",
                                    vd.""Cantidad"", vd.""PrecioUnitario"", vd.""Bonificacion"", vd.""SubTotal"",
    
                                    c.""Id"" as idc, c.*,
                                    p.""Id"" as idp, p.*,
    
                                    r.""Id"" as idr, r.""Nombre"" as ""NombreRepuesto"", r.""Descripcion"" as ""DescripcionRepuesto"", 
                                    r.""PrecioCosto"" as ""CostoRepuesto"", r.""PrecioVenta"" as ""VentaRepuesto"",
    
                                    s.""Id"" as ids, s.""Nombre"" as ""NombreServicio"", s.""Descripcion"" as ""DescripcionServicio"", 
                                    s.""PrecioCosto"" as ""CostoServicio"", s.""PrecioVenta"" as ""VentaServicio""
    
                                FROM public.""Ventas"" v
                                LEFT JOIN public.""VentaDetalles"" vd ON v.""Id"" = vd.""VentaId""
                                LEFT JOIN public.""Clientes"" c ON v.""ClienteId"" = c.""Id""
                                LEFT JOIN public.""Personas"" p ON c.""PersonaId"" = p.""Id""
                                LEFT JOIN public.""Repuestos"" r ON vd.""RepuestoId"" = r.""Id""
                                LEFT JOIN public.""Servicios"" s ON vd.""ServicioId"" = s.""Id""
                                WHERE v.""Id"" = @Id
                                ORDER BY v.""Id"" desc;";

            using (var connection = dbConnection())
            {
                var result = await connection.QueryAsync<Venta, ItemVentaCreateDTO, Cliente, Persona, dynamic, dynamic, Venta>(
                    sql_query,
                    (venta, item, cliente, persona, repuesto, servicio) =>
                    {

                        
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

                        venta.Items ??= new List<ItemVentaCreateDTO>();
                        venta.Items.Add(itemFinal);

                        venta.Cliente ??= cliente; // Asigna si es null
                        if (venta.Cliente != null)
                        {
                            venta.Cliente.Persona = persona;
                        }

                        return venta;
                    },
                    new { Id = id },
                    splitOn: "iddv,idc,idp,idr,ids"
                );

                var ventas = result
                        .GroupBy(v => v.Id)
                        .Select(g =>
                        {
                            var venta = g.First();
                            venta.Items = g.SelectMany(x => x.Items ?? new List<ItemVentaCreateDTO>()).ToList();
                            return venta;
                        }).ToList();

                return ventas.FirstOrDefault(); // o retornar la lista si esperás varias
            }
        }

        public async Task<IEnumerable<Venta>> ObtenerTodas()
        {

           
            var sql_query = @" select       v.""Id"" as idv, v.*,
	  	                                        c.""Id"" as idc, c.*, 
	  	                                        p.""Id"" as idp, p.* 
                                        from public.""Ventas"" as v 

                                            inner join public.""Clientes"" as c
                                        on v.""ClienteId"" = c.""Id""

                                            inner join public.""Personas"" as p
                                        on c.""PersonaId"" = p.""Id""
                                 ORDER BY v.""Id"" desc;";



            using (var connection = dbConnection())
            {

                var oVentas = await connection.QueryAsync<Venta, Cliente, Persona, Venta>(
                                sql_query,
                                (venta, cliente, persona) =>
                                {
                                    venta.Cliente =  cliente;
                                    venta.Cliente.Persona =  persona;

                                    return  venta;
                                },
                                splitOn: "idc, idp"
                            );
                    
                

                return oVentas;
                
            }
        }

        public async Task<int> AgregarVentaOrden(OrdenDTO orden)
        {
            string insert = @"INSERT INTO public.""Ventas""(
	                                                        ""ClienteId"", 
                                                            ""VehiculoId"", 
                                                            ""FechaEmision"", 
                                                            ""Observaciones"", 
                                                            ""MontoTotal"", 
                                                            ""Usuario"", 
                                                            ""Descuento"", 
                                                            ""Efectivo"", 
                                                            ""TarjetaCredito"",
                                                            ""MontoTarjetaCredito"", 
                                                            ""CuentaCorriente"", 
                                                            ""Estado"",
                                                            ""NroVenta"", 
                                                            ""OrdenId"")
	                                               VALUES ( @ClienteId, 
                                                            @VehiculoId, 
                                                            @FechaEmision, 
                                                            @Observaciones, 
                                                            @MontoTotal,
                                                            @Usuario,
                                                            @Descuento, 
                                                            @Efectivo, 
                                                            @TarjetaCredito, 
                                                            @MontoTarjetaCredito,
                                                            @CuentaCorriente, 
                                                            @Estado, @NroVenta, @OrdenId) returning ""Id""";

            Venta venta = null ;

            using (var connection = dbConnection())
            {
             
                var paramCabecera = new
                {
                   
                        @ClienteId = orden.IdCliente,
                        @VehiculoId = orden.IdVehiculo,
                        @FechaEmision = (DateTime?)null,
                        @Observaciones = (string?)null,
                        @Usuario = orden.Usuario,
                        @OrdenId = orden.IdOrden,
                        @MontoTotal = (decimal?)null,
                        @Descuento = (decimal?)null,
                        @Efectivo = (decimal?)null,
                        @TarjetaCredito = (decimal?)null,
                        @MontoTarjetaCredito = (decimal?)null,
                        @CuentaCorriente = (decimal?)null,
                        @Estado = "presupuesto",
                        @NroVenta = (int?)null
                         

                };

                int VentaId = await connection.QuerySingleAsync<int>(insert, paramCabecera);

                if (orden.Servicios != null && orden.Servicios.Any())
                {
                      this.CrearDetalles(VentaId, orden.Servicios);
                };

                return VentaId;
            }; 
            
           
        } 

        public void AgregarVenta(VentaCreateDTO venta)
        {
            using (var connection = dbConnection())
            {

                string query;

                if (venta.TipoOperacion == "venta")
                {
                    query = @"
                                INSERT INTO public.""Ventas"" (
                                    ""ClienteId"", ""FechaEmision"", ""Efectivo"", ""TarjetaCredito"",
                                    ""MontoTarjetaCredito"", ""CuentaCorriente"", ""MontoTotal"",  ""Descuento"",
                                    ""Observaciones"", ""Estado"", ""Usuario"", ""NroVenta"")
                                VALUES (
                                    @ClienteId, @FechaEmision, @Efectivo, @TarjetaCredito,
                                    @MontoTarjetaCredito, @CuentaCorriente, @MontoTotal, @Descuento,
                                    @Observaciones, @Estado, @Usuario, nextval('nro_venta_seq'))
                                RETURNING ""Id"", ""NroVenta"";
                            ";
                }
                else
                {
                    query = @"
                                INSERT INTO public.""Ventas"" (
                                    ""ClienteId"", ""FechaEmision"", ""Efectivo"", ""TarjetaCredito"",
                                    ""MontoTarjetaCredito"", ""CuentaCorriente"", ""MontoTotal"",  ""Descuento"",
                                    ""Observaciones"",  ""Estado"", ""Usuario"")
                                VALUES (
                                    @ClienteId, @FechaEmision, @Efectivo, @TarjetaCredito,
                                    @MontoTarjetaCredito, @CuentaCorriente, @MontoTotal, @Descuento,
                                    @Observaciones, @Estado, @Usuario)
                                RETURNING ""Id"";
                            ";
                }


                var paramCabecera = new
                {
                    @ClienteId = venta.ClienteId,
                    @FechaEmision = venta.FechaEmision, 
                    @Descuento = venta.Descuento,
                    @Efectivo = venta.Efectivo,
                    @TarjetaCredito = venta.TarjetaCredito,
                    @MontoTarjetaCredito = venta.MontoTarjetaCredito,
                    @CuentaCorriente = venta.CuentaCorriente,
                    @MontoTotal = venta.MontoTotal,
                    @Observaciones = venta.Observaciones,
                    @Usuario = venta.Usuario,
                    @Estado =  venta.TipoOperacion 
                };  

                if (venta.TipoOperacion == "venta")
                {
                    // Devolver Id y NroVenta
                    var result = connection.QueryFirst<(int Id, int NroVenta)>(query, paramCabecera);
                    venta.Id = result.Id;
                    venta.NroVenta = result.NroVenta;
                }
                else
                {
                    venta.Id = connection.ExecuteScalar<int>(query, paramCabecera);
                }


                if (venta.Items != null && venta.Items.Any())
                {
                    this.CrearDetalles(venta.Id, venta.Items);
                };
                /*
                foreach (var detalle in venta.Items)
                    {
                        detalle.VentaId = (int)venta.Id;
                        var detalleQuery = @"
                        INSERT INTO public.""VentaDetalles"" 
                        (""VentaId"", ""ServicioId"", ""RepuestoId"", ""Cantidad"", ""PrecioUnitario"", ""Bonificacion"", ""SubTotal"") 
                        VALUES (@VentaId, @ServicioId, @RepuestoId, @Cantidad, @PrecioUnitario, @Descuento, @Subtotal);
                    ";

                    var paramItem = new
                    {
                        @VentaId = detalle.VentaId,
                        @ServicioId = (detalle.Tipo == "servicio") ? (object)detalle.ItemId : DBNull.Value,
                        @RepuestoId = (detalle.Tipo == "repuesto") ? (object)detalle.ItemId : DBNull.Value, 
                        @Cantidad = detalle.Cantidad,
                        @PrecioUnitario = detalle.PrecioUnitario,
                        @Descuento = detalle.Bonificacion,
                        @Subtotal = detalle.Subtotal
                    };

                    connection.Execute(detalleQuery, paramItem);
                   */
                

               
            }
        }

        private void CrearDetalles(int? IdVenta, List<ItemVentaCreateDTO> Items)
        {

           
            if (Items != null)
            {

                using (var connection = dbConnection())
                {
                    foreach (var detalle in Items)
                    {
                        detalle.VentaId = (int)IdVenta;
                        var detalleQuery = @"
                                INSERT INTO public.""VentaDetalles"" 
                                (""VentaId"", ""ServicioId"", ""RepuestoId"", ""Cantidad"", ""PrecioUnitario"", ""Bonificacion"", ""SubTotal"") 
                                VALUES (@VentaId, @ServicioId, @RepuestoId, @Cantidad, @PrecioUnitario, @Descuento, @Subtotal);
                            ";

                        var paramItem = new
                        {
                            @VentaId = IdVenta,
                            @ServicioId = (detalle.Tipo.ToLower() == "servicio") ? (object)detalle.ItemId : DBNull.Value,
                            @RepuestoId = (detalle.Tipo.ToLower() == "repuesto") ? (object)detalle.ItemId : DBNull.Value,
                            @Cantidad = detalle.Cantidad,
                            @PrecioUnitario = detalle.PrecioUnitario,
                            @Descuento = detalle.Bonificacion,
                            @Subtotal = detalle.Subtotal
                        };

                        connection.Execute(detalleQuery, paramItem);

                    }
                }
            }

        }

        public int? ModificarVenta(VentaCreateDTO venta)
        {
            using (var connection = dbConnection())
            {
                //using (var transaction = connection.BeginTransaction())
                //{ 

          
                    var query = @"
                    UPDATE public.""Ventas""
	                    SET  
		                    ""ClienteId""= @ClienteId , 
		                    ""VehiculoId""= @VehiculoId, 
		                    ""FechaEmision""= @FechaEmision, 
		                    ""Observaciones""= @Observaciones, 
		                    ""MontoTotal""= @MontoTotal, 
		                    ""Usuario""= @Usuario, 
		                    ""Descuento""= @Descuento, 
		                    ""Efectivo""= @Efectivo, 
		                    ""TarjetaCredito""= @TarjetaCredito, 
		                    ""MontoTarjetaCredito""= @MontoTarjetaCredito, 
		                    ""CuentaCorriente""= @CuentaCorriente, 
		                    ""Estado""= @Estado,  
                            ""NroVenta"" = CASE 
                                                WHEN @Estado = 'venta'
                                                THEN nextval('nro_venta_seq')
                                                ELSE ""NroVenta""
                                             END
                        WHERE ""Id""= @Id 
                        Returning ""NroVenta""
                ";

                var paramCabecera = new
                {

                    @ClienteId = venta.ClienteId,
                    @VehiculoId = venta.VehiculoId,
                    @FechaEmision = (DateTime?) DateTime.Now,
                    @Observaciones = (string?)venta.Observaciones,
                    @Usuario = venta.Usuario,
                    @MontoTotal = (decimal?)venta.MontoTotal,
                    @Descuento = (decimal?)venta.Descuento,
                    @Efectivo = (decimal?)venta.Efectivo,
                    @TarjetaCredito = (string?)venta.TarjetaCredito,
                    @MontoTarjetaCredito = (decimal?)venta.MontoTarjetaCredito,
                    @CuentaCorriente = (decimal?)venta.CuentaCorriente,
                    @Estado = venta.TipoOperacion,
                    @Id = venta.Id,


                };

                //connection.Execute(query,  paramCabecera  );
                int? NroVenta =   connection.QuerySingle<int?>(query, paramCabecera);

                var deleteDetallesQuery = "DELETE FROM public.\"VentaDetalles\" WHERE \"VentaId\" = @Id;";
                    connection.Execute(deleteDetallesQuery, new { Id = venta.Id });

                    if (venta.Items != null && venta.Items.Any())
                    {
                        this.CrearDetalles(venta.Id, venta.Items);
                    };

                /*
                    foreach (var detalle in venta.Items)
                    {
                        detalle.VentaId = (int)venta.Id;
                        var detalleQuery = @"
                        INSERT INTO public.""VentaDetalles"" 
                        (""VentaId"", ""ServicioId"", ""RepuestoId"", ""Cantidad"", ""PrecioUnitario"", ""Descuento"", ""Subtotal"") 
                        VALUES (@VentaId, @ServicioId, @RepuestoId, @Cantidad, @PrecioUnitario, @Descuento, @Subtotal);
                    ";
                        connection.Execute(detalleQuery, detalle);
                    }
                */

                return NroVenta;

            }
        }

        public void DeleteVenta(int id)
        {
            using (var connection = dbConnection())
            {
                using (var transaction = connection.BeginTransaction())
                {
                    var deleteDetallesQuery = "DELETE FROM public.\"VentaDetalles\" WHERE \"VentaId\" = @Id;";
                    connection.Execute(deleteDetallesQuery, new { Id = id }, transaction);

                    var deleteVentaQuery = "DELETE FROM public.\"Venta\" WHERE \"Id\" = @Id;";
                    connection.Execute(deleteVentaQuery, new { Id = id }, transaction);

                    transaction.Commit();
                }
            }
        }

        public int CancelarVenta(int id)
        {
            var db = dbConnection();

            var sql = @" UPDATE  public.""Ventas"" 
                         SET ""Estado"" = 'cancelado'
                         WHERE ""Id"" = @Id";

            var filasAfectadas = db.Execute(sql, new { Id = id });

            db.Close();

            return filasAfectadas;
        }
    }
}
