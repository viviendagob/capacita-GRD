using CapacitaGRDApi;
using CapacitaGRDApi.EndPoints;
using CapacitaGRDApi.Repositorios;
using CapacitaGRDApi.Util;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;


var builder = WebApplication.CreateBuilder(args);
var origenesPermitidos = builder.Configuration.GetValue<string>("origenesPermitidos") ?? "http://localhost:5065,http://localhost:3000";
//var jwtSettings = builder.Configuration.GetSection("JwtSettings");
//var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);

// Configurar la zona horaria directamente
TimeZoneInfo defaultTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

// Registrar la zona horaria como un servicio
AppContext.SetData("DefaultTimeZone", defaultTimeZone);


builder.Services.AddDbContext<DbContextClass>();

builder.Services.AddCors(opciones =>
{ 
    //opciones.AddDefaultPolicy(configuracion =>
    //{
    //    configuracion.WithOrigins(origenesPermitidos).AllowAnyHeader().AllowAnyMethod();
    //});

    opciones.AddPolicy("origenesPermitidos", policy =>
    {
        var origenes = origenesPermitidos.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        policy.WithOrigins(origenes)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });

    //opciones.AddPolicy("libre", configuracion =>
    //{
    //    configuracion.WithOrigins(origenesPermitidos).AllowAnyHeader().AllowAnyMethod();
    //});
}
);

builder.Services.AddOutputCache();

//builder.Services.AddSwaggerGen();



QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

builder.Services.AddScoped<IRepositorioCargos, RepositorioCargos>();
builder.Services.AddScoped<IRepositorioEntidades, RepositorioEntidades>();
builder.Services.AddScoped<IRepositorioProfesiones, RepositorioProfesiones>();
builder.Services.AddScoped< IRepositorioPais, RepositorioPais>();
builder.Services.AddScoped<IRepositorioDistritos, RepositorioDistritos>();
 

builder.Services.AddScoped<IRepositorioTipoDocumentos, RepositorioTipoDocumentos>();
builder.Services.AddScoped<IRepositorioEstados, RepositorioEstados>();
builder.Services.AddScoped<IRepositorioTipoEventos, RepositorioTipoEventos>();
builder.Services.AddScoped<IRepositorioModalidad, RepositorioModalidad>();
builder.Services.AddScoped<IRepositorioTipoEncuestaRespuestas, RepositorioTipoEncuestaRespuestas>();
builder.Services.AddScoped<IRepositorioEncuestas, RepositorioEncuestas>();
builder.Services.AddScoped<IRepositorioCuestionarios, RepositorioCuestionarios>();
builder.Services.AddScoped<IRepositorioCuestionarioPreguntas, RepositorioCuestionarioPreguntas>();
builder.Services.AddScoped<IRepositorioCuestionarioPreguntaRespuestas, RepositorioCuestionarioPreguntaRespuestas>();
builder.Services.AddScoped<IRepositorioEncuestaRespuestas, RepositorioEncuestaRespuestas>();
builder.Services.AddScoped<IRepositorioPersonas, RepositorioPersonas>();
builder.Services.AddScoped<IRepositorioPersonasData, RepositorioPersonasData>();

builder.Services.AddScoped<IRepositorioEventos, RepositorioEventos>();
builder.Services.AddScoped<IRepositorioEventosFechas, RepositorioEventosFechas>();
builder.Services.AddScoped<IRepositorioEventosParticipantes, RepositorioEventosParticipantes>();
builder.Services.AddScoped<IRepositorioEventosAsistencias, RepositorioEventosAsistencias>();
builder.Services.AddScoped<IRepositorioEventoEncuestaRespuestas, RepositorioEventoEncuestaRespuestas>();
builder.Services.AddScoped<IRepositorioEventoConstancias, RepositorioEventoConstancias>();
builder.Services.AddScoped<IRepositorioReportes, RepositorioReportes>();
builder.Services.AddScoped<IRepositorioDashboard, RepositorioDashboard>();
builder.Services.AddScoped<IRepositorioPIDE, RepositorioPIDE>();
builder.Services.AddScoped<IRepositorioSecciones, RepositorioSecciones>();
builder.Services.AddScoped<IRepositorioDocumentos, RepositorioDocumentos>();
builder.Services.AddScoped<IRepositorioEventosDocumentos, RepositorioEventosDocumentos>();
builder.Services.AddScoped<IRepositorioConfiguracion, RepositorioConfiguracion>();

