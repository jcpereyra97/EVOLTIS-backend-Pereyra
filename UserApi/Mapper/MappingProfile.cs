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
            CreateMap<DomicilioDTO, Domicilio>().ReverseMap();
            CreateMap<ObtenerDomicilioDTO, Domicilio>().ReverseMap();

        }

    }
}
