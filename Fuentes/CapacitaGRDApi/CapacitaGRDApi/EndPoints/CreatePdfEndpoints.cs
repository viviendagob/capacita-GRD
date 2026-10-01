using AutoMapper;
using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;
using CapacitaGRDApi.Repositorios;
using CapacitaGRDApi.Util;
using HtmlRendererCore.PdfSharp;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using PdfSharpCore.Drawing;
using PdfSharpCore;
using PdfSharpCore.Pdf;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using PuppeteerSharp;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace CapacitaGRDApi.EndPoints
{
    public static class CreatePdfEndpoints
    {
        public static RouteGroupBuilder MapCreatePdf(this RouteGroupBuilder group)
        {
            group.MapPost("/crear", Crear);
            return group;
        }

        static async Task<Results<Ok<Entidades.Response<dynamic>>, NotFound<Entidades.Response<dynamic>>>> Crear(
            [FromBody] CrearPdfDTO crearPdfDTO,
            IMapper mapper,
            IOutputCacheStore outputCacheStore,
            IWebHostEnvironment env,
            IConfiguration Configuration,
            IEmailSender emailSender,
            IHttpContextAccessor httpContextAccessor)
        {
            var response = new Entidades.Response<dynamic>();

            // Validación de entrada
            if (string.IsNullOrEmpty(crearPdfDTO.HTML))
            {
                response.titulo = "Error";
                response.mensaje = "El contenido HTML no puede estar vacío.";
                return TypedResults.NotFound(response);
            }

            try
            {
                // Configuración para guardar el PDF
                var carpetaPDFs = Configuration.GetSection("CarpetaPDFs").Value ?? "PDFs";
                var rutaCarpetaPDFs = Path.Combine(env.WebRootPath, carpetaPDFs);
                if (!Directory.Exists(rutaCarpetaPDFs))
                {
                    Directory.CreateDirectory(rutaCarpetaPDFs);
                }

                var nombrePDF = $"{Guid.NewGuid()}.pdf";
                var rutaPDF = Path.Combine(rutaCarpetaPDFs, nombrePDF);

                // Configuración de PdfSharp para crear un documento en orientación horizontal con una imagen de fondo
                using (var document = new PdfDocument())
                {
                    var page = document.AddPage();
                    page.Orientation = PageOrientation.Landscape; // Configuración para orientación horizontal
                    page.Size = PdfSharpCore.PageSize.A4; // Tamaño A4
                    page.TrimMargins.Top = 2; // Sin márgenes superiores
                    page.TrimMargins.Bottom = 2; // Sin márgenes inferiores
                    page.TrimMargins.Left = 2;
                    page.TrimMargins.Right = 2;

                    using (var gfx = XGraphics.FromPdfPage(page))
                    {
                        // Agregar una imagen de fondo
                        var backgroundImagePath = Path.Combine(env.WebRootPath, "images", "fondopdf.jpg"); // Ruta de la imagen de fondo
                        if (File.Exists(backgroundImagePath))
                        {
                            var backgroundImage = XImage.FromFile(backgroundImagePath);
                            gfx.DrawImage(backgroundImage, 0, 0, page.Width, page.Height);
                        }

                        // Convertir el HTML en una imagen para incrustarla en el PDF
                        var htmlImage = await ConvertHtmlToImage(crearPdfDTO.HTML);  // Método asíncrono
                        gfx.DrawImage(htmlImage, 
                            page.TrimMargins.Left, 
                            page.TrimMargins.Top, 
                            page.Width - page.TrimMargins.Left - page.TrimMargins.Right, 
                            page.Height - page.TrimMargins.Top - page.TrimMargins.Bottom
                        ); // Ajuste con márgenes
                    }

                    // Guardar el PDF en el servidor
                    document.Save(rutaPDF);
                }

                // Generar la URL del PDF
                var urlBase = $"{httpContextAccessor.HttpContext!.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
                var urlPDF = $"{urlBase}/{carpetaPDFs}/{nombrePDF}";

                response.titulo = "Éxito";
                response.mensaje = "PDF creado correctamente.";
                response.data = urlPDF;

                return TypedResults.Ok(response);
            }
            catch (Exception ex)
            {
                response.titulo = "Error";
                response.mensaje = $"Ocurrió un error al crear el PDF: {ex.Message}";
                return TypedResults.NotFound(response);
            }
        }

        // Método para convertir HTML a imagen utilizando PuppeteerSharp
        public static async Task<XImage> ConvertHtmlToImage(string htmlContent)
        {
            // Inicializa BrowserFetcher para la descarga de Chromium (se asegura de que esté disponible)
            var browserFetcher = new BrowserFetcher();

            // Descarga Chromium si no está disponible
            await browserFetcher.DownloadAsync(); //BrowserFetcher.DefaultRevision

            // Lanza el navegador en modo headless
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
            var page = await browser.NewPageAsync();
            await page.SetContentAsync(htmlContent);

            // Toma una captura de pantalla de la página HTML con fondo transparente
            var screenshot = await page.ScreenshotDataAsync(new ScreenshotOptions
            {
                FullPage = true,
                Type = ScreenshotType.Png,
                OmitBackground = true // Configura el fondo transparente
            });

            // Cierra el navegador
            await browser.CloseAsync();

            // Convierte la captura de pantalla en un XImage de PdfSharpCore
            return XImage.FromStream(() => new MemoryStream(screenshot));
        }
    }
}
