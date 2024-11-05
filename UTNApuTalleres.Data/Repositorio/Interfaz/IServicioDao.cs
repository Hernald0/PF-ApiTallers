using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UTNApiTalleres.Model;

namespace UTNApiTalleres.Data.Repositorio.Interfaz
{
    public interface IServicioDao
    {
        //Crear el nuevo objeto en la base
        Task<bool> create(Servicio Servicio);

        //actualizar el objeto en la base
        Task<bool> update(Servicio Servicio);

        //eliminar el objeto de la base 
        Task<bool> delete(int id);

        //recuperar un objeto desde la base        
        Task<Servicio> find(int id);

        //recuperar todos los objetos desde la base
        Task<IEnumerable<Servicio>> findAll();

     

    }
}


