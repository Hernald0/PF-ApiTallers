using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Model
{

    public class VentaCreateDTO
    {
       
        public int? Id { get; set; }
        public DateTime FechaEmision { get; set; }

        public String Usuario { get; set; }

        public int ClienteId { get; set; }
         
        public List<ItemVentaCreateDTO> Items { get; set; } = new List<ItemVentaCreateDTO>();
 
        public decimal MontoTotal { get; set; }
        
        public decimal? Efectivo { get; set; }

        public string? TarjetaCredito { get; set; }

        public decimal? MontoTarjetaCredito { get; set; }

        public decimal? CuentaCorriente { get; set; }

        public decimal? Descuento { get; set; }

    
        public string? Observaciones { get; set; }

        public string TipoOperacion { get; set; }

        public int? NroVenta { get; set; }        
    }
    
}
