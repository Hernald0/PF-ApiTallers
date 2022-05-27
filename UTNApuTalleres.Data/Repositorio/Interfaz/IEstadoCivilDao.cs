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
        Task<bool> create(EstadoCivil EstadoCivil);

        //actualizar el objeto en la base
        Task<bool> update(EstadoCivil EstadoCivil);

        //eliminar el objeto de la base 
        Task<bool> delete(int id);

        //recuperar un objeto desde la base        
        Task<EstadoCivil> find(int id);

        //recuperar todos los objetos desde la base
        Task<IEnumerable<EstadoCivil>> findAll();
    }
}
