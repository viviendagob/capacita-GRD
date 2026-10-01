using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net.Http.Headers;

namespace CapacitaGRD_Admin.Controllers
{
    public class HttpClientService 
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpClientService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public HttpClient ConfigureHttpClient()
        {
            var httpClient = new HttpClient();
            var token = _httpContextAccessor.HttpContext.Session.GetString("AuthToken");
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            return httpClient;
        }
    }
}
