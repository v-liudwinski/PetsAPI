using FluentValidation;
using PetsAPI.Models.DTO;

namespace PetsAPI.Validators;

public class GetAllDogsWithAttributesRequestValidator : AbstractValidator<GetAllDogsWithAttributesRequest>
{
    public GetAllDogsWithAttributesRequestValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
                .WithMessage("Page number can not be negative!");
        RuleFor(x => x.PageSize)
            .GreaterThan(0)
                .WithMessage("Page size can not be negative!");
    }
}