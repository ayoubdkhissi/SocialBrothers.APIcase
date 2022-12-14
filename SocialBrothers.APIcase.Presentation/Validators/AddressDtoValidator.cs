using FluentValidation;
using SocialBrothers.APIcase.Presentation.DTOs;

namespace SocialBrothers.APIcase.Presentation.Validators;

public class AddressDtoValidator : AbstractValidator<AddressDto>
{
    public AddressDtoValidator()
    {
        // Validating street field : not null, not empty and its length does not exceed 200
        RuleFor(dto => dto.Street)
            .NotNull()
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("The value entered for street is not valid");

        // Validating House number: not null, positive number, does not exceed 10^9(int)
        RuleFor(dto => dto.HouseNumber)
            .NotNull()
            .GreaterThan(0)
            .LessThan(1000000000)
            .WithMessage("The value entered for House Number is not valid");

        // Validating ZipCode, not null, positive number, does not exceed 10^9(int)
        RuleFor(dto => dto.ZipCode)
            .NotNull()
            .GreaterThan(0)
            .LessThan(1000000000)
            .WithMessage("The value entered for Zip Code is not valid");

        // Validating City field: Not null and not empty.
        
        RuleFor(dto => dto.City)
            .NotNull()
            .NotEmpty()
            .WithMessage("The field City is mandatory");

        //Validating Country field: Not null and not empty. I
        RuleFor(dto => dto.Country)
            .NotNull()
            .NotEmpty()
            .WithMessage("The field Country is mandatory");

        // PS: in a real scenario, we can validate the values of City and Country using a public API
        // that tells us whether the given value is a real country and that the city belongs to that country
    }
}
