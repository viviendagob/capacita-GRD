using AutoMapper;
using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;
using CapacitaGRDApi.Repositorios;
using CapacitaGRDApi.Util;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using static CapacitaGRDApi.Util.Paginado;

namespace CapacitaGRDApi.EndPoints
{
    public static class EncuestasEnpoints
    {
        public static RouteGroupBuilder MapEncuestas(this RouteGroupBuilder group)
        {
            group.MapGet("/", Listar);
            group.MapGet("/{id:int}", Obtener);
            group.MapPost("/paginado", Paginar);
            group.MapPost("/", Agregar);
            group.MapPut("/", Actualizar);
            group.MapDelete("/", Eliminar);

            return group;
        }

        static async Task<Ok<List<EncuestaDTO>>> Listar(
              IRepositorioEncuestas repositorio 
            , IMapper mapper)
        {
            var encuestas = await repositorio.Listar();
            var encuestasDTO = mapper.Map<List<EncuestaDTO>>(encuestas);
             
            return TypedResults.Ok(encuestasDTO);
        }

        static async Task<Ok<PaginadorDTO<EncuestaDTO>>> Paginar(
           BusquedaGenericaDTO filtro
           , IRepositorioEncuestas repositorio
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
            var encuestasDTO = mapper.Map<List<EncuestaDTO>>(encuestas);


            var response = new PaginadorDTO<EncuestaDTO>
            {
                draw = paginado.draw,
                recordsFiltered = (encuestas.Any() ? encuestas.FirstOrDefault().REGISTROS : 0),
                recordsTotal = (encuestas.Any() ? encuestas.FirstOrDefault().REGISTROS : 0),
                data = encuestasDTO
            };

            return TypedResults.Ok(response);

        }


        static async Task<Results<Ok<EncuestaDTO>, NotFound>> Obtener(
                IRepositorioEncuestas repositorio
            , IRepositorioEncuestaRespuestas repositorioEncuestaRespuestas
            , IRepositorioTipoEncuestaRespuestas repositorioTipoEncuestaRespuestas
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

    
            var encuestaDTO = mapper.Map<EncuestaDTO>(result.FirstOrDefault());
            var respuestas = await repositorioEncuestaRespuestas.Listar(encuestaDTO.ID_ENCUESTA);

            List<EncuestaRespuestaDTO> encuestaRespuestaDTO = [];
            foreach (EncuestaRespuesta item in respuestas)
            {
                var tipoEncuestaRespuesta = await repositorioTipoEncuestaRespuestas.Obtener(item.ID_TIPO_ENCUESTA_PREGUNTA);
                var tipoEncuestaRespuestaDTO = new TipoEncuestaRespuestaDTO
                {
                    ID_TIPO_ENCUESTA_PREGUNTA = tipoEncuestaRespuesta.FirstOrDefault().ID_TIPO_ENCUESTA_PREGUNTA,
                    NOMBRE = tipoEncuestaRespuesta.FirstOrDefault().NOMBRE
                };

                encuestaRespuestaDTO.Add(new EncuestaRespuestaDTO
                {
                    NOMBRE = item.NOMBRE,
                    RESPUESTAS = item.RESPUESTAS,
                    ID_TIPO_ENCUESTA_PREGUNTA = item.ID_TIPO_ENCUESTA_PREGUNTA, 
                    ID_RESPUESTA = item.ID_RESPUESTA,
                    ID_ENCUESTA = item.ID_ENCUESTA,
                    TIPO_ENCUESTA_PREGUNTA = tipoEncuestaRespuestaDTO
                });
            }

            encuestaDTO.RESPUESTAS = encuestaRespuestaDTO;

            return TypedResults.Ok(encuestaDTO);

        }

        static async Task<Created<EncuestaDTO>> Agregar(
            CrearEncuestaDTO crearEncuestaDTO
           , IRepositorioEncuestas repositorio
            , IRepositorioEncuestaRespuestas repositorioEncuestaRespuestas
           , IOutputCacheStore outputCacheStore
            , IMapper mapper
           )
        {              
            var encuesta = new Encuesta
            {
                NOMBRE = crearEncuestaDTO.NOMBRE,           
                USER_REG = "JFLORES"               
            };

            var id = await repositorio.Agregar(encuesta);

            var encuestaDTO = mapper.Map<EncuestaDTO>(encuesta);
            encuestaDTO.ID_ENCUESTA = id;

            if (crearEncuestaDTO.RESPUESTAS is not null)
            {
                foreach (CrearEncuestaRespuestaDTO item in crearEncuestaDTO.RESPUESTAS)
                {
                    var encuestaRespuesta = mapper.Map<EncuestaRespuesta>(item);
                    encuestaRespuesta.ID_ENCUESTA = encuestaDTO.ID_ENCUESTA;
                    await repositorioEncuestaRespuestas.Agregar(encuestaRespuesta);
                }
            }

            return TypedResults.Created($"/encuesta/0/{id}", encuestaDTO);
        }

        static async Task<Results<NoContent, NotFound>> Actualizar(int id
            , CrearEncuestaDTO crearEncuestaDTO
            , IRepositorioEncuestas repositorio
            , IRepositorioEncuestaRespuestas repositorioEncuestaRespuestas
            , IOutputCacheStore outputCacheStore
            , IMapper mapper
            )
        {
            var result = await repositorio.Obtener(id);
            if (result is null)
            {
                return TypedResults.NotFound();
            }

            if (!result.Any())
            {
                return TypedResults.NotFound();
            }
             

            if (crearEncuestaDTO.RESPUESTAS is not null)
            {
                await repositorio.EliminarRespuestas(id);

                foreach (CrearEncuestaRespuestaDTO item in crearEncuestaDTO.RESPUESTAS)
                {
                    var encuestaRespuesta = mapper.Map<EncuestaRespuesta>(item);
                    encuestaRespuesta.ID_ENCUESTA = id;

                    await repositorioEncuestaRespuestas.Agregar(encuestaRespuesta);
                }
            }

            var encuesta = new Encuesta
            {
                ID_ENCUESTA = id,
                NOMBRE = crearEncuestaDTO.NOMBRE,
                USER_UPD = "JFLORES"
            };

            await repositorio.Actualizar(encuesta);
            return TypedResults.NoContent();             
        }


        static async Task<Results<NoContent, NotFound>> Eliminar(int id
            , IRepositorioEncuestas repositorio
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

            await repositorio.Eliminar(id);
            return TypedResults.NoContent();
        }

    }
}
