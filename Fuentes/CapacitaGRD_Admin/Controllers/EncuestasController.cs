using CapacitaGRD_Admin.DTOs;
using CapacitaGRD_Admin.Entidades;
using CapacitaGRDApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;

namespace CapacitaGRD_Admin.Controllers
{
    [ValidateSessionFilter]
    public class EncuestasController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;

        public EncuestasController(IConfiguration configuration)
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

        [HttpGet]
        public async Task<List<EncuestaDTO>> Listar()
        {
            List<EncuestaDTO> lista = [];
            string apiUrl = $"{_apiBaseUrl}/encuestas";
            using (var httpClient = ConfigureHttpClient())
            {
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var response = await httpClient.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode();
                    var responseData = await response.Content.ReadAsStringAsync();
                    lista = System.Text.Json.JsonSerializer.Deserialize<List<EncuestaDTO>>(responseData, options);
                }
                catch (HttpRequestException)
                {
                }
            }

            return lista;
        }

        [HttpPost]
        public async Task<ActionResult<PaginadorDTO<EncuestaDTO>>> Paginar(
            [FromBody] BusquedaGenericaDTO paginado
        )
        {
            var paginador = new PaginadorDTO<EncuestaDTO>();

            string apiUrl = $"{_apiBaseUrl}/encuestas/paginado";
            using (var httpClient = ConfigureHttpClient())
            {
                try
                {
                    var content = new StringContent(JsonConvert.SerializeObject(paginado), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(apiUrl, content);
                    var status = response.EnsureSuccessStatusCode().StatusCode;
                    var responseData = await response.Content.ReadAsStringAsync();
                    paginador = JsonConvert.DeserializeObject<PaginadorDTO<EncuestaDTO>>(responseData);
                }
                catch (HttpRequestException ex)
                {
                    return Json(new { success = false, titulo = "Upss", mensaje = ex.Message });
                }
            }

            return paginador;
        }


        [HttpGet]
        public async Task<EncuestaDTO> Obtener(int id)
        {
            EncuestaDTO encuesta = new();
            string apiUrl = $"{_apiBaseUrl}/encuestas/{id}";
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
                    encuesta = System.Text.Json.JsonSerializer.Deserialize<EncuestaDTO>(responseData, options);
                }
                catch (HttpRequestException)
                {

                }
            }
            return encuesta;
        }

        [HttpPost]
        public async Task<ActionResult> Agregar([FromBody] CrearEncuestaDTO crearEncuestaDTO)
        {
            string apiUrl = $"{_apiBaseUrl}/encuestas";
            using (var httpClient = ConfigureHttpClient())
            {
                try
                {
                     
                    var content = new StringContent(JsonConvert.SerializeObject(crearEncuestaDTO), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(apiUrl, content);
                    var status = response.EnsureSuccessStatusCode().StatusCode;
                    var responseData = await response.Content.ReadAsStringAsync();
                    var datos = JsonConvert.DeserializeObject<Encuesta>(responseData);

                    return Json(new { success = true, titulo = "Bien", mensaje = "Registro guardado correctamente", data = datos });
                }
                catch (HttpRequestException)
                {
                    return Json(new { success = false, titulo = "Upss", mensaje = "Registro no pudo ser guardado" });
                }
            }
        }

        [HttpPut]
        public async Task<ActionResult> Actualizar([FromBody] CrearEncuestaDTO crearEncuestaDTO, int id)
        {
            string apiUrl = $"{_apiBaseUrl}/encuestas?id={id}";
            using (var httpClient = ConfigureHttpClient())
            {
                try
                {
                     
                    var content = new StringContent(JsonConvert.SerializeObject(crearEncuestaDTO), Encoding.UTF8, "application/json");
                    var response = await httpClient.PutAsync(apiUrl, content);
                    var status = response.EnsureSuccessStatusCode().StatusCode;

                    return Json(new { success = true, titulo = "Bien", mensaje = "Registro actualizado correctamente" });
                }
                catch (HttpRequestException)
                {
                    return Json(new { success = false, titulo = "Upss", mensaje = "Registro no pudo ser actualizado" });
                }
            }
        }

        [HttpDelete]
        public async Task<ActionResult> Eliminar(int id)
        {
            string apiUrl = $"{_apiBaseUrl}/encuestas?id={id}";
            using (var httpClient = ConfigureHttpClient())
            {
                try
                {                    
                    var response = await httpClient.DeleteAsync(apiUrl);
                    var status = response.EnsureSuccessStatusCode().StatusCode;
                    return Json(new { success = true, titulo = "Bien", mensaje = "Registro eliminado correctamente" });
                }
                catch (HttpRequestException)
                {
                    return Json(new { success = false, titulo = "Upss", mensaje = "Registro no puedo ser eliminado" });
                }
            }

        }



    }
}
