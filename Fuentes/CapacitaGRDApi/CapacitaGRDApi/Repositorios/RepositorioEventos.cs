using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;
using CapacitaGRDApi.Util;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using static CapacitaGRDApi.Util.Paginado;

namespace CapacitaGRDApi.Repositorios
{
    public class RepositorioEventos : IRepositorioEventos
    {
        private readonly DbContextClass DbContext;

        public RepositorioEventos(DbContextClass DbContext) {
            this.DbContext = DbContext;
        }

        public async Task<List<Evento>> Listar()
        {
            return await DbContext.Evento.FromSqlRaw<Evento>("EXEC USP_EVENTO_ALL").AsNoTracking().ToListAsync();
        }

        public async Task<List<Evento>> Paginar(Filter filter)
        {

            var parameter = new List<SqlParameter>
            {
                new("@WHERE", filter.Where),
                new("@ORDER", filter.Order),
                new("@LIMIT", filter.Limit)
            };

            return await DbContext.Evento.FromSqlRaw<Evento>("EXEC USP_EVENTO_PAGINADOR @ORDER, @WHERE, @LIMIT", [.. parameter]).AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Evento>> Obtener(int id)
        {
            var param = new SqlParameter("@ID_EVENTO", id);
            var result = await Task.Run(() => DbContext.Evento.FromSqlRaw(@"exec USP_EVENTO_GET @ID_EVENTO", param).AsNoTracking().ToListAsync());
            return result;
        }

        public async Task<int> Agregar(Evento evento)
        {
            var LastID = new SqlParameter();
            LastID.ParameterName = "@ID_EVENTO";
            LastID.Direction = ParameterDirection.Output;
            LastID.DbType = DbType.Int32;

            var parameter = new List<SqlParameter>
            {
                new("@NOMBRE", evento.NOMBRE),
                new("@FECHA_INICIO", evento.FECHA_INICIO),
                new("@FECHA_FIN", evento.FECHA_FIN),
                new("@NOMBRE_LUGAR", evento.NOMBRE_LUGAR),
                new("@ID_ESTADO", evento.ID_ESTADO),
                new("@ID_UBIGEO", evento.ID_UBIGEO),
                new("@BANNER", evento.BANNER),
                new("@NUM_PARTICIPANTES", evento.NUM_PARTICIPANTES),
                new("@DESCRIPCION", evento.DESCRIPCION),
                new("@ENLACE_WHATSAPP", evento.ENLACE_WHATSAPP),
                new("@HORA_INICIO", evento.HORA_INICIO),
                new("@HORA_FIN", evento.HORA_FIN),
                new("@RED_SOCIAL", evento.RED_SOCIAL),
                new("@ID_TIPO_EVENTO", evento.ID_TIPO_EVENTO),
                new("@ID_MODALIDAD", evento.ID_MODALIDAD),
                new("@USER_REG", evento.USER_REG),
                new("@GENERA_TICKET", evento.GENERA_TICKET),
                new("@ENVIA_CORREO", evento.ENVIA_CORREO),

                new("@ID_ENCUESTA",  (evento.ID_ENCUESTA is null ? DBNull.Value : evento.ID_ENCUESTA )),
                new("@ID_CUESTIONARIO",  (evento.ID_CUESTIONARIO is null ? DBNull.Value : evento.ID_CUESTIONARIO )),
                new("@FORMATO",  (evento.FORMATO is null ? DBNull.Value : evento.FORMATO )),
                new("@REQUIERE_DOCUMENTO", evento.REQUIERE_DOCUMENTO),
                LastID
            };

            var result = await Task.Run(() => DbContext.Database.ExecuteSqlRawAsync(@"exec USP_EVENTO_INS @NOMBRE, @FECHA_INICIO , @FECHA_FIN , @NOMBRE_LUGAR , @ID_ESTADO , @ID_UBIGEO , @BANNER , @NUM_PARTICIPANTES , @DESCRIPCION , @ENLACE_WHATSAPP , @HORA_INICIO , @HORA_FIN , @RED_SOCIAL , @ID_TIPO_EVENTO , @ID_MODALIDAD , @USER_REG, @GENERA_TICKET, @ENVIA_CORREO, @ID_ENCUESTA, @ID_CUESTIONARIO, @FORMATO, @REQUIERE_DOCUMENTO, @ID_EVENTO OUTPUT", parameter));
            return LastID.Value as int? ?? 0;
             
        }

        public async Task<int> Actualizar(Evento evento)
        {
 
            var parameter = new List<SqlParameter>
            {
                new("@NOMBRE", evento.NOMBRE),
                new("@FECHA_INICIO", evento.FECHA_INICIO),
                new("@FECHA_FIN", evento.FECHA_FIN),
                new("@NOMBRE_LUGAR", evento.NOMBRE_LUGAR),
                new("@ID_ESTADO", evento.ID_ESTADO),
                new("@ID_UBIGEO", evento.ID_UBIGEO),
                new("@BANNER", evento.BANNER),
                new("@NUM_PARTICIPANTES", evento.NUM_PARTICIPANTES),
                new("@DESCRIPCION", evento.DESCRIPCION),
                new("@ENLACE_WHATSAPP", evento.ENLACE_WHATSAPP),
                new("@HORA_INICIO", evento.HORA_INICIO),
                new("@HORA_FIN", evento.HORA_FIN),
                new("@RED_SOCIAL", evento.RED_SOCIAL),
                new("@ID_TIPO_EVENTO", evento.ID_TIPO_EVENTO),
                new("@ID_MODALIDAD", evento.ID_MODALIDAD),
                new("@USER_UPD", evento.USER_UPD),
                new("@GENERA_TICKET", evento.GENERA_TICKET),
                new("@ENVIA_CORREO", evento.ENVIA_CORREO),

                new("@ID_ENCUESTA",  (evento.ID_ENCUESTA is null ? DBNull.Value : evento.ID_ENCUESTA )),
                new("@ID_CUESTIONARIO",  (evento.ID_CUESTIONARIO is null ? DBNull.Value : evento.ID_CUESTIONARIO )),
                new("@FORMATO",  (evento.FORMATO is null ? DBNull.Value : evento.FORMATO )),
                new("@REQUIERE_DOCUMENTO", evento.REQUIERE_DOCUMENTO),

                new("@ID_EVENTO", evento.ID_EVENTO)
            };
 
            return await Task.Run(() => DbContext.Database.ExecuteSqlRawAsync(@"exec USP_EVENTO_UPD @NOMBRE, @FECHA_INICIO , @FECHA_FIN , @NOMBRE_LUGAR , @ID_ESTADO , @ID_UBIGEO , @BANNER , @NUM_PARTICIPANTES , @DESCRIPCION , @ENLACE_WHATSAPP , @HORA_INICIO , @HORA_FIN , @RED_SOCIAL , @ID_TIPO_EVENTO , @ID_MODALIDAD , @USER_UPD, @GENERA_TICKET, @ENVIA_CORREO, @ID_ENCUESTA, @ID_CUESTIONARIO , @FORMATO, @REQUIERE_DOCUMENTO, @ID_EVENTO", parameter));
        }

        public async Task<int> Eliminar(int id)
        {
            var result = await Task.Run(() => DbContext.Database.ExecuteSqlInterpolatedAsync($"USP_EVENTO_DEL {id}"));
            return result;

        }

        public async Task<EventoEstadisticaDTO> Estadisticas()
        {
            var resultado = await DbContext.Database.SqlQuery<EventoEstadisticaDTO>($@"
                SELECT
                    (SELECT COUNT(*) FROM dbo.EVENTO) AS TOTAL_EVENTOS,
                    (SELECT COUNT(*) FROM dbo.EVENTO WHERE ID_ESTADO = {Constantes.estadoEventoActivo}) AS EVENTOS_ACTIVOS,
                    (SELECT COUNT(*) FROM dbo.EVENTO WHERE ID_ESTADO = {Constantes.estadoEventoRealizado}) AS EVENTOS_REALIZADOS,
                    (SELECT COUNT(*) FROM dbo.EVENTO WHERE ID_ESTADO = {Constantes.estadoEventoCancelado}) AS EVENTOS_CANCELADOS,
                    (SELECT ISNULL(SUM(NUM_PARTICIPANTES), 0) FROM dbo.EVENTO) AS TOTAL_CUPOS,
                    (SELECT COUNT(*) FROM dbo.EVENTO_PARTICIPANTE) AS TOTAL_INSCRITOS").ToListAsync();

            return resultado.FirstOrDefault() ?? new EventoEstadisticaDTO();
        }

    }
}
