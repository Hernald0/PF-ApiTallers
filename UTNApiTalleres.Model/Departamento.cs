using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiTalleres.Models
{
    public partial class Departamento
    {
        public Departamento()
        {
            Localidades = new HashSet<Localidad>();
        }

        public int Id { get; set; }
        public int? IdProvincia { get; set; }
        public string Nombre { get; set; }
        public string NombreCompleto { get; set; }
        public string Categoria { get; set; }

        public virtual ICollection<Localidad> Localidades { get; set; }
    }
}
