using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Model
{
    public class Venta
    {
        public int Id { get; set; }

        public Cliente Cliente { get; set; }

        public List<ItemVentaCreateDTO> Items { get; set; }

        public string Usuario { get; set; }

        public decimal Total { get; set; }
   
        public decimal Descuento { get; set; }
       
        public decimal Iva { get; set; }

        public DateTime FechaEmision { get; set; }

        public decimal Efectivo { get; set; }
        
        public decimal MontoTotal { get; set; }

        public int nrooperacion { get; set; }

        public decimal cuentaCorriente { get; set; }

        public string tarjetaCredito { get; set; }

        public decimal montoTarjetaCredito { get; set; }

        public string observaciones { get; set; }

    }
}
