namespace PetsAPI.Models.DTO;

public class UpdateDogRequest
{
    public string Name { get; set; }
    public string Color { get; set; }
    public double TailLength { get; set; }
    public double Weight { get; set; }
}