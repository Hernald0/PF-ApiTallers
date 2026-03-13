using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTNApiTalleres.Model;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Data.Repositorio.Interfaz
{
    public interface IOrdenDao
    {
        Task<Orden> GetOrden(int id);

        Task<IEnumerable<Orden>> GetOrdenes(string rol, int? idEmpleado);

        Task<int> AgregarOrder(int? RecepcionId, RecepcionTurnoDTO orden);

        void ModificarOrder(OrdenDTO orden);

        void DeleteOrder(int id);

        int CancelarOrder(int id);

        Task<List<EmpleadosComboDTO>> getEmpleadosMecanicos();

        Task<bool> ModificarEmpleadoAsignado(EmpleadoAsignadoDTO empleadoAsignadoDTO);

        Task<int> deficionClienteOrder(int orderId, int estado);

        int UpdateVentaId(int? OrdenId, int? VentaId);

    }
}
