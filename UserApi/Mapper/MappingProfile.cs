using AutoMapper;
using UserApplication.DTOs;
using UserDomain.Domain;

namespace UserApi.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UsuarioDTO, Usuario>().ReverseMap();                
            CreateMap<ObtenerUsuarioDTO, Usuario>().ReverseMap();
            CreateMap<ActualizarUsuarioDTO, Usuario>()
                .ForMember(p => p.Nombre, x => x.Ignore())
                .ForMember(p => p.Email, x => x.Ignore())
                .ForMember(p => p.Domicilios, x => x.Ignore())
                .AfterMap((dto, user) =>
                {
                    if (!string.IsNullOrEmpty(dto.Nombre)) user.Renombrar(dto.Nombre);
                    if (!string.IsNullOrEmpty(dto.Email)) user.CambiarEmail(dto.Email);

                });

        }

    }
}
