using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiTalleres.Models
{
    public partial class Menu
    {
        public int Id { get; set; }
        public string Menu1 { get; set; }
        public string MenuDisplay { get; set; }
        public string Tipo { get; set; }
        public string Path { get; set; }
        public string Color { get; set; }
        public int? Icono { get; set; }
        public int? IdPadre { get; set; }
        public bool? Activo { get; set; }
        public int? Dashboard { get; set; }
        public int? Orden { get; set; }
    }
}
