using AutoMapper;
using DogsHouseService.Data.Entities;
using DogsHouseService.DTOs;

namespace DogsHouseService.Mapping
{
    public class DogProfile : Profile
    {
        public DogProfile()
        {
            CreateMap<Dog, DogDto>().ReverseMap();
            CreateMap<CreateDogDto, Dog>();
        }
    }
}
