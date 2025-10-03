using Application.DTOs;
using FluentValidation;

namespace Application.Validators.Auction
{
    public class ItemFiltersValidator : AbstractValidator<ItemFiltersDto>
    {
        public ItemFiltersValidator()
        {
            RuleFor(x => x.NumPage)
                .GreaterThanOrEqualTo(0)
                .WithMessage("The NumPage field cannot be less than 0");

            RuleFor(x => x.Mileage)
                .Must(r => r.MinValue <= r.MaxValue)
                .When(r => r.Mileage.MinValue.HasValue && r.Mileage.MaxValue.HasValue)
                .WithMessage("MinValue of field Mileage cannot be greater than MaxValue");

            RuleFor(x => x.ReservePrice)
                .Must(r => r.MinValue <= r.MaxValue)
                .When(r => r.ReservePrice.MinValue.HasValue && r.ReservePrice.MaxValue.HasValue)
                .WithMessage("MinValue of field ReservePrice cannot be greater than MaxValue");

            RuleFor(x => x.StartDate)
                .NotEmpty()
                .When(x => !string.IsNullOrEmpty(x.EndDate))
                .WithMessage("StartDate field is required when EndDate field is given");

            RuleFor(x => x.StartDate)
                .Must(BeAValidDate)
                .When(x => !string.IsNullOrEmpty(x.StartDate))
                .WithMessage("StartDate must be a valid date (yyyy-MM-dd or ISO format)");

            RuleFor(x => x.EndDate)
                .NotEmpty()
                .When(x => !string.IsNullOrEmpty(x.StartDate))
                .WithMessage("EndDate field is required when StartDate field is given");

            RuleFor(x => x.EndDate)
                .Must(BeAValidDate)
                .When(x => !string.IsNullOrEmpty(x.EndDate))
                .WithMessage("EndDate must be a valid date (yyyy-MM-dd or ISO format)");

            RuleFor(x => new { x.StartDate, x.EndDate })
                .Must(d => DateTime.Parse(d.StartDate) <= DateTime.Parse(d.EndDate))
                .When(d => !string.IsNullOrEmpty(d.StartDate) && !string.IsNullOrEmpty(d.EndDate)
                            && BeAValidDate(d.StartDate) && BeAValidDate(d.EndDate))
                .WithMessage("StartDate cannot be greater than EndDate");

        }
        private bool BeAValidDate(string date)
        {
            return DateTime.TryParse(date, out _);
        }
    }
}
