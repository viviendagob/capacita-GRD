using CapacitaGRDApi.Entidades;
using Microsoft.EntityFrameworkCore;

namespace CapacitaGRDApi
{
    public class DbContextClass : DbContext
    {
        protected readonly IConfiguration Configuration;
       
        public DbContextClass(IConfiguration configuration) 
        {
            this.Configuration = configuration;
         }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
        }

        public DbSet<Entidad> Entidad { get; set; }
        public DbSet<TipoDocumento> TipoDocumento { get; set; }
        public DbSet<Estado> Estado { get; set; }
        public DbSet<TipoEvento> TipoEvento { get; set; }
        public DbSet<Modalidad> Modalidad { get; set; }
        public DbSet<TipoEncuestaRespuesta> TipoEncuestaRespuesta { get; set; }
        

        public DbSet<Cargo> Cargo { get; set; }        
        public DbSet<Profesion> Profesion { get; set; }
        public DbSet<Pais> Pais{ get; set; }
        public DbSet<Distrito> Distrito { get; set; }
        public DbSet<Encuesta> Encuesta { get; set; }
        public DbSet<EncuestaRespuesta> EncuestaRespuesta { get; set; }
        public DbSet<Cuestionario> Cuestionario { get; set; }
        public DbSet<CuestionarioPregunta> CuestionarioPregunta { get; set; }
        public DbSet<CuestionarioPreguntaRespuesta> CuestionarioPreguntaRespuesta { get; set; }
        public DbSet<Evento> Evento { get; set; }
        public DbSet<EventoFecha> EventoFecha { get; set; }
        public DbSet<EventoAsistencia> EventoAsistencia   { get; set; }
        public DbSet<EventoParticipante> EventoParticipante { get; set; }
        public DbSet<EventoDocumento> EventoDocumento { get; set; }
        public DbSet<EventoEncuestaRespuesta> EventoEncuestaRespuesta { get; set; }
        public DbSet<EventoConstancia> EventoConstancia { get; set; }

        public DbSet<Seccion> Seccion { get; set; }
        public DbSet<Persona> Persona { get; set; }
        public DbSet<Documento> Documento { get; set; }

        public DbSet<Configuracion> Configuracion { get; set; }

        public DbSet<PersonaData> PersonaData { get; set; }

        public DbSet<Usuario> Usuario { get; set; }

        public DbSet<Usuarios> Usuarios { get; set; }

    }
}
