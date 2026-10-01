using CapacitaGRD_Admin.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;

namespace CapacitaGRD_Admin.Controllers
{
    [ValidateSessionFilter]
    public class EventosController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;
        private readonly string _baseFile;

        public EventosController(IConfiguration configuration)
        {
            _configuration = configuration;
            _apiBaseUrl = _configuration.GetValue<string>("ApiSettings:BaseUrl");
            _baseFile = configuration.GetValue<string>("ApiSettings:BaseFiles");
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
        public async Task<ActionResult<PaginadorDTO<EventoDTO>>> Paginar(
        [FromBody] BusquedaEventoDTO paginado)
        {
            var paginador = new PaginadorDTO<EventoDTO>();

            string apiUrl = $"{_apiBaseUrl}/eventos/paginado";
            using (var httpClient = ConfigureHttpClient())
            {
                try
                {
                    var content = new StringContent(JsonConvert.SerializeObject(paginado), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(apiUrl, content);
                    var status = response.EnsureSuccessStatusCode().StatusCode;
                    var responseData = await response.Content.ReadAsStringAsync();
                    paginador = JsonConvert.DeserializeObject<PaginadorDTO<EventoDTO>>(responseData);
                }
                catch (HttpRequestException ex)
                {
                    return Json(new { success = false, titulo = "Upss", mensaje = ex.Message });
                }
            }

            return paginador;
        }

        [HttpPost]
        public async Task<ActionResult> Agregar([FromBody] CrearEventoDTO crearEventoDTO)
        {
            string apiUrl = $"{_apiBaseUrl}/eventos";
            using (var httpClient = ConfigureHttpClient())
            {
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var content = new StringContent(JsonConvert.SerializeObject(crearEventoDTO), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(apiUrl, content);
                    var status = response.EnsureSuccessStatusCode().StatusCode;
                    var responseData = await response.Content.ReadAsStringAsync();
                    var datos = JsonConvert.DeserializeObject<EventoDTO>(responseData);

                    return Json(new { success = true, titulo = "Bien", mensaje = "Registro guardado correctamente", data = datos });
                }
                catch (HttpRequestException ex)
                {
                    return Json(new { success = false, titulo = "Upss", mensaje = ex.Message });
                }
            }
        }


        [HttpPut]
        public async Task<ActionResult> Actualizar([FromBody] CrearEventoDTO crearEventoDTO, int id)
        {
            string apiUrl = $"{_apiBaseUrl}/eventos?id={id}";
            using (var httpClient = ConfigureHttpClient())
            {
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var content = new StringContent(JsonConvert.SerializeObject(crearEventoDTO), Encoding.UTF8, "application/json");
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
            string apiUrl = $"{_apiBaseUrl}/eventos?id={id}";
            using (var httpClient = ConfigureHttpClient())
            {
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
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


        [HttpGet]
            public async Task<EventoDTO> Obtener(int id)
            {
                EventoDTO evento = new();
                string apiUrl = $"{_apiBaseUrl}/eventos/{id}";
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
                        evento = System.Text.Json.JsonSerializer.Deserialize<EventoDTO>(responseData, options);
                    }
                    catch (HttpRequestException)
                    {

                    }
                }

                return evento;
            }

        [HttpGet]
        public async Task<ActionResult> ObtenerBanner(int id)
        {
            EventoDTO evento = new();
            string apiUrl = $"{_apiBaseUrl}/eventos/{id}";
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
                    evento = System.Text.Json.JsonSerializer.Deserialize<EventoDTO>(responseData, options);
                    return Json(new { success = true, titulo = "Bien", mensaje = "", url = _baseFile + "eventos/" + evento.COD_EVENTO + "/banner/" +  evento.BANNER});
                }
                catch (HttpRequestException)
                {
                    return Json(new { success = true, titulo = "Advertencia", mensaje = "Evento no encontrado" });
                }
            }
         }

        [HttpGet]
        public async Task<ActionResult> ObtenerFormato(int id)
        {
            EventoDTO evento = new();
            string apiUrl = $"{_apiBaseUrl}/eventos/{id}";
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
                    evento = System.Text.Json.JsonSerializer.Deserialize<EventoDTO>(responseData, options);
                    return Json(new { success = true, titulo = "Bien", mensaje = "", url = _baseFile + "eventos/" + evento.COD_EVENTO + "/formato/" + evento.FORMATO });
                }
                catch (HttpRequestException)
                {
                    return Json(new { success = true, titulo = "Advertencia", mensaje = "Evento no encontrado" });
                }
            }
        }

        [HttpGet]
        public async Task<ActionResult> Tablero()
        {
            var estadisticas = new EventoEstadisticaDTO();
            string apiUrl = $"{_apiBaseUrl}/eventos/estadisticas";
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
                    estadisticas = System.Text.Json.JsonSerializer.Deserialize<EventoEstadisticaDTO>(responseData, options);
                }
                catch (HttpRequestException)
                {
                }
            }

            return View(estadisticas);
        }

        [HttpGet]
        public async Task<ActionResult> Reporte(int id)
        {
            string apiUrl = $"{_apiBaseUrl}/eventosparticipantes/reporte/{id}";
            using (var httpClient = ConfigureHttpClient())
            {
                var response = await httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                var bytes = await response.Content.ReadAsByteArrayAsync();
                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "reporte_evento.xlsx");
            }
        }

        [HttpPut]
        public async Task<ActionResult> ActualizarFechas([FromBody] List<CrearEventoFechaDTO> crearEventoFechaDTO, int id)
        {
            string apiUrl = $"{_apiBaseUrl}/eventos/fechas?id={id}";
            using (var httpClient = ConfigureHttpClient())
            {
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var content = new StringContent(JsonConvert.SerializeObject(crearEventoFechaDTO), Encoding.UTF8, "application/json");
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


    }
}
