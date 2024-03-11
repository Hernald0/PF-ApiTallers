using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiTalleres.Models
{
    public partial class Aseguradora 
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public DateTime? FechaAlta { get; set; }
        public string? UsuarioAlta { get; set; }
        public DateTime? FechaBaja { get; set; }
        public string? UsuarioBaja { get; set; }

    }
}
