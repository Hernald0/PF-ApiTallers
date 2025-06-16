using System;
using System.Collections.Generic;
using System.Text;

namespace UTNApiTalleres.Model
{
    public class ItemVentaDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; } // 'servicio' o 'repuesto'
        public string Descripcion { get; set; }
        public decimal PrecioCosto { get; set; }
        public decimal PrecioVenta { get; set; }
        public int? DuracionAproximada { get; set; } // Solo para servicios
        public string Clase { get; set; } // Solo para servicios
        public int? Stock { get; set; } // Solo para repuestos
    }
}
