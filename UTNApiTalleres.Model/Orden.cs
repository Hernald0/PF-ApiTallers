using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiTalleres.Models;

namespace WebApiTalleres.Models
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

        public DateTime? FechaRecepcion { get; set; }

        public DateTime? FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        public int? Combustible { get; set; }

        public int? Kilometraje { get; set; }
        public int? IdAseguradora { get; set; }
        public string? Inspector { get; set; }

        public string? NroSiniestro { get; set; }

        public int? Franquicia { get; set; }

        public string? MotivoConsulta { get; set; }

        public string? ObservacionTecnico { get; set; }

        public int? IdTurno { get; set; }

        public int? IdRecepcion { get; set; }

        public int? VentaId { get; set; }

        public int? IdEmpleadoAsignado { get; set; }

        public int? Estado { get; set; }
    }
}
