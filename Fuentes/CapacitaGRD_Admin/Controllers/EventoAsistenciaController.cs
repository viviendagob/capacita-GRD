using CapacitaGRD_Admin.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CapacitaGRD_Admin.Controllers
{
    [ValidateSessionFilter]
    public class EventoAsistenciaController : Controller
    {
        private readonly string _apiBaseUrl;

        public EventoAsistenciaController(IConfiguration configuration)
        {
            _apiBaseUrl = configuration.GetValue<string>("ApiSettings:BaseUrl");
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

        public async Task<ActionResult> Index()
        {
            List<EventoDTO> eventos = new();
            string apiUrl = $"{_apiBaseUrl}/eventos";
            using (var httpClient = ConfigureHttpClient())
            {
                try
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var response = await httpClient.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode();
                    var responseData = await response.Content.ReadAsStringAsync();
                    eventos = JsonSerializer.Deserialize<List<EventoDTO>>(responseData, options) ?? new();
                }
                catch (HttpRequestException)
                {
                }
            }

            ViewData["Eventos"] = eventos;
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Listar(int idEvento)
        {
            string apiUrl = $"{_apiBaseUrl}/eventosasistencias/evento/{idEvento}";
            using (var httpClient = ConfigureHttpClient())
            {
                try
                {
                    var response = await httpClient.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode();
                    var responseData = await response.Content.ReadAsStringAsync();
                    return Content(responseData, "application/json");
                }
                catch (HttpRequestException ex)
                {
                    return Json(new { success = false, titulo = "Upss", mensaje = ex.Message });
                }
            }
        }

        public async Task<ActionResult> Reporte(int idEvento)
        {
            string apiUrl = $"{_apiBaseUrl}/eventosasistencias/reporte/{idEvento}";
            using (var httpClient = ConfigureHttpClient())
            {
                var response = await httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                var bytes = await response.Content.ReadAsByteArrayAsync();
                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "reporte_asistencias.xlsx");
            }
        }
    }
}
