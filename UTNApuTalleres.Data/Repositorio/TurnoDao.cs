using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTNApiTalleres.Data.Repositorio.Interfaz;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Data.Repositorio
{
    public class TurnoDao : ITurnoDao
    {
        private PostgresqlConfiguration _connectionString;
        private IClienteDao _clienteDao;

        public TurnoDao(PostgresqlConfiguration connectionString,
                        IClienteDao             clienteDao)
        {
            this._connectionString = connectionString;
            this._clienteDao = clienteDao;
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
            
                var TurnoId = db.Execute(sql, new { Fecha = turno.Fecha, 
                                                    Hora =  turno.Hora  ,
                                                    IdCliente = turno.Cliente.Id,
                                                    IdVehiculo = vehiculo.Id,
                                                    MotivoConsulta = turno.MotivoConsulta,
                                                    Status = "reservado"
                 });

                /*/Dar de alta los servicios
                foreach servicio as Servicio in turno.ServiciosElegidos
                {
                    var sql = @"
                                INSERT INTO Turno (Fecha, Hora, ClienteId, VehiculoId, Motivo)
                                VALUES (@Fecha, @Hora, @ClienteId, @VehiculoId, @Motivo)
                                returning  ""Id"" ";

                                    var TurnoId = db.Execute(sql, new
                                    {
                                        Fecha = turno.Fecha,
                                        Hora = turno.Hora,
                                        ClienteId = turno.Cliente.Id,
                                        VehiculoId = turno.Vehiculo.Id,
                                        Motivo = turno.MotivoConsulta
                                    });
                }*/
            
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
    }
}
