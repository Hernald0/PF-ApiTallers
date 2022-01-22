using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiTalleres.Models
{
    public partial class Rol
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string NombreDisplay { get; set; }
        public string Descripcion { get; set; }
    }
}
