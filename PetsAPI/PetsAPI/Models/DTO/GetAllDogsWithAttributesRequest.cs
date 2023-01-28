namespace PetsAPI.Models.DTO;

public class GetAllDogsWithAttributesRequest
{
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
    public string? Attribute { get; set; }
    public string? SortingOrder { get; set; }
}