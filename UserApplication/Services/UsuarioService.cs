using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public async Task AgregarUsuarioAsync(UsuarioDTO usuarioDTO)
        {
            var user = _mapper.Map<Usuario>(usuarioDTO);
            await _repository.AgregarUsuarioAsync(user);
        }

        public async Task<ObtenerUsuarioDTO> ObtenerUsuarioPorIdAsync(int usuarioID)
        {
            var user = await _repository.ObtenerUsuarioPorIdAsync(usuarioID);
            return _mapper.Map<ObtenerUsuarioDTO>(user);

        }

        public async Task<IEnumerable<ObtenerUsuarioDTO>> ObtenerUsuariosConFiltrosAsync(string? nombre, string? provincia, string? ciudad)
        {
            var listaUsuarios = await _repository.ObtenerUsuariosPorFiltrosAsync(x =>
                                     (x.Activo) &&
                                     (string.IsNullOrEmpty(nombre) || x.Nombre.Contains(nombre)) &&
                                     (string.IsNullOrEmpty(provincia) || x.Domicilios.Any(p => p.Provincia.Contains(provincia))) &&
                                     (string.IsNullOrEmpty(ciudad) || x.Domicilios.Any(p => p.Ciudad.Contains(ciudad))));

            return listaUsuarios.Select(_mapper.Map<ObtenerUsuarioDTO>);
        }

        public async Task EliminarUsuarioAsync(int usuarioID)
        {
            var usuario = await _repository.ObtenerUsuarioPorIdAsync(usuarioID);

            if (usuario == null) throw new FileNotFoundException("No existe usuario");

            usuario.EliminarUsuario();
            await _repository.ActualizarUsuarioAsync(usuario);
        }

        public async Task ActualizarUsuarioAsync(int usuarioID,ActualizarUsuarioDTO usuarioDTO)
        {
            usuarioDTO.SetID(usuarioID);
            var usuario = await _repository.ObtenerUsuarioPorIdAsync(usuarioDTO.ID);
            if (usuario == null)
                throw new FileNotFoundException("No existe usuario");

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
                        throw new FileNotFoundException($"No existe Domicilio con ID{usuarioDTO.Domicilio.ID}");
                    }

                }
                else
                {
                    usuario.AgregarDomicilio(usuarioDTO.Domicilio.Calle, usuarioDTO.Domicilio.Numero, usuarioDTO.Domicilio.Provincia, usuarioDTO.Domicilio.Ciudad);
                }
            }

            await _repository.ActualizarUsuarioAsync(usuario);
        }
    }
}
