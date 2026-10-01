using AutoMapper;
using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;
using CapacitaGRDApi.Repositorios;
using CapacitaGRDApi.Util;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using System.Net.WebSockets;
using static CapacitaGRDApi.Util.Paginado;

namespace CapacitaGRDApi.EndPoints
{
    public static class CuestionariosEndpoints
    {
        public static RouteGroupBuilder MapCuestionarios(this RouteGroupBuilder group)
        {
            group.MapGet("/", Listar);
            group.MapGet("/{id:int}", Obtener);
            group.MapPost("/paginado", Paginar);
            group.MapPost("/", Agregar);
            group.MapPut("/", Actualizar);
            group.MapDelete("/", Eliminar);

            return group;
        }

        static async Task<Ok<List<CuestionarioDTO>>> Listar(
              IRepositorioCuestionarios repositorio
 

            , IMapper mapper)
        {
            var cuestionarios = await repositorio.Listar();
            var cuestionariosDTO = mapper.Map<List<CuestionarioDTO>>(cuestionarios);
             
            return TypedResults.Ok(cuestionariosDTO);
        }

        static async Task<Ok<PaginadorDTO<CuestionarioDTO>>> Paginar(
           BusquedaGenericaDTO filtro
           , IRepositorioCuestionarios repositorio
          , IMapper mapper)
        {
            var paginado = filtro.Filtro;
            var dato = filtro.Dato;

            int pagActual = paginado.start;
            int pagDesde = (pagActual == 0) ? 1 : (pagActual + 1);
            int pagHasta = pagActual + paginado.length;
            var inxColumn = paginado.order[0].column;
            var orderColumn = paginado.order[0].dir;
            var orderBy = paginado.columns[inxColumn].name + ' ' + orderColumn;

            var where = " ";
            if (dato.Trim() != "")
            {
                where += " AND NOMBRE LIKE '%" + dato.Replace("'", "''") + "%' ";
            }


            Filter filter = new()
            {
                Limit = "FILA BETWEEN " + pagDesde + " AND " + pagHasta,
                Order = orderBy,
                Where = where
            };

            var encuestas = await repositorio.Paginar(filter);
            var encuestasDTO = mapper.Map<List<CuestionarioDTO>>(encuestas);


            var response = new PaginadorDTO<CuestionarioDTO>
            {
                draw = paginado.draw,
                recordsFiltered = (encuestas.Any() ? encuestas.FirstOrDefault().REGISTROS : 0),
                recordsTotal = (encuestas.Any() ? encuestas.FirstOrDefault().REGISTROS : 0),
                data = encuestasDTO
            };

            return TypedResults.Ok(response);

        }


        static async Task<Results<Ok<CuestionarioDTO>, NotFound>> Obtener(
                IRepositorioCuestionarios repositorio
            , IRepositorioCuestionarioPreguntas repositorioPreguntas
            , IRepositorioCuestionarioPreguntaRespuestas repositorioRespuestas
            , int id
            , IMapper mapper
            )
        {
            var result = await repositorio.Obtener(id);
            if (result is null)
            {
                return TypedResults.NotFound();
            }

            if (result.Count() == 0)
            {
                return TypedResults.NotFound();
            }

            var cuestionario = new Cuestionario()
            {
                ID_CUESTIONARIO = result.First().ID_CUESTIONARIO,
                NOMBRE = result.First().NOMBRE,
                USER_REG = result.First().USER_REG,
                FECHA_REG = result.First().FECHA_REG,
                USER_UPD  = result.First().USER_UPD,
                FECHA_UPD = result.First().FECHA_UPD,
            };

            var cuestionarioDTO = mapper.Map<CuestionarioDTO>(cuestionario);

            var preguntas = await repositorioPreguntas.Listar(cuestionarioDTO.ID_CUESTIONARIO);
            var preguntasDTO = mapper.Map<List<CuestionarioPreguntaDTO>>(preguntas);

            foreach (var preguntaDTO in preguntasDTO)
            {
                var respuestas = await repositorioRespuestas.Listar(preguntaDTO.ID_CUESTIONARIO_PREGUNTA);
                preguntaDTO.RESPUESTAS = mapper.Map<List<CuestionarioPreguntaRespuestaDTO>>(respuestas);
            }

            cuestionarioDTO.PREGUNTAS = preguntasDTO;

            return TypedResults.Ok(cuestionarioDTO);

        }

