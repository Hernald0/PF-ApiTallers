using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTNApiTalleres.Model
{
    public class RecepcionTurnoDTO
    {
      

        public int IdTurno { get; set; }
        public int IdCliente  { get; set; }
        public int IdVehiculo  { get; set; }

        public DateTime FechaRecepcion { get; set; }
       
        public int? Combustible { get; set; }
       
        public int? Kilometraje  { get; set; }
        public int? IdAseguradora  { get; set; }
        public string? Inspector { get; set; }

        public string? NroSiniestro  { get; set; }

        public int? Franquicia  { get; set; }
        
        public string? Observaciones  { get; set; }

        public string? Usuario { get; set; }

        //public IList<UTNApiTalleres.Model.Servicio> Servicios { get; set; }
        public List<ItemVentaCreateDTO> Servicios { get; set; } = new List<ItemVentaCreateDTO>();
    }
}
