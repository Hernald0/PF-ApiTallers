using System;
using System.Collections.Generic;
using System.Text;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Model
{
    public class vwTurnos
    {

        public int Id { get; set; }

        public Cliente Cliente { get; set; }

        public Vehiculo Vehiculo { get; set; }

        public String status { get; set; }

        public string MotivoConsulta { get; set; }

        public IList<Servicio> ServiciosElegidos { get; set; }

        public DateTime Fecha { get; set; }

        public TimeSpan Hora { get; set; }

    }
}
