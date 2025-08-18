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

        public async Task<int>  AgregarOrder(RecepcionTurnoDTO orden)
        {

            var db = dbConnection();

            var sql = @"
                    INSERT INTO public.""Ordenes""(""IdTurno"", ""IdCliente"", ""IdVehiculo"")
	                VALUES(@IdTurno, @IdCliente, @IdVehiculo)  
                    returning  ""Id""";

            var rowNum =   db.Execute(sql, new
            {
                @IdTurno = orden.IdTurno,
                @IdCliente = orden.IdCliente,
                @IdVehiculo = orden.IdVehiculo

            });

            return rowNum;
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

            var sql_query = @"select   o.""Id"" as oId, o.""Id"", 
			                            c.""Id"" as clId, c.""Id"", c.""PersonaId"", c.""TallerId"",
			                            p.""Id"" as peId, p.""Id"", p.""Nombre"", p.""RazonSocial"", p.""Apellido"", p.""FecNacimiento"", p.""IdLocalidad"", p.""Barrio"", p.""Direccion"", p.""NroDireccion"", p.""Dpto"", p.""Piso"", p.""Telcelular"", p.""Telfijo"", p.""Email"", p.""IdTipoIdentificador"", p.""NroIdentificacion"", p.""TipoPersona"", p.""IdGenero"", p.""Ocupacion"", p.""IdEstadoCivil"", p.""FechaAlta"", p.""UsrAlta"", p.""FechaBaja"", p.""UsrBaja"", p.""FechaMod"", p.""UsrMod"",
			                            v.""Id"" as veId, v.""Id"", v.""IdModelo"", v.""Patente"", v.""Color"", v.""NumeroSerie"", anio, v.""IdCliente"", v.""FechaAlta"", v.""UsrAlta"", v.""FechaMod"", v.""UsrMod"", v.""FechaBaja"", v.""UsrBaja"",
			                            mv.""Id"" as mvId, mv.""Id"", mv.""IdMarca"", mv.""NombreModelo"",
			                            ma.""Id"" as maId, ma.""Id"", ma.""Nombre"" 
                                                                from public.""Ordenes"" as o
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

                var oOrders = await connection.QueryAsync<Orden, Cliente, Persona, Vehiculo, Modelovehiculo, Marcavehiculo, Orden>(
                                sql_query,
                                (orden, cliente, persona, vehiculo, modelovehiculo, marcavehiculo) =>
                                {
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

                                    return orden;
                                },
                                new { Id = id },
                                splitOn: "clId,peId,veId,mvId,maId"
                            );



                return oOrders.FirstOrDefault(); 

            }
        }
    }
}
