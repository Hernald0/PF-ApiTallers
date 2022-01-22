using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiTalleres.Models
{
    public partial class Provincia
    {
        public Provincia()
        {
            Localidades = new HashSet<Localidad>();
        }

        public int Id { get; set; }
        public int? IdPais { get; set; }
        public string Nombre { get; set; }

        public virtual ICollection<Localidad> Localidades { get; set; }
    }
}
