using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiTalleres.Models;


namespace UTNApiTalleres.Data.Repositorio.Interfaz
{
    public interface IVentaDao
    {

        Task<Venta> ObtenerPorId(int id);
    
        Task<IEnumerable<Venta>> ObtenerTodas();

        void AgregarVenta(VentaCreateDTO venta);

        int? ModificarVenta(VentaCreateDTO venta);

        void DeleteVenta(int id);

        int CancelarVenta(int id);

        Task<int> AgregarVentaOrden(OrdenDTO orden);



    }
}