        static async Task<Created<CuestionarioDTO>> Agregar(
            CrearCuestionarioDTO crearCuestionarioDTO
           , IRepositorioCuestionarios repositorio
           , IRepositorioCuestionarioPreguntas repositorioPreguntas
           , IRepositorioCuestionarioPreguntaRespuestas repositorioRespuestas
           , IOutputCacheStore outputCacheStore
            , IMapper mapper
           )
        {
            var cuestionario = new Cuestionario
            {
                NOMBRE = crearCuestionarioDTO.NOMBRE,
                USER_REG = "JFLORES"
            };

            var id = await repositorio.Agregar(cuestionario);

            var cuestionarioDTO = mapper.Map<CuestionarioDTO>(cuestionario);
            cuestionarioDTO.ID_CUESTIONARIO = id;

            if (crearCuestionarioDTO.PREGUNTAS is not null)
            {
                foreach (CrearCuestionarioPreguntaDTO item in crearCuestionarioDTO.PREGUNTAS)
                {
                    var pregunta = mapper.Map<CuestionarioPregunta>(item);
                    pregunta.ID_CUESTIONARIO = cuestionarioDTO.ID_CUESTIONARIO;
                    var idPregunta = await repositorioPreguntas.Agregar(pregunta);

                    if (item.RESPUESTAS is not null)
                    {
                        foreach (CrearCuestionarioPreguntaRespuestaDTO itemRespuesta in item.RESPUESTAS)
                        {
                            var respuesta = mapper.Map<CuestionarioPreguntaRespuesta>(itemRespuesta);
                            respuesta.ID_CUESTIONARIO_PREGUNTA = idPregunta;
                            await repositorioRespuestas.Agregar(respuesta);
                        }
                    }
                }
            }

            return TypedResults.Created($"/cuestionarios/{id}", cuestionarioDTO);
        }

        static async Task<Results<NoContent, NotFound>> Actualizar(int id
            , CrearCuestionarioDTO crearCuestionarioDTO
            , IRepositorioCuestionarios repositorio
            , IRepositorioCuestionarioPreguntas repositorioPreguntas
            , IRepositorioCuestionarioPreguntaRespuestas repositorioRespuestas
            , IOutputCacheStore outputCacheStore
            , IMapper mapper
            )
        {
            var result = await repositorio.Obtener(id);
            if (result is null)
            {
                return TypedResults.NotFound();
            }

            if (result.Count() == 0)
            {
                return TypedResults.NotFound();
            }

            if (crearCuestionarioDTO.PREGUNTAS is not null)
            {
                await repositorioPreguntas.EliminarPorCuestionario(id);

                foreach (CrearCuestionarioPreguntaDTO item in crearCuestionarioDTO.PREGUNTAS)
                {
                    var pregunta = mapper.Map<CuestionarioPregunta>(item);
                    pregunta.ID_CUESTIONARIO = id;
                    var idPregunta = await repositorioPreguntas.Agregar(pregunta);

                    if (item.RESPUESTAS is not null)
                    {
                        foreach (CrearCuestionarioPreguntaRespuestaDTO itemRespuesta in item.RESPUESTAS)
                        {
                            var respuesta = mapper.Map<CuestionarioPreguntaRespuesta>(itemRespuesta);
                            respuesta.ID_CUESTIONARIO_PREGUNTA = idPregunta;
                            await repositorioRespuestas.Agregar(respuesta);
                        }
                    }
                }
            }

            var cuestionario = new Cuestionario
            {
                ID_CUESTIONARIO = id,
                NOMBRE = crearCuestionarioDTO.NOMBRE,
                USER_UPD = "JFLORES"
            };


            await repositorio.Actualizar(cuestionario);
            return TypedResults.NoContent();

        }


        static async Task<Results<NoContent, NotFound>> Eliminar(int id
            , IRepositorioCuestionarios repositorio
            , IRepositorioCuestionarioPreguntas repositorioPreguntas
            , IOutputCacheStore outputCacheStore)
        {
            var result = await repositorio.Obtener(id);
            if (result is null)
            {
                return TypedResults.NotFound();
            }

            if (result.Count() == 0)
            {
                return TypedResults.NotFound();
            }

            await repositorioPreguntas.EliminarPorCuestionario(id);
            await repositorio.Eliminar(id);
            return TypedResults.NoContent();
        }

    }
}
