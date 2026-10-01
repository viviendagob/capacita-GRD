using CapacitaGRD_Admin.DTOs;
using CapacitaGRD_Admin.Entidades;
using CapacitaGRDApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;
using System.Security.Cryptography;

namespace CapacitaGRD_Admin.Controllers
{
    [ValidateSessionFilter]
    public class UsuariosController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;

        public UsuariosController(IConfiguration configuration)
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

        public static string EncriptarClave(string clave)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] claveBytes = Encoding.UTF8.GetBytes(clave);
                byte[] hashBytes = sha256.ComputeHash(claveBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        [HttpGet]
        public async Task<List<UsuarioDTO>> Listar()
        {
            List<UsuarioDTO> lista = [];
            string apiUrl = $"{_apiBaseUrl}/usuarios";
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
                    lista = System.Text.Json.JsonSerializer.Deserialize<List<UsuarioDTO>>(responseData, options);
                }
                catch (HttpRequestException)
                {
                }
            }

            return lista;
        }

        [HttpPost]
        public async Task<ActionResult<PaginadorDTO<UsuarioDTO>>> Paginar(
            [FromBody] BusquedaGenericaDTO paginado
        )
        {
            var paginador = new PaginadorDTO<UsuarioDTO>();

            string apiUrl = $"{_apiBaseUrl}/usuarios/paginado";
            using (var httpClient = ConfigureHttpClient())
            {
                try
                {
                    var content = new StringContent(JsonConvert.SerializeObject(paginado), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(apiUrl, content);
                    var status = response.EnsureSuccessStatusCode().StatusCode;
                    var responseData = await response.Content.ReadAsStringAsync();
                    paginador = JsonConvert.DeserializeObject<PaginadorDTO<UsuarioDTO>>(responseData);
                }
                catch (HttpRequestException ex)
                {
                    return Json(new { success = false, titulo = "Upss", mensaje = ex.Message });
                }
            }

            return paginador;
        }


        [HttpGet]
        public async Task<UsuarioDTO> Obtener(int id)
        {
            UsuarioDTO pais = new();
            string apiUrl = $"{_apiBaseUrl}/usuarios/{id}";
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
                    pais = System.Text.Json.JsonSerializer.Deserialize<UsuarioDTO>(responseData, options);
                }
                catch (HttpRequestException)
                {

                }
            }
            return pais;
        }

        [HttpPost]
        public async Task<ActionResult> Agregar([FromBody] CrearUsuarioDTO crearUsuarioDTO)
        {
            string apiUrl = $"{_apiBaseUrl}/usuarios";

            // Encriptar la clave antes de enviar
            crearUsuarioDTO.CLAVE = EncriptarClave(crearUsuarioDTO.CLAVE);

            using (var httpClient = ConfigureHttpClient())
            {
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var content = new StringContent(JsonConvert.SerializeObject(crearUsuarioDTO), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(apiUrl, content);
                    var status = response.EnsureSuccessStatusCode().StatusCode;
                    var responseData = await response.Content.ReadAsStringAsync();
                    var datos = JsonConvert.DeserializeObject<Seccion>(responseData);

                    return Json(new { success = true, titulo = "Bien", mensaje = "Registro guardado correctamente", data = datos });
                }
                catch (HttpRequestException)
                {
                    return Json(new { success = false, titulo = "Upss", mensaje = "Registro no pudo ser guardado" });
                }
            }
        }

        [HttpPut]
        public async Task<ActionResult> Actualizar([FromBody] CrearUsuarioDTO crearUsuarioDTO, int id)
        {
            string apiUrl = $"{_apiBaseUrl}/usuarios?id={id}";

            // Encriptar la clave antes de enviar
            if (!String.IsNullOrEmpty(crearUsuarioDTO.CLAVE))
            {
                crearUsuarioDTO.CLAVE = EncriptarClave(crearUsuarioDTO.CLAVE);
            }
            

            using (var httpClient = ConfigureHttpClient())
            {
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var content = new StringContent(JsonConvert.SerializeObject(crearUsuarioDTO), Encoding.UTF8, "application/json");
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
            string apiUrl = $"{_apiBaseUrl}/usuarios?id={id}";
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



    }
}
