using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UTNApiTalleres.Application.Interfaces;
using UTNApiTalleres.Infrastructure.Repositories.Interface;
using UTNApiTalleres.Model;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Application.Services
{
    public class ServRepService : IServRepService
    {
        private readonly IServRepRepository _repository;
        //private readonly JwtTokenGenerator _tokenGenerator;

        public ServRepService(IServRepRepository repository
                            //  , JwtTokenGenerator tokenGenerator
                            )
        {
            _repository = repository;
            //_tokenGenerator = tokenGenerator;
        }
        public Task<Servicio> ActualizarServicio(Servicio servicio)
        {
            return _repository.UpdateServicio(servicio);
        }

        public Task<Servicio> CrearServicio(Servicio servicio)
        {
            return   _repository.CreateServicio(servicio);
        }

        public async Task<int> EliminarServicio(int IdServicio)
        {

            if (await _repository.TieneItems(IdServicio, "servicio"))

                return await _repository.BajaLogicaServicio(IdServicio);
            //throw new BusinessException("No se puede eliminar porque tiene presupuestos.");
            else
              
                return await _repository.DeleteServicio( IdServicio);
        }

        public Task<IEnumerable<Servicio>> GetAllServicios()
        {
            return _repository.FindAllServicio();
        }

        public Task<IEnumerable<ItemVentaDTO>> RecuperarFiltradoServRep(string pBusqueda, string? pTipo)
        {
            if (pBusqueda != null)
            {
                return _repository.FindFilterServRep(pBusqueda, pTipo);
            }
            else
            {
                //return null;
                return Task.FromResult(Enumerable.Empty<ItemVentaDTO>());
            }
        }

        #region REPUESTOS
        public Task<IEnumerable<Repuesto>> GetAllRepuestos()
            {
                return _repository.FindAllRepuestos();
            }

            public Task<Repuesto> CrearRepuesto(Repuesto repuesto)
            {
                return _repository.CreateRepuesto(repuesto);
            }

            public Task<Repuesto> ActualizarRepuesto(Repuesto repuesto)
            {
                return _repository.UpdateRepuesto(repuesto);
            }

            public async Task<int> EliminarRepuesto(int IdRepuesto)
            {

                if (await _repository.TieneItems(IdRepuesto, "repuesto"))
                   
                    return await _repository.BajaLogicaRepuesto(IdRepuesto);
                    //throw new BusinessException("No se puede eliminar porque tiene presupuestos.");
                else
                    return await _repository.DeleteRepuesto(IdRepuesto);
            }

        public async Task<bool> ExisteNombreServicio(string nombre)
        {
            return await _repository.ValidarNombreServicio(nombre);
        }

        public async Task<bool> ExisteNombreRepuesto(string nombre)
        {
            return await _repository.ValidarNombreRepuesto(nombre);
        }


        #endregion
    }
}
