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
using UTNApiTalleres.Model;

namespace UTNApiTalleres.Data.Repositorio
{
    public class ClienteDao : IClienteDao
    {
        private PostgresqlConfiguration _connectionString;
        private IPersonaDao _personaDao;

        public ClienteDao(PostgresqlConfiguration connectionString,
                          IPersonaDao personaDao)
        {
            this._connectionString = connectionString;
            this._personaDao = personaDao;
        }

        protected NpgsqlConnection dbConnection()
        {
            return new NpgsqlConnection(this._connectionString.ConnectionString);
        }



        public async Task<Cliente> create(Cliente pCliente)
        {
            var db = dbConnection();


            if ( pCliente.Id == null || pCliente.Id == 0 ) {

                int IdPersona = await  this._personaDao.create(pCliente.Persona);

                Cliente newCliente = new Cliente();

                 var sql_insert = @"INSERT INTO  public.""Clientes""( ""PersonaId"", ""TallerId"")
                                    values( @PersonaId, @TallerId ) returning ""Id""";

                pCliente.Id = await db.QuerySingleAsync<int>(
                                            sql_insert,
                                            new
                                            {
                                                PersonaId = IdPersona,
                                                TallerId  = 1//pCliente.Taller.Id
                                            });

                db.Close();

            }

           

            if (pCliente.Id > 0) {
                /*
                var sql_insert = @"INSERT INTO public.""Vehiculos""( ""IdModelo"", 
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

                */

                foreach (Vehiculo vehiculo in pCliente.Vehiculos)
                {

                    if ( !vehiculo.Id.HasValue )
                    {
                        /*    
                        var newVehiculo = await db.QuerySingleAsync<int>(  sql_insert,
                                                                                new
                                                                                {
                                                                                    IdModelo = vehiculo.Modelovehiculo.Id,
                                                                                    Patente = vehiculo.Patente,
                                                                                    Color = vehiculo.Color,
                                                                                    NumeroSerie = vehiculo.NumeroSerie,
                                                                                    anio = vehiculo.Anio,
                                                                                    IdCliente = pCliente.Id
                                                                                }                                      
                                                                           );
                        */

                        this.insVehiculo(vehiculo, pCliente.Id);


                    }

                }



            }

            return await find(pCliente.Id);


          

        }

        public Vehiculo insVehiculo(Vehiculo pVehiculo,    int? pIdCliente)
        {
            var db = dbConnection();

            var sql_insert = @"INSERT INTO public.""Vehiculos""(   ""IdModelo"", 
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

            var newVehiculo =  db.QuerySingle<Vehiculo>(sql_insert,
                                                    new
                                                    {
                                                        IdModelo = pVehiculo.Modelovehiculo.Id,
                                                        Patente  = pVehiculo.Patente,
                                                        Color    = pVehiculo.Color,
                                                        NumeroSerie = pVehiculo.NumeroSerie,
                                                        anio      = pVehiculo.Anio,
                                                        IdCliente = pIdCliente
                                                    }
                                            );


            db.Close();

            return newVehiculo;

        }

        public async Task<Cliente> update(Cliente pCliente)
        {
            var db = dbConnection();


            if (pCliente.Id > 0)
            {

                Cliente newCliente = new Cliente();

                var sql_insert = @"UPDATE  public.""Clientes""
	                                SET  ""PersonaId""=@PersonaId, 
		                                    ""TallerId""=@TallerId
	                                WHERE ""Id"" = @Id";

               var affectedRows =  db.Execute(
                                            sql_insert,
                                            new
                                            {
                                                PersonaId = pCliente.Persona.Id,
                                                TallerId = pCliente.Taller.Id,
                                                Id = pCliente.Id
                                            });

            }

            
            {
                var sql_insert = @"INSERT INTO public.""Vehiculos""( ""IdModelo"", 
                                                                     ""Patente"", 
                                                                     ""Color"", 
                                                                     ""NumeroSerie"", 
                                                                     ""anio"",
                                                                     ""IdCliente"")
	                              VALUES (@IdModelo, 
                                          @Patente,
                                          @Color, 
                                          @NumeroSerie,
                                          @anio,
                                          @IdCliente) returning ""Id"";";


                var sql_update = @"UPDATE public.""Vehiculos""
                                    SET
                                        ""IdModelo"" = @IdModelo, 
		                                ""Patente"" = @Patente, 
		                                ""Color"" = @Color, 
		                                ""NumeroSerie"" = @NumeroSerie, 
		                                ""anio"" = @anio, 
		                                ""IdCliente"" = @IdCliente
                                    WHERE ""Id"" = @Id";


                foreach (Vehiculo vehiculo in pCliente.Vehiculos)
                {

                    if (vehiculo.Id == null)
                    {
                        var newVehiculo =// await db.QuerySingleAsync<int>(sql_insert,
                                            db.Execute(sql_insert,
                                                                             new
                                                                             {
                                                                                 IdModelo = vehiculo.Modelovehiculo.Id,
                                                                                 Patente = vehiculo.Patente,
                                                                                 Color = vehiculo.Color,
                                                                                 NumeroSerie = vehiculo.NumeroSerie,
                                                                                 anio = vehiculo.Anio,
                                                                                 IdCliente = pCliente.Id
                                                                             }
                                                                        );
                    }
                    else {
                        var newVehiculo =// await db.QuerySingleAsync<int>(sql_update,
                                            db.Execute(sql_update,
                                                                             new
                                                                             {
                                                                                 IdModelo = vehiculo.Modelovehiculo.Id,
                                                                                 Patente = vehiculo.Patente,
                                                                                 Color = vehiculo.Color,
                                                                                 NumeroSerie = vehiculo.NumeroSerie,
                                                                                 anio = vehiculo.Anio,
                                                                                 IdCliente = pCliente.Id,
                                                                                 Id = vehiculo.Id
                                                                             }
                                                                        );

                    }
                }



            }

            return await find(pCliente.Id);
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

        public async Task<bool> deleteVehiculo(int id)
        {

            var db = dbConnection();
            /*
            var sql_script = @"
                                DELETE 
                                FROM  public.""Vehiculos""
                                WHERE ""Id"" = @Id  
                            ";
            */

            var sql_script = @"
                                UPDATE public.""Vehiculos""
                                   SET ""FechaBaja"" = @fechaBaja,
                                       ""UsrBaja""   = @usuarioBaja
                                 WHERE ""Id""        = @Id  
                            ";

            var result = await db.ExecuteAsync(sql_script, new {
                                                                fechaBaja = DateTime.Now,
                                                                usuarioBaja = "HDCelaya",
                                                                Id = id 
                                                                });

            return result > 0;
        }


        public async Task<Cliente> find(int? pIdCliente)
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
			                               m.""Id"" as idma, m.*,
                                          tip.""Id"" as idti, tip.*
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
                                        left join
				                            public.""Tipoidentificadores"" as tip
                                            on tip.""Id"" = per.""IdTipoIdentificador""
                                 where cli.""Id"" = @Id
                                   and v.""FechaBaja"" is null";

            //var clientes =  null;

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


                /*
                var oCliente =  await  connection.QueryAsync<Cliente, Persona, Taller, Vehiculo, Modelovehiculo, Marcavehiculo, Cliente>(sql_query,
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


                                                      if (cliente.Vehiculos == null)
                                                      {
                                                          cliente.Vehiculos = new List<Vehiculo>();
                                                      }
                                                      
                                                      Vehiculo vehiculoCliente = new Vehiculo();

                                                      vehiculoCliente = (Vehiculo) vehiculo;

                                                      if (modelovehiculo.Id > 0)
                                                          vehiculoCliente.Modelovehiculo = (Modelovehiculo)modelovehiculo;

                                                      if (marcavehiculo.Id > 0)
                                                          vehiculoCliente.Modelovehiculo.Marcavehiculo = (Marcavehiculo)marcavehiculo;


                                                      cliente.Vehiculos.Add((Vehiculo)vehiculoCliente);
                                                  }

                                               };

                                              return cliente;
                                          },
                                          new { Id = idCliente },
                                          splitOn: "idcl, idpe, idta, idve, idmv, idma").ConfigureAwait(false);
                */

                var result = await connection.QueryAsync<Cliente, Persona, Taller, Vehiculo, Modelovehiculo, Marcavehiculo, TipoIdentificador, Cliente>(
                                sql_query,
                                (cliente, persona, taller, vehiculo, modelovehiculo, marcavehiculo, tipoidentificador) =>
                                {
                                    cliente.Persona = persona;
                                    cliente.Taller = taller;
                                    cliente.Persona.TipoIdentificador = tipoidentificador;
                                   
                                    if (vehiculo.Id != null)
                                    {
                                        if (cliente.Vehiculos == null)
                                        {
                                            cliente.Vehiculos = new List<Vehiculo>();
                                        }

                                        vehiculo.Modelovehiculo = modelovehiculo;
                                        //modelovehiculo.Marcavehiculo = marcavehiculo;
                                        cliente.Vehiculos.Add(vehiculo);

                                    }
                                    return cliente;
                                },
                                new { Id = pIdCliente },
                                splitOn: "idcl, idpe, idta, idve, idmv, idma, idti"
                            );

                var clientes = result.GroupBy(c => c.Id).Select(g =>
                {
                    var cliente = g.First();
                    cliente.Vehiculos = g.SelectMany(c => c.Vehiculos).ToList();
                    return cliente;
                });

                return (Cliente) clientes.FirstOrDefault();

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

                //return (Cliente) oCliente.FirstOrDefault();
            }

           
        }

        public async Task<Cliente> findByNroIdentificacion(vmIdentificador pvmIdentificador)
        {
            var sql_query = @" select     cli.""Id"" as idcl, cli.*,
			                              per.""Id"" as idpe, per.*, 
			                              tal.""Id"" as idta, tal.*, 
			                              v.""Id"" as idve, v.*,
			                              mv.""Id"" as idmv, mv.*, 
			                               m.""Id"" as idma, m.*,
                                          tip.""Id"" as idti, tip.*
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
                                        left join
				                            public.""Tipoidentificadores"" as tip
                                            on tip.""Id"" = per.""IdTipoIdentificador""
                                 where per.""NroIdentificacion"" = @NroIdentificacion
                                   and per.""IdTipoIdentificador"" = @IdTipoIdentificador
                                   and v.""FechaBaja"" is null";

            //var clientes =  null;

            using (var connection = dbConnection())
            {
                var result = await connection.QueryAsync<Cliente, Persona, Taller, Vehiculo, Modelovehiculo, Marcavehiculo, TipoIdentificador, Cliente>(
                                sql_query,
                                (cliente, persona, taller, vehiculo, modelovehiculo, marcavehiculo, tipoidentificador) =>
                                {
                                    cliente.Persona = persona;
                                    cliente.Taller = taller;
                                    cliente.Persona.TipoIdentificador = tipoidentificador;

                                    if (vehiculo.Id != null)
                                    {
                                        if (cliente.Vehiculos == null)
                                        {
                                            cliente.Vehiculos = new List<Vehiculo>();
                                        }

                                        vehiculo.Modelovehiculo = modelovehiculo;
                                        modelovehiculo.Marcavehiculo = marcavehiculo;
                                        cliente.Vehiculos.Add(vehiculo);

                                    }
                                    return cliente;
                                },
                                new {
                                    NroIdentificacion = pvmIdentificador.NroIdentificacion,
                                    IdTipoIdentificador = pvmIdentificador.TipoIdentificador
                                },
                                splitOn: "idcl, idpe, idta, idve, idmv, idma, idti"
                            );

            var clientes = result.GroupBy(c => c.Id).Select(g =>
            {
                var cliente = g.First();
                cliente.Vehiculos = g.SelectMany(c => c.Vehiculos).ToList();
                return cliente;
            });

            return (Cliente)clientes.FirstOrDefault();
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
                                left join 
                                     public.""Tipoidentificadores"" as t
                                      on(t.""Id"" = b.""IdTipoIdentificador"") 
                              ";
            

            using (var connection = dbConnection())
            {                

                var oCliente = await connection.QueryAsync<Cliente, Persona, Taller, TipoIdentificador, Cliente>(

                             sql_query,
                             (cliente, persona, taller, tipoidentificador) =>
                             {
                                 cliente.Taller = taller;
                                 cliente.Persona = persona;
                                 cliente.Persona.TipoIdentificador = tipoidentificador;


                                 return cliente;
                             }                         
                         )
                   ;
                ;

                return oCliente;
            }
        }

        public async Task<Cliente> InsertVehiculo( int? pIdCliente, Vehiculo pVehiculo)
        {
            var db = dbConnection();

            {
                var sql_insert = @"INSERT INTO public.""Vehiculos""( ""IdModelo"", 
                                                                     ""Patente"", 
                                                                     ""Color"", 
                                                                     ""NumeroSerie"", 
                                                                     ""anio"",
                                                                     ""IdCliente"")
	                              VALUES (@IdModelo, 
                                          @Patente,
                                          @Color, 
                                          @NumeroSerie,
                                          @anio,
                                          @IdCliente) returning ""Id"";";

                  
                var newVehiculo =  db.Execute(sql_insert,
                                                new
                                                {
                                                    IdModelo = pVehiculo.Modelovehiculo.Id,
                                                    Patente = pVehiculo.Patente,
                                                    Color = pVehiculo.Color,
                                                    NumeroSerie = pVehiculo.NumeroSerie,
                                                    anio = pVehiculo.Anio,
                                                    IdCliente = pIdCliente
                                                }
                                        );
                    
                 



            }

            return await find(pIdCliente);

        }

        public async Task<Cliente> UpdateVehiculo(int pIdCliente, Vehiculo pVehiculo)
        {
            var db = dbConnection();

            {
             
                var sql_update = @"UPDATE public.""Vehiculos""
                                    SET
                                        ""IdModelo"" = @IdModelo, 
		                                ""Patente"" = @Patente, 
		                                ""Color"" = @Color, 
		                                ""NumeroSerie"" = @NumeroSerie, 
		                                ""anio"" = @anio, 
		                                ""IdCliente"" = @IdCliente
                                    WHERE ""Id"" = @Id";


                
                
                var newVehiculo = db.Execute(sql_update,
                                                new
                                                {
                                                    IdModelo = pVehiculo.Modelovehiculo.Id,
                                                    Patente = pVehiculo.Patente,
                                                    Color = pVehiculo.Color,
                                                    NumeroSerie = pVehiculo.NumeroSerie,
                                                    anio = pVehiculo.Anio,
                                                    IdCliente = pIdCliente,
                                                    Id = pVehiculo.Id
                                                }
                                        );

                 
                



            }

            return await find(pIdCliente);
        }


    }
}

