using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiTalleres.Models
{
    public partial class Cliente
    {
        public int Id { get; set; }
       
        public Persona Persona { get; set; }

        public List<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();

        public Taller Taller { get; set; }
        
    }
}
