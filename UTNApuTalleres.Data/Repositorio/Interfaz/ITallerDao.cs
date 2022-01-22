using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Data.Repositorio.Interfaz
{

    public interface ITallerDao
    {
        //Crear el nuevo objeto en la base
        Task<bool> create(Taller Taller);

        //actualizar el objeto en la base
        Task<bool> update(Taller Taller);

        //eliminar el objeto de la base 
        Task<bool> delete(int id);

        //recuperar un objeto desde la base        
        Task<Taller> find(int id);

        //recuperar todos los objetos desde la base
        Task<IEnumerable<Taller>> findAll();


    }

}
