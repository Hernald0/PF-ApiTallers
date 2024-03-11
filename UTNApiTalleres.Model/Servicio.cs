using System;
using System.Collections.Generic;
using System.Text;

namespace UTNApiTalleres.Model
{
    public partial class Servicio
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public DateTime FechaAlta { get; set; }

        public string UsuarioAlta { get; set; }

        public DateTime? FechaBaja { get; set; }

        public string? UsuarioBaja { get; set; }
    }
}
