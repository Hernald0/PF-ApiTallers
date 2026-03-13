using System;
using System.Collections.Generic;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Application.DTOs
{
    public class CreateRolDto
    {
        public int RolId { get; set; }
        public string Nombre { get; set; }

        public string Descripcion { get; set; }
        public bool? Activo { get; set; } = false;
        public List<Acceso>? Accesos { get; set; } = new List<Acceso>();
    }
}
