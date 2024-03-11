using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiTalleres.Models
{
    public partial class Localidad
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public short? CodigoPostal { get; set; }

        public string cpNombre { get; set; }
        //public virtual Departamento IdDepartamentoNavigation { get; set; }
        //public virtual Provincia IdProvinciaNavigation { get; set; }
    }
}
