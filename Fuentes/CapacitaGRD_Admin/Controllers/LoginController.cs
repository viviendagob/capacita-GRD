using CapacitaGRD_Admin.DTOs;
using CapacitaGRD_Admin.Entidades;
using CapacitaGRDApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
 
namespace CapacitaGRD_Admin.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;
        private readonly string _baseFile;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
            _apiBaseUrl = _configuration.GetValue<string>("ApiSettings:BaseUrlLogin");
            _baseFile = configuration.GetValue<string>("ApiSettings:BaseFiles");
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
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoginReturn()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            string apiUrl = $"{_apiBaseUrl}/login";

            try
            {
                loginDTO.password = EncriptarClave(loginDTO.password);

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestVersion = HttpVersion.Version20;

                    // Preparar el contenido de la solicitud
                    var content = new StringContent(
                        JsonConvert.SerializeObject(loginDTO),
                        Encoding.UTF8,
                        "application/json"
                    );

                    // Realizar la solicitud POST
                    var response = await httpClient.PostAsync(apiUrl, content);

                    // Validar el estado de la respuesta
                    if (!response.IsSuccessStatusCode)
                    {
                        return Json(new
                        {
                            success = false,
                            titulo = "Error de autenticación",
                            mensaje = $"Error {response.StatusCode}: {response.ReasonPhrase}"
                        });
                    }

                    // Leer y deserializar el contenido de la respuesta
                    var responseData = await response.Content.ReadAsStringAsync();
                    var authResponse = JsonConvert.DeserializeObject<AuthResponseDTO>(responseData);

                    if (authResponse == null || string.IsNullOrEmpty(authResponse.Token))
                    {
                        return Json(new
                        {
                            success = false,
                            titulo = "Error de autenticación",
                            mensaje = "El token no fue generado correctamente."
                        });
                    }

                    Sessiones(authResponse);


                    // Retornar la respuesta exitosa
                    return Json(new
                    {
                        success = true,
                        titulo = "¡Bienvenido!",
                        mensaje = "Inicio de sesión exitoso.",
                        //data = authResponse
                    });
                }
            }
            catch (HttpRequestException ex)
            {
                // Manejo de errores de red
                return Json(new
                {
                    success = false,
                    titulo = "Error de conexión",
                    mensaje = ex.Message
                });
            }
            catch (Exception ex)
            {
                // Manejo de errores genéricos
                return Json(new
                {
                    success = false,
                    titulo = "Ups, algo salió mal",
                    mensaje = ex.Message
                });
            }
        }



        [HttpPost]
        public IActionResult CerrarSesion()
        {
            // Limpiar todas las variables de la sesión
            HttpContext.Session.Clear();

            return Json(new
            {
                success = true,
                titulo = "Sesión cerrada",
                mensaje = "Se cerró la sesión correctamente. Redirigiendo al login..."
            });
        }


        private void Sessiones(AuthResponseDTO authResponse)
        {
            // Guardar el token y su expiración en la sesión
            HttpContext.Session.SetString("AuthToken", authResponse.Token);
            HttpContext.Session.SetString("AuthTokenExpiration", authResponse.Expiration.ToString("o"));
        }



    }

    public class AuthResponseDTO
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
