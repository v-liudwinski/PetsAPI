using AutoMapper;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using PetsAPI.Data;
using PetsAPI.Models.Domain;
using PetsAPI.Models.DTO;
using PetsAPI.Models.Profiles;
using PetsAPI.Repositories;

namespace CoreServices.Test;

public class DogTests
{
    private List<Dog>? dogs;
    private DbContextMock<PetsDbContext> _dbContextMock;
    private IMapper _mapper;
    
    [SetUp]
    public void Setup()
    {
        var dogProfile = new DogProfile();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(dogProfile));
        _mapper = new Mapper(configuration);
        
        dogs = new List<Dog>
        {
            new()
            {
                Id = Guid.Parse("da613008-4116-4629-8892-5626f58ca577"),
                Name = "Hattiko",
                Color = "yellow",
                TailLength = 30,
                Weight = 10
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Justa",
                Color = "brow",
                TailLength = 20,
                Weight = 7
            }
        };
        
        _dbContextMock = new DbContextMock<PetsDbContext>(new DbContextOptionsBuilder<PetsDbContext>().Options);
        _dbContextMock.CreateDbSetMock(x => x.Dogs, dogs);
    }

    [Test]
    public async Task GetAllAsync_CorrectDataReading_Success()
    {
        // Arrange
        var repository = new DogRepository(_dbContextMock.Object, _mapper);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.That(result, Has.Count.EqualTo(2));
    }

    [Test]
    [TestCase("name", "asc", null)]
    [TestCase("color", "asc", null)]
    [TestCase("tail_length", "asc", null)]
    [TestCase("weight", "asc", null)]
    [TestCase("name", "desc", null)]
    [TestCase("color", "desc", null)]
    [TestCase("tail_length", "desc", null)]
    [TestCase("weight", "desc", null)]
    public async Task SortByAttributeAsync_SpecificSortingByNameAndAttribute_Success
        (string? attribute, string? sortingOrder, List<Dog>? expectedResult)
    {
        // Arrange
        expectedResult = (attribute, sortingOrder) switch
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
        var repository = new DogRepository(_dbContextMock.Object, _mapper);

        // Act
        var result = await repository.SortByAttribute(attribute, sortingOrder);

        // Assert
        Assert.NotNull(result);
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.First().Id, Is.EqualTo(expectedResult.First().Id));
        Assert.That(result.Last().Id, Is.EqualTo(expectedResult.Last().Id));
    }
    
    [Test]
    [TestCase("", "")]
    public async Task SortByAttributeAsync_SpecificSortingByNameAndAttribute_Null
        (string? attribute, string? sortingOrder)
    {
        // Arrange
        var repository = new DogRepository(_dbContextMock.Object, _mapper);

        // Act
        var result = await repository.SortByAttribute(attribute, sortingOrder);

        // Assert
        Assert.Null(result);
    }

    [Test]
    [TestCase(1, 1, "name", "asc", null)]
    [TestCase(2, 1, "weight", "desc", null)]
    [TestCase(1, 2, "tail_length", "asc", null)]
    [TestCase(null, null, "tail_length", "asc", null)]
    [TestCase(1, 2, null, null, null)]
    [TestCase(null, null, null, null, null)]
    public async Task GetAllWithAttributesAsync_SortAndGetListInSpecificWay_Success
        (int? pageNumber, int? pageSize, string? attribute, string? sortingOrder, List<Dog>? expectedResult)
    {
        // Arrange
        var repository = new DogRepository(_dbContextMock.Object, _mapper);
        
        if (attribute is not null && sortingOrder is not null)
            expectedResult = await repository.SortByAttribute(attribute, sortingOrder);
        else
            expectedResult = await repository.GetAllAsync();
        
        if (pageNumber is not null && pageSize is not null)
        {
            expectedResult = expectedResult
                .Skip((int)((pageNumber - 1) * pageSize))
                .Take((int)pageSize)
                .ToList();
        }


        // Act
        var result = await repository.GetAllWithAttributesAsync(pageNumber, pageSize, attribute, sortingOrder);

        // Assert
        Assert.NotNull(result);
        Assert.That(result.First().Id, Is.EqualTo(expectedResult.First().Id));
        Assert.That(result.Last().Id, Is.EqualTo(expectedResult.Last().Id));
    }

    [Test]
    [TestCase("da613008-4116-4629-8892-5626f58ca577")]
    public async Task GetAsync_GettingDogById_Success(string id)
    {
        // Arrange
        var repsitory = new DogRepository(_dbContextMock.Object, _mapper);

        // Act
        var result = await repsitory.GetAsync(Guid.Parse(id));

        // Assert
        Assert.That(result.Id, Is.EqualTo(Guid.Parse(id)));
    }
    
    [Test]
    [TestCase("da613008-4116-4629-8892-5626f58ca333")]
    public async Task GetAsync_GettingDogById_Null(string id)
    {
        // Arrange
        var repsitory = new DogRepository(_dbContextMock.Object, _mapper);

        // Act
        var result = await repsitory.GetAsync(Guid.Parse(id));

        // Assert
        Assert.Null(result);
    }
    
    [Test]
    [TestCase("Poppy", "white & black", 28, 15)]
    public async Task AddAsync_AddingNewDogToDb_Success(string name, string color, double tailLength, double weight)
    {
        // Arrange
        var addDogRequest = new AddDogRequest
        {
            Name = name,
            Color = color,
            TailLength = tailLength,
            Weight = weight
        };
        var repository = new DogRepository(_dbContextMock.Object, _mapper);

        // Act
        var dog = await repository.AddAsync(addDogRequest);
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(dog);
        Assert.That(result, Has.Count.EqualTo(3));
    }
    
    [Test]
    [TestCase("Hattiko", "white & black", 28, 15)]
    public async Task AddAsync_AddingNewDogToDb_Null(string name, string color, double tailLength, double weight)
    {
        // Arrange
        var addDogRequest = new AddDogRequest
        {
            Name = name,
            Color = color,
            TailLength = tailLength,
            Weight = weight
        };
        var repository = new DogRepository(_dbContextMock.Object, _mapper);

        // Act
        var dog = await repository.AddAsync(addDogRequest);
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Null(dog);
        Assert.That(result, Has.Count.EqualTo(2));
    }
    
    [Test]
    [TestCase("Poppy", "white & black", 28, 15, "da613008-4116-4629-8892-5626f58ca577")]
    public async Task AddAsync_UpdatingDogInDb_Success
        (string name, string color, double tailLength, double weight, string id)
    {
        // Arrange
        var updateDogRequest = new UpdateDogRequest()
        {
            Name = name,
            Color = color,
            TailLength = tailLength,
            Weight = weight
        };
        var repository = new DogRepository(_dbContextMock.Object, _mapper);

        // Act
        var result = await repository.UpdateAsync(Guid.Parse(id), updateDogRequest);

        // Assert
        Assert.NotNull(result);
        Assert.That(result?.Name, Is.EqualTo(name));
        Assert.That(result?.Color, Is.EqualTo(color));
        Assert.That(result?.TailLength, Is.EqualTo(tailLength));
        Assert.That(result?.Weight, Is.EqualTo(weight));
    }
    
    [Test]
    [TestCase("Poppy", "white & black", 28, 15, "da613008-4116-4629-8892-5626f58ca333")]
    public async Task AddAsync_UpdatingDogInDb_Null
        (string name, string color, double tailLength, double weight, string id)
    {
        // Arrange
        var updateDogRequest = new UpdateDogRequest()
        {
            Name = name,
            Color = color,
            TailLength = tailLength,
            Weight = weight
        };
        var repository = new DogRepository(_dbContextMock.Object, _mapper);

        // Act
        var result = await repository.UpdateAsync(Guid.Parse(id), updateDogRequest);

        // Assert
        Assert.Null(result);
    }
    
    [Test]
    [TestCase("da613008-4116-4629-8892-5626f58ca577")]
    public async Task DeleteAsync_UpdatingDogInDb_Success(string id)
    {
        // Arrange
        var repository = new DogRepository(_dbContextMock.Object, _mapper);

        // Act
        var dog = await repository.GetAsync(Guid.Parse(id));
        var result = await repository.DeleteAsync(Guid.Parse(id));
        var dogAfterDeleting = await repository.GetAsync(Guid.Parse(id));
        var allDogs = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(dog);
        Assert.NotNull(result);
        Assert.Null(dogAfterDeleting);
        Assert.That(allDogs, Has.Count.EqualTo(1));
        Assert.That(result.Name, Is.EqualTo(dog.Name));
        Assert.That(result.Color, Is.EqualTo(dog.Color));
        Assert.That(result.TailLength, Is.EqualTo(dog.TailLength));
        Assert.That(result.Weight, Is.EqualTo(dog.Weight));
    }
}