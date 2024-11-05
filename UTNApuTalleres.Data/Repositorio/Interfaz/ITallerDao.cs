using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using WebApiTalleres.Models;
using UTNApiTalleres.Model;

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

        Task<IEnumerable<Empleado>> findEmpleadoAll(int id);

        Task<bool> createEmpleado(Empleado empleado);

        Task<Modelovehiculo>createMarcaModelo(mvvmModelovehiculo marcaModelo);

        Task<IEnumerable<Cliente>> findClienteAll(int id);

        Task<IEnumerable<Marcavehiculo>> findMarcaVehiculoAll();

        Task<Marcavehiculo> findMarcaModeloVehiculo(int idMarca);

        Task<List<Marcavehiculo>> findMarcaModeloVehiculoAll();
        Task<mvvmModelovehiculo?> updateMarcaModelo(mvvmModelovehiculo marcaModelo);

        Task<int?> deleteMarcaModelo(int id);//(mvvmModelovehiculo marcaModelo);

        Task<Vehiculo> getVehiculo(int idVehiculo);

    


    }

}
