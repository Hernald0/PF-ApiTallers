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
    public class TurnoDao : ITurnoDao
    {
        private PostgresqlConfiguration _connectionString;
        private IClienteDao _clienteDao;
        private IOrdenDao _ordenDao;

        public TurnoDao(PostgresqlConfiguration connectionString,
                        IClienteDao             clienteDao,
                        IOrdenDao               ordenDao)
        {
            this._connectionString = connectionString;
            this._clienteDao = clienteDao;
            this._ordenDao = ordenDao;
        }

        protected NpgsqlConnection dbConnection()
        {
            return new NpgsqlConnection(this._connectionString.ConnectionString);
        }

        public async Task<IEnumerable<Turno>> GetTurnos()
        {
            using (var db = dbConnection())
            {
                var sql_query = @"select        t.""Id"" as tuId, t.*,
                                                v.""Id"" as veId, v.*,
                                                c.""Id"" as clId, c.*,
                                                p.""Id"" as peId, p.*
                                               
                                              
                                    from public.""Turnos"" as t
	                                    inner join public.""Clientes"" as c
		                                    on t.""IdCliente"" = c.""Id""
	                                    inner join public.""Vehiculos"" as v
		                                    on t.""IdVehiculo"" = v.""Id""
	                                    inner join public.""Personas"" as p
		                                    on c.""PersonaId"" = p.""Id""
                                    ";

                var oTurnos = await db.QueryAsync<Turno, Vehiculo, Cliente, Persona, Turno>(
                   sql_query,
                   map: (turno, vehiculo, cliente, persona) =>
                   {
                       if (cliente != null)
                       {
                           cliente.Persona = persona;
                           turno.Cliente = cliente;
                       }

                       if (vehiculo != null)
                       {
                           turno.Vehiculo = vehiculo;
                       }

                       return turno;
                   },
                   splitOn: "tuId,veId,clId,peId"
               );

                return oTurnos.ToList();
            }

        }

            public IEnumerable<TimeSpan> GetDisponibleSlots(string date)
        {
            var db = dbConnection();

            DateTime Fecha = Convert.ToDateTime(date);

            var dayOfWeek = Fecha.DayOfWeek.ToString();

            var command = @"SELECT * FROM public.""Diaslaborales"" WHERE ""dia_semana"" = @DayOfWeek";

            var workingDay = db.QueryFirstOrDefault<Model.DiaLaboral>(command, new { DayOfWeek = dayOfWeek });

            var command2 = @"SELECT * FROM public.""Feriados"" WHERE ""dia_feriado"" = @Date";

            if (workingDay == null || db.QueryFirstOrDefault<Model.Feriado>(command2, new { Date = Fecha }) != null)
            {
                return Enumerable.Empty<TimeSpan>();
            }

            var slots = new List<TimeSpan>();
            for (var time = workingDay.Hora_inicio; time < workingDay.Hora_fin; time = time.Add(new TimeSpan(0, 30, 0)))
            {
                slots.Add(time);
            }

            var command3 = @"SELECT ""Hora"" FROM public.""Turnos"" WHERE ""Fecha"" = @Date AND ""Status"" = 'reservado'";

            var bookedSlots = db.Query<TimeSpan>(command3, new { Date = Fecha }).ToList();

            return slots.Except(bookedSlots);
        }

        public async void AgregarTurno(WebApiTalleres.Models.Turno turno)
        {
            var db = dbConnection();

            //Guardo la patente
            var patente = turno.Cliente.Vehiculos[0].Patente;
            Cliente cliente = null;
            Vehiculo vehiculo = null;

            //
            if (!turno.Cliente.Id.HasValue) 
                {  cliente = await this._clienteDao.create(turno.Cliente); }
            else
                { cliente = turno.Cliente;  }
            //si no tiene valor, dar de alta el cliente con los datos de la persona. 

            //Evaluar si el vehiculo existe
            if (!turno.Cliente.Vehiculos[0].Id.HasValue) 
                { cliente = await this._clienteDao.InsertVehiculo( cliente.Id, turno.Cliente.Vehiculos[0] ); }
            
            vehiculo = cliente.Vehiculos.FirstOrDefault(v => v.Patente == patente); 
                

                //Dar de alta el Turno

                var sql = @"
                INSERT INTO public.""Turnos"" (""Fecha"", ""Hora"", ""IdCliente"", ""IdVehiculo"", ""MotivoConsulta"", ""Status"")
                VALUES (@Fecha, @Hora, @IdCliente, @IdVehiculo, @MotivoConsulta, @Status)
                returning  ""Id"" ";
            
                var TurnoId = await db.QuerySingleAsync<int>(sql, new { Fecha = turno.Fecha, 
                                                    Hora =  turno.Hora  ,
                                                    IdCliente = turno.Cliente.Id,
                                                    IdVehiculo = vehiculo.Id,
                                                    MotivoConsulta = turno.MotivoConsulta,
                                                    Status = "reservado"
                 });

                // Dar de alta los servicios
                foreach (Servicio servicio in turno.Servicios)
                {
                     sql = @"
                                INSERT INTO public.""TurnoTareas"" (""IdTurno"", ""IdTarea"")
                                values (@IdTurno, @IdServicio)";

                    var rowNum = db.Execute(sql, new
                    {
                        IdTurno = TurnoId,
                        IdServicio = servicio.Id

                    });
                } 
            
                db.Close();
            //}
        }

        public  int CancelarTurno(int id)
        {
            var db = dbConnection();

            var sql = @" UPDATE  public.""Turnos"" 
                         SET ""Status"" = 'cancelado'
                         WHERE ""Id"" = @Id";

            var filasAfectadas = db.Execute(sql, new { Id = id });

            db.Close();

            return filasAfectadas;
        }

        public int ModificarTurno(Turno turno)
        {
            var db = dbConnection();

            var sql = @" UPDATE  public.""Turnos"" 
                         SET ""Status"" = 'reservado',
                             ""Fecha""  = @Fecha,
                             ""Hora""   = @Hora         
                         WHERE ""Id"" = @Id";

            var filasAfectadas = db.Execute(sql,
                                            new { Id = turno.Id,
                                                  Hora = turno.Hora,
                                                  Fecha = turno.Fecha});

            db.Close();

            return filasAfectadas;
        }

        public void DeleteTurno(int id)
        {
            var db = dbConnection();

            var sql = "DELETE FROM Appointments WHERE Id = @Id";
            
            db.Execute(sql, new { Id = id });
        }

        private void MapearRelaciones<T>(
                                            Dictionary<int, Turno> dict,
                                            T turno,
                                            Servicio servicio,
                                            Cliente cliente,
                                            Persona persona,
                                            Vehiculo vehiculo,
                                            Modelovehiculo modelovehiculo,
                                            Marcavehiculo marcavehiculo
                                        ) where T : Turno
        {
            if (!dict.TryGetValue(turno.Id, out var currentTurno))
            {
                currentTurno = turno;
                currentTurno.Servicios = new List<Servicio>();

                if (cliente != null)
                {
                    cliente.Persona = persona;
                    turno.Cliente = cliente;
                }

                if (vehiculo != null)
                {
                    turno.Vehiculo = vehiculo;
                }

                if (modelovehiculo != null)
                {
                    turno.Vehiculo.Modelovehiculo = modelovehiculo;
                }

                if (marcavehiculo != null)
                {
                    turno.Vehiculo.Modelovehiculo.Marcavehiculo = marcavehiculo;
                }

                dict.Add(turno.Id, currentTurno);
            }

            // Agregar servicio si existe
            if (servicio != null && servicio.Id != 0)
            {
                currentTurno.Servicios.Add(servicio);
            }
        }
        public async Task<Turno> GetTurno(int id)
        {

            var sql_recepcion = @"select  t.""Id"" as tuId, t.""Id"", t.""FechaRecepcion"",  t.""IdCliente"", 
		                                            t.""FechaRecepcion"", t.""HoraRecepcion"", t.""Combustible"", t.""Kilometraje"", t.""IdAseguradora"", t.""Inspector"", t.""NroSiniestro"", t.""Franquicia"", t.""MotivoConsulta"",	  
		                                             t.""MotivoConsulta"", t.""IdVehiculo"", t.""HoraRecepcion"",
		                                            s.""Id"" as sId, s.""Id"", s.""Nombre"", s.""Descripcion"", s.""FechaAlta"", s.""UsuarioAlta"", s.""FechaBaja"", s.""UsuarioBaja"", s.""Duracion_aproximada"", s.""Tipo"", s.""precioCosto"", s.""precioVenta"",	
		                                            c.""Id"" as clId, c.""Id"", c.""PersonaId"", c.""TallerId"",
		                                            p.""Id"" as peId, p.""Id"", p.""Nombre"", p.""RazonSocial"", p.""Apellido"", p.""FecNacimiento"", p.""IdLocalidad"", p.""Barrio"", p.""Direccion"", p.""NroDireccion"", p.""Dpto"", p.""Piso"", p.""Telcelular"", p.""Telfijo"", p.""Email"", p.""IdTipoIdentificador"", p.""NroIdentificacion"", p.""TipoPersona"", p.""IdGenero"", p.""Ocupacion"", p.""IdEstadoCivil"", p.""FechaAlta"", p.""UsrAlta"", p.""FechaBaja"", p.""UsrBaja"", p.""FechaMod"", p.""UsrMod"",
		                                            v.""Id"" as veId, v.""Id"", v.""IdModelo"", v.""Patente"", v.""Color"", v.""NumeroSerie"", anio, v.""IdCliente"", v.""FechaAlta"", v.""UsrAlta"", v.""FechaMod"", v.""UsrMod"", v.""FechaBaja"", v.""UsrBaja"",
		                                            mv.""Id"" as mvId, mv.""Id"", mv.""IdMarca"", mv.""NombreModelo"",
		                                            ma.""Id"" as maId, ma.""Id"", ma.""Nombre"" 
                                            from  public.""RecepcionVehiculo"" t
	                                            inner join public.""Clientes"" as c
		                                             on t.""IdCliente"" = c.""Id""
	                                            left join public.""RecepcionTareas"" as tt
		                                             on (t.""IdTurno"" = tt.""IdTurno"")
	                                            left join public.""Servicios"" as s
		                                             on (tt.""IdServicio"" = s.""Id"")
	                                            inner join public.""Personas"" as p
		                                             on c.""PersonaId"" = p.""Id""
	                                            inner join public.""Vehiculos"" as v
		                                             on t.""IdVehiculo"" = v.""Id""
	                                            inner join public.""Modelovehiculos"" as mv
		                                             on v.""IdModelo"" = mv.""Id""
	                                            inner join public.""Marcavehiculos"" as ma
		                                             on mv.""IdMarca"" = ma.""Id""
                                            where t.""IdTurno""  = @Id";

            var sql_turno = @"select      t.""Id"" as tuId, t.""Id"", t.""Fecha"", t.""IdTaller"", t.""IdCliente"", 
		                                            t.""FechaAlta"", t.""UsuarioAlta"", t.""FechaMod"", t.""UsuarioMod"", 
		                                            t.""FechaBaja"", t.""UsuarioBaja"", t.""MotivoCancelacion"", t.""Status"", t.""MotivoConsulta"", t.""IdVehiculo"", t.""Hora"",
		                                            s.""Id"" as sId, s.""Id"", s.""Nombre"", s.""Descripcion"", s.""FechaAlta"", s.""UsuarioAlta"", s.""FechaBaja"", s.""UsuarioBaja"", s.""Duracion_aproximada"", s.""Tipo"", s.""precioCosto"", s.""precioVenta"",	
	                                                c.""Id"" as clId, c.""Id"", c.""PersonaId"", c.""TallerId"",
	                                                p.""Id"" as peId, p.""Id"", p.""Nombre"", p.""RazonSocial"", p.""Apellido"", p.""FecNacimiento"", p.""IdLocalidad"", p.""Barrio"", p.""Direccion"", p.""NroDireccion"", p.""Dpto"", p.""Piso"", p.""Telcelular"", p.""Telfijo"", p.""Email"", p.""IdTipoIdentificador"", p.""NroIdentificacion"", p.""TipoPersona"", p.""IdGenero"", p.""Ocupacion"", p.""IdEstadoCivil"", p.""FechaAlta"", p.""UsrAlta"", p.""FechaBaja"", p.""UsrBaja"", p.""FechaMod"", p.""UsrMod"",
	                                                v.""Id"" as veId, v.""Id"", v.""IdModelo"", v.""Patente"", v.""Color"", v.""NumeroSerie"", anio, v.""IdCliente"", v.""FechaAlta"", v.""UsrAlta"", v.""FechaMod"", v.""UsrMod"", v.""FechaBaja"", v.""UsrBaja"",
	                                                mv.""Id"" as mvId, mv.""Id"", mv.""IdMarca"", mv.""NombreModelo"",
                                                    ma.""Id"" as maId, ma.""Id"", ma.""Nombre"" 
                                            from public.""Turnos"" as t
                                                inner join public.""Clientes"" as c
                                                     on t.""IdCliente"" = c.""Id""
                                                left join public.""TurnoTareas"" as tt
                                                     on (t.""Id"" = tt.""IdTurno"" and t.""Id"" = tt.""IdTurno"")
                                                left join public.""Servicios"" as s
                                                     on (tt.""IdTarea"" = s.""Id"")
                                                inner join public.""Personas"" as p
                                                     on c.""PersonaId"" = p.""Id""
                                                inner join public.""Vehiculos"" as v
                                                     on t.""IdVehiculo"" = v.""Id""
                                                inner join public.""Modelovehiculos"" as mv
                                                     on v.""IdModelo"" = mv.""Id""
                                                inner join public.""Marcavehiculos"" as ma
                                                     on mv.""IdMarca"" = ma.""Id""
                                            where t.""Id"" = @Id";

            using var db = dbConnection();
            var turnoDict = new Dictionary<int, Turno>();

            // 1. Verificar si existe Recepcion para este turno
            var existeRecepcion = await db.ExecuteScalarAsync<int>(
                @"SELECT COUNT(*) FROM ""RecepcionVehiculo"" WHERE ""IdTurno"" = @Id",
                new { Id = id });

            if (existeRecepcion > 0)
            {
                // Si existe Recepcion, traemos TurnoRecepcion
                var turnoRecepcion = await db.QueryAsync<TurnoRecepcion, Servicio, Cliente, Persona, Vehiculo, Modelovehiculo, Marcavehiculo, TurnoRecepcion>(
                    sql_recepcion,
                    map: (turno, servicio, cliente, persona, vehiculo, modelovehiculo, marcavehiculo) =>
                    {
                        MapearRelaciones(turnoDict, turno, servicio, cliente, persona, vehiculo, modelovehiculo, marcavehiculo);
                        return turno;
                    },
                    param: new { Id = id },
                    splitOn: "sId,clId,peId,veId,mvId,maId"
                );

                return turnoRecepcion.FirstOrDefault();
            }
            else
            {
                // Si no hay Recepcion, traemos Turno original
                var turno = await db.QueryAsync<Turno, Servicio, Cliente, Persona, Vehiculo, Modelovehiculo, Marcavehiculo, Turno>(
                    sql_turno,
                    map: (t, servicio, cliente, persona, vehiculo, modelovehiculo, marcavehiculo) =>
                    {
                        MapearRelaciones(turnoDict, t, servicio, cliente, persona, vehiculo, modelovehiculo, marcavehiculo);
                        return t;
                    },
                    param: new { Id = id },
                    splitOn: "sId,clId,peId,veId,mvId,maId"
                );

                return turno.FirstOrDefault();
            }
        }


        public async Task<Turno>  GetTurnoOriginal(int id)
        {

            using (var db = dbConnection())
            {
                var turnoDict = new Dictionary<int, Turno>();

                var sql_query_recepcion = @"select  t.""Id"" as tuId, t.""Id"", t.""FechaRecepcion"",  t.""IdCliente"", 
		                                            t.""FechaRecepcion"", t.""HoraRecepcion"", t.""Combustible"", t.""Kilometraje"", t.""IdAseguradora"", t.""Inspector"", t.""NroSiniestro"", t.""Franquicia"", t.""MotivoConsulta"",	  
		                                             t.""MotivoConsulta"", t.""IdVehiculo"", t.""HoraRecepcion"",
		                                            s.""Id"" as sId, s.""Id"", s.""Nombre"", s.""Descripcion"", s.""FechaAlta"", s.""UsuarioAlta"", s.""FechaBaja"", s.""UsuarioBaja"", s.""Duracion_aproximada"", s.""Tipo"", s.""precioCosto"", s.""precioVenta"",	
		                                            c.""Id"" as clId, c.""Id"", c.""PersonaId"", c.""TallerId"",
		                                            p.""Id"" as peId, p.""Id"", p.""Nombre"", p.""RazonSocial"", p.""Apellido"", p.""FecNacimiento"", p.""IdLocalidad"", p.""Barrio"", p.""Direccion"", p.""NroDireccion"", p.""Dpto"", p.""Piso"", p.""Telcelular"", p.""Telfijo"", p.""Email"", p.""IdTipoIdentificador"", p.""NroIdentificacion"", p.""TipoPersona"", p.""IdGenero"", p.""Ocupacion"", p.""IdEstadoCivil"", p.""FechaAlta"", p.""UsrAlta"", p.""FechaBaja"", p.""UsrBaja"", p.""FechaMod"", p.""UsrMod"",
		                                            v.""Id"" as veId, v.""Id"", v.""IdModelo"", v.""Patente"", v.""Color"", v.""NumeroSerie"", anio, v.""IdCliente"", v.""FechaAlta"", v.""UsrAlta"", v.""FechaMod"", v.""UsrMod"", v.""FechaBaja"", v.""UsrBaja"",
		                                            mv.""Id"" as mvId, mv.""Id"", mv.""IdMarca"", mv.""NombreModelo"",
		                                            ma.""Id"" as maId, ma.""Id"", ma.""Nombre"" 
                                            from  public.""RecepcionVehiculo"" t
	                                            inner join public.""Clientes"" as c
		                                             on t.""IdCliente"" = c.""Id""
	                                            left join public.""RecepcionTareas"" as tt
		                                             on (t.""IdTurno"" = tt.""IdTurno"")
	                                            left join public.""Servicios"" as s
		                                             on (tt.""IdServicio"" = s.""Id"")
	                                            inner join public.""Personas"" as p
		                                             on c.""PersonaId"" = p.""Id""
	                                            inner join public.""Vehiculos"" as v
		                                             on t.""IdVehiculo"" = v.""Id""
	                                            inner join public.""Modelovehiculos"" as mv
		                                             on v.""IdModelo"" = mv.""Id""
	                                            inner join public.""Marcavehiculos"" as ma
		                                             on mv.""IdMarca"" = ma.""Id""
                                            where t.""IdTurno""  = @Id";

                var sql_query_turno = @"select      t.""Id"" as tuId, t.""Id"", t.""Fecha"", t.""IdTaller"", t.""IdCliente"", 
		                                            t.""FechaAlta"", t.""UsuarioAlta"", t.""FechaMod"", t.""UsuarioMod"", 
		                                            t.""FechaBaja"", t.""UsuarioBaja"", t.""MotivoCancelacion"", t.""Status"", t.""MotivoConsulta"", t.""IdVehiculo"", t.""Hora"",
		                                            s.""Id"" as sId, s.""Id"", s.""Nombre"", s.""Descripcion"", s.""FechaAlta"", s.""UsuarioAlta"", s.""FechaBaja"", s.""UsuarioBaja"", s.""Duracion_aproximada"", s.""Tipo"", s.""precioCosto"", s.""precioVenta"",	
	                                                c.""Id"" as clId, c.""Id"", c.""PersonaId"", c.""TallerId"",
	                                                p.""Id"" as peId, p.""Id"", p.""Nombre"", p.""RazonSocial"", p.""Apellido"", p.""FecNacimiento"", p.""IdLocalidad"", p.""Barrio"", p.""Direccion"", p.""NroDireccion"", p.""Dpto"", p.""Piso"", p.""Telcelular"", p.""Telfijo"", p.""Email"", p.""IdTipoIdentificador"", p.""NroIdentificacion"", p.""TipoPersona"", p.""IdGenero"", p.""Ocupacion"", p.""IdEstadoCivil"", p.""FechaAlta"", p.""UsrAlta"", p.""FechaBaja"", p.""UsrBaja"", p.""FechaMod"", p.""UsrMod"",
	                                                v.""Id"" as veId, v.""Id"", v.""IdModelo"", v.""Patente"", v.""Color"", v.""NumeroSerie"", anio, v.""IdCliente"", v.""FechaAlta"", v.""UsrAlta"", v.""FechaMod"", v.""UsrMod"", v.""FechaBaja"", v.""UsrBaja"",
	                                                mv.""Id"" as mvId, mv.""Id"", mv.""IdMarca"", mv.""NombreModelo"",
                                                    ma.""Id"" as maId, ma.""Id"", ma.""Nombre"" 
                                            from public.""Turnos"" as t
                                                inner join public.""Clientes"" as c
                                                     on t.""IdCliente"" = c.""Id""
                                                left join public.""TurnoTareas"" as tt
                                                     on (t.""Id"" = tt.""IdTurno"" and t.""Id"" = tt.""IdTurno"")
                                                left join public.""Servicios"" as s
                                                     on (tt.""IdTarea"" = s.""Id"")
                                                inner join public.""Personas"" as p
                                                     on c.""PersonaId"" = p.""Id""
                                                inner join public.""Vehiculos"" as v
                                                     on t.""IdVehiculo"" = v.""Id""
                                                inner join public.""Modelovehiculos"" as mv
                                                     on v.""IdModelo"" = mv.""Id""
                                                inner join public.""Marcavehiculos"" as ma
                                                     on mv.""IdMarca"" = ma.""Id""
                                            where t.""Id"" = @Id";

                var oTurno = await db.QueryAsync<Turno, Servicio, Cliente, Persona, Vehiculo, Modelovehiculo, Marcavehiculo,  Turno>(
                   sql_query_recepcion,
                   map: (turno, servicio, cliente, persona, vehiculo, modelovehiculo, marcavehiculo) =>
                   {

                       //Evalua si el turno existe, lo reutiliza sin crear unos nuevo
                       if (!turnoDict.TryGetValue(turno.Id, out var currentTurno))
                       {
                           currentTurno = turno;
                           currentTurno.Servicios = new List<Servicio>();

                               if (cliente != null)
                               {
                                   cliente.Persona = persona;
                                   turno.Cliente = cliente;
                               }

                               if (vehiculo != null)
                               {
                                   turno.Vehiculo = vehiculo;   
                               }

                               if (modelovehiculo != null)
                               {
                                   turno.Vehiculo.Modelovehiculo = modelovehiculo;  
                               }

                               if (marcavehiculo != null)
                               {
                                   turno.Vehiculo.Modelovehiculo.Marcavehiculo = marcavehiculo; 
                               }

                               if (servicio.Id != 0 )
                               {

                                   if (turno.Servicios == null)
                                   {
                                       turno.Servicios = new List<Servicio>();
                                   }

                                   turno.Servicios.Add(servicio) ;
                               }

                           turnoDict.Add(turno.Id, currentTurno);
                       }

                       // Agregar el servicio (si corresponde)
                       if (servicio != null && servicio.Id != 0)
                       {
                           currentTurno.Servicios.Add(servicio);
                       }

                       return currentTurno;
                   },
                   new { Id = id },
                   splitOn: "sId,clId,peId,veId,mvId,maId"
                   );
                var turno = oTurno.FirstOrDefault(); 
                
                if (turno != null  && turno.Status != "recibido")
                {

                    var oTurnoRecepcion = await db.QueryAsync<TurnoRecepcion, Servicio, Cliente, Persona, Vehiculo, Modelovehiculo, Marcavehiculo, Turno>(
                    sql_query_turno,
                    map: (turno, servicio, cliente, persona, vehiculo, modelovehiculo, marcavehiculo) =>
                    {

                        //Evalua si el turno existe, lo reutiliza sin crear unos nuevo
                        if (!turnoDict.TryGetValue(turno.Id, out var currentTurno))
                        {
                            currentTurno = turno;
                            currentTurno.Servicios = new List<Servicio>();

                            if (cliente != null)
                            {
                                cliente.Persona = persona;
                                turno.Cliente = cliente;
                            }

                            if (vehiculo != null)
                            {
                                turno.Vehiculo = vehiculo;
                            }

                            if (modelovehiculo != null)
                            {
                                turno.Vehiculo.Modelovehiculo = modelovehiculo;
                            }

                            if (marcavehiculo != null)
                            {
                                turno.Vehiculo.Modelovehiculo.Marcavehiculo = marcavehiculo;
                            }

                            if (servicio.Id != 0)
                            {

                                if (turno.Servicios == null)
                                {
                                    turno.Servicios = new List<Servicio>();
                                }

                                turno.Servicios.Add(servicio);
                            }

                            turnoDict.Add(turno.Id, currentTurno);
                        }

                        // Agregar el servicio (si corresponde)
                        if (servicio != null && servicio.Id != 0)
                        {
                            currentTurno.Servicios.Add(servicio);
                        }

                        return currentTurno;
                    },
                    new { Id = id },
                    splitOn: "sId,clId,peId,veId,mvId,maId"

                    );
                    
                    return oTurnoRecepcion.FirstOrDefault();
                }
                else {
                    return oTurno.FirstOrDefault();
                }

                

                

            }

        }
   
         public async Task<int>  PostRecepcionTurno(Model.RecepcionTurnoDTO recepcionTurno)
        {
            int idAltaRecepcion;
                
            if (recepcionTurno == null)  
                return 0;

            var db = dbConnection();


            //Dar de alta el Turno
             
            var sql = @"
                INSERT INTO public.""RecepcionVehiculo"" (""FechaRecepcion"", ""HoraRecepcion"",""IdCliente"", ""IdVehiculo"", ""IdTurno"", ""Combustible"", ""Kilometraje"", ""IdAseguradora"", ""Inspector"", ""NroSiniestro"", ""Franquicia"", ""MotivoConsulta"", ""UsuarioAlta"", ""FechaAlta"")
                VALUES (@FechaRecepcion, @HoraRecepcion, @IdCliente, @IdVehiculo, @IdTurno ,@Combustible, @Kilometraje, @IdAseguradora, @Inspector, @NroSiniestro, @Franquicia, @MotivoConsulta, @UsuarioAlta, @FechaAlta)
                returning  ""Id"" ";
           /*
            var sql = 
                        ";*/


            var RecepcionId = await db.QuerySingleAsync<int>(sql, new
            {       

                FechaRecepcion  = recepcionTurno.FechaRecepcion,
                HoraRecepcion = recepcionTurno.HoraRecepcion,
                IdTurno = recepcionTurno.IdTurno,
                IdCliente = recepcionTurno.IdCliente,
                IdVehiculo = recepcionTurno.IdVehiculo,
                Combustible = recepcionTurno.Combustible,
                Kilometraje = recepcionTurno.Kilometraje,
                IdAseguradora = recepcionTurno.IdAseguradora,
                Inspector = recepcionTurno.Inspector,
                NroSiniestro = recepcionTurno.NroSiniestro,
                Franquicia = recepcionTurno.Franquicia,
                MotivoConsulta = recepcionTurno.MotivoConsulta,
                UsuarioAlta = "HCELAYA" , 
                FechaAlta = DateTime.Now
            });

            // Dar de alta los servicios
            foreach (ItemVentaCreateDTO servicio in recepcionTurno.Servicios)
            {
                sql = @"
                                INSERT INTO public.""RecepcionTareas"" (""IdTurno"",  ""IdServicio"")
                                values (@IdTurno,  @IdServicio)";

                var rowNum = db.Execute(sql, new
                {
                    IdTurno = recepcionTurno.IdTurno,
                    IdServicio = servicio.ItemId
                     
                });
            }


            if (recepcionTurno.IdTurno > 0)
            {

                var sqlUpdateTurno = @"update public.""Turnos""
                                            set                                     
                                                ""Status"" = @Status
                                            where ""Id"" = @Id";

                var TurnoActualicadoId = await db.ExecuteAsync(sqlUpdateTurno, new
                {

                    Id = recepcionTurno.IdTurno,
                    Status = "recibido"
                });

            }

            db.Close();


            await this._ordenDao.AgregarOrder(RecepcionId, recepcionTurno);

            return RecepcionId;

        }


       
    }

}
