using AutoMapper;
using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;
using CapacitaGRDApi.Repositorios;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using static CapacitaGRDApi.Util.Paginado;

namespace CapacitaGRDApi.EndPoints
{
    public static class UsuariosEndpoints
    {
        public static RouteGroupBuilder MapUsuarios(this RouteGroupBuilder group)
        {
            group.MapGet("/", Listar);
            group.MapGet("/{id:int}", Obtener);
            group.MapPost("/", Agregar);
            group.MapPost("/paginado", Paginar);
            group.MapPut("/", Actualizar);
            group.MapDelete("/", Eliminar);

            return group;
        }

        static async Task<Ok<PaginadorDTO<UsuarioDTO>>> Paginar(
            BusquedaGenericaDTO filtro
            , IRepositorioUsuarios repositorio
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
                where += " AND USUARIO LIKE '%" + dato.Replace("'", "''") + "%' ";
            }
             

            Filter filter = new()
            {
                Limit = "FILA BETWEEN " + pagDesde + " AND " + pagHasta,
                Order = orderBy,
                Where = where
            };

            var usuarios = await repositorio.Paginar(filter);
            var usuariosDTO = mapper.Map<List<UsuarioDTO>>(usuarios);

     
            var response = new PaginadorDTO<UsuarioDTO>
            {
                draw = paginado.draw,
                recordsFiltered = (usuarios.Any() ? usuarios.FirstOrDefault().REGISTROS : 0),
                recordsTotal = (usuarios.Any() ? usuarios.FirstOrDefault().REGISTROS : 0),
                data = usuariosDTO
            };

            return TypedResults.Ok(response);

        }

        static async Task<Ok<List<UsuarioDTO>>> Listar(IRepositorioUsuarios repositorio
            , IMapper mapper)
        {
            var usuarios = await repositorio.Listar();
            var usuariosDTO = mapper.Map<List<UsuarioDTO>>(usuarios);
            return TypedResults.Ok(usuariosDTO);
        }

        static async Task<Results<Ok<UsuarioDTO>, NotFound>> Obtener(IRepositorioUsuarios repositorio
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

            var usuarioDTO = mapper.Map<UsuarioDTO>(result.FirstOrDefault());
            return TypedResults.Ok(usuarioDTO);
        }

        static async Task<Created<UsuarioDTO>> Agregar(CrearUsuarioDTO crearUsuarioDTO
           , IRepositorioUsuarios repositorio
           , IOutputCacheStore outputCacheStore
           , IMapper mapper
           )
        {
            var usuario = mapper.Map<Usuarios>(crearUsuarioDTO);
            var id = await repositorio.Agregar(usuario);
            var usuarioDTO = mapper.Map<UsuarioDTO>(usuario);
            usuarioDTO.ID_USUARIO = id;

            return TypedResults.Created($"/usuarios/{id}", usuarioDTO);
        }

        static async Task<Results<NoContent, NotFound>> Actualizar(int id
            , CrearUsuarioDTO crearUsuarioDTO
            , IRepositorioUsuarios repositorio
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

            var usuario = mapper.Map<Usuarios>(crearUsuarioDTO);
            usuario.ID_USUARIO = id;

            await repositorio.Actualizar(usuario);
            return TypedResults.NoContent();
        }


        static async Task<Results<NoContent, NotFound>> Eliminar(int id
            , IRepositorioUsuarios repositorio
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
