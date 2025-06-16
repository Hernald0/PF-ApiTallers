using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTNApiTalleres.Model
{
    public class ItemVentaCreateDTO
    {

      
        public int? VentaId { get; set; }

        public int ItemId { get; set; }
        /*
      public string? Nombre { get; set; }

      

      public string Tipo { get; set; }

      public int Cantidad { get; set; }

      public decimal Importe { get; set; }

      public decimal Iva { get; set; }

      public decimal? Bonificacion { get; set; }

      public decimal Subtotal { get; set; }

      public decimal PrecioUnitario { get; set; } */

        public int? ServicioId { get; set; }
        public int? RepuestoId { get; set; }

        public string Tipo { get; set; } // "Servicio" o "Repuesto"
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public int Cantidad { get; set; }
        public decimal? Bonificacion { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        //public decimal? Descuento { get; set; }

    }
}
