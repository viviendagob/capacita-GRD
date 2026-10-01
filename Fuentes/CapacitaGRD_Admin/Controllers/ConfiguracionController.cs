using CapacitaGRD_Admin.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace CapacitaGRD_Admin.Controllers
{
    [ValidateSessionFilter]
    public class ConfiguracionController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;

        public ConfiguracionController(IConfiguration configuration)
        {
            _configuration = configuration;
            _apiBaseUrl = _configuration.GetValue<string>("ApiSettings:BaseUrl");
        }

        private HttpClient ConfigureHttpClient()
        {
            var httpClient = new HttpClient();
            var token = HttpContext.Session.GetString("AuthToken");
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            return httpClient;
        }

        [HttpPut]
        public async Task<ActionResult> Actualizar(
            [FromBody] CrearConfiguracionDTO crearConfiguracionDTO
        )
        {
            string apiUrl = $"{_apiBaseUrl}/configuracion";
            using (var httpClient = ConfigureHttpClient())
            {
                try
                {
                    var content = new StringContent(JsonConvert.SerializeObject(crearConfiguracionDTO), Encoding.UTF8, "application/json");
                    var response = await httpClient.PutAsync(apiUrl, content);
                    var status = response.EnsureSuccessStatusCode().StatusCode;

                    return Json(new { success = true, titulo = "Bien", mensaje = "Registro actualizaco correctamente" });
                }
                catch (HttpRequestException)
                {
                    return Json(new { success = false, titulo = "Upss", mensaje = "Registro no pudo ser actualizaco" });
                }
            }
        }


    }
}
