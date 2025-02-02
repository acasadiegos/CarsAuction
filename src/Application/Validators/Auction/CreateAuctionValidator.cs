using Application.DTOs;
using FluentValidation;

namespace Application.Validators.Auction
{
    public class CreateAuctionValidator : AbstractValidator<CreateAuctionDto>
    {
        public CreateAuctionValidator()
        {
            RuleFor(x => x.AuctionEnd)
                .NotNull().WithMessage("El campo AuctionEnd no puede ser nulo")
                .NotEmpty().WithMessage("El campo AuctionEnd no puede estar vacío.")
                .GreaterThanOrEqualTo(DateTime.UtcNow).WithMessage("El valor del campo AuctionEnd no puede ser menor a la fecha actual.");

            RuleFor(x => x.Color)
                .NotNull().WithMessage("El campo Color no puede ser nulo.")
                .NotEmpty().WithMessage("El campo Color no puede estar vacío.");

            RuleFor(x => x.ImageUrl)
                .NotNull().WithMessage("El campo ImageUrl no puede ser nulo.")
                .NotEmpty().WithMessage("El campo ImageUrl no puede estar vacío.");

            RuleFor(x => x.Make)
                .NotNull().WithMessage("El campo Make no puede ser nulo.")
                .NotEmpty().WithMessage("El campo Make no puede estar vacío.");

            RuleFor(x => x.Mileage)
                .NotNull().WithMessage("El campo Mileage no puede ser nulo.")
                .GreaterThanOrEqualTo(0).WithMessage("El valor del campo Mileage debe ser mayor o igual a 0.");

            RuleFor(x => x.Model)
                .NotNull().WithMessage("El campo Model no puede ser nulo.")
                .NotEmpty().WithMessage("El campo Model no puede estar vacío.");

            RuleFor(x => x.ReservePrice)
                .NotNull().WithMessage("El campo ReservePrice no puede ser nulo.")
                .GreaterThan(0).WithMessage("El valor del campo ReservePrice debe ser mayor a 0.");

            RuleFor(x => x.Year)
                .NotNull().WithMessage("El campo ReservePrice no puede ser nulo.")
                .GreaterThan(999).WithMessage("El valor del campo Year debe corresponder a un año válido.")
                .LessThanOrEqualTo(DateTime.UtcNow.Year).WithMessage("El valor del campo Year no debe ser mayor al año actual.");
        }
    }
}
