using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTNApiTalleres.Data.Repositorio.Interfaz;
using UTNApiTalleres.Model;
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
                                    vd.""Cantidad"", vd.""PrecioUnitario"", vd.""Descuento"", vd.""SubTotal"",
    
                                    c.""Id"" as idc, c.*,
                                    p.""Id"" as idp, p.*,
    
                                    r.""Id"" as idr, r.""Nombre"" as ""NombreRepuesto"", r.""Descripcion"" as ""DescripcionRepuesto"", 
                                    r.""precioCosto"" as ""CostoRepuesto"", r.""precioVenta"" as ""VentaRepuesto"",
    
                                    s.""Id"" as ids, s.""Nombre"" as ""NombreServicio"", s.""Descripcion"" as ""DescripcionServicio"", 
                                    s.""precioCosto"" as ""CostoServicio"", s.""precioVenta"" as ""VentaServicio""
    
                                FROM public.""Ventas"" v
                                LEFT JOIN public.""VentaDetalles"" vd ON v.""Id"" = vd.""VentaId""
                                LEFT JOIN public.""Clientes"" c ON v.""ClienteId"" = c.""Id""
                                LEFT JOIN public.""Personas"" p ON c.""PersonaId"" = p.""Id""
                                LEFT JOIN public.""Repuestos"" r ON vd.""RepuestoId"" = r.""Id""
                                LEFT JOIN public.""Servicios"" s ON vd.""ServicioId"" = s.""Id""
                                WHERE v.""Id"" = @Id;";

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
                                        on c.""PersonaId"" = p.""Id""";



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

                foreach (var detalle in venta.Items)
                    {
                        detalle.VentaId = (int)venta.Id;
                        var detalleQuery = @"
                        INSERT INTO public.""VentaDetalles"" 
                        (""VentaId"", ""ServicioId"", ""RepuestoId"", ""Cantidad"", ""PrecioUnitario"", ""Descuento"", ""SubTotal"") 
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
                   
                }

               
            }
        }

        public void ModificarVenta(VentaCreateDTO venta)
        {
            using (var connection = dbConnection())
            {
                using (var transaction = connection.BeginTransaction())
                {
                    var query = @"
                    UPDATE public.""Venta"" 
                    SET ""ClienteId"" = @ClienteId, ""Fecha"" = @Fecha, ""MontoTotal"" = @MontoTotal, ""Estado"" = @Estado
                    WHERE ""Id"" = @Id;
                ";

                    connection.Execute(query, venta, transaction);

                    var deleteDetallesQuery = "DELETE FROM public.\"VentaDetalles\" WHERE \"VentaId\" = @Id;";
                    connection.Execute(deleteDetallesQuery, new { Id = venta.Id }, transaction);

                    foreach (var detalle in venta.Items)
                    {
                        detalle.VentaId = (int)venta.Id;
                        var detalleQuery = @"
                        INSERT INTO public.""VentaDetalles"" 
                        (""VentaId"", ""ServicioId"", ""RepuestoId"", ""Cantidad"", ""PrecioUnitario"", ""Descuento"", ""Subtotal"") 
                        VALUES (@VentaId, @ServicioId, @RepuestoId, @Cantidad, @PrecioUnitario, @Descuento, @Subtotal);
                    ";
                        connection.Execute(detalleQuery, detalle, transaction);
                    }

                    transaction.Commit();

                }
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

        public void CancelarVenta(int id)
        {
            new NotImplementedException();
        }
    }
}
