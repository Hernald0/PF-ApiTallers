using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiTalleres.Models
{
    public partial class Empleado
    {
        public int Id { get; set; }
        public int? IdPersona { get; set; }
        public int? IdTaller { get; set; }

        public virtual Persona IdPersonaNavigation { get; set; }
        public virtual Taller IdTallerNavigation { get; set; }
    }
}
