using AutoMapper;
using DatingApp.API.Models;
using DatingApp.API.Dtos;
using System.Linq;

namespace DatingApp.API.Helpers
{
  public class AutoMapperProfiles : Profile
  {
    public AutoMapperProfiles()
    {
      CreateMap<User, UserFoListDto>()
        .ForMember(dest => dest.PhotoUrl, opt =>
        {
          opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
        })
        .ForMember(dest => dest.Age, opt =>
        {
          opt.ResolveUsing(d => d.DateOfBirth.CalculateAge());
        });

      CreateMap<User, UserForDetailsDto>()
        .ForMember(dest => dest.PhotoUrl, opt =>
        {
          opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
        })
        .ForMember(dest => dest.Age, opt =>
        {
          opt.ResolveUsing(d => d.DateOfBirth.CalculateAge());
        });

      CreateMap<Photo, PhotosForDetilsDto>();
      CreateMap<UserForUpdateDto, User>();
      CreateMap<PhotoForCreationDto, Photo>();
      CreateMap<Photo, PhotoForReturnDto>();
    }
  }
}