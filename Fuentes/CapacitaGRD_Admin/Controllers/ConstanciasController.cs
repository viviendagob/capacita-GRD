using CapacitaGRD_Admin.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace CapacitaGRD_Admin.Controllers
{
    [ValidateSessionFilter]
    public class ConstanciasController : Controller
    {
        private readonly string _apiBaseUrl;
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        public ConstanciasController(IConfiguration configuration)
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
            using (var httpClient = ConfigureHttpClient())
            {
                try
                {
                    var response = await httpClient.GetAsync($"{_apiBaseUrl}/eventos");
                    response.EnsureSuccessStatusCode();
                    var responseData = await response.Content.ReadAsStringAsync();
                    eventos = JsonSerializer.Deserialize<List<EventoDTO>>(responseData, JsonOptions) ?? new();
                }
                catch (HttpRequestException)
                {
                }
            }

            ViewData["Eventos"] = eventos;
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Elegibles(int idEvento)
        {
            using var httpClient = ConfigureHttpClient();
            try
            {
                var response = await httpClient.GetAsync($"{_apiBaseUrl}/constancias/elegibles/{idEvento}");
                response.EnsureSuccessStatusCode();
                var responseData = await response.Content.ReadAsStringAsync();
                return Content(responseData, "application/json");
            }
            catch (HttpRequestException ex)
            {
                return Json(new { success = false, mensaje = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> Generar(int idEvento, int idPersona)
        {
            using var httpClient = ConfigureHttpClient();
            var response = await httpClient.PostAsync(
                $"{_apiBaseUrl}/constancias/generar?idEvento={idEvento}&idPersona={idPersona}", null);
            var responseData = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                return Json(new { success = false, mensaje = responseData });
            }
            return Content(responseData, "application/json");
        }

        [HttpGet]
        public async Task<ActionResult> Buscar(string? documento, int? idEvento, string? codigo)
        {
            using var httpClient = ConfigureHttpClient();
            var query = new StringBuilder("?");
            if (!string.IsNullOrWhiteSpace(documento)) query.Append("documento=" + Uri.EscapeDataString(documento) + "&");
            if (idEvento.HasValue && idEvento.Value > 0) query.Append("idEvento=" + idEvento.Value + "&");
            if (!string.IsNullOrWhiteSpace(codigo)) query.Append("codigo=" + Uri.EscapeDataString(codigo) + "&");

            var response = await httpClient.GetAsync($"{_apiBaseUrl}/constancias/buscar{query}");
            response.EnsureSuccessStatusCode();
            var responseData = await response.Content.ReadAsStringAsync();
            return Content(responseData, "application/json");
        }

        [HttpGet]
        public async Task<ActionResult> Descargar(int idConstancia)
        {
            using var httpClient = ConfigureHttpClient();
            var response = await httpClient.GetAsync($"{_apiBaseUrl}/constancias/{idConstancia}/descargar");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }
            var responseData = await response.Content.ReadAsStringAsync();
            var constancia = JsonSerializer.Deserialize<ConstanciaDTO>(responseData, JsonOptions);
            if (constancia?.URL_PDF is null)
            {
                return NotFound();
            }
            return Redirect(constancia.URL_PDF);
        }

        [HttpPost]
        public async Task<ActionResult> Anular(int idConstancia, string motivo)
        {
            using var httpClient = ConfigureHttpClient();
            var body = new AnularConstanciaDTO { MOTIVO = motivo };
            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            var response = await httpClient.PutAsync($"{_apiBaseUrl}/constancias/{idConstancia}/anular", content);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return Json(new { success = false, mensaje = error });
            }
            return Json(new { success = true });
        }
    }
}
