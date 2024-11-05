using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UTNApiTalleres.Model;

namespace UTNApiTalleres.Data.Repositorio.Interfaz
{
    public interface IServRepDao
    {

        #region Servicios
        Task<Servicio> CreateServicio(Servicio servicio);
        Task<int> DeleteServicio(int IdServicio);
        Task<Servicio> UpdateServicio(Servicio servicio);
        Task<Servicio> FindServicio(int IdServicio);
        Task<IEnumerable<Servicio>> FindAllServicio();

        #endregion

        Task<IEnumerable<ItemDto>> FindFilterServRep(string pBusqueda);

    }
}
