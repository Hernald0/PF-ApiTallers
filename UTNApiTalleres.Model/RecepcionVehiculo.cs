using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTNApiTalleres.Model
{
    public class RecepcionVehiculo
    {
        public DateTime? FechaRecepcion { get; set; }

        public string? HoraRecepcion { get; set; }

        public string? Combustible { get; set; }

        public int? Kilometraje { get; set; }

        public string? Inspector { get; set; }

        public string? NroSiniestro { get; set; }

        public int? Franquicia { get; set; }

        public string? MotivoConsulta { get; set; }



        /*
        public  Cliente? Cliente { get; set; } 
        public Vehiculo? Vehiculo { get; set; }
        public Aseguradora? Aseguradora { get; set; }
        public string? UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        */

    }
}
