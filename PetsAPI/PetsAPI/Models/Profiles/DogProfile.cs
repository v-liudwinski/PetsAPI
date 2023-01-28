using AutoMapper;
using PetsAPI.Models.Domain;
using PetsAPI.Models.DTO;

namespace PetsAPI.Models.Profiles;

public class DogProfile : Profile
{
    public DogProfile()
    {
        CreateMap<Dog, AddDogRequest>().ReverseMap();
        CreateMap<Dog, UpdateDogRequest>().ReverseMap();
    }
}