using AutoMapper;
using PetsAPI.Models.Domain;
using PetsAPI.Models.DTO;
using PetsAPI.Repositories.Interfaces;
using PetsAPI.Services.Interfaces;

namespace PetsAPI.Services;

public class DogService : IDogService
{
    private readonly IDogRepository _dogRepository;

    public DogService(IDogRepository dogRepository)
    {
        _dogRepository = dogRepository;
    }

    public async Task<List<Dog>> GetAllAsync()
    {
        return await _dogRepository.GetAllAsync();
    }

    public async Task<List<Dog>?> GetAllWithAttributesAsync(int? pageNumber, int? pageSize,
                                                            string? attribute, string? sortingOrder)
    {
        return await _dogRepository.GetAllWithAttributesAsync(pageNumber, pageSize, attribute, sortingOrder);
    }

    public async Task<Dog?> GetAsync(Guid id)
    {
        return await _dogRepository.GetAsync(id);
    }

    public async Task<Dog?> AddAsync(AddDogRequest addDogRequest)
    {
        return await _dogRepository.AddAsync(addDogRequest);
    }

    public async Task<Dog?> UpdateAsync(Guid id, UpdateDogRequest updateDogRequest)
    {
        return await _dogRepository.UpdateAsync(id, updateDogRequest);
    }

    public async Task<Dog?> DeleteAsync(Guid id)
    {
        return await _dogRepository.DeleteAsync(id);
    }
    
    public async Task<List<Dog>?> SortByAttribute(string? attribute, string? sortingOrder)
    {
        return await _dogRepository.SortByAttribute(attribute, sortingOrder);
    }
}