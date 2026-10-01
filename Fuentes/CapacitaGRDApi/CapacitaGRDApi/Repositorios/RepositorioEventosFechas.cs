using CapacitaGRDApi.Entidades;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CapacitaGRDApi.Repositorios
{
    public class RepositorioEventosFechas : IRepositorioEventosFechas
    {
        private readonly DbContextClass DbContext;

        public RepositorioEventosFechas(DbContextClass DbContext) {
            this.DbContext = DbContext;
        }

        public async Task<List<EventoFecha>> Listar(int id)
        {
            var param = new SqlParameter("@ID_EVENTO", id);
            return await DbContext.EventoFecha.FromSqlRaw<EventoFecha>("EXEC USP_EVENTO_FECHA_ALL @ID_EVENTO", param).AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<EventoFecha>> Obtener(int id, DateTime fecha)
        {
            var parameter = new List<SqlParameter>
            {
                new("@ID_EVENTO", id),
                new("@FECHA", fecha.Date.ToString("yyyy-MM-dd"))                
            };
     
            var result = await Task.Run(() => DbContext.EventoFecha.FromSqlRaw(@"exec USP_EVENTO_FECHA_GET @ID_EVENTO, @FECHA", parameter.ToArray()).AsNoTracking().ToListAsync());
            return result;
        }

        public async Task<int> Agregar(EventoFecha eventoFecha)
        {
            var parameter = new List<SqlParameter>
            {
                new("@ID_EVENTO", eventoFecha.ID_EVENTO),
                new("@FECHA", eventoFecha.FECHA),
                new("@HORA_INICIO", eventoFecha.HORA_INICIO),
                new("@HORA_FIN", eventoFecha.HORA_FIN),

                new("@ES_ENCUESTA", eventoFecha.ES_ENCUESTA),
                new("@ID_ENCUESTA",  (eventoFecha.ID_ENCUESTA is null ? DBNull.Value : eventoFecha.ID_ENCUESTA )),
                
                new("@ES_CUESTIONARIO", eventoFecha.ES_CUESTIONARIO),
                new("@ID_CUESTIONARIO",  (eventoFecha.ID_CUESTIONARIO is null ? DBNull.Value : eventoFecha.ID_CUESTIONARIO )),
            };

            return await Task.Run(() => DbContext.Database.ExecuteSqlRawAsync(@"exec USP_EVENTO_FECHA_INS @ID_EVENTO , @FECHA , @HORA_INICIO , @HORA_FIN , @ES_ENCUESTA , @ID_ENCUESTA , @ES_CUESTIONARIO , @ID_CUESTIONARIO ", parameter));
        }


        public async Task<int> Actualizar(EventoFecha eventoFecha)
        {

            var parameter = new List<SqlParameter>
            {
                new("@ID_EVENTO", eventoFecha.ID_EVENTO),
                new("@FECHA", eventoFecha.FECHA),
                new("@HORA_INICIO", eventoFecha.HORA_INICIO),
                new("@HORA_FIN", eventoFecha.HORA_FIN),

                new("@ES_ENCUESTA", eventoFecha.ES_ENCUESTA),
                new("@ID_ENCUESTA",  (eventoFecha.ID_ENCUESTA is null ? DBNull.Value : eventoFecha.ID_ENCUESTA )),

                new("@ES_CUESTIONARIO", eventoFecha.ES_CUESTIONARIO),
                new("@ID_CUESTIONARIO",  (eventoFecha.ID_CUESTIONARIO is null ? DBNull.Value : eventoFecha.ID_CUESTIONARIO )),
            };


            return await Task.Run(() => DbContext.Database.ExecuteSqlRawAsync(@"exec USP_EVENTO_FECHA_UPD @ID_EVENTO , @FECHA , @HORA_INICIO , @HORA_FIN , @ES_ENCUESTA , @ID_ENCUESTA , @ES_CUESTIONARIO , @ID_CUESTIONARIO ", parameter));
        }

        public async Task<int> Eliminar(int id, DateTime fecha)
        {
            var result = await Task.Run(() => DbContext.Database.ExecuteSqlInterpolatedAsync($"USP_EVENTO_FECHA_DEL {id}, {fecha:yyyy-mm-dd} "));
            return result;
        }

    }
}
