using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PetsAPI.Data;
using PetsAPI.Models.Domain;
using PetsAPI.Models.DTO;
using PetsAPI.Repositories.Interfaces;

namespace PetsAPI.Repositories;

public class DogRepository : IDogRepository
{
    private readonly PetsDbContext _dbContext;
    private readonly IMapper _mapper;

    public DogRepository(PetsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<Dog>> GetAllAsync()
    {
        return await _dbContext.Dogs
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Dog>?> GetAllWithAttributesAsync(int? pageNumber, int? pageSize, string? attribute, string? sortingOrder)
    {
        var dogs = new List<Dog>();
        switch (pageNumber, pageSize, attribute, sortingOrder)
        {
            case (not null, not null, null, null):
                var currentPageNumber = pageNumber ?? 1;
                var currentPageSize = pageSize ?? 5;

                dogs = await GetAllAsync();

                return dogs
                    .Skip((currentPageNumber - 1) * currentPageSize)
                    .Take(currentPageSize)
                    .ToList();
            
            case (null, null, not null, not null):
                dogs = await SortByAttribute(attribute, sortingOrder);
                return dogs ?? default;
            
            case (not null, not null, not null, not null):
                currentPageNumber = pageNumber ?? 1;
                currentPageSize = pageSize ?? 5;
                dogs = await SortByAttribute(attribute, sortingOrder);
                return dogs?.Skip((currentPageNumber - 1) * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();
            
            default:
                return await GetAllAsync();
        }
    }

    public async Task<Dog?> GetAsync(Guid id)
    {
        var dog = await _dbContext.Dogs.FirstOrDefaultAsync(x => x.Id == id);
        return dog ?? default;
    }

    public async Task<Dog?> AddAsync(AddDogRequest addDogRequest)
    {
        if (addDogRequest is null) return default;
        var dogs = await GetAllAsync();
        if (dogs.Any(x => x.Name == addDogRequest.Name)) return default;
        var dog = _mapper.Map<Dog>(addDogRequest);
        dog.Id = Guid.NewGuid();
        await _dbContext.Dogs.AddAsync(dog);
        await _dbContext.SaveChangesAsync();
        return dog;
    }

    public async Task<Dog?> UpdateAsync(Guid id, UpdateDogRequest updateDogRequest)
    {
        var dogs = await GetAllAsync();
        if (dogs.Any(x => x.Name == updateDogRequest.Name)) return default;
        
        var dog = await _dbContext.Dogs.FirstOrDefaultAsync(x => x.Id == id);
        if (dog is null || updateDogRequest is null) return default;
        
        dog.Name = updateDogRequest.Name;
        dog.Color = updateDogRequest.Color;
        dog.TailLength = updateDogRequest.TailLength;
        dog.Weight = updateDogRequest.Weight;
        await _dbContext.SaveChangesAsync();
        
        return dog;
    }

    public async Task<Dog?> DeleteAsync(Guid id)
    {
        var dog = await _dbContext.Dogs.FirstOrDefaultAsync(x => x.Id == id);
        if (dog is null) return default;
        _dbContext.Dogs.Remove(dog);
        await _dbContext.SaveChangesAsync();
        return dog;
    }

    public async Task<List<Dog>?> SortByAttribute(string? attribute, string? sortingOrder)
    {
        var dogs = await GetAllAsync();
        return (attribute, sortingOrder) switch
        {
            ("name", "asc") => dogs.OrderBy(x => x.Name).ToList(),
            ("color", "asc") => dogs.OrderBy(x => x.Color).ToList(),
            ("tail_length", "asc") => dogs.OrderBy(x => x.TailLength).ToList(),
            ("weight", "asc") => dogs.OrderBy(x => x.Weight).ToList(),
            ("name", "desc") => dogs.OrderByDescending(x => x.Name).ToList(),
            ("color", "desc") => dogs.OrderByDescending(x => x.Color).ToList(),
            ("tail_length", "desc") => dogs.OrderByDescending(x => x.TailLength).ToList(),
            ("weight", "desc") => dogs.OrderByDescending(x => x.Weight).ToList(),
            _ => default
        };
    }
}