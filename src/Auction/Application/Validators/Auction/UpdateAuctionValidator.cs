using Application.DTOs;
using FluentValidation;

namespace Application.Validators.Auction
{
    public class UpdateAuctionValidator : AbstractValidator<UpdateAuctionDto>
    {
        public UpdateAuctionValidator()
        {
            RuleFor(x => x.Make)
                .NotNull().WithMessage("The Make field cannot be null.")
                .NotEmpty().WithMessage("The Make field cannot be empty.");

            RuleFor(x => x.Model)
                .NotNull().WithMessage("The Model field cannot be null.")
                .NotEmpty().WithMessage("The Model field cannot be empty.");

            RuleFor(x => x.Year)
                .NotNull().WithMessage("The Year field cannot be null.")
                .GreaterThan(999).WithMessage("The Year field value must correspond to a valid year.")
                .LessThanOrEqualTo(DateTime.UtcNow.Year).WithMessage("The Year field value cannot be greater than the current year.");

            RuleFor(x => x.Mileage)
                .NotNull().WithMessage("The Mileage field cannot be null.")
                .GreaterThanOrEqualTo(0).WithMessage("The Mileage field value must be greater than or equal to 0.");

            RuleFor(x => x.Color)
                .NotNull().WithMessage("The Color field cannot be null.")
                .NotEmpty().WithMessage("The Color field cannot be empty.");
        }
    }
}