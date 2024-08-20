using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using WebApiTalleres.Models;
using UTNApiTalleres.Model;

namespace UTNApiTalleres.Data.Repositorio.Interfaz
{

    public interface IClienteDao
    {
        //Crear el nuevo objeto en la base
        Task<Cliente> create(Cliente pCliente);

        //actualizar el objeto en la base
        Task<Cliente> update(Cliente Cliente);

        //eliminar el objeto de la base 
        Task<bool> delete(int id);

        //recuperar un objeto desde la base        
        Task<Cliente> find(int? id);

        Task<Cliente> findByNroIdentificacion(vmIdentificador pvmIdentificador);  

        //recuperar todos los objetos desde la base
        Task<IEnumerable<Cliente>> findAll();

        Task<Cliente> InsertVehiculo(int? pIdCliente, Vehiculo pVehiculo);

        Task<Cliente> UpdateVehiculo(int pIdCliente, Vehiculo pVehiculo);

        Task<bool> deleteVehiculo(int id);


    }

}
