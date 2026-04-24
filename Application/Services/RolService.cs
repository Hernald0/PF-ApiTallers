using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UTNApiTalleres.Application.DTOs;
using UTNApiTalleres.Application.Interfaces;
using UTNApiTalleres.Infrastructure.Repositories.Interface;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Application.Services
{
    public class RolService : IRolService
    {

        private readonly IRolRepository _rolRepository;
        private readonly IRolAccesoRepository _rolAccesoRepository;
        private readonly IRolAccesoPermisoRepository _rolAccesoPermisoRepository;

        public RolService(
            IRolRepository rolRepository,
            IRolAccesoRepository rolAccesoRepository,
            IRolAccesoPermisoRepository rolAccesoPermisoRepository)
        {
            _rolRepository = rolRepository;
            _rolAccesoRepository = rolAccesoRepository;
            _rolAccesoPermisoRepository = rolAccesoPermisoRepository;
        }

        public async Task<IEnumerable<Rol>> GetAllAsync()
        {
            return await _rolRepository.GetAllAsync();
        }

        public async Task<Rol?> GetByIdAsync(int id)
        {
            return await _rolRepository.GetByIdAsync(id);
        }

        public async Task<int> CreateAsync(CreateRolDto dto)
        {

            var rol = new Rol
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Activo = true,
                Accesos = dto.Accesos
            };

            return await _rolRepository.AddAsync(rol);
            
        }

        public async Task UpdateAsync(Rol rol)
        {
             
            var rolEncontrada = await _rolRepository.GetByIdAsync(rol.RolId)
                ?? throw new ArgumentException("Rol no encontrado");



            await _rolRepository.UpdateAsync(rol);
        }

        public async Task DeleteAsync(int id)
        {
            if (!await _rolRepository.ExistsAsync(id))
                throw new ArgumentException("Rol no encontrado");

            await _rolRepository.DeleteAsync(id);
        }

        // -----------------------------
        // Relaciones
        // -----------------------------

        public async Task<int> AssignAccesoAsync(int rolId, int accesoId)
        {
            if (!await _rolRepository.ExistsAsync(rolId))
                throw new ArgumentException("Rol inválido");

            if (await _rolAccesoRepository.ExistsAsync(rolId, accesoId))
                throw new ArgumentException("El acceso ya está asignado");

            return await _rolAccesoRepository.AddRolAccesoAsync(rolId, accesoId);
        }

        public async Task<int> RemoveRolAccesoAsync(int rolId, int accesoId)
        {
            if (!await _rolAccesoRepository.ExistsAsync(rolId, accesoId))
                throw new ArgumentException("La relación no existe");

            return await _rolAccesoRepository.DeleteRolAccesoAsync(rolId, accesoId);
        }

        public async Task<int> AssignPermisoAsync(int rolId, int accesoId, int permisoId)
        {
            if (await _rolAccesoPermisoRepository.ExistsAsync(rolId, accesoId, permisoId))
                throw new ArgumentException("El permiso ya está asignado");

            return await _rolAccesoPermisoRepository.AddRolAccesoPermisoAsync(rolId, accesoId, permisoId);
        }

        public async Task<int> RemovePermisoAsync(int rolId, int accesoId, int permisoId)
        {
            if (!await _rolAccesoPermisoRepository.ExistsAsync(rolId, accesoId, permisoId))
                throw new ArgumentException("La relación no existe");

            return await _rolAccesoPermisoRepository.deleteRolAccesoPermisoAsync(rolId, accesoId, permisoId);
        }

        public async Task<bool> ExisteNombreRol(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("Se debe ingresar un nombre para el Rol.");

            return await _rolRepository.existeNombreAsync(nombre);
        }
    }
}
