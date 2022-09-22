using ApiVet.Models;
using AutoMapper;

namespace ApiVet.DTO.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Veterinario, VeterinarioDTO>().ReverseMap();
            CreateMap<Cliente, ClienteDTO>().ReverseMap();
            CreateMap<Cachorro, CachorroDTO>().ReverseMap();
            CreateMap<Consulta, ConsultaDTO>().ReverseMap();
            CreateMap<Usuario, UsuarioDTO>().ReverseMap();
        }
    }
}