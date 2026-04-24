using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTNApiTalleres.Model
{
    public class Repuesto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        
        public int Stock { get; set; }
 

        public Decimal PrecioCosto { get; set; }

        public Decimal PrecioVenta { get; set; }

        public DateTime? FechaBaja { get; set; }

        public string? UsuarioBaja { get; set; }
    }
}
