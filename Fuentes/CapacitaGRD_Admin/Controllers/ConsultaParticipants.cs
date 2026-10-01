using CapacitaGRD_Admin.DTOs;
using CapacitaGRDApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;

namespace CapacitaGRD_Admin.Controllers
{
    [ValidateSessionFilter]
    public class ConsultaParticipants : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;

        public ConsultaParticipants(IConfiguration configuration)
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

        [HttpPost]
        public async Task<ActionResult<PaginadorDTO<PersonaDTO>>> Paginar(
        [FromBody] BusquedaConsultaParticipanteDTO paginado
)
        {
            var paginador = new PaginadorDTO<PersonaDTO>();

            string apiUrl = $"{_apiBaseUrl}/consultaparticipantes/paginado";
            using (var httpClient = ConfigureHttpClient())
            {
                try
                {
                    var content = new StringContent(JsonConvert.SerializeObject(paginado), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(apiUrl, content);
                    var status = response.EnsureSuccessStatusCode().StatusCode;
                    var responseData = await response.Content.ReadAsStringAsync();
                    paginador = JsonConvert.DeserializeObject<PaginadorDTO<PersonaDTO>>(responseData);
                }
                catch (HttpRequestException ex)
                {
                    return Json(new { success = false, titulo = "Upss", mensaje = ex.Message });
                }
            }

            return paginador;
        }


        [HttpGet]
        public async Task<PersonaDTO> Obtener(int id)
        {
            PersonaDTO persona = new();
            string apiUrl = $"{_apiBaseUrl}/personas/{id}";
            using (var httpClient = ConfigureHttpClient())
            {
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var response = await httpClient.GetAsync(apiUrl);
                    var status = response.EnsureSuccessStatusCode().StatusCode;
                    var responseData = await response.Content.ReadAsStringAsync();
                    persona = System.Text.Json.JsonSerializer.Deserialize<PersonaDTO>(responseData, options);
                }
                catch (HttpRequestException)
                {

                }
            }
            return persona;
        }


        [HttpGet]
        public async Task<CrearPersonaDataDTO> ObtenerData(int id)
        {
            CrearPersonaDataDTO persona = new();
            string apiUrl = $"{_apiBaseUrl}/personasdata/{id}";
            using (var httpClient = ConfigureHttpClient())
            {
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var response = await httpClient.GetAsync(apiUrl);
                    var status = response.EnsureSuccessStatusCode().StatusCode;
                    var responseData = await response.Content.ReadAsStringAsync();
                    persona = System.Text.Json.JsonSerializer.Deserialize<CrearPersonaDataDTO>(responseData, options);
                }
                catch (HttpRequestException)
                {

                }
            }
            return persona;
        }

        [HttpPut]
        public async Task<ActionResult> Actualizar([FromBody] CrearPersonaDTO crearPersonaDTO, int id)
        {
            string apiUrl = $"{_apiBaseUrl}/personas?id={id}";
            using (var httpClient = ConfigureHttpClient())
            {
                try
                {
                    var content = new StringContent(JsonConvert.SerializeObject(crearPersonaDTO), Encoding.UTF8, "application/json");
                    var response = await httpClient.PutAsync(apiUrl, content);
                    var status = response.EnsureSuccessStatusCode().StatusCode;

                    return Json(new { success = true, titulo = "Bien", mensaje = "Registro actualizado correctamente" });
                }
                catch (HttpRequestException ex)
                {
                    return Json(new { success = false, titulo = "Upss", mensaje = ex.Message });
                }
            }
        }

    }
}
