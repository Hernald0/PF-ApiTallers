using System.Collections.Generic;

namespace UTNApiTalleres.Application.DTOs
{
    public class LoginResponseDTO
    {
        public int UsuarioId { get; set; }

        public string User { get; set; }
        public string NombreCompleto { get; set; }

        public int? IdEmpleado { get; set; }
        
        //public List<string> Roles { get; set; }

        public string Rol { get; set; }

        public List<AccesoDTO> Accesos { get; set; }

        public string Token { get; set; }
    }

}
