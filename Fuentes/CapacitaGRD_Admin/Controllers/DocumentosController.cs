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
    public class DocumentosController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;

        public DocumentosController(IConfiguration configuration)
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
        public async Task<List<DocumentoDTO>> Listar()
        {
            List<DocumentoDTO> lista = [];
            string apiUrl = $"{_apiBaseUrl}/documentosrequeridos";
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
                    lista = System.Text.Json.JsonSerializer.Deserialize<List<DocumentoDTO>>(responseData, options);
                }
                catch (HttpRequestException)
                {
                }
            }

            return lista;
        }

        [HttpPost]
        public async Task<ActionResult<PaginadorDTO<DocumentoDTO>>> Paginar(
            [FromBody] BusquedaGenericaDTO paginado
        )
        {
            var paginador = new PaginadorDTO<DocumentoDTO>();

            string apiUrl = $"{_apiBaseUrl}/documentosrequeridos/paginado";
            using (var httpClient = ConfigureHttpClient())
            {
                try
                {
                    var content = new StringContent(JsonConvert.SerializeObject(paginado), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(apiUrl, content);
                    var status = response.EnsureSuccessStatusCode().StatusCode;
                    var responseData = await response.Content.ReadAsStringAsync();
                    paginador = JsonConvert.DeserializeObject<PaginadorDTO<DocumentoDTO>>(responseData);                     
                }
                catch (HttpRequestException ex)
                {
                    return Json(new { success = false, titulo = "Upss", mensaje = ex.Message });
                }
            }

            return paginador;
        }

        [HttpGet]
        public async Task<DocumentoDTO> Obtener(int id)
        {
            DocumentoDTO data = new();
            string apiUrl = $"{_apiBaseUrl}/documentosrequeridos/{id}";
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
                    data = System.Text.Json.JsonSerializer.Deserialize<DocumentoDTO>(responseData, options);
                }
                catch (HttpRequestException)
                {
                    
                }
            }
            return data;
        }

        [HttpPost]
        public async Task<ActionResult> Agregar([FromBody] CrearDocumentoDTO crearDocumentoDTO)
        {
            string apiUrl = $"{_apiBaseUrl}/documentosrequeridos";
            using (var httpClient = ConfigureHttpClient())
            {
                try
                {

                    var content = new StringContent(JsonConvert.SerializeObject(crearDocumentoDTO), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(apiUrl, content);
                    var status = response.EnsureSuccessStatusCode().StatusCode;
                    var responseData = await response.Content.ReadAsStringAsync();
                    var datos = JsonConvert.DeserializeObject<Documento>(responseData);

                    return Json(new { success = true, titulo = "Bien", mensaje = "Registro guardado correctamente", data = datos });
                }
                catch (HttpRequestException)
                {
                    return Json(new { success = false, titulo = "Upss", mensaje = "Registro no pudo ser guardado" });
                }
            }
        }

        [HttpPut]
        public async Task<ActionResult> Actualizar([FromBody] CrearDocumentoDTO crearDocumentoDTO, int id)
        {
            string apiUrl = $"{_apiBaseUrl}/documentosrequeridos?id={id}";
            using (var httpClient = ConfigureHttpClient())
            {
                try
                {

                    var content = new StringContent(JsonConvert.SerializeObject(crearDocumentoDTO), Encoding.UTF8, "application/json");
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

        [HttpDelete]
        public async Task<ActionResult> Eliminar(int id)
        {
            string apiUrl = $"{_apiBaseUrl}/documentosrequeridos?id={id}";
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
