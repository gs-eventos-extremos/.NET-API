using AutoMapper;
using WeatherEmergencyAPI.DTOs.EmergencyContact;
using WeatherEmergencyAPI.DTOs.Location;
using WeatherEmergencyAPI.DTOs.User;
using WeatherEmergencyAPI.Models;

namespace WeatherEmergencyAPI.Configurations
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // User Mappings
            CreateMap<CreateUserDto, User>();
            CreateMap<User, UserResponseDto>();
            CreateMap<UpdateUserDto, User>()  // ADICIONE ESTA LINHA
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Location Mappings
            CreateMap<CreateLocationDto, Location>();
            CreateMap<UpdateLocationDto, Location>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Location, LocationResponseDto>();

            // EmergencyContact Mappings
            CreateMap<CreateEmergencyContactDto, EmergencyContact>();
            CreateMap<EmergencyContact, EmergencyContactResponseDto>();
        }
    }
}