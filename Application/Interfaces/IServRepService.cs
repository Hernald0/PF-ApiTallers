using System.Collections.Generic;
using System.Threading.Tasks;
using UTNApiTalleres.Model;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Application.Interfaces
{
    public interface IServRepService
    {
        #region SERVICIO
            Task<Servicio> CrearServicio(Servicio servicio);


            Task<Servicio>  ActualizarServicio(Servicio servicio);

            Task<int> EliminarServicio(int IdServicio);

            Task<IEnumerable<Servicio>> GetAllServicios();

            Task<IEnumerable<ItemVentaDTO>> RecuperarFiltradoServRep(string pBusqueda, string? pTipo);
        
            Task<bool> ExisteNombreServicio(string nombre);


        #endregion

        #region REPUESTO

        Task<Repuesto> CrearRepuesto(Repuesto repuesto);

            Task<Repuesto> ActualizarRepuesto(Repuesto repuesto);

            Task<int> EliminarRepuesto(int IdRepuesto);

            Task<IEnumerable<Repuesto>> GetAllRepuestos();

            Task<bool> ExisteNombreRepuesto(string nombre);

        #endregion

    }
}
