using Microsoft.AspNetCore.Mvc;
using PetsAPI.Models.DTO;
using PetsAPI.Repositories.Interfaces;
using PetsAPI.Services.Interfaces;

namespace PetsAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class DogController : Controller
{
    private const string PingMessage = "Dogs house service. Version 1.0.1";

    private readonly IDogService _dogService;

    public DogController(IDogService dogService)
    {
        _dogService = dogService;
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<IActionResult> Ping()
    {
        return Ok(PingMessage);
    }

    [HttpGet]
    [Route("dogs")]
    public async Task<IActionResult> GetAllDogsAsync([FromQuery] GetAllDogsWithAttributesRequest getDogsRequest)
    {
        var pageNumber = getDogsRequest.PageNumber;
        var pageSize = getDogsRequest.PageSize;
        var attribute = getDogsRequest.Attribute;
        var sortingOrder = getDogsRequest.SortingOrder;
        
        var dogs = await _dogService
            .GetAllWithAttributesAsync(pageNumber, pageSize, attribute, sortingOrder);
        if (dogs is null) return BadRequest("Invalid request!");
        return Ok(dogs);
    }

    [HttpGet]
    [Route("dog/{id:guid}")]
    public async Task<IActionResult> GetDogByIdAsync(Guid id)
    {
        var dog = await _dogService.GetAsync(id);
        if (dog is null) return NotFound("Invalid Id!");
        return Ok(dog);
    }

    [HttpPost]
    [Route("dog")]
    public async Task<IActionResult> AddDogAsync([FromBody] AddDogRequest addDogRequest)
    {
        var dog = await _dogService.AddAsync(addDogRequest);
        if (dog is null) return NotFound("Your form is invalid or dog`s name have already exist!");
        return Ok(dog);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateDogAsync(Guid id, [FromBody] UpdateDogRequest updateDogRequest)
    {
        var dog = await _dogService.UpdateAsync(id, updateDogRequest);
        if (dog is null) return NotFound("Invalid Id or form or dog`s name have already exist!");
        return Ok(dog);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteDogAsync(Guid id)
    {
        var dog = await _dogService.DeleteAsync(id);
        if (dog is null) return NotFound("Invalid Id!");
        return Ok(dog);
    }
}