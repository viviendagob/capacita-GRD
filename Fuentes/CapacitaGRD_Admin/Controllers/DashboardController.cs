using CapacitaGRD_Admin.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace CapacitaGRD_Admin.Controllers
{
    [ValidateSessionFilter]
    public class DashboardController : Controller
    {
        private readonly string _apiBaseUrl;
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        public DashboardController(IConfiguration configuration)
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
            using var httpClient = ConfigureHttpClient();

            MaestroEventoDTO? maestroEvento = null;
            ConsultaParticipantesDTO? maestroParticipantes = null;

            try
            {
                var responseEvento = await httpClient.GetAsync($"{_apiBaseUrl}/maestroseventos");
                responseEvento.EnsureSuccessStatusCode();
                maestroEvento = JsonSerializer.Deserialize<MaestroEventoDTO>(await responseEvento.Content.ReadAsStringAsync(), JsonOptions);

                var responseParticipantes = await httpClient.GetAsync($"{_apiBaseUrl}/consultaparticipantes/maestros");
                responseParticipantes.EnsureSuccessStatusCode();
                maestroParticipantes = JsonSerializer.Deserialize<ConsultaParticipantesDTO>(await responseParticipantes.Content.ReadAsStringAsync(), JsonOptions);
            }
            catch (HttpRequestException)
            {
            }

            ViewData["Eventos"] = maestroParticipantes?.Evento ?? new List<EventoDTO>();
            ViewData["Modalidades"] = maestroEvento?.ModalidadEvento ?? new List<ModalidadDTO>();
            ViewData["Estados"] = maestroEvento?.EstadoEvento ?? new List<EstadoDTO>();
            ViewData["Entidades"] = maestroParticipantes?.Entidad ?? new List<EntidadDTO>();
            ViewData["Distritos"] = maestroEvento?.Distritos ?? new List<DistritoDTO>();

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Kpis([FromBody] DashboardFiltroDTO filtro)
        {
            using var httpClient = ConfigureHttpClient();
            var content = new StringContent(JsonSerializer.Serialize(filtro), Encoding.UTF8, "application/json");
            try
            {
                var response = await httpClient.PostAsync($"{_apiBaseUrl}/dashboard/kpis", content);
                response.EnsureSuccessStatusCode();
                var responseData = await response.Content.ReadAsStringAsync();
                return Content(responseData, "application/json");
            }
            catch (HttpRequestException ex)
            {
                return Json(new { success = false, mensaje = ex.Message });
            }
        }
    }
}
