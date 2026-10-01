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
    public class CuestionariosController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;

        public CuestionariosController(IConfiguration configuration)
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

        //[HttpGet]
        //public async Task<List<EncuestaDTO>> Listar()
        //{
        //    List<EncuestaDTO> lista = [];
        //    string apiUrl = $"{_apiBaseUrl}/encuestas";
        //    using (var httpClient = new HttpClient())
        //    {
        //        try
        //        {
        //            var options = new JsonSerializerOptions
        //            {
        //                PropertyNameCaseInsensitive = true
        //            };
        //            var response = await httpClient.GetAsync(apiUrl);
        //            response.EnsureSuccessStatusCode();
        //            var responseData = await response.Content.ReadAsStringAsync();
        //            lista = System.Text.Json.JsonSerializer.Deserialize<List<EncuestaDTO>>(responseData, options);
        //        }
        //        catch (HttpRequestException)
        //        {
        //        }
        //    }

        //    return lista;
        //}

        [HttpPost]
        public async Task<ActionResult<PaginadorDTO<CuestionarioDTO>>> Paginar(
            [FromBody] BusquedaGenericaDTO paginado
        )
        {
            var paginador = new PaginadorDTO<CuestionarioDTO>();

            string apiUrl = $"{_apiBaseUrl}/cuestionarios/paginado";
            using (var httpClient = ConfigureHttpClient())
            {
                try
                {
                    var content = new StringContent(JsonConvert.SerializeObject(paginado), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(apiUrl, content);
                    var status = response.EnsureSuccessStatusCode().StatusCode;
                    var responseData = await response.Content.ReadAsStringAsync();
                    paginador = JsonConvert.DeserializeObject<PaginadorDTO<CuestionarioDTO>>(responseData);
                }
                catch (HttpRequestException ex)
                {
                    return Json(new { success = false, titulo = "Upss", mensaje = ex.Message });
                }
            }

            return paginador;
        }


        [HttpGet]
        public async Task<ActionResult> Obtener(int id)
        {
            string apiUrl = $"{_apiBaseUrl}/cuestionarios/{id}";
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
                    var cuestionario = System.Text.Json.JsonSerializer.Deserialize<CuestionarioDTO>(responseData, options);

                    return Json(new
                    {
                        iD_CUESTIONARIO = cuestionario.ID_CUESTIONARIO,
                        nombre = cuestionario.NOMBRE,
                        preguntas = (cuestionario.PREGUNTAS ?? new List<CuestionarioPreguntaDTO>()).Select(p => new
                        {
                            iD_CUESTIONARIO_PREGUNTA = p.ID_CUESTIONARIO_PREGUNTA,
                            nombre = p.NOMBRE,
                            peso = p.PESO,
                            respuestas = (p.RESPUESTAS ?? new List<CuestionarioPreguntaRespuestaDTO>()).Select(r => new
                            {
                                iD_PREGUNTA_RESPUESTA = r.ID_PREGUNTA_RESPUESTA,
                                nombre = r.NOMBRE
                            })
                        })
                    });
                }
                catch (HttpRequestException ex)
                {
                    return Json(new { success = false, titulo = "Upss", mensaje = ex.Message, preguntas = new List<object>() });
                }
            }
        }

        [HttpPost]
        public async Task<ActionResult> Agregar([FromBody] CrearCuestionarioDTO crearCuestionarioDTO)
        {
            string apiUrl = $"{_apiBaseUrl}/cuestionarios";
            using (var httpClient = ConfigureHttpClient())
            {
                try
                {
                    var content = new StringContent(JsonConvert.SerializeObject(crearCuestionarioDTO), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(apiUrl, content);
                    var status = response.EnsureSuccessStatusCode().StatusCode;
                    var responseData = await response.Content.ReadAsStringAsync();
                    var datos = JsonConvert.DeserializeObject<CuestionarioDTO>(responseData);

                    return Json(new { success = true, titulo = "Bien", mensaje = "Registro guardado correctamente", data = datos });
                }
                catch (HttpRequestException ex)
                {
                    return Json(new { success = false, titulo = "Upss", mensaje = ex.Message });
                }
            }
        }

        [HttpPut]
        public async Task<ActionResult> Actualizar([FromBody] CrearCuestionarioDTO crearCuestionarioDTO, int id)
        {
            string apiUrl = $"{_apiBaseUrl}/cuestionarios?id={id}";
            using (var httpClient = ConfigureHttpClient())
            {
                try
                {
                    var content = new StringContent(JsonConvert.SerializeObject(crearCuestionarioDTO), Encoding.UTF8, "application/json");
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

        [HttpDelete]
        public async Task<ActionResult> Eliminar(int id)
        {
            string apiUrl = $"{_apiBaseUrl}/cuestionarios?id={id}";
            using (var httpClient = ConfigureHttpClient())
            {
                try
                {
                    var response = await httpClient.DeleteAsync(apiUrl);
                    var status = response.EnsureSuccessStatusCode().StatusCode;
                    return Json(new { success = true, titulo = "Bien", mensaje = "Registro eliminado correctamente" });
                }
                catch (HttpRequestException ex)
                {
                    return Json(new { success = false, titulo = "Upss", mensaje = ex.Message });
                }
            }
        }

    }
}
