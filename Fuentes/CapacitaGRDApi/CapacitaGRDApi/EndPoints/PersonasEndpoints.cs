using AutoMapper;
using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;
using CapacitaGRDApi.Repositorios;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using System.Security.Cryptography;
using System.Text;
using static CapacitaGRDApi.Util.Paginado;

namespace CapacitaGRDApi.EndPoints
{
    public static class PersonasEndpoints
    {
        public static RouteGroupBuilder MapPersonas(this RouteGroupBuilder group)
        {
            group.MapGet("/", Listar);
            group.MapGet("/{id:int}", Obtener);
            group.MapGet("/existe", Existe).AllowAnonymous();
            group.MapGet("/validadni", ValidaDNI);
            group.MapGet("/validace", ValidaCE);
            group.MapPost("/paginado", Paginar);
            group.MapGet("/exportar", Exportar);

            group.MapPost("/", Agregar).AllowAnonymous();
            group.MapPut("/", Actualizar);
            group.MapPut("/reiniciar/{id:int}", Reiniciar);
            group.MapDelete("/", Eliminar);

            return group;
        }

        static async Task<Ok<List<PersonaDTO>>> Listar(
              IRepositorioPersonas repositorio         
            , IMapper mapper)
        {
            var personas = await repositorio.Listar();
            var personasDTO = mapper.Map<List<PersonaDTO>>(personas);
             
            return TypedResults.Ok(personasDTO);
        }

        static async Task<Ok<PaginadorDTO<PersonaDTO>>> Paginar(
            BusquedaPersonaDTO filtro
            , IRepositorioPersonas repositorio
            , IRepositorioTipoDocumentos repositorioTipoDocumentos
            , IMapper mapper)
        {
            var paginado = filtro.Filtro;
            var tipoDocumento = filtro.TipoDocumento;
            var numeroDocumento = filtro.NumeroDocumento;
            var paterno = filtro.Paterno;
            var materno = filtro.Materno;


            int pagActual = paginado.start;
            int pagDesde = (pagActual == 0) ? 1 : (pagActual + 1);
            int pagHasta = pagActual + paginado.length;
            var inxColumn = paginado.order[0].column;
            var orderColumn = paginado.order[0].dir;
            var orderBy = paginado.columns[inxColumn].name + ' ' + orderColumn;
            
            var where = " ";
            if (tipoDocumento != 0)
            {
                where += " AND ID_TIPO_DOCUMENTO = " + tipoDocumento;
            }

            if (numeroDocumento.Trim() != "")
            {
                where += " AND NUM_DOCUMENTO LIKE '%" + numeroDocumento.Replace("'", "''") + "%' ";
            }

            if (paterno.Trim() != "")
            {
                where += " AND APELLIDO_PATERNO LIKE '%" + paterno.Replace("'", "''") + "%' ";
            }

            if (materno.Trim() != "")
            {
                where += " AND APELLIDO_MATERNO LIKE '%" + materno.Replace("'", "''") + "%' ";
            }


            Filter filter = new()
            {
                Limit = "  FILA BETWEEN " + pagDesde + " AND " + pagHasta,
                Order = orderBy,
                Where = where
            };

            var personas = await repositorio.Paginar(filter);
            var personasDTO = mapper.Map<List<PersonaDTO>>(personas);
            
            foreach(var personaDTO in personasDTO)
            {
                var resultTipoDocumento = await repositorioTipoDocumentos.Obtener(personaDTO.ID_TIPO_DOCUMENTO);
                var _tipoDocumento = resultTipoDocumento.FirstOrDefault();
                var tipoDocumentDTO = mapper.Map<TipoDocumentDTO>(_tipoDocumento);
                personaDTO.TIPO_DOCUMENTO = tipoDocumentDTO;
            }

            var response = new PaginadorDTO<PersonaDTO>
            {
                draw = paginado.draw,
                recordsFiltered = (personas.Any() ? personas.FirstOrDefault().REGISTROS : 0),
                recordsTotal = (personas.Any() ? personas.FirstOrDefault().REGISTROS : 0),
                data = personasDTO
            };

            return TypedResults.Ok(response);
        }

