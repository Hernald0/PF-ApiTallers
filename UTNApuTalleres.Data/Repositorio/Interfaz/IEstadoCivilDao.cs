using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Data.Repositorio.Interfaz
{
    public interface IEstadoCivilDao
    {
        //Crear el nuevo objeto en la base
        Task<bool> create(Estadocivil estadocivil);

        //actualizar el objeto en la base
        Task<bool> update(Estadocivil estadocivil);

        //eliminar el objeto de la base 
        Task<bool> delete(int id);

        //recuperar un objeto desde la base        
        Task<Estadocivil> find(int id);

        //recuperar todos los objetos desde la base
        Task<IEnumerable<Estadocivil>> findAll();
    }
}
