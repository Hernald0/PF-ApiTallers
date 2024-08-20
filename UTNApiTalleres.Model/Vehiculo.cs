using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiTalleres.Models
{
    public partial class Vehiculo
    {
        public int? Id { get; set; }
        public Modelovehiculo Modelovehiculo { get; set; }
        public string Patente { get; set; }

        public string NumeroSerie { get; set; }

        public string Color { get; set; }

        public int? Anio { get; set; }
    }
}
