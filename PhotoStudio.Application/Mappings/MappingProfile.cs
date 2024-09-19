using AutoMapper;
using PhotoStudio.Domain.Models;
using PhotoStudio.Application.DTOs;

namespace PhotoStudio.Domain.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<Booking, BookingDTO>().ReverseMap();
            CreateMap<Payment, PaymentDTO>().ReverseMap();
            CreateMap<Media, MediaDTO>().ReverseMap();
            CreateMap<Album, AlbumDTO>().ReverseMap();

        }
    }
}
