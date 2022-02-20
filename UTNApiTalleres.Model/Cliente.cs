using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiTalleres.Models
{
    public partial class Cliente
    {
        public int Id { get; set; }
       
        public Persona Persona { get; set; }
        
        public Taller Taller { get; set; }
        
    }
}
