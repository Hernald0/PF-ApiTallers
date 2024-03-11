using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTNApiTalleres.Data.Repositorio.Interfaz;
using UTNApiTalleres.Model;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Data.Repositorio
{
    public class TallerDao : ITallerDao
    {
        private PostgresqlConfiguration _connectionString;
        private readonly IPersonaDao _personaDao;

        public TallerDao(PostgresqlConfiguration connectionString)
        {
            this._connectionString = connectionString;
        }

        protected NpgsqlConnection dbConnection()
        {
            return new NpgsqlConnection(this._connectionString.ConnectionString);
        }
        public async Task<bool> create(Taller taller)
        {
            var db = dbConnection();

            var sql_insert = @"
                            INSERT INTO public.""Talleres""(""NombreTaller"", ""IdPersonaTit"")
                            VALUES ( @NombreTaller, @IdPersona)
                            ";

            var result = await db.ExecuteAsync(sql_insert, new
            {
                NombreTaller = taller.Nombretaller,
                IdPersona = taller.PersonaTitular.Id
            });

            return result > 0;
        }

        public Task<bool> delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Taller> find(int id)
        {
            var db = dbConnection();

            var sql_query = @"
                            SELECT *
                            FROM ""Personas"" as p
                            inner join ""Talleres"" as t
                                on(p.""Id"" = t.""ReferenteId"");
                            WHERE  t.""Id""= @Id ";

            using (var connection = dbConnection())
            {

                var oTaller = await connection.QueryAsync<Taller, Persona, Taller>(
                             sql_query,
                             (taller,persona) =>
                             {
                                 taller.PersonaTitular = persona;
                                
                                 return taller;
                             },
                             new { Id = id }

                         )
                   ;
                
                return oTaller.FirstOrDefault();
            }


        }

        public async Task<IEnumerable<Taller>> findAll()
        {
            var db = dbConnection();

            var sql_query = @"
                            SELECT *
                            FROM ""Personas"" as p
                            inner join ""Talleres"" as t
                                on(p.""Id"" = t.""ReferenteId"");
                             ";

            using (var connection = dbConnection())
            {

                var oTaller = await connection.QueryAsync<Taller, Persona, Taller>(
                             sql_query,
                             (taller, persona) =>
                             {
                                 taller.PersonaTitular = persona;

                                 return taller;
                             } 
                         )
                   ;

                return oTaller;
            }
        }

        public Task<bool> update(Taller Taller)
        {
            throw new NotImplementedException();
        }

 
        public async Task<IEnumerable<Empleado>> findEmpleadoAll( int idTaller)      
        {
            var sql_query = @" SELECT    emp.""Id"" as idEm, emp.*,
		                                  a.""Id"" as idpe, a.*, 
		                                  b.""Id"" as idec, b.*, 
		                                  c.""Id"" as idti, c.*, 
		                                  d.""Id"" as idge, d.*,
		                                  e.""Id"" as idlo, e.*, CONCAT(e.""CodigoPostal"", '-', e.""Nombre"") as cpNombre
                                FROM 	public.""Talleres"" as tal
			                                inner join 	
		                                public.""Empleados"" as emp 
			                                on tal.""Id"" = emp.""IdTaller""
			                                inner join
		                                public.""Personas"" as a 
			                                on emp.""IdPersona"" = a.""Id"" 
			                                left join
		                                public.""Estadociviles"" as b
			                                on b.""Id"" = a.""IdEstadoCivil"" 
			                                left join
		                                public.""Tipoidentificadores"" as c    
			                                on c.""Id"" = a.""IdTipoIdentificador""  
			                                left join
		                                public.""Generos"" as d    
			                                on d.""Id"" = a.""IdGenero"" 
			                                left join
		                                public.""Localidades"" as e    
			                                on e.""Id"" = a.""IdLocalidad"" where tal.""Id"" = @Id";


            using (var connection = dbConnection())
            {
             

                var oEmpleados = await connection.QueryAsync<Empleado, Persona, EstadoCivil, TipoIdentificador, Genero, Localidad, Empleado>(sql_query,
                                               map: (empleado, persona, EstadoCivil, tipoIdentificador, genero, localidad) =>
                                               {
                                                   if (persona.Id > 0)
                                                   {    
                                                       empleado.Persona = (Persona) persona;

                                                       if (EstadoCivil.Id > 0)
                                                           empleado.Persona.EstadoCivil = (EstadoCivil)EstadoCivil;

                                                       if (tipoIdentificador.Id > 0)
                                                           empleado.Persona.TipoIdentificador = (TipoIdentificador)tipoIdentificador;

                                                       if (genero.Id > 0)
                                                           empleado.Persona.Genero = (Genero)genero;

                                                       if (localidad.Id > 0)
                                                           empleado.Persona.Localidad = (Localidad)localidad;
                                                       
                                                   }; return empleado;
                                               },
                                               new { Id = idTaller },
                                               splitOn: "idEm,idpe,idec,idti,idge, idlo").ConfigureAwait(false);
              

                return oEmpleados;
            }


        }

        public async Task<bool> createEmpleado(Empleado empleado)
        {
            var db = dbConnection();

            //_personaDao.create(empleado.Persona);

            var sql_insert = @"
                            INSERT INTO public.""Empleados""(""IdTaller"", ""IdPersona"")
                            VALUES ( @IdTaller, @IdPersona)
                            ";

            var result = await db.ExecuteAsync(sql_insert, new
            {
                IdTaller = 1,
                IdPersona = 1
            }); 



            return result > 0;
        }
        public async Task<IEnumerable<Cliente>> findClienteAll(int idTaller)
        {
            var sql_query = @" SELECT    cli.""Id"" as idCl, cli.*,
		                                  a.""Id"" as idpe, a.*, 
		                                  b.""Id"" as idec, b.*, 
		                                  c.""Id"" as idti, c.*, 
		                                  d.""Id"" as idge, d.*,
		                                  e.""Id"" as idlo, e.*, CONCAT(e.""CodigoPostal"", '-', e.""Nombre"") as cpNombre
                                FROM 	public.""Talleres"" as tal
			                                inner join 	
		                                public.""Clientes"" as cli 
			                                on tal.""Id"" = cli.""TallerId""
			                                inner join
		                                public.""Personas"" as a 
			                                on cli.""PersonaId"" = a.""Id"" 
			                                left join
		                                public.""Estadociviles"" as b
			                                on b.""Id"" = a.""IdEstadoCivil"" 
			                                left join
		                                public.""Tipoidentificadores"" as c    
			                                on c.""Id"" = a.""IdTipoIdentificador""  
			                                left join
		                                public.""Generos"" as d    
			                                on d.""Id"" = a.""IdGenero"" 
			                                left join
		                                public.""Localidades"" as e    
			                                on e.""Id"" = a.""IdLocalidad"" where tal.""Id"" = @Id";


            using (var connection = dbConnection())
            {


                var oClientes = await connection.QueryAsync<Cliente, Persona, EstadoCivil, TipoIdentificador, Genero, Localidad, Cliente>(sql_query,
                                               map: (cliente, persona, EstadoCivil, tipoIdentificador, genero, localidad) =>
                                               {
                                                   if (persona.Id > 0)
                                                   {
                                                       cliente.Persona = (Persona)persona;

                                                       if (EstadoCivil.Id > 0)
                                                           cliente.Persona.EstadoCivil = (EstadoCivil)EstadoCivil;

                                                       if (tipoIdentificador.Id > 0)
                                                           cliente.Persona.TipoIdentificador = (TipoIdentificador)tipoIdentificador;

                                                       if (genero.Id > 0)
                                                           cliente.Persona.Genero = (Genero)genero;

                                                       if (localidad.Id > 0)
                                                           cliente.Persona.Localidad = (Localidad)localidad;

                                                   }; return cliente;
                                               },
                                               new { Id = idTaller },
                                               splitOn: "idCl,idpe,idec,idti,idge, idlo").ConfigureAwait(false);


                return oClientes;
            }


        }


        public async Task<IEnumerable<Marcavehiculo>> findMarcaVehiculoAll()
        {
            //var db = dbConnection();

            var sql_query = @"
                            SELECT ""Id"", ""Nombre""
                            FROM public.""Marcavehiculos"" 
                           ";

            using (var db = dbConnection())
            {
                var oMarcasVehiculo = await db.QueryAsync<Marcavehiculo>(sql_query);

                return oMarcasVehiculo.ToList();

            }
        }

        public async Task<Marcavehiculo> findMarcaModeloVehiculo(int idMarca)
        {


            var query = @"SELECT * FROM public.""Marcavehiculos""  WHERE ""Id""  = @Id  order by ""Id"" asc;  
                          SELECT * FROM public.""Modelovehiculos"" WHERE ""IdMarca""  = @Id  order by ""Id"" asc";
            using (var connection = dbConnection())
            
            using (var multi = await connection.QueryMultipleAsync(query, new { Id = idMarca }))
            {
                var oMarcaVehiculo = await multi.ReadSingleOrDefaultAsync<Marcavehiculo>();
                if (oMarcaVehiculo != null)
                    oMarcaVehiculo.Modelovehiculos = (await multi.ReadAsync<Modelovehiculo>()).ToList();
                return oMarcaVehiculo;
            }
            /*
            var sql_query = @"
                            SELECT mar.""Id"" as IdMa, mar.*,
                                   mod.""Id"" as IdMo, mod.*
                                FROM public.""Marcavehiculos"" as mar
                                inner join public.""Modelovehiculos"" as mod
                            on  mar.""Id"" = mod.""IdMarca""
                            where mar.""Id"" = @Id or @Id = null
                             ";

            using (var connection = dbConnection())
            {

                var oMarcavehiculo = await connection.QueryAsync<Marcavehiculo, Modelovehiculo, Marcavehiculo>(
                             sql_query,
                             (marca, modelo) =>
                             {
                                 marca.Modelovehiculos.Add (modelo);

                                 return marca;
                             },
                             new { Id = idMarca },
                             splitOn:  "IdMo"  
                         ) ;
                return oMarcavehiculo.ToList();


            }*/
        }


        public async Task<List<Marcavehiculo>> findMarcaModeloVehiculoAll()
        {
            var query = @" SELECT mar.""Id"" as IdMa, mar.*,
                                   mod.""Id"" as IdMo, mod.*
                             FROM public.""Marcavehiculos"" as mar
                                inner join public.""Modelovehiculos"" as mod
                            on  mar.""Id"" = mod.""IdMarca""
                            "; ;
            using (var connection = dbConnection())
            {
                var marcaDict = new Dictionary<int, Marcavehiculo>();
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
                return marcas.Distinct().ToList();
            }
        }

        public async Task<Modelovehiculo> createMarcaModelo(mvvmModelovehiculo marcaModelo)
        {
            int newMarcaId = -1;
            var db = dbConnection();

            Modelovehiculo newModelo = null;
            /*
            if (marcaModelo.idMarca == null & !String.IsNullOrEmpty(marcaModelo.nombreMarca))
            { 

                var sql_insert = @"
                INSERT INTO public.""Marcavehiculos""(""Nombre"")       
                VALUES (@NombreMarca)
                returning ""Id"";";


                newMarcaId = await db.QuerySingleAsync<int>(
                                            sql_insert,
                                            new { 
                                                    NombreMarca = marcaModelo.nombreMarca 
                                                });

                return newMarcaId;
            }
            */

            if (marcaModelo.idMarca  != null & //Si la marca tiene id
                marcaModelo.idModelo == null & 
                !String.IsNullOrEmpty(marcaModelo.nombreModelo)
                )
            {
               

                var sql_insert = @"
                INSERT INTO public.""Modelovehiculos""(""Nombre"", ""IdMarca"")       
                VALUES (@NombreModelo, @IdMarca)
                returning *;";


                 newModelo = await db.QuerySingleAsync<Modelovehiculo>(
                                            sql_insert,
                                            new
                                            {
                                                NombreModelo = marcaModelo.nombreModelo,
                                                IdMarca = marcaModelo.idMarca
                                            });



            }

            return newModelo;
        }

        //Actualizar Datos Marca
        //Actualizar Datos Modelo
        public async Task<mvvmModelovehiculo?> updateMarcaModelo(mvvmModelovehiculo marcaModelo)
        {
         
            var db = dbConnection();

            mvvmModelovehiculo? updatedModelo = marcaModelo;

            if (marcaModelo.idMarca != null & 
                marcaModelo.idModelo != null & 
                !String.IsNullOrEmpty(marcaModelo.nombreMarca) &
                !String.IsNullOrEmpty(marcaModelo.nombreModelo))
            {

                var sql_update = @"
                update public.""Modelovehiculos"" as Modelo
                    set ""Nombre"" = @NombreModelo
                    FROM  public.""Marcavehiculos"" as Marca
                    WHERE Marca.""Id"" = Modelo.""IdMarca""
                    and Modelo.""Id"" = @IdModelo
                    and Marca.""Id"" = @IdMarca
                    returning Marca.""Id"" as idMarca,
                                Marca.""Nombre"" as nombreMarca,
                                Modelo.""Id"" as idModelo,
                                Modelo.""Nombre"" as nombreModelo;";

        



                 updatedModelo = await db.QuerySingleAsync<mvvmModelovehiculo>(
                                            sql_update,
                                            new
                                            {   
                                                IdMarca = marcaModelo.idMarca,
                                                IdModelo = marcaModelo.idModelo,
                                                NombreModelo = marcaModelo.nombreModelo
                                            });

                
            }

            return updatedModelo;

            }

        //Eliminar Marca
        //Eliminar Modelo
        public async Task<int?> deleteMarcaModelo(int id)//(mvvmModelovehiculo marcaModelo)
        {
            int affectedRows = 0;
            var db = dbConnection();

            //mvvmModelovehiculo? deleteModelo = marcaModelo;

            /*if (
                marcaModelo.idModelo != null)*/
            {

                var sql_delete = @"
                delete from public.""Modelovehiculos"" as Modelo
                 WHERE Modelo.""Id"" = @IdModelo
                     ;";


                 affectedRows = await db.ExecuteAsync(
                                                            sql_delete,
                                                            new
                                                            {
                                                                IdModelo = id //marcaModelo.idModelo
                                                            }
                                                      );


            }

            return affectedRows;

        }

        public async Task<Servicio> createServicio(Servicio servicio)
        {
            var sql_insert = @"INSERT INTO public.""Servicios"" (""Nombre"",
                                                                 ""Descripcion"",                                                                 
                                                                 ""UsuarioAlta"",
                                                                 ""FechaAlta"")
                               VALUES ( @Nombre, @Descripcion, @UsuarioAlta, @FechaAlta  )
                             returning *;";
            /*
            var parameters = new DynamicParameters();

            parameters.Add("Nombre", servicio.Nombre, DbType.String);
            parameters.Add("Descripcion", servicio.Descripcion, DbType.String);
            parameters.Add("UsuarioAlta", "HCELAYA", DbType.String);
            parameters.Add("FechaAlta", DateTime.Now, DbType.DateTime);
            */
            using (var db = dbConnection())
            {
                //var result = await connection.ExecuteAsync(sql_insert, parameters);

                //return result > 0;
                var newServicio = await db.QuerySingleAsync<Servicio>(
                                            sql_insert,
                                            new
                                            {
                                                Nombre= servicio.Nombre,
                                                Descripcion = servicio.Descripcion,
                                                FechaAlta = DateTime.Now,
                                                UsuarioAlta = servicio.UsuarioAlta
                                            });

                return newServicio;
            }
        }


        public async Task<IEnumerable<Servicio>> findAllServicio()
        {

            var sql_query = @"
                            SELECT ""Id"", 
                                    ""Nombre"",
                                    ""Descripcion"",
                                    ""UsuarioAlta"",
                                    ""FechaAlta"",
                                    ""UsuarioBaja"",
                                    ""FechaBaja""
                            FROM public.""Servicios"" 
                           ";

            using (var db = dbConnection())
            {
                var oServicio = await db.QueryAsync<Servicio>(sql_query);

                return oServicio.ToList();

            }
        }

        public async Task<int> deleteServicio(int IdServicio)
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

        public async Task<Servicio> updateServicio(Servicio servicio)
        {
            var db = dbConnection();

                   
                var sql_update = @"
                update public.""Servicios"" as Servicios
                    set ""Nombre"" = @NombreServicio,
                        ""Descripcion"" = @DescripcionServicio      
                    WHERE public.""Servicios"".""Id"" = @IdServicio
                     returning  Servicios.""Id"" as idMarca,
                                Servicios.""Nombre"" as nombre,
                                Servicios.""FechaAlta"" as fechaAlta,
                                Servicios.""UsuarioAlta"" as usuarioAlta
                                Servicios.""FechaBaja"" as fechaBaja
                                Servicios.""UsuarioBaja"" as usuarioBaja;";


              var  updatedSerivio= await db.QuerySingleAsync<Servicio>(
                                           sql_update,
                                           new
                                           {
                                               IdServicio = servicio.Id,
                                               NombreServicio = servicio.Nombre,
                                               DescripcionServicio = servicio.Descripcion
                                           });



            return updatedSerivio;
        }

        public Task<Servicio> findServicio(int IdServicio)
        {
            throw new NotImplementedException();
        }
    }

        

    }