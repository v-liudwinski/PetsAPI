using PetsAPI.Models.Domain;
using PetsAPI.Models.DTO;

namespace PetsAPI.Services.Interfaces;

public interface IDogService
{
    Task<List<Dog>> GetAllAsync();
    Task<List<Dog>?> GetAllWithAttributesAsync(int? pageNumber, int? pageSize, string? attribute, string? sortingOrder);
    Task<Dog?> GetAsync(Guid id);
    Task<Dog?> AddAsync(AddDogRequest addDogRequest);
    Task<Dog?> UpdateAsync(Guid id, UpdateDogRequest updateDogRequest);
    Task<Dog?> DeleteAsync(Guid id);
    Task<List<Dog>?> SortByAttribute(string? attribute, string? sortingOrder);
}