using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UserApplication.Common.Exceptions;
using UserApplication.Common.Pagination;
using UserApplication.DTOs;
using UserApplication.Interfaces;
using UserDomain.Domain;
using UserInfrastructure.Interfaces;

namespace UserApplication.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repository;
        private readonly IMapper _mapper;
        public UsuarioService(IUsuarioRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        /// <summary>
        /// Agrega Usuario a BD por medio de un mapper;
        /// </summary>
        /// <param name="usuarioDTO"></param>
        /// <returns></returns>
        public async Task<int> AgregarUsuarioAsync(UsuarioDTO usuarioDTO)
        {
            var usuario = _mapper.Map<Usuario>(usuarioDTO);
            return await _repository.AgregarUsuarioAsync(usuario);

        }
        /// <summary>
        /// Obtiene Usuario por ID, si no existe lanza NotFoundException
        /// </summary>
        /// <param name="usuarioID"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        public async Task<ObtenerUsuarioDTO> ObtenerUsuarioPorIdAsync(int usuarioID)
        {
            var usuario = await _repository.ObtenerUsuarioPorIdAsync(usuarioID);
            if (usuario == null) throw new NotFoundException(nameof(Usuario), usuarioID);
            return _mapper.Map<ObtenerUsuarioDTO>(usuario);
        }

        /// <summary>
        /// Obtiene Usuarios con filtros opcionales y paginacion
        /// </summary>
        /// <param name="nombre"></param>
        /// <param name="provincia"></param>
        /// <param name="ciudad"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<PaginationResponse<ObtenerUsuarioDTO>> ObtenerUsuariosConFiltrosAsync(string? nombre, string? provincia, string? ciudad,
                                                                                                int page = 1, int pageSize = 20)
        {
            // Construir los filtros de manera dinámica
            Expression<Func<Usuario,bool>> filtros = x =>
                                     (x.Activo) &&
                                     (string.IsNullOrEmpty(nombre) || x.Nombre.Contains(nombre)) &&
                                     (string.IsNullOrEmpty(provincia) || x.Domicilios.Any(p => p.Provincia.Contains(provincia))) &&
                                     (string.IsNullOrEmpty(ciudad) || x.Domicilios.Any(p => p.Ciudad.Contains(ciudad)));


            var listaUsuarios = await _repository.ObtenerUsuariosPorFiltrosAsync(filtros, page, pageSize);
            var lstDtos = listaUsuarios.Items.Select(_mapper.Map<ObtenerUsuarioDTO>).ToList();
            // Retornar paginacion
            return new PaginationResponse<ObtenerUsuarioDTO>(lstDtos, listaUsuarios.Page, listaUsuarios.PageSize, listaUsuarios.TotalCount);
        }

        /// <summary>
        /// Elimina Usuario por ID, si no existe lanza NotFoundException
        /// </summary>
        /// <param name="usuarioID"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        public async Task EliminarUsuarioAsync(int usuarioID)
        {
            var usuario = await _repository.ObtenerUsuarioPorIdAsync(usuarioID);

            if (usuario == null) throw new NotFoundException(nameof(Usuario), usuarioID);
            // Si el usuario ya está inactivo, lanzar excepción
            if (!usuario.Activo) throw new NotFoundException(nameof(Usuario), usuarioID, $"Este usuario ya ha sido eliminado. Ultima modificacion: {usuario.FechaUltimaActualizacion}"); ;

            usuario.EliminarUsuario();
            await _repository.ActualizarUsuarioAsync(usuario);
        }
        /// <summary>
        /// Actualiza Usuario por ID, si no existe lanza NotFoundException
        /// </summary>
        /// <param name="usuarioID"></param>
        /// <param name="usuarioDTO"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        public async Task<ObtenerUsuarioDTO> ActualizarUsuarioAsync(int usuarioID,ActualizarUsuarioDTO usuarioDTO)
        {
            usuarioDTO.SetID(usuarioID);
            var usuario = await _repository.ObtenerUsuarioPorIdAsync(usuarioDTO.ID);
            if (usuario == null)
                throw new NotFoundException(nameof(Usuario),usuarioDTO.ID);
            // Si el usuario está inactivo, lanzar excepción
            if (!usuario.Activo)
                throw new NotFoundException(nameof(Usuario),usuarioDTO.ID,"Inactivo");

            // Actualizar solo los campos que no son nulos o vacíos
            if (!string.IsNullOrEmpty(usuarioDTO.Nombre)) usuario.Renombrar(usuarioDTO.Nombre);
            if (!string.IsNullOrEmpty(usuarioDTO.Email)) usuario.CambiarEmail(usuarioDTO.Email);

            if (usuarioDTO.Domicilio != null)
            {
                // Si el ID del domicilio es proporcionado, buscar y actualizar ese domicilio
                if (usuarioDTO.Domicilio.Id != null)
                {
                    var domicilioUsuario = usuario.BuscarDomicilioPorID((int)usuarioDTO.Domicilio.Id);
                    // Actualizar solo si el domicilio existe y no es duplicado
                    if (domicilioUsuario != null && !usuario.ExisteDomicilio(usuarioDTO.Domicilio.Calle, usuarioDTO.Domicilio.Numero, usuarioDTO.Domicilio.Provincia, usuarioDTO.Domicilio.Ciudad))
                    {
                        if (!string.IsNullOrEmpty(usuarioDTO.Domicilio.Calle)) domicilioUsuario.ActualizarCalle(usuarioDTO.Domicilio.Calle);
                        if (!string.IsNullOrEmpty(usuarioDTO.Domicilio.Numero)) domicilioUsuario.ActualizarNumero(usuarioDTO.Domicilio.Numero);
                        if (!string.IsNullOrEmpty(usuarioDTO.Domicilio.Provincia)) domicilioUsuario.ActualizarProvincia(usuarioDTO.Domicilio.Provincia);
                        if (!string.IsNullOrEmpty(usuarioDTO.Domicilio.Ciudad)) domicilioUsuario.ActualizarCiudad(usuarioDTO.Domicilio.Ciudad);
                    }
                    else
                    {
                        throw new NotFoundException(nameof(Domicilio), usuarioDTO.Domicilio.Id);
                    }

                }
                else
                {
                    // Si no se proporciona ID, agregar un nuevo domicilio
                    usuario.AgregarDomicilio(usuarioDTO.Domicilio.Calle, usuarioDTO.Domicilio.Numero, usuarioDTO.Domicilio.Provincia, usuarioDTO.Domicilio.Ciudad);
                }
            }

            await _repository.ActualizarUsuarioAsync(usuario);
            return _mapper.Map<ObtenerUsuarioDTO>(usuario);
        }
    }
}
