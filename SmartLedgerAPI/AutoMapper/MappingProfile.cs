using APILibrary.Data.Models;
using APILibrary.Services.DTOs.Auth;
using AutoMapper;

namespace SmartLedgerAPI.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, LoginRegisterResponseDTO>();
        }
    }
}
