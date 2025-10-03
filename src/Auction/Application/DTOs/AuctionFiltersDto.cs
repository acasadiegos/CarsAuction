using Application.Commons.Base.Request;

namespace Application.DTOs
{
    public class AuctionFiltersDto : BasePaginationRequest
    {
        private string _model = string.Empty;
        private string _color = string.Empty;
        private string _startDate = string.Empty;
        private string _endDate = string.Empty;
        public string Model { get => _model.Trim(); set => _model = value ?? string.Empty; }
        public string Color { get => _color.Trim(); set => _color = value ?? string.Empty; }
        public int? Year { get; set; }
        public NumericRangeFilter Mileage { get; set; } = new();
        public NumericRangeFilter ReservePrice { get; set; } = new();
        public int? StateFilter { get; set; }
        public string StartDate { get => _startDate.Trim(); set => _startDate = value ?? string.Empty; }
        public string EndDate { get => _endDate.Trim(); set => _endDate = value ?? string.Empty; }

    }
}
