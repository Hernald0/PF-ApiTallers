using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTNApiTalleres.Model;

namespace WebApiTalleres.Models
{
    public class Acceso
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Ruta { get; set; }  // ej: "/usuarios", "/roles"
        public bool Activo { get; set; }

        public List<Permiso>? Permisos { get; set; } = new List<Permiso>();

    }
}
