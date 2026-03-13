using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using WebApiTalleres.Models;
using UTNApiTalleres.Data.Repositorio.Interfaz;

namespace UTNApiTalleres.Data.Repositorio
{
    public class UsuarioDao : IUsuarioDao
    {

        private PostgresqlConfiguration _connectionString;

        public UsuarioDao(PostgresqlConfiguration connectionString)
        {
            this._connectionString = connectionString;
        }

        protected NpgsqlConnection dbConnection()
        {
            return new NpgsqlConnection(this._connectionString.ConnectionString);
        }

        public async Task<IEnumerable<WebApiTalleres.Models.Usuario>> GetAllAsync()
        {
            using var db = dbConnection();

            var usuarios = await db.QueryAsync<Usuario>(@"SELECT      u.""Id"",
	                           u.""User"",
	                           pe.""Apellido"" ||', '|| pe.""Nombre"" as ""NombreCompleto"",
	                           u.""Email"",
	                           u.""Activo""
                        FROM public.""Usuarios"" u
	                        left JOIN public.""Personas"" pe
		                        ON pe.""Id"" = u.""IdPersona""
                        order by u.""Id"" ");
            foreach (var u in usuarios)
            {
                var rol = await db.QueryAsync<Rol>(
                    @"SELECT r.* FROM public.""Roles"" r
                      INNER JOIN public.""UsuarioRol"" ur ON ur.""RolId"" = r.""RolId""
                      WHERE ur.""UserId"" = @id", new { id = u.Id });

                u.Rol = rol.ToList().FirstOrDefault();
            }
            return usuarios.ToList();
        }

        public async Task<WebApiTalleres.Models.Usuario> GetUsuario(int id)
        {
            using var db = dbConnection();

            var usuario =  await db.QueryFirstOrDefaultAsync<Usuario>(
                 @"SELECT      u.""Id"",
	                           u.""User"",
	                           pe.""Apellido"" ||', '|| pe.""Nombre"" as ""NombreCompleto"",
	                           u.""Email"",
	                           u.""Activo""
                        FROM public.""Usuarios"" u
	                        left JOIN public.""Personas"" pe
		                        ON pe.""Id"" = u.""IdPersona""
                        WHERE u.""Id"" = @id ",
       
                 new { id = id });

            if (usuario == null)
                return null;

            var rol = await db.QueryFirstOrDefaultAsync<Rol>(
                      @"SELECT r.* FROM public.""Roles"" r
                      INNER JOIN public.""UsuarioRol"" ur ON ur.""RolId"" = r.""RolId""
                      WHERE ur.""UserId"" = @id",new { id = usuario.Id });


            usuario.Rol = rol;

            return usuario;
        }

        public async Task<int> InsertAsync(Usuario usuario)
        {
            using var db = dbConnection();
            var sql = @"INSERT INTO public.""Usuarios"" (""User"", ""Password"", ""Email"", ""IdPersona"", ""Activo"", ""FecAlta"")
                        VALUES (@User, @Password, @Email, @IdPersona, @Activo, @FecAlta)
                        RETURNING ""Id"";";
            var idUser = await db.ExecuteScalarAsync<int>(sql, new
            {
                User = usuario.User,
                Password = usuario.Password,
                Email = usuario.Email,
                IdPersona = usuario.IdPersona,
                Activo = usuario.Activo,
                FecAlta = DateTime.Now
            });

            
            sql = @"INSERT INTO public.""UsuarioRol""(""RolId"",""UserId"") VALUES (@RolId, @UserId)";

            return await db.ExecuteScalarAsync<int>(sql, new
            {
                RolId = usuario.Rol.RolId,
                UserId = idUser

            });
        }

        public async Task UpdateAsync(Usuario usuario)
        {
            using var db = dbConnection();
            var sql = @"UPDATE public.""Usuarios"" 
                        SET ""User""=@user, ""Activo""=@Activo, ""Email""=@email
                        WHERE ""Id""=@Id";
            await db.ExecuteAsync(sql, usuario);
        }

        public async Task DeleteAsync(int id)
        {
            using var db = dbConnection();
            await db.ExecuteAsync(@"DELETE FROM public.""Usuarios"" WHERE ""Id""=@id", new { id });
        }
    }
}