        static async Task<FileContentHttpResult> Exportar(
            int tipoDocumento
            , string numeroDocumento
            , string paterno
            , string materno
            , IRepositorioPersonas repositorio
            , IRepositorioTipoDocumentos repositorioTipoDocumentos
            , IMapper mapper)
        {
            var where = " ";
            if (tipoDocumento != 0)
            {
                where += " AND ID_TIPO_DOCUMENTO = " + tipoDocumento;
            }

            if (!string.IsNullOrWhiteSpace(numeroDocumento))
            {
                where += " AND NUM_DOCUMENTO LIKE '%" + numeroDocumento.Replace("'", "''") + "%' ";
            }

            if (!string.IsNullOrWhiteSpace(paterno))
            {
                where += " AND APELLIDO_PATERNO LIKE '%" + paterno.Replace("'", "''") + "%' ";
            }

            if (!string.IsNullOrWhiteSpace(materno))
            {
                where += " AND APELLIDO_MATERNO LIKE '%" + materno.Replace("'", "''") + "%' ";
            }

            var personas = await repositorio.ListarFiltrado(where, "ID_PERSONA ASC");
            var personasDTO = mapper.Map<List<PersonaDTO>>(personas);

            foreach (var personaDTO in personasDTO)
            {
                var resultTipoDocumento = await repositorioTipoDocumentos.Obtener(personaDTO.ID_TIPO_DOCUMENTO);
                var _tipoDocumento = resultTipoDocumento.FirstOrDefault();
                personaDTO.TIPO_DOCUMENTO = mapper.Map<TipoDocumentDTO>(_tipoDocumento);
            }

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Personas");

            ws.Cell(1, 1).Value = "Id";
            ws.Cell(1, 2).Value = "Validado PIDE";
            ws.Cell(1, 3).Value = "Tipo Documento";
            ws.Cell(1, 4).Value = "Num Documento";
            ws.Cell(1, 5).Value = "Nombres";
            ws.Cell(1, 6).Value = "Apellido Paterno";
            ws.Cell(1, 7).Value = "Apellido Materno";
            ws.Cell(1, 8).Value = "Sexo";
            ws.Cell(1, 9).Value = "Fecha Nacimiento";
            ws.Cell(1, 10).Value = "Email";
            ws.Cell(1, 11).Value = "Celular";
            ws.Range(1, 1, 1, 11).Style.Font.Bold = true;

            var fila = 2;
            foreach (var persona in personasDTO)
            {
                ws.Cell(fila, 1).Value = persona.ID_PERSONA;
                ws.Cell(fila, 2).Value = persona.VALIDADO_PIDE == "S" ? "SI" : "NO";
                ws.Cell(fila, 3).Value = persona.TIPO_DOCUMENTO?.NOMBRE;
                ws.Cell(fila, 4).Value = persona.NUM_DOCUMENTO;
                ws.Cell(fila, 5).Value = persona.NOMBRES;
                ws.Cell(fila, 6).Value = persona.APELLIDO_PATERNO;
                ws.Cell(fila, 7).Value = persona.APELLIDO_MATERNO;
                ws.Cell(fila, 8).Value = persona.SEXO;
                if (persona.FECHA_NACIMIENTO.HasValue)
                {
                    ws.Cell(fila, 9).Value = persona.FECHA_NACIMIENTO.Value.ToString("dd/MM/yyyy");
                }
                ws.Cell(fila, 10).Value = persona.EMAIL;
                ws.Cell(fila, 11).Value = persona.CELULAR;
                fila++;
            }

            ws.Columns(1, 11).Width = 18;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return TypedResults.Bytes(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "personas.xlsx");
        }

        static async Task<Results<Ok<PersonaDTO>, NotFound>> Existe(
             IRepositorioPersonas repositorio
            , IRepositorioPersonasData repositorioPersonasData
            , int tipo
            , string documento
            , IMapper mapper
            )
        {
            var result = await repositorio.Existe(tipo, documento);
            if (result is null)
            {
                return TypedResults.NotFound();
            }

            if (result.Count() == 0)
            {
                return TypedResults.NotFound();
            }

            var personaDTO = mapper.Map<PersonaDTO>(result.FirstOrDefault());

            var personaData = await repositorioPersonasData.ObtenerPorPersona(personaDTO.ID_PERSONA);
            personaDTO.ID_PERSONA_DATA = personaData?.FirstOrDefault()?.ID_PERSONA_DATA;

            return TypedResults.Ok(personaDTO);
        }

        static async Task<Results<Ok<PersonaDTO>, NotFound>> Obtener(
             IRepositorioPersonas repositorio
            , IRepositorioEventosParticipantes repositorioEventosParticipantes
            , IRepositorioEventos repositorioEventos
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
     
            var personaDTO = mapper.Map<PersonaDTO>(result.FirstOrDefault());
            var eventoParticipante = await repositorioEventosParticipantes.Eventos(id);
            var eventoParticipanteDTO = mapper.Map<List<EventoParticipanteDTO>>(eventoParticipante);
            foreach (var item in eventoParticipanteDTO)
            {
                var evento = await repositorioEventos.Obtener(item.ID_EVENTO);
                var eventoDTO = mapper.Map<EventoDTO>(evento.FirstOrDefault());
                item.Evento = eventoDTO;
            }
            personaDTO.Eventos = eventoParticipanteDTO;

            return TypedResults.Ok(personaDTO);
        }

