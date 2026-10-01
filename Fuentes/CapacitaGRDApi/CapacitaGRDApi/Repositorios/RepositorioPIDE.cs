using CapacitaGRDApi.Entidades;
using Newtonsoft.Json;

namespace CapacitaGRDApi.Repositorios
{
    public class RepositorioPIDE : IRepositorioPIDE
    {
        private readonly DbContextClass DbContext;
        private readonly IConfiguration Configuration;

        public RepositorioPIDE(DbContextClass DbContext
            , IConfiguration Configuration)
        {
            this.DbContext = DbContext;
            this.Configuration = Configuration;
        }
 

        public async Task<Persona> ValidaDNI(string dni)
        {
            Persona persona = null;

            string servicioPIDE = Configuration.GetSection("Servicios:PIDE").Value;
            string servicioPIDEDNI = Configuration.GetSection("Servicios:DNI").Value.Replace("$DNI$", dni);

            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(servicioPIDEDNI);
            var base64 = System.Convert.ToBase64String(plainTextBytes);

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, servicioPIDE);
            var collection = new List<KeyValuePair<string, string>>();
            collection.Add(new("", base64));
            var content = new FormUrlEncodedContent(collection);
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var json_string = await response.Content.ReadAsStringAsync();

            var resp = JsonConvert.DeserializeObject<dynamic>(json_string);

            var data = resp.data;
            var objects = data.objects;
            var table1 = objects.table1;
            var _data = table1.data;
            var coResultado = _data[0].coResultado;

            if (coResultado == "0000")
            {
                var prenombres = _data[0].prenombres;
                var apPrimer = _data[0].apPrimer;
                var apSegundo = _data[0].apSegundo;
                var ubigeo = _data[0].ubigeo;

                persona = new Persona
                {
                    NOMBRES = prenombres,
                    APELLIDO_PATERNO = apPrimer,
                    APELLIDO_MATERNO = apSegundo,
                    COD_PERSONA = ubigeo
                };
            }

            return persona;
        }


        public async Task<Persona> ValidaCE(string ce)
        {
            Persona persona = null;

            string servicioPIDE = Configuration.GetSection("Servicios:PIDE").Value;
            string servicioPIDEDNI = Configuration.GetSection("Servicios:CE").Value.Replace("$CE$", ce);

            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(servicioPIDEDNI);
            var base64 = System.Convert.ToBase64String(plainTextBytes);

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, servicioPIDE);
            var collection = new List<KeyValuePair<string, string>>();
            collection.Add(new("", base64));
            var content = new FormUrlEncodedContent(collection);
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var json_string = await response.Content.ReadAsStringAsync();

            var resp = JsonConvert.DeserializeObject<dynamic>(json_string);

            var data = resp.data;
            var objects = data.objects;
            var table1 = objects.table1;
            var _data = table1.data;
            var codRespuesta = _data[0].codRespuesta;

            if (codRespuesta == "0000")
            {
                var prenombres = _data[0].nombres;
                var apPrimer = _data[0].apepaterno;
                var apSegundo = _data[0].apematerno;

                persona = new Persona
                {
                    NOMBRES = prenombres,
                    APELLIDO_PATERNO = apPrimer,
                    APELLIDO_MATERNO = apSegundo
                };
            }

            return persona;
        }

    }
}
