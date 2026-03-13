using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiTalleres.Models
{
    public class Feriado
    {
        public int Id { get; set; }
        public DateTime FeriadoDate { get; set; }
        public string Descripcion { get; set; }
    }

}