        static async Task<Results<Created<PersonaDTO>, Conflict, NotFound, BadRequest<string>>> Agregar(
            CrearPersonaDTO crearPersonaDTO
            , IRepositorioPersonas repositorio
            , IRepositorioPersonasData repositorioPersonasData
            , IRepositorioUsuarios repositorioUsuarios
            , IOutputCacheStore outputCacheStore
            , IMapper mapper
           )
        {
            if (crearPersonaDTO.CrearPersonaDataDTO is null)
            {
                return TypedResults.BadRequest("CrearPersonaDataDTO es requerido.");
            }

            var result = await repositorio.Existe(crearPersonaDTO.ID_TIPO_DOCUMENTO, crearPersonaDTO.NUM_DOCUMENTO);

            if (result.Count() == 0)
            {
                var persona = mapper.Map<Persona>(crearPersonaDTO);
                var id = await repositorio.Agregar(persona);
                var personaDTO = mapper.Map<PersonaDTO>(persona);
                personaDTO.ID_PERSONA = id;

                var resultPersona = await repositorio.Obtener(id);
                personaDTO.COD_PERSONA = resultPersona.FirstOrDefault().COD_PERSONA;

                var crearPersonaDataDTO = crearPersonaDTO.CrearPersonaDataDTO;
                var crearPersonaData = mapper.Map<PersonaData>(crearPersonaDataDTO);
                crearPersonaData.ID_PERSONA = id;

                await repositorioPersonasData.Agregar(crearPersonaData);

                var claveHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(crearPersonaDTO.NUM_DOCUMENTO)));
                await repositorioUsuarios.Agregar(new Usuarios
                {
                    USUARIO = crearPersonaDTO.NUM_DOCUMENTO,
                    CLAVE = claveHash,
                    ESTADO = 1
                });

                return TypedResults.Created($"/personas/{id}", personaDTO);
            }
            return TypedResults.Conflict();
        }

        static async Task<Results<NoContent, NotFound>> Actualizar(int id
            , CrearPersonaDTO crearPersonaDTO
            , IRepositorioPersonas repositorio
            , IRepositorioPersonasData repositorioPersonasData
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

            var persona = mapper.Map<Persona>(crearPersonaDTO);
            persona.ID_PERSONA = id;


            var crearPersonaDataDTO = crearPersonaDTO.CrearPersonaDataDTO;
            if (crearPersonaDataDTO is not null)
            {
                var crearPersonaData = mapper.Map<PersonaData>(crearPersonaDataDTO);
                crearPersonaData.ID_PERSONA = id;
                await repositorioPersonasData.Agregar(crearPersonaData);
            }            

            await repositorio.Actualizar(persona);
            return TypedResults.NoContent();
        }
         
        static async Task<Results<NoContent, NotFound>> Reiniciar(int id
            , IRepositorioPersonas repositorio
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

            await repositorio.Reiniciar(id);

            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> Eliminar(int id
            , IRepositorioPersonas repositorio
            , IRepositorioPersonasData repositorioPersonasData
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
            await repositorioPersonasData.Eliminar(id);

            await repositorio.Eliminar(id);
            
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> ValidaDNI(
              string dni
            , string departamento
            , string provincia
            , string distrito
            , IOutputCacheStore outputCacheStore
            , IRepositorioPIDE repositorio)
        {
            var persona = await repositorio.ValidaDNI(dni);
            if (persona is null)
            {
                return TypedResults.NotFound();
            }

            var ubigeo = ((departamento == "CALLAO" ? "" : departamento) + "/" + provincia + "/" + distrito);

            if (persona.COD_PERSONA  == ubigeo)
            {
                return TypedResults.NoContent();
            }

            return TypedResults.NotFound();

        }

        static async Task<Results<NoContent, NotFound>> ValidaCE(
             string ce
           , IOutputCacheStore outputCacheStore
           , IRepositorioPIDE repositorio)
        {
            var persona = await repositorio.ValidaCE(ce);
            if (persona is null)
            {
                return TypedResults.NotFound();
            } 
           
            return TypedResults.NoContent();

        }
    }
}
