using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiTalleres.Models
{
    public partial class Turno
    {
        public int Id { get; set; }
        public DateTime? Fecha { get; set; }
        public TimeSpan? Hora { get; set; }
        public int? IdTaller { get; set; }
        public int? IdCliente { get; set; }
        public string Observaciones { get; set; }
        public DateTime? FechaAlta { get; set; }
        public string UsuarioAlta { get; set; }
        public DateTime? FechaMod { get; set; }
        public string UsuarioMod { get; set; }
        public DateTime? FechaBaja { get; set; }
        public string UsuarioBaja { get; set; }
        public string MotivoCancelación { get; set; }
    }
}
