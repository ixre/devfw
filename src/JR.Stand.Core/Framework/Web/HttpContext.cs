using Microsoft.AspNetCore.Http;

namespace JR.Stand.Core.Framework.Web
{
    public static class HttpContext

    {
        private static IHttpContextAccessor _accessor;


        public static Microsoft.AspNetCore.Http.HttpContext Current => _accessor.HttpContext;


        public static void Configure(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }
    }
}