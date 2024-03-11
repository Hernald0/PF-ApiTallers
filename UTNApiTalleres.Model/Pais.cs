using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiTalleres.Models
{
    public partial class Pais
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int? CodigoArea { get; set; }
        public ICollection<Provincia> Provincias { get; set; }

}
}
