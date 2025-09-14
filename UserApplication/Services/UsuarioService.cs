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
        public async Task<int> AgregarUsuarioAsync(UsuarioDTO usuarioDTO)
        {
            var usuario = _mapper.Map<Usuario>(usuarioDTO);
            return await _repository.AgregarUsuarioAsync(usuario);
        }

        public async Task<ObtenerUsuarioDTO> ObtenerUsuarioPorIdAsync(int usuarioID)
        {
            var usuario = await _repository.ObtenerUsuarioPorIdAsync(usuarioID);
            if (usuario == null) throw new NotFoundException(nameof(Usuario), usuarioID);
            return _mapper.Map<ObtenerUsuarioDTO>(usuario);
        }

        public async Task<PaginationResponse<ObtenerUsuarioDTO>> ObtenerUsuariosConFiltrosAsync(string? nombre, string? provincia, string? ciudad,
                                                                                                int page = 1, int pageSize = 20)
        {
            Expression<Func<Usuario,bool>> filtros = x =>
                                     (x.Activo) &&
                                     (string.IsNullOrEmpty(nombre) || x.Nombre.Contains(nombre)) &&
                                     (string.IsNullOrEmpty(provincia) || x.Domicilios.Any(p => p.Provincia.Contains(provincia))) &&
                                     (string.IsNullOrEmpty(ciudad) || x.Domicilios.Any(p => p.Ciudad.Contains(ciudad)));


            var listaUsuarios = await _repository.ObtenerUsuariosPorFiltrosAsync(filtros, page, pageSize);
            var lstDtos = listaUsuarios.Items.Select(_mapper.Map<ObtenerUsuarioDTO>).ToList();
            return new PaginationResponse<ObtenerUsuarioDTO>(lstDtos, listaUsuarios.Page, listaUsuarios.PageSize, listaUsuarios.TotalCount);
        }

        public async Task EliminarUsuarioAsync(int usuarioID)
        {
            var usuario = await _repository.ObtenerUsuarioPorIdAsync(usuarioID);

            if (usuario == null) throw new NotFoundException(nameof(Usuario), usuarioID);
            if (!usuario.Activo) throw new NotFoundException(nameof(Usuario), usuarioID, $"Este usuario ya ha sido eliminado. Ultima modificacion: {usuario.FechaUltimaActualizacion}"); ;

            usuario.EliminarUsuario();
            await _repository.ActualizarUsuarioAsync(usuario);
        }

        public async Task<ObtenerUsuarioDTO> ActualizarUsuarioAsync(int usuarioID,ActualizarUsuarioDTO usuarioDTO)
        {
            usuarioDTO.SetID(usuarioID);
            var usuario = await _repository.ObtenerUsuarioPorIdAsync(usuarioDTO.ID);
            if (usuario == null)
                throw new NotFoundException(nameof(Usuario),usuarioDTO.ID);

            if (!usuario.Activo)
                throw new NotFoundException(nameof(Usuario),usuarioDTO.ID,"Inactivo");

            if (!string.IsNullOrEmpty(usuarioDTO.Nombre)) usuario.Renombrar(usuarioDTO.Nombre);
            if (!string.IsNullOrEmpty(usuarioDTO.Email)) usuario.CambiarEmail(usuarioDTO.Email);

            if (usuarioDTO.Domicilio != null)
            {
                if (usuarioDTO.Domicilio.ID != null)
                {
                    var domicilioUsuario = usuario.BuscarDomicilioPorID((int)usuarioDTO.Domicilio.ID);

                    if(domicilioUsuario != null && !usuario.ExisteDomicilio(usuarioDTO.Domicilio.Calle, usuarioDTO.Domicilio.Numero, usuarioDTO.Domicilio.Provincia, usuarioDTO.Domicilio.Ciudad))
                    {
                        if (!string.IsNullOrEmpty(usuarioDTO.Domicilio.Calle)) domicilioUsuario.ActualizarCalle(usuarioDTO.Domicilio.Calle);
                        if (!string.IsNullOrEmpty(usuarioDTO.Domicilio.Numero)) domicilioUsuario.ActualizarNumero(usuarioDTO.Domicilio.Numero);
                        if (!string.IsNullOrEmpty(usuarioDTO.Domicilio.Provincia)) domicilioUsuario.ActualizarProvincia(usuarioDTO.Domicilio.Provincia);
                        if (!string.IsNullOrEmpty(usuarioDTO.Domicilio.Ciudad)) domicilioUsuario.ActualizarCiudad(usuarioDTO.Domicilio.Ciudad);
                    }
                    else
                    {
                        throw new NotFoundException(nameof(Domicilio), usuarioDTO.Domicilio.ID);
                    }

                }
                else
                {
                    usuario.AgregarDomicilio(usuarioDTO.Domicilio.Calle, usuarioDTO.Domicilio.Numero, usuarioDTO.Domicilio.Provincia, usuarioDTO.Domicilio.Ciudad);
                }
            }

            await _repository.ActualizarUsuarioAsync(usuario);
            return _mapper.Map<ObtenerUsuarioDTO>(usuario);
        }
    }
}