builder.Services.AddScoped<IEmailSender, EmailSender>();
// builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IRepositorioUsuarios, RepositorioUsuarios>();

builder.Services.Configure<EmailSenderOptions>(builder.Configuration.GetSection("EmailSenderOptions"));

builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddProblemDetails();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer( options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))

            };
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine($"Token inv�lido: {context.Exception.Message}");
                    return Task.CompletedTask;
                }
            };
        }
    );


builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "API de Eventos",
        Description = "Documentaci�n de la API para gestionar eventos",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Soporte T�cnico",
            Email = "soporte@example.com",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Licencia MIT",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Configurar autenticaci�n en Swagger para usar JWT
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Escribe 'Bearer {tu_token}' para autenticarte."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});



builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Combinado con builder.Services.AddProblemDetails(), devuelve una respuesta
    // ProblemDetails genérica sin exponer detalles internos de la excepción.
    app.UseExceptionHandler();
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors("origenesPermitidos");


app.UseOutputCache();
app.UseAuthentication();
app.UseAuthorization();


//Libres
app.MapGet("test",   [EnableCors(policyName:"libre")] () => "Capacita-DGR API enlinea");

//Generar el Token
app.MapGroup("/auth").MapAuth();
app.MapGroup("/createpdf").MapCreatePdf();


//Autentificados
app.MapGroup("/cargos").MapCargos().RequireAuthorization();
app.MapGroup("/entidades").MapEntidades().RequireAuthorization();
app.MapGroup("/profesiones").MapProfesiones().RequireAuthorization();

app.MapGroup("/paises").MapPaises().RequireAuthorization();
app.MapGroup("/usuarios").MapUsuarios().RequireAuthorization();

app.MapGroup("/distritos").MapDistritos().RequireAuthorization();
app.MapGroup("/tiposdocumentos").MapTipoDocumentos().RequireAuthorization();
app.MapGroup("/estados").MapEstados().RequireAuthorization();
app.MapGroup("/tiposeventos").MapTipoEventos().RequireAuthorization();

app.MapGroup("/eventos")
    .MapEventos().RequireAuthorization();

app.MapGroup("/tipoEncuestaRespuestas").MapTipoEncuestaRespuestas().RequireAuthorization();
app.MapGroup("/encuestas").MapEncuestas().RequireAuthorization();
app.MapGroup("/cuestionarios").MapCuestionarios().RequireAuthorization();
app.MapGroup("/eventofechas").MapEventosFechas().RequireAuthorization();
app.MapGroup("/encuestarespuestas").MapEncuestaRespuestas().RequireAuthorization();
app.MapGroup("/maestroseventos").MapMaestrosEventos().RequireAuthorization();
app.MapGroup("/maestrospersonas").MapMaestrosPersona().RequireAuthorization();
app.MapGroup("/personas").MapPersonas().RequireAuthorization();
app.MapGroup("/eventosparticipantes").MapEventosParticipantes().RequireAuthorization();
app.MapGroup("/eventosasistencias").MapEventosAsistencias().RequireAuthorization();
app.MapGroup("/secciones").MapSecciones().RequireAuthorization();
app.MapGroup("/app").MapApp().RequireAuthorization();
app.MapGroup("/registro").MapRegistro().RequireAuthorization();
app.MapGroup("/personasdata").MapPersonasData().RequireAuthorization();
app.MapGroup("/documentosrequeridos").MapDocumentosRequeridos().RequireAuthorization();
app.MapGroup("/configuracion").MapConfiguracion().RequireAuthorization();

app.MapGroup("/consultaparticipantes").ConsultaParticipantes().RequireAuthorization();

app.MapGroup("/eventoencuestas").MapEventoEncuestaRespuestas().RequireAuthorization();
app.MapGroup("/constancias").MapEventoConstancias();
app.MapGroup("/reportes").MapReportes().RequireAuthorization();
app.MapGroup("/dashboard").MapDashboard().RequireAuthorization();


app.Run();
