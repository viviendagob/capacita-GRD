var builder = WebApplication.CreateBuilder(args);

var origenesPermitidos = builder.Configuration.GetValue<string>("AllowedHosts")!;


builder.Services.AddCors(opciones =>
{
    opciones.AddDefaultPolicy(configuracion =>
    {
        configuracion.WithOrigins(origenesPermitidos).AllowAnyHeader().AllowAnyMethod();
    });
}
);

builder.Services.AddDistributedMemoryCache(); // Almacena los datos de la sesi�n en memoria
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // Duraci�n de la sesi�n
    options.Cookie.HttpOnly = true; // Restringe el acceso a las cookies desde el cliente
    options.Cookie.IsEssential = true; // Marca la cookie como esencial
});

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(typeof(Program));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
     app.UseHsts();
}

app.UseHttpsRedirection();
// Sin esto el navegador re-descarga TODO el CSS/JS de las librerías (jQuery, Bootstrap,
// DataTables, etc.) en cada navegación de página completa, lo que se siente como una
// pausa larga con el loader del ministerio en cada clic de menú.
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.CacheControl = $"public,max-age={60 * 60 * 24 * 7}";
    }
});

app.UseRouting();
app.UseSession(); // Habilita el uso de sesiones
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Principal}/{action=Index}/{id?}");

app.Run();
