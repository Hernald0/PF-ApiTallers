using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Data.Repositorio.Interfaz
{

    public interface IPersonaDao
    {
        //Crear el nuevo objeto en la base
        Task<bool> create(Persona Persona);

        //actualizar el objeto en la base
        Task<bool> update(Persona Persona);

        //eliminar el objeto de la base 
        Task<bool> delete(int id);

        //recuperar un objeto desde la base        
        Task<Persona> find(int id);

        //recuperar todos los objetos desde la base
        Task<IEnumerable<Persona>> findAll();


    }

}
