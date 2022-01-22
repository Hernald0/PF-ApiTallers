using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiTalleres.Models
{
    public partial class Taller
    {
        public Taller()
        {
            Empleados = new HashSet<Empleado>();
        }

        public int Id { get; set; }
        public string Nombretaller { get; set; }
        public string Idpersonatit { get; set; }

        public virtual ICollection<Empleado> Empleados { get; set; }
    }
}
