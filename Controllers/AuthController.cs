using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using UTNApiTalleres.Application.DTOs;
using UTNApiTalleres.Application.Interfaces;
 
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Collections.Generic;

namespace UTNApiTalleres.Controllers
{


       
    //POST /api/auth/login

    /*
     * {
          "usuarioId": 1,
          "nombre": "Juan Perez",
          "roles": ["ADMIN"],
          "accesos": [
            { "nombre": "Usuarios", "ruta": "/usuarios" },
            { "nombre": "Accesos", "ruta": "/accesos" }
          ]
        }
     */
        [ApiController]
        [Route("api/auth")]
        public class AuthController : ControllerBase
        {
            private readonly IAuthService _service;

            public AuthController(IAuthService service)
            {
                _service = service;
            }

            [HttpPost("login")]
            public async Task<IActionResult> Login(LoginRequestDTO request)          
            {
                var result = await _service.Login(request);

                if (result == null)
                    return Unauthorized("Usuario o contraseña incorrectos");

            

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, result.User),
                new Claim(ClaimTypes.Role, result.Rol),
                new Claim("IdEmpleado", result.IdEmpleado.ToString())
            };

            //var identity = new ClaimsIdentity(claims, "Cookies");
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                                            CookieAuthenticationDefaults.AuthenticationScheme,
                                            principal
                                        );

            //await HttpContext.SignInAsync("Cookies", principal);



            return Ok(result);
            }
    }
}

