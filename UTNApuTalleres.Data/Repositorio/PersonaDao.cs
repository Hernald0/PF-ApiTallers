using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTNApiTalleres.Data.Repositorio.Interfaz;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Data.Repositorio
{
    public class PersonaDao : IPersonaDao
    {
        private PostgresqlConfiguration _connectionString;


        public PersonaDao(PostgresqlConfiguration connectionString)
        {
            this._connectionString = connectionString;
        }

        protected NpgsqlConnection dbConnection()
        {
            return new NpgsqlConnection(this._connectionString.ConnectionString);
        }

        public  async Task<int> create(Persona persona)
        {
            var db = dbConnection();

            var sql_insert = @"
                            INSERT INTO public.""Personas""(""Nombre"", ""RazonSocial"", ""Apellido"", ""FecNacimiento"", ""IdLocalidad"", ""Barrio"", ""Direccion"", ""NroDireccion"", ""Dpto"", ""Piso"", ""Telcelular"", ""Telfijo"", ""Email"", ""IdTipoIdentificador"", ""NroIdentificacion"", ""TipoPersona"", ""IdGenero"", ""Ocupacion"", ""IdEstadoCivil"", ""FechaAlta"", ""UsrAlta"", ""FechaBaja"", ""UsrBaja"", ""FechaMod"", ""UsrMod"")
                            VALUES ( @Nombre, @RazonSocial, @Apellido, @FecNacimiento, @LocalidadId, @Barrio, @Direccion, @NroDireccion, @Dpto, @Piso, @Telcelular, @Telfijo, @Email, @TipoIdentificadorId, @NroIdentificacion, @TipoPersona, @GeneroId, @Ocupacion, @IdEstadoCivil, @FechaAlta, @UsrAltaId, @FechaBaja, @UsrBajaId, @FechaMod, @UsrModId )
                            returning ""Id"";";

            int newPersonaId = await db.QuerySingleAsync<int>(
                                            sql_insert,                                           
                                            new
                                            {
                                                Nombre = persona.Nombre,
                                                RazonSocial = persona.RazonSocial,
                                                Apellido = persona.Apellido,
                                                FecNacimiento = persona.FecNacimiento,
                                                LocalidadId =   persona.Localidad?.Id,
                                                Barrio = persona.Barrio,
                                                Direccion = persona.Direccion,
                                                NroDireccion = persona.NroDireccion,
                                                Dpto = persona.Dpto,
                                                Piso = persona.Piso,
                                                Telcelular = persona.Telcelular,
                                                Telfijo = persona.Telfijo,
                                                Email = persona.Email,
                                                TipoIdentificadorId = persona.TipoIdentificador?.Id,
                                                NroIdentificacion = persona.NroIdentificacion,
                                                TipoPersona = persona.TipoPersona,
                                                GeneroId = persona.Genero?.Id,
                                                Ocupacion = persona.Ocupacion ,
                                                IdEstadoCivil =   persona.EstadoCivil?.Id,
                                                FechaAlta =  persona.FechaAlta ?? DateTime.Now,
                                                UsrAltaId = persona.UsrAlta,
                                                FechaBaja = persona.FechaBaja,
                                                UsrBajaId = persona.UsrBaja,
                                                FechaMod = persona.FechaMod,
                                                UsrModId = persona.UsrMod 
                                             });
        

            return newPersonaId;
        }

        public async Task<bool> update(Persona persona)
        {

            var query = @"UPDATE ""Personas""                             
                          SET  
                            ""Nombre"" = @Nombre, 
                            ""RazonSocial"" = @RazonSocial, 
                            ""Apellido"" = @Apellido,   
                            ""FecNacimiento"" = @FecNacimiento, 
                            ""IdLocalidad"" = @IdLocalidad, 
                            ""Barrio"" = @Barrio, 
                            ""Direccion"" = @Direccion, 
                            ""NroDireccion"" = @NroDireccion, 
                            ""Dpto"" = @Dpto, 
                            ""Piso"" = @Piso, 
                            ""Telcelular"" = @Telcelular, 
                            ""Telfijo"" = @Telfijo, 
                            ""Email"" = @Email, 
                            ""IdTipoIdentificador"" = @IdTipoIdentificador, 
                            ""NroIdentificacion"" = @NroIdentificacion, 
                            ""TipoPersona"" = @TipoPersona, 
                            ""IdGenero"" = @IdGenero, 
                            ""Ocupacion"" = @Ocupacion, 
                            ""IdEstadoCivil"" = @IdEstadoCivil,                                                         
                            ""FechaMod"" = @FechaMod, 
                            ""UsrMod"" = @UsrMod 
                        WHERE ""Id"" = @Id";
            
            var parameters = new DynamicParameters();
            
            parameters.Add("Id", persona.Id, DbType.Int32);
            parameters.Add("Nombre", persona.Nombre, DbType.String);
            parameters.Add("RazonSocial", persona.RazonSocial, DbType.String);
            parameters.Add("Apellido", persona.Apellido, DbType.String);
            parameters.Add("FecNacimiento", persona.FecNacimiento, DbType.DateTime);
            parameters.Add("IdLocalidad", persona.Localidad.Id, DbType.Int32);
            parameters.Add("Barrio", persona.Barrio, DbType.String);
            parameters.Add("Direccion", persona.Direccion, DbType.String);
            parameters.Add("NroDireccion", persona.NroDireccion, DbType.Int32);
            parameters.Add("Dpto", persona.Dpto, DbType.String);
            parameters.Add("Piso", persona.Piso, DbType.Int16);
            parameters.Add("Telcelular", persona.Telcelular, DbType.String);
            parameters.Add("Telfijo", persona.Telfijo, DbType.String);
            parameters.Add("Email", persona.Email, DbType.String);
            //var tipoIdentificador = (persona.Tipoidentificador == null ? SqlInt32.Null : persona.Tipoidentificador.Id);
            parameters.Add("IdTipoIdentificador", 
                        persona.TipoIdentificador.Id, 
                        DbType.Int32 );
            parameters.Add("NroIdentificacion", persona.NroIdentificacion, DbType.Int32);
            parameters.Add("TipoPersona", persona.TipoPersona, DbType.String);
            //var genero = (persona.Genero.Id null ? SqlInt32.Null : persona.Genero.Id);
            parameters.Add("IdGenero", 
                         persona.Genero.Id, 
                        DbType.Int32);
            parameters.Add("Ocupacion", persona.Ocupacion, DbType.String);
            //var EstadoCivil = (persona.EstadoCivil.Id null ? SqlInt32.Null : persona.EstadoCivil.Id);
            parameters.Add("IdEstadoCivil", 
                        persona.EstadoCivil.Id, 
                        DbType.Int32);
            //parameters.Add("FechaAlta", persona.FechaAlta, DbType.DateTime);
            //parameters.Add("UsrAlta", persona.UsrAlta, DbType.String);
            //parameters.Add("FechaBaja", persona.FechaBaja, DbType.DateTime);
            //parameters.Add("UsrBaja", persona.UsrBaja, DbType.String);
            parameters.Add("FechaMod", DateTime.Now, DbType.DateTime);
            parameters.Add("UsrMod", "HCELAYA", DbType.String);
            
             using (var connection = dbConnection())
            {
                var result =  await connection.ExecuteAsync(query, parameters);

                return result > 0;
            }
        

            

            /*
            var db = dbConnection();

            var sql_update = @"
                                UPDATE public.""Personas""
                                SET   Nombre = " + persona.Nombre + 
                                ",RazonSocial = " + persona.RazonSocial +
                                ",Apellido = " + persona.Apellido +
                                ",FecNacimiento = " + persona.FecNacimiento +
                                //",LocalidadId = " + persona.Localidad. +
                                ",Barrio = " + persona.Barrio +
                                ",Direccion = " + persona.Direccion +
                                ",NroDireccion = " + persona.NroDireccion +
                                ",Dpto = " + persona.Dpto +
                                ",Piso = " + persona.Piso +
                                ",Telcelular = " + persona.Telcelular +
                                ",Telfijo = " + persona.Telfijo +
                                ",Email = " + persona.Email +
                                //",TipoIdentificadorId = " + persona.TipoIdentificadorId +
                                ",NroIdentificacion = " + persona.NroIdentificacion +
                                ",TipoPersona = " + persona.TipoPersona +
                                //",GeneroId = " + persona.GeneroId +
                                ",Ocupacion = " + persona.Ocupacion +
                                //",EstadoCivilId = " + persona.EstadoCivilId +
                                //",FechaAlta = " + persona.FechaAlta +
                                //",UsrAltaId = " + persona.UsrAltaId +
                                ",FechaBaja = " + persona.FechaBaja +
                                //",UsrBajaId = " + persona.UsrBajaId +
                                ",FechaMod = " + persona.FechaMod 
                                //",UsrModId = " + persona.UsrModId +
                            ;

            var result = await db.ExecuteAsync(sql_update, new { });

            return result > 0;
            */
        }

        public async Task<bool> delete(int id)
        {
            var db = dbConnection();

            var sql_script = @"
                                DELETE 
                                FROM  public.""Personas""
                                WHERE ""Id"" = @Id  
                            ";

            var result = await db.ExecuteAsync(sql_script, new { Id = id });

            return result > 0;
        }

        public async Task<Persona> find2(int id)
        {
            /*
            var sql_query1 = @"SELECT     a.""Id"", a.""Id"" as idpe,a.""Nombre"",a.""RazonSocial"",a.""Apellido"",a.""FecNacimiento"",a.""IdLocalidad"",a.""Barrio"",
                                         a.""Direccion"",a.""NroDireccion"",a.""Dpto"",a.""Piso"",a.""Telcelular"",a.""Telfijo"",a.""Email"",
                                         a.""NroIdentificacion"",a.""TipoPersona"",a.""Ocupacion"",a.""FechaAlta"",a.""UsrAlta"",
                                         a.""FechaBaja"",a.""UsrBaja"",a.""FechaMod"",a.""UsrMod"",
                                         b.""Id"",  b.""Id"" as idec,b.""Descripcion"",b.""FechaAlta"",b.""FechaBaja"",b.""UsuarioAlta"",b.""UsuarioBaja"",
                                         c.""Id"",  c.""Id"" as idti,c.""Identificador"",c.""DescripcionIdentificador"",c.""FechaAlta"",c.""UsuarioAlta"",c.""FechaBaja"",c.""UsuarioBaja"",
                                         d.""Id"",  d.""Id"" as idge,d.""Descripcion"",d.""FechaAlta"",d.""UsuarioAlta"",d.""FechaBaja"",d.""UsuarioBaja""
                                FROM public.""Personas"" as a 
	                                left join
		                                public.""EstadoCiviles"" as b
	                                on b.""Id"" = a.""IdEstadoCivil"" 
	                                left join
		                                public.""Tipoidentificadores"" as c    
	                                on c.""Id"" = a.""IdTipoIdentificador""  
	                                left join
		                                public.""Generos"" as d    
	                                on d.""Id"" = a.""IdGenero"" 
                                WHERE a.""Id"" = @Id";*/
           
            var sql_query  = @"SELECT     a.""Id"" as idpe, a.*, 
		                                  b.""Id"" as idec, b.*, 
		                                  c.""Id"" as idti, c.*, 
                                          d.""Id"" as idge, d.*,
                                          e.""Id"" as idlo, e.*
                                 FROM public.""Personas"" as a 
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
	                                on e.""Id"" = a.""IdLocalidad"" 
                                WHERE a.""Id"" = @Id";
            /*
            var sql = @" SELECT ""Id"",   
		                                    ""Nombre"", 
		                                    ""RazonSocial"", 
		                                    ""Apellido"", 
		                                    ""FecNacimiento"", 
		                                    ""IdLocalidad"", 
		                                    ""Barrio"", 
		                                    ""Direccion"", 
		                                    ""NroDireccion"", 
		                                    ""Dpto"", 
		                                    ""Piso"", 
		                                    ""Telcelular"", 
		                                    ""Telfijo"", 
		                                    ""Email"", 
		                                    ""IdTipoIdentificador"", 
		                                    ""NroIdentificacion"", 
		                                    ""TipoPersona"", 
		                                    ""IdGenero"", 
		                                    ""Ocupacion"", 
		                                    ""IdEstadoCivil"", 
		                                    ""FechaAlta"", 
		                                    ""UsrAlta"", 
		                                    ""FechaBaja"", 
		                                    ""UsrBaja"", 
		                                    ""FechaMod"", 
		                                    ""UsrMod""
		                         FROM public.""Personas"" as a left join public.""EstadoCiviles"" as b 
                                        on a.""IdEstadoCivil"" = b.""Id""
                                      public.""Tipoidentificadores"" as c    
                                        on a.""IdTipoIdentificador"" = c.""Id""
		                         WHERE  ""Id"" = @Id
                                 ORDER BY ""Id"" desc";*/

            using (var connection = dbConnection())
            {

                /*
                 Persona oPersona = (Persona) await connection.QueryAsync<Persona, EstadoCivil, Persona>(sql_query,
                                                map: (persona, EstadoCivil) =>
                                                {
                                                    persona.EstadoCivil  = (EstadoCivil) EstadoCivil ;
                                                    return persona;
                                                },
                                                param: new { id },
                                                splitOn: "IdEstadoCivil");
                */
                var oPersona = await connection.QueryAsync<Persona, EstadoCivil, TipoIdentificador, Genero, Localidad, Persona>(sql_query,
                                               map: (persona, EstadoCivil, tipoIdentificador, genero, localidad) =>
                                               {
                                                   if (EstadoCivil.Id > 0)
                                                       persona.EstadoCivil = (EstadoCivil)EstadoCivil;

                                                   if (tipoIdentificador.Id > 0)
                                                       persona.TipoIdentificador = (TipoIdentificador)tipoIdentificador;

                                                   if (genero.Id > 0)
                                                       persona.Genero = (Genero)genero;

                                                   if (localidad.Id > 0)
                                                       persona.Localidad = (Localidad)localidad;
                                                   return persona;
                                               }, 
                                               param: new { id },
                                               splitOn: "idpe,idec,idti,idge,idlo").ConfigureAwait(false);
                return (Persona) oPersona.FirstOrDefault();
            }

        }

        public async Task<Persona> find(int id)
        {
            /*
            var db = dbConnection();

            var sql = @" SELECT ""Id"",   
		                                    ""Nombre"" 
		                                    
		                         FROM public.""Personas""
		                         WHERE  ""Id"" = @Id";

            var oPersona = await db.QueryAsync<Persona>(sql, new { Id = id });

            return oPersona;
            */
            var sql = @" SELECT ""Id"",   
		                                    ""Nombre"", 
		                                    ""RazonSocial"", 
		                                    ""Apellido"", 
		                                    ""FecNacimiento"", 
		                                    ""IdLocalidad"", 
		                                    ""Barrio"", 
		                                    ""Direccion"", 
		                                    ""NroDireccion"", 
		                                    ""Dpto"", 
		                                    ""Piso"", 
		                                    ""Telcelular"", 
		                                    ""Telfijo"", 
		                                    ""Email"", 
		                                    ""IdTipoIdentificador"", 
		                                    ""NroIdentificacion"", 
		                                    ""TipoPersona"", 
		                                    ""IdGenero"", 
		                                    ""Ocupacion"", 
		                                    ""IdEstadoCivil"", 
		                                    ""FechaAlta"", 
		                                    ""UsrAlta"", 
		                                    ""FechaBaja"", 
		                                    ""UsrBaja"", 
		                                    ""FechaMod"", 
		                                    ""UsrMod""
		                         FROM public.""Personas""
		                         WHERE  ""Id"" = @Id
                                 ORDER BY ""Id"" desc";

            using (var connection = dbConnection())
            {
                var oPersona =  await connection.QuerySingleOrDefaultAsync<Persona>(sql, new { Id = id });

                return oPersona;
                
            }
           
        }

        public async Task<IEnumerable<Persona>> findAll()
        {
            /*
            var query = @" SELECT ""Id"",   
		                            ""Nombre"", 
		                            ""RazonSocial"", 
		                            ""Apellido"", 
		                            ""FecNacimiento"", 
		                            ""IdLocalidad"", 
		                            ""Barrio"", 
		                            ""Direccion"", 
		                            ""NroDireccion"", 
		                            ""Dpto"", 
		                            ""Piso"", 
		                            ""Telcelular"", 
		                            ""Telfijo"", 
		                            ""Email"", 
		                            ""IdTipoIdentificador"", 
		                            ""NroIdentificacion"", 
		                            ""TipoPersona"", 
		                            ""IdGenero"", 
		                            ""Ocupacion"", 
		                            ""IdEstadoCivil"", 
		                            ""FechaAlta"", 
		                            ""UsrAlta"", 
		                            ""FechaBaja"", 
		                            ""UsrBaja"", 
		                            ""FechaMod"", 
		                            ""UsrMod""
		                    FROM public.""Personas""";
            */

            var sql_query = @"SELECT     a.""Id"" as idpe, a.*, 
		                                  b.""Id"" as idec, b.*, 
		                                  c.""Id"" as idti, c.*, 
                                          d.""Id"" as idge, d.*,
                                          e.""Id"" as idlo, e.*, CONCAT(e.""CodigoPostal"", '-', e.""Nombre"") as cpNombre 
                                 FROM public.""Personas"" as a 
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
	                                on e.""Id"" = a.""IdLocalidad"" ";

            using (var db = dbConnection())
            {
                //var oPersonas = await db.QueryAsync<Persona>(query);
                //return oPersonas.ToList();
                var oPersona = await db.QueryAsync<Persona, EstadoCivil, TipoIdentificador, Genero, Localidad, Persona>(sql_query,
                                               map: (persona, EstadoCivil, tipoIdentificador, genero, localidad) =>
                                               {
                                                   if (EstadoCivil.Id > 0)
                                                       persona.EstadoCivil = (EstadoCivil)EstadoCivil;

                                                   if (tipoIdentificador.Id > 0)
                                                       persona.TipoIdentificador = (TipoIdentificador)tipoIdentificador;

                                                   if (genero.Id > 0)
                                                       persona.Genero = (Genero)genero;

                                                   if (localidad.Id > 0)
                                                       persona.Localidad = (Localidad) localidad;

                                                   return persona;
                                               },                                             
                                               splitOn: "idpe,idec,idti,idge, idlo").ConfigureAwait(false);
                return oPersona;
            }

            /*var db = dbConnection();            

            await db.OpenAsync();

            NpgsqlCommand command = new NpgsqlCommand();

            command.Connection = db;
            command.CommandType = CommandType.Text;
            command.CommandText = @" SELECT ""Id"",   
		                                    ""Nombre"", 
		                                    ""RazonSocial"", 
		                                    ""Apellido"", 
		                                    ""FecNacimiento"", 
		                                    ""IdLocalidad"", 
		                                    ""Barrio"", 
		                                    ""Direccion"", 
		                                    ""NroDireccion"", 
		                                    ""Dpto"", 
		                                    ""Piso"", 
		                                    ""Telcelular"", 
		                                    ""Telfijo"", 
		                                    ""Email"", 
		                                    ""IdTipoIdentificador"", 
		                                    ""NroIdentificacion"", 
		                                    ""TipoPersona"", 
		                                    ""IdGenero"", 
		                                    ""Ocupacion"", 
		                                    ""IdEstadoCivil"", 
		                                    ""FechaAlta"", 
		                                    ""UsrAlta"", 
		                                    ""FechaBaja"", 
		                                    ""UsrBaja"", 
		                                    ""FechaMod"", 
		                                    ""UsrMod""
		                         FROM public.""Personas""
		                         ";
            

            NpgsqlDataReader dr = (NpgsqlDataReader)await command.ExecuteReaderAsync();

            List<Persona> ListaPersonas = null;

            while (dr.Read())
            {
                Persona oPersona = new Persona();
                oPersona.Apellido = dr["Apellido"].ToString();
                oPersona.Nombre = dr["Nombre"].ToString();
                oPersona.RazonSocial = dr["RazonSocial"].ToString();
                oPersona.Apellido = dr["Apellido"].ToString();
                oPersona.FecNacimiento = dr["FecNacimiento"].ToString();
                oPersona.LocalidadId = dr["LocalidadId"].ToString();
                
                oPersona.Barrio = dr["Barrio"].ToString();
                oPersona.Direccion = dr["Direccion"].ToString();
                //oPersona.NroDireccion = dr["NroDireccion"].ToString();
                oPersona.Dpto = dr["Dpto"].ToString();
                //oPersona.Piso = dr["Piso"].ToString();
                oPersona.Telcelular = dr["Telcelular"].ToString();
                oPersona.Telfijo = dr["Telfijo"].ToString();
                oPersona.Email = dr["Email"].ToString();
                //oPersona.TipoIdentificadorId = dr["TipoIdentificadorId"].ToString();
                //oPersona.NroIdentificacion = dr["NroIdentificacion"].ToString();
                oPersona.TipoPersona = dr["TipoPersona"].ToString();
                //oPersona.GeneroId = dr["GeneroId"].ToString();
                oPersona.Ocupacion = dr["Ocupacion"].ToString();
                //oPersona.EstadoCivilId = dr["EstadoCivilId"].ToString();
                 oPersona.FechaAlta = dr["FechaAlta"].ToString();
                oPersona.UsrAltaId = dr["UsrAltaId"].ToString();
                oPersona.FechaBaja = dr["FechaBaja"].ToString();
                oPersona.UsrBajaId = dr["UsrBajaId"].ToString();
                oPersona.FechaMod = dr["FechaMod"].ToString();
                oPersona.UsrModId = dr["UsrModId"].ToString();
                 
                ListaPersonas.Add(oPersona);
            }

            await db.CloseAsync();

            return ListaPersonas;*/
        }

       
    }
}
