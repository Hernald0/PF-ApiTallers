using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
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

        public  async Task<bool> create(Persona persona)
        {
            var db = dbConnection();

            var sql_insert = @"
                            INSERT INTO public.""Personas""(""Nombre"", ""RazonSocial"", ""Apellido"", ""FecNacimiento"", ""IdLocalidad"", ""Barrio"", ""Direccion"", ""NroDireccion"", ""Dpto"", ""Piso"", ""Telcelular"", ""Telfijo"", ""Email"", ""IdTipoIdentificador"", ""NroIdentificacion"", ""TipoPersona"", ""IdGenero"", ""Ocupacion"", ""IdEstadoCivil"", ""FechaAlta"", ""UsrAlta"", ""FechaBaja"", ""UsrBaja"", ""FechaMod"", ""UsrMod"")
                            VALUES ( @Nombre, @RazonSocial, @Apellido, @FecNacimiento, @LocalidadId, @Barrio, @Direccion, @NroDireccion, @Dpto, @Piso, @Telcelular, @Telfijo, @Email, @TipoIdentificadorId, @NroIdentificacion, @TipoPersona, @GeneroId, @Ocupacion, @IdEstadoCivil, @FechaAlta, @UsrAltaId, @FechaBaja, @UsrBajaId, @FechaMod, @UsrModId )
                            ";

            var result = await db.ExecuteAsync(sql_insert, new{
                                                                    Nombre = persona.Nombre,
                                                                    RazonSocial = persona.RazonSocial,
                                                                    Apellido = persona.Apellido,
                                                                    FecNacimiento = persona.FecNacimiento,
                                                                    LocalidadId = persona.IdLocalidad,
                                                                    Barrio = persona.Barrio,
                                                                    Direccion = persona.Direccion,
                                                                    NroDireccion = persona.NroDireccion,
                                                                    Dpto = persona.Dpto,
                                                                    Piso = persona.Piso,
                                                                    Telcelular = persona.Telcelular,
                                                                    Telfijo = persona.Telfijo,
                                                                    Email = persona.Email,
                                                                    TipoIdentificadorId = persona.IdTipoIdentificador,
                                                                    NroIdentificacion = persona.NroIdentificacion,
                                                                    TipoPersona = persona.TipoPersona,
                                                                    GeneroId = persona.IdGenero,
                                                                    Ocupacion = persona.Ocupacion ,
                                                                    IdEstadoCivil =   persona.IdEstadoCivil,
                                                                    FechaAlta =  persona.FechaAlta,
                                                                    UsrAltaId = persona.UsrAlta,
                                                                    FechaBaja = persona.FechaBaja,
                                                                    UsrBajaId = persona.UsrBaja,
                                                                    FechaMod = persona.FechaMod,
                                                                    UsrModId = persona.UsrMod 
            });

            return result > 0;

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
            Console.WriteLine(query);
            var parameters = new DynamicParameters();
            
            parameters.Add("Id", persona.Id, DbType.Int32);
            parameters.Add("Nombre", persona.Nombre, DbType.String);
            parameters.Add("RazonSocial", persona.RazonSocial, DbType.String);
            parameters.Add("Apellido", persona.Apellido, DbType.String);
            parameters.Add("FecNacimiento", persona.FecNacimiento, DbType.DateTime);
            parameters.Add("IdLocalidad", persona.IdLocalidad, DbType.Int32);
            parameters.Add("Barrio", persona.Barrio, DbType.String);
            parameters.Add("Direccion", persona.Direccion, DbType.String);
            parameters.Add("NroDireccion", persona.NroDireccion, DbType.Int32);
            parameters.Add("Dpto", persona.Dpto, DbType.String);
            parameters.Add("Piso", persona.Piso, DbType.Int16);
            parameters.Add("Telcelular", persona.Telcelular, DbType.String);
            parameters.Add("Telfijo", persona.Telfijo, DbType.String);
            parameters.Add("Email", persona.Email, DbType.String);
            parameters.Add("IdTipoIdentificador", persona.IdTipoIdentificador, DbType.Int32);
            parameters.Add("NroIdentificacion", persona.NroIdentificacion, DbType.Int32);
            parameters.Add("TipoPersona", persona.TipoPersona, DbType.String);
            parameters.Add("IdGenero", persona.IdGenero, DbType.Int32);
            parameters.Add("Ocupacion", persona.Ocupacion, DbType.String);
            parameters.Add("IdEstadoCivil", persona.IdEstadoCivil, DbType.Int32);
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
		                         WHERE  ""Id"" = @Id";

            using (var connection = dbConnection())
            {
                var oPersona =  await connection.QuerySingleOrDefaultAsync<Persona>(sql, new { Id = id });

                return oPersona;
                
            }
            /*
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
		                         WHERE  ""Id"" = @Id";

            command.Parameters.AddWithValue("Id", id);
            */


            //NpgsqlDataReader dr = (NpgsqlDataReader) await command.ExecuteReaderAsync();

            //Persona oPersona = new Persona();

            /*
            while (dr.Read())
            {
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
                

            }*/

            //await db.CloseAsync();


            //return await db.QueryFirstOrDefaultAsync<Persona>(command, new { Id = id });
        }

        public async Task<IEnumerable<Persona>> findAll()
        {

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

            using (var db = dbConnection())
            {
                var oPersonas = await db.QueryAsync<Persona>(query);
                return oPersonas.ToList();
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
