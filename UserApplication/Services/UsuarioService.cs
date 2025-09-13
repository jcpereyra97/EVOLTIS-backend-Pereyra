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
        public UsuarioService(IUsuarioRepository repository,IMapper mapper)
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
           var a = await _repository.ObtenerUsuariosPorFiltrosAsync(x =>
                            (string.IsNullOrEmpty(nombre) || x.Nombre.Contains(nombre)) &&
                            (string.IsNullOrEmpty(provincia) || x.Domicilios.Any(p => p.Provincia.Contains(provincia))) &&
                            (string.IsNullOrEmpty(ciudad) || x.Domicilios.Any(p => p.Ciudad.Contains(ciudad))));
            var asd = a.Select(_mapper.Map<ObtenerUsuarioDTO>);

            return asd;
        }

        public Task EliminarUsuarioAsync(int usuarioID)
        {
            throw new NotImplementedException();
        }

        public async Task ActualizarUsuario(ActualizarUsuarioDTO usuarioDTO)
        {
            var usuario = await _repository.ObtenerUsuarioPorIdAsync(usuarioDTO.ID);
            if(usuario == null)
                throw new FileNotFoundException("No existe usuario");


                  
            await _repository.ActualizarUsuarioAsync(usuario);
        }
    }
}
