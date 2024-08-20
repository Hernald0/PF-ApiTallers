using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiTalleres.Models
{
    public class Turno
    {
        public int Id { get; set; }

        public Cliente Cliente { get; set; }

        public Vehiculo Vehiculo { get; set; }

        public string MotivoConsulta { get; set; }

        public string Status { get; set; }

        public IList<UTNApiTalleres.Model.Servicio> ServiciosElegidos { get; set; }

        public DateTime Fecha { get; set; }

        public TimeSpan Hora { get; set; }
  
    
    }
}
