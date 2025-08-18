using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiTalleres.Models
{
    public partial class Modelovehiculo
    {
        public int? Id { get; set; }    
        
        public string NombreModelo { get; set; }

        public int? IdMarca { get; set; }

        public Marcavehiculo Marcavehiculo  { get; set; }

        /*
        public Marcavehiculo Marcavehiculo  { get; set; }
            
        public Modelovehiculo()
        {
            this.Marcavehiculo = null;
        }*/
    }   

}
