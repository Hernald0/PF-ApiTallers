using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UTNApiTalleres.Data;
using UTNApiTalleres.Data.Repositorio.Interfaz;
using WebApiTalleres.Models;
using System.Linq;
using System.Data;

namespace UTNApiTalleres.Data.Repositorio
{
    public class ClienteDao : IClienteDao
    {
        private PostgresqlConfiguration _connectionString;


        public ClienteDao(PostgresqlConfiguration connectionString)
        {
            this._connectionString = connectionString;
        }

        protected NpgsqlConnection dbConnection()
        {
            return new NpgsqlConnection(this._connectionString.ConnectionString);
        }

        public async Task<Cliente> create(Cliente pCliente)
        {
            var db = dbConnection();

            Cliente newCliente = new Cliente();

            var sql_insert = @"INSERT INTO  public.""Clientes""( ""PersonaId"", ""TallerId"")
                                values( @PersonaId, @TallerId ) returning ""Id""";

            /*
            var parameters = new DynamicParameters();

            parameters.Add("PersonaId", Cliente.Persona.Id, DbType.Int32);
            parameters.Add("TallerId",  Cliente.Taller.Id,  DbType.Int32);

            var result = await db.ExecuteAsync(query, parameters);
            */

            int newClienteId = await db.QuerySingleAsync<int>(
                                            sql_insert,
                                            new
                                            {
                                                PersonaId = pCliente.Persona.Id,
                                                TallerId  = pCliente.Taller.Id
                                            });

            if (newClienteId > 0) {
                    sql_insert = @"INSERT INTO public.""Vehiculos""( ""IdModelo"", 
                                                                     ""Patente"", 
                                                                     ""Color"", 
                                                                     ""NumeroSerie"", 
                                                                     ""anio"",
                                                                     ""IdCliente"")
	                              VALUES (@idModelo, 
                                          @Patente,
                                          @Color, 
                                          @NumeroSerie,
                                          @anio,
                                          @IdCliente) returning ""Id"";";


                foreach (Vehiculo vehiculo in pCliente.Vehiculos)
                {

                    var newVehiculo = await db.QuerySingleAsync<int>(  sql_insert,
                                                                            new
                                                                            {
                                                                                IdModelo = vehiculo.Modelovehiculo.Id,
                                                                                Patente = vehiculo.Patente,
                                                                                Color = vehiculo.Color,
                                                                                NumeroSerie = vehiculo.NumeroSerie,
                                                                                anio = vehiculo.Anio,
                                                                                IdCliente = newClienteId
                                                                            }                                      
                                                                       );

                }



            }

            newCliente = await find(newClienteId);


            return newCliente;

        }


        public async Task<bool> update(Cliente Cliente)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> delete(int id)
        {

            var db = dbConnection();

            var sql_script = @"
                                DELETE 
                                FROM  public.""Clientes""
                                WHERE ""Id"" = @Id  
                            ";

            var result = await db.ExecuteAsync(sql_script, new { Id = id });

            return result > 0;
        }

        public async Task<Cliente> find(int idCliente)
        {
            /*var sql_query = @" select  * 
                                from public.""Clientes"" as a 
                                inner join
                                     public.""Personas"" as b
                                      on(a.""Id"" = b.""Id"")
                                inner join 
                                     public.""Talleres"" as c
                                      on(a.""TallerId"" = c.""Id"")
                                where b.""Id"" = @Id";    */
          
            var sql_query = @" select     cli.""Id"" as idcl, cli.*,
			                              per.""Id"" as idpe, per.*, 
			                              tal.""Id"" as idta, tal.*, 
			                              v.""Id"" as idve, v.*,
			                              mv.""Id"" as idmv, mv.*, 
			                               m.""Id"" as idma, m.*
	                            from public.""Clientes"" as cli 
			                            inner join
				                             public.""Personas"" as per
				                              on(cli.""PersonaId"" = per.""Id"")
			                            inner join 
				                             public.""Talleres"" as tal
				                              on(cli.""TallerId"" = tal.""Id"")
			                            left join
				                             public.""Vehiculos"" v
				                             on v.""IdCliente"" = cli.""Id""	 
			                            left join 
				                             public.""Modelovehiculos"" as mv
				                             on v.""IdModelo"" = mv.""Id""
			                            left join
				                             public.""Marcavehiculos"" as m
				                             on mv.""IdMarca"" = m.""Id""
                                 where cli.""Id"" = @Id";
     

            using (var connection = dbConnection())
            {
                /*
                var oCliente =  await connection.QueryAsync<Cliente, Persona, Taller, Cliente>(                             
                             sql_query,                             
                             (cliente, persona, taller) =>
                             {                                 
                                 cliente.Persona = persona;
                                 cliente.Taller = taller;
                                 return cliente;
                             },
                             new { Id = id }
                  
                         )
                   ;
                ;   */


                var oCliente = await connection.QueryAsync<Cliente, Persona, Taller, Vehiculo, Modelovehiculo, Marcavehiculo, Cliente>(sql_query,
                                          map: (cliente, persona, taller, vehiculo, modelovehiculo, marcavehiculo) =>
                                          {

                                              if (persona.Id > 0)
                                              {
                                                  cliente.Persona = (Persona)persona;
                                              
                                                  if (taller.Id > 0)
                                                  {
                                                      cliente.Taller = (Taller)taller;
                                                  }

                                                  if (vehiculo.Id > 0)
                                                  {
                                                 

                                                      Vehiculo vehiculoCliente = new Vehiculo();

                                                      vehiculoCliente = (Vehiculo) vehiculo;

                                                      if (modelovehiculo.Id > 0)
                                                          vehiculoCliente.Modelovehiculo = (Modelovehiculo)modelovehiculo;

                                                      if (marcavehiculo.Id > 0)
                                                          vehiculoCliente.Modelovehiculo.Marcavehiculo = (Marcavehiculo)marcavehiculo;


                                                      cliente.Vehiculos.Add((Vehiculo)vehiculoCliente);
                                                  }

                                               }; return cliente;
                                           
                                          
                                          },
                                          new { Id = idCliente },
                                          splitOn: "idcl, idpe, idta, idve, idmv, idma").ConfigureAwait(false);

                /*
                 * var marcaDict = new Dictionary<int, Marcavehiculo>();
                var marcas = await connection.QueryAsync<Marcavehiculo, Modelovehiculo, Marcavehiculo>(
                    query, (marca, modelo) =>
                    {
                        if (!marcaDict.TryGetValue(marca.Id, out var currentMarca))
                        {
                            currentMarca = marca;
                            marcaDict.Add(currentMarca.Id, currentMarca);
                        }
                        currentMarca.Modelovehiculos.Add(modelo);
                        return currentMarca;
                    }
                );
                return marcas.Distinct().ToList();*/
                
                return oCliente.FirstOrDefault();
            }
        }

        public async Task<IEnumerable<Cliente>> findAll()
        {
            var sql_query = @" select  * 
                                from public.""Clientes"" as a 
                                inner join
                                     public.""Personas"" as b
                                      on(a.""PersonaId"" = b.""Id"")
                                inner join 
                                     public.""Talleres"" as c
                                      on(a.""TallerId"" = c.""Id"")
                              ";
            

            using (var connection = dbConnection())
            {                

                var oCliente = await connection.QueryAsync<Cliente, Persona, Taller, Cliente>(

                             sql_query,
                             (cliente, persona, taller) =>
                             {
                                 cliente.Taller = taller;
                                 cliente.Persona = persona;
                                 
                                 return cliente;
                             }                         
                         )
                   ;
                ;

                return oCliente;
            }
        }
    }
}

