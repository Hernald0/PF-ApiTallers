using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Data.Repositorio.Interfaz
{

    public interface IClienteDao
    {
        //Crear el nuevo objeto en la base
        Task<bool> create(Cliente Cliente);

        //actualizar el objeto en la base
        Task<bool> update(Cliente Cliente);

        //eliminar el objeto de la base 
        Task<bool> delete(int id);

        //recuperar un objeto desde la base        
        Task<Cliente> find(int id);

        //recuperar todos los objetos desde la base
        Task<IEnumerable<Cliente>> findAll();


    }

}
