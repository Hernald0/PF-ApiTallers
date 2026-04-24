using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;
using UTNApiTalleres.Application.Interfaces;

namespace UTNApiTalleres.Application.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {   
            _httpContextAccessor = httpContextAccessor;
        }

        /*
        public string UserName =>
            _httpContextAccessor.HttpContext?.User?                
                .FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
        */
        public string UserName
        {   /*
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;

                var claims = user?.Claims.Select(c => new { c.Type, c.Value }).ToList();

                // Poné breakpoint acá
                return user?.FindFirst(ClaimTypes.Name)?.Value;
            }*/
            get
            {
                var context = _httpContextAccessor.HttpContext;

                if (context == null)
                    return "SIN_CONTEXTO"; // HttpContext es null

                var user = context.User;

                if (user == null)
                    return "SIN_USER";

                if (!user.Identity?.IsAuthenticated ?? true)
                    return "NO_AUTENTICADO"; // Llegó pero sin autenticar

                var name = user.FindFirst(ClaimTypes.Name)?.Value;

                return name ?? "SIN_CLAIM_NAME"; // Autenticado pero sin el claim
            }
        }

    }
}
