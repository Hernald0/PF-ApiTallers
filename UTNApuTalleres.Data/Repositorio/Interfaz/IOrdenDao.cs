using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTNApiTalleres.Model;

namespace UTNApiTalleres.Data.Repositorio.Interfaz
{
    public interface IOrdenDao
    {
        Task<Orden> GetOrden(int id);

        Task<IEnumerable<Orden>> GetOrders();

        Task<int> AgregarOrder(int? RecepcionId, RecepcionTurnoDTO orden);

        void ModificarOrder(Orden orden);

        void DeleteOrder(int id);

        int CancelarOrder(int id);

    }
}
