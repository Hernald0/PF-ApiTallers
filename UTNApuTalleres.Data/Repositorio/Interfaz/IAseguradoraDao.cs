using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Data.Repositorio.Interfaz
{

    public interface IAseguradoraDao
    {
        //Crear el nuevo objeto en la base
        Task<bool> create(Aseguradora aseguradora);

        //actualizar el objeto en la base
        Task<bool> update(Aseguradora aseguradora);

        //eliminar el objeto de la base 
        Task<bool> delete(int id);

        //recuperar un objeto desde la base        
        Task<Aseguradora> find(int id);

        //recuperar todos los objetos desde la base
        Task<IEnumerable<Aseguradora>> findAll();


    }

}
