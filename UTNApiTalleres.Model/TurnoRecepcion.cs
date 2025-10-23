using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Model
{
    public class TurnoRecepcion : Turno
    {
        public DateTime FechaRecepcion { get; set; }

        public string HoraRecepcion { get; set; }

        public int? Combustible { get; set; }
        public int? Kilometraje { get; set; }
        public int? IdAseguradora { get; set; }
        public string? Inspector { get; set; }

        public string? NroSiniestro { get; set; }

        public int? Franquicia { get; set; }

        public string? MotivoConsulta { get; set; }
    }
}
