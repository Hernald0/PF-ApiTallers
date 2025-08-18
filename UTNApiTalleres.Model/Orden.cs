using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Model
{
    public class Orden
    {

        public int Id { get; set; }

        public Cliente Cliente { get; set; }

        public Vehiculo Vehiculo { get; set; }

        public List<ItemVentaCreateDTO> Items { get; set; }

        public string Usuario { get; set; }

        public decimal Total { get; set; }

        public decimal Descuento { get; set; }

        public decimal Iva { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public string observaciones { get; set; }

        public string estado { get; set; }
    }
}
