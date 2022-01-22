using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiTalleres.Models
{
    public partial class Pais
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public sbyte? CodigoArea { get; set; }
    }
}
