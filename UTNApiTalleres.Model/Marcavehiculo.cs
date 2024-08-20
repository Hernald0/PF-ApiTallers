using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiTalleres.Models
{
    public partial class Marcavehiculo
    {
        /*public Marcavehiculo()
        {
            Modelovehiculos = new HashSet<Modelovehiculo>();
        }*/

        public int? Id { get; set; }
        public string Nombre { get; set; }
        public virtual ICollection<Modelovehiculo> Modelovehiculos { get; set; }

        public Marcavehiculo()
        {
            this.Modelovehiculos = new List<Modelovehiculo>();
        }
    }
}
