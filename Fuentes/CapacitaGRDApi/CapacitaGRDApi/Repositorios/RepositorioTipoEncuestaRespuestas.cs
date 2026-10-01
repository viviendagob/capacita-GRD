using CapacitaGRDApi.Entidades;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using System.Formats.Asn1;

namespace CapacitaGRDApi.Repositorios
{
    public class RepositorioTipoEncuestaRespuestas : IRepositorioTipoEncuestaRespuestas
    {
        private readonly DbContextClass DbContext;

        public RepositorioTipoEncuestaRespuestas(DbContextClass DbContext) {
            this.DbContext = DbContext;
        }

        public async Task<List<TipoEncuestaRespuesta>> Listar()
        {
            return await DbContext.TipoEncuestaRespuesta.FromSqlRaw<TipoEncuestaRespuesta>("EXEC USP_MAE_TIPO_ENCUESTA_RESPUESTA_ALL").AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<TipoEncuestaRespuesta>> Obtener(int id)        
        {             
            var param = new SqlParameter("@ID_TIPO_ENCUESTA_PREGUNTA", id);
            var result = await Task.Run(() => DbContext.TipoEncuestaRespuesta.FromSqlRaw(@"exec USP_MAE_TIPO_ENCUESTA_RESPUESTA_GET @ID_TIPO_ENCUESTA_PREGUNTA", param).AsNoTracking().ToListAsync());            
            return  result;
        }
         

    }
}
