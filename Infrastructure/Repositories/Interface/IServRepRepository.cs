using System.Collections.Generic;
using System.Threading.Tasks;
using UTNApiTalleres.Model;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Infrastructure.Repositories.Interface
{
    public interface IServRepRepository
    {

        #region Servicios
            Task<Servicio> CreateServicio(Servicio servicio);
            Task<int> DeleteServicio(int IdServicio);
            Task<Servicio> UpdateServicio(Servicio servicio);
            Task<Servicio> FindServicio(int IdServicio);
            Task<IEnumerable<Servicio>> FindAllServicio();

            Task<int> BajaLogicaServicio(int IdServicio);
            Task<bool> ValidarNombreServicio(string nombre);

        #endregion

        Task<IEnumerable<ItemVentaDTO>> FindFilterServRep(string pBusqueda, string? pTipo);

        Task<bool> TieneItems(int id, string tipo);

        #region Repuestos

            Task<Repuesto> CreateRepuesto(Repuesto repuesto);
            Task<int> DeleteRepuesto(int IdRepuesto);
            Task<Repuesto> UpdateRepuesto(Repuesto repuesto);
            Task<Repuesto> FindRepuesto(int IdRepuesto);
            Task<IEnumerable<Repuesto>> FindAllRepuestos();
            Task<int> BajaLogicaRepuesto(int IdRepuesto);
            Task<bool> ValidarNombreRepuesto(string nombre);

        #endregion

    }
}
