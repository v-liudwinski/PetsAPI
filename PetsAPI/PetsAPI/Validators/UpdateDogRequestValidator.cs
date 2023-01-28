using FluentValidation;
using PetsAPI.Models.DTO;

namespace PetsAPI.Validators;

public class UpdateDogRequestValidator : AbstractValidator<UpdateDogRequest>
{
    public UpdateDogRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotNull()
                .WithMessage("Name can not be null!")
            .NotEmpty()
                .WithMessage("Name can not be empty!");
        RuleFor(x => x.Color)
            .NotNull()
                .WithMessage("Color can not be null!")
            .NotEmpty()
                .WithMessage("Color can not be empty!");
        RuleFor(x => x.TailLength)
            .NotNull()
                .WithMessage("This value can not be null")
            .GreaterThanOrEqualTo(0)
                .WithMessage("Tail length can not be negative value!");
        RuleFor(x => x.Weight)
            .NotNull()
                .WithMessage("This value can not be null")
            .GreaterThan(0)
                .WithMessage("Weight can not be zero or lower!");
    }
}