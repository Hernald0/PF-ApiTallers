using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiTalleres.Models
{
    public partial class Vehiculo
    {
        public int? Id { get; set; }
        public int? IdModelo { get; set; }
        public string Patente { get; set; }
    }
}
