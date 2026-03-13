using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiTalleres.Models
{
    public partial class Empleado
    {
        public int Id { get; set; }
        public Persona Persona { get; set; }
        
        public Usuario? Usuario { get; set; }
    }
}
