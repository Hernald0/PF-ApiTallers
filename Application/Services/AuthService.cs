using UTNApiTalleres.Application.Interfaces;
using UTNApiTalleres.Application.DTOs;
using UTNApiTalleres.Infrastructure.Repositories.Interface;
using System.Threading.Tasks;

namespace UTNApiTalleres.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repository;
        //private readonly JwtTokenGenerator _tokenGenerator;

        public AuthService(IAuthRepository repository
                          //  , JwtTokenGenerator tokenGenerator
                            )
        {
            _repository = repository;
            //_tokenGenerator = tokenGenerator;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO request)
        {
            var usuario = _repository.ValidarUsuario(request.User, request.Password);

            if (usuario == null)
                return null;
                
            //var roles = _repository.ObtenerRoles(usuario.Id);
            var rol     = _repository.ObtenerRol(usuario.Id);
            var accesos = await _repository.ObtenerAccesosAsync(usuario.Id);

           // var token = _tokenGenerator.GenerateToken(usuario, roles);

            return new LoginResponseDTO
            {
                UsuarioId = usuario.Id,
                NombreCompleto = usuario.NombreCompleto,
                User = usuario.User,
                IdEmpleado = usuario.IdEmpleado,
                Rol = rol,
                Accesos = accesos
               // Token = token
            };
        }
    }

}
