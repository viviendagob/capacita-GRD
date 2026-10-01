using AutoMapper;
using CapacitaGRD_Admin.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.Json;

namespace CapacitaGRD_Admin.Controllers
{
    [ValidateSessionFilter]
    public class PrincipalController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;
        private readonly string apiBaseUrl;
        private readonly string baseFile;
        private readonly string apiCaptcha;

        public PrincipalController(IConfiguration configuration
           , IMapper mapper)
        {
            this.configuration = configuration;
            this.mapper = mapper;
            this.apiBaseUrl = configuration.GetValue<string>("ApiSettings:BaseUrl");
            this.baseFile = configuration.GetValue<string>("ApiSettings:BaseFiles");
            this.apiCaptcha = configuration.GetValue<string>("ApiCaptcha");
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

        public ActionResult Index()
        {
            return View();
        }

        private bool IsSessionActive()
        {
            var authToken = HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(authToken))
            {
                return false;
            }

            var expirationString = HttpContext.Session.GetString("AuthTokenExpiration");
            if (DateTime.TryParse(expirationString, out DateTime expiration))
            {
                return expiration > DateTime.UtcNow;
            }

            return false;
        }

        public ActionResult BandejaPrincipal()
        {
            if (!IsSessionActive())
            {
                return RedirectToAction("LoginReturn", "Login");
            }

            return View(); 
        }


        public async Task<ActionResult> Participantes()
        {
            MaestroPersonaDTO maestros = new();
            string apiUrl = $"{apiBaseUrl}/maestrospersonas";

            using (var httpClient = ConfigureHttpClient())
            {
                try
                {
                    JsonSerializerOptions options = new()
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var response = await httpClient.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode();
                    var responseData = await response.Content.ReadAsStringAsync();
                    maestros = JsonSerializer.Deserialize<MaestroPersonaDTO>(responseData, options);
                }
                catch (HttpRequestException)
                {
                }
            }

            ViewData["Paises"] = maestros.Pais;
            ViewData["Profesiones"] = maestros.Profesion?.OrderBy(p => p.NOMBRE).ToList();
            ViewData["Distritos"] = maestros.Distrito;
            ViewData["Cargos"] = maestros.Cargo?.OrderBy(c => c.NOMBRE).ToList();
            ViewData["Entidades"] = maestros.Entidad;
            ViewData["Areas"] = await ObtenerAreasLaborales();

            return View();
        }

        private async Task<List<string>> ObtenerAreasLaborales()
        {
            using (var httpClient = ConfigureHttpClient())
            {
                try
                {
                    var response = await httpClient.GetAsync($"{apiBaseUrl}/personasdata/areas");
                    response.EnsureSuccessStatusCode();
                    var responseData = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<string>>(responseData) ?? new();
                }
                catch (HttpRequestException)
                {
                    return new();
                }
            }
        }

        public async Task<ActionResult> ConsultaParticipants()
        {
            ConsultaParticipantesDTO maestros = new();
            string apiUrl = $"{apiBaseUrl}/consultaparticipantes/maestros";

            using (var httpClient = ConfigureHttpClient())
            {
                try
                {
                    JsonSerializerOptions options = new()
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var response = await httpClient.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode();
                    var responseData = await response.Content.ReadAsStringAsync();
                    maestros = JsonSerializer.Deserialize<ConsultaParticipantesDTO>(responseData, options);
                }
                catch (HttpRequestException)
                {
                }
            }

            ViewData["Paises"] = maestros.Pais;
            ViewData["Profesiones"] = maestros.Profesion;
            ViewData["Distritos"] = maestros.Distrito;
            ViewData["Cargos"] = maestros.Cargo;
            ViewData["Entidades"] = maestros.Entidad;
            ViewData["Eventos"] = maestros.Evento;

            return View();
        }

        public async Task<ActionResult> Eventos()
        {
            MaestroEventoDTO maestros = new();
            string apiUrl = $"{apiBaseUrl}/maestroseventos";

            using (var httpClient = ConfigureHttpClient())
            {
                try
                {
                    JsonSerializerOptions options = new()
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var response = await httpClient.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode();
                    var responseData = await response.Content.ReadAsStringAsync();
                    maestros = JsonSerializer.Deserialize<MaestroEventoDTO>(responseData, options);
                }
                catch (HttpRequestException ex)
                {

                }
            }

            ViewData["Modalidades"] = maestros.ModalidadEvento;
            ViewData["Tipos"] = maestros.TipoEvento;
            ViewData["Estados"] = maestros.EstadoEvento;

            ViewData["Encuestas"] = maestros.Encuestas;
            ViewData["Cuestionarios"] = maestros.Cuestionarios;
            ViewData["Distritos"] = maestros.Distritos;
            ViewData["Documentos"] = maestros.Documentos;
            ViewData["apiCaptcha"] = apiCaptcha;

            return View();
        }

        public async Task<ActionResult> Configuracion()
        {
            ConfiguracionDTO configuracion = new();

            string apiUrl = $"{apiBaseUrl}/configuracion";
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
                    configuracion = JsonSerializer.Deserialize<ConfiguracionDTO>(responseData, options);
                }
                catch (HttpRequestException)
                {
                }
            }

            return View(configuracion);

        }


        public ActionResult Documentos()
        {
            
            return View();
        }

        public ActionResult Encuestas()
        {
            return View();
        }

        public ActionResult Cuestionarios()
        {
            return View();
        }

        public ActionResult Cargos()
        {         
            return View();
        }

        public async Task<ActionResult> Distritos()
        {
            List<PaisDTO> paises = [];
            string apiUrl = $"{apiBaseUrl}/paises";

            using (var httpClient = ConfigureHttpClient())
            {
                try
                {
                    JsonSerializerOptions options = new()
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var response = await httpClient.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode();
                    var responseData = await response.Content.ReadAsStringAsync();
                    paises = JsonSerializer.Deserialize<List<PaisDTO>>(responseData, options);
                }
                catch (HttpRequestException)
                {
                }
            }

            ViewData["Paises"] = paises;

            return View();
        }

        public ActionResult Paises()
        {            
            return View();
        }

        public ActionResult Usuarios()
        {
            return View();
        }

        public ActionResult profesiones()
       {            
            return View();
        }

        public async Task<ActionResult> Secciones()
        {
            List<SeccionDTO> lista = [];
            string apiUrl = $"{apiBaseUrl}/secciones";
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
                    lista = JsonSerializer.Deserialize<List<SeccionDTO>>(responseData, options);
                }
                catch (HttpRequestException)
                {
                }
             }

            return View(lista);
        }
         
    }
}
