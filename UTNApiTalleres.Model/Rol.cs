using System;
using System.Collections.Generic;
using UTNApiTalleres.Model;

#nullable disable

namespace WebApiTalleres.Models
{
    public   class Rol
    {
        public int RolId { get; set; }
        public string Nombre { get; set; }

        public string Descripcion { get; set; }
        public bool? Activo { get; set; } = false;
        public List<Acceso>? Accesos { get; set; } = new List<Acceso>();

    }
}
