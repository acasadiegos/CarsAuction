using Application.Commons.Base.Request;
using Application.Commons.Bases.Request;

namespace Application.DTOs
{
    public class ItemFiltersDto : BasePaginationRequest
    {
        private string _fullSearchText = string.Empty;
        private string _model = string.Empty;
        private string _color = string.Empty;
        private string _make = string.Empty;
        private string _winner = string.Empty;
        private string _seller = string.Empty;
        private string _startDate = string.Empty;
        private string _endDate = string.Empty;
        public string FullSearchText { get => _fullSearchText.Trim(); set => _fullSearchText = value ?? string.Empty; }
        public string Model { get => _model.Trim(); set => _model = value ?? string.Empty; }
        public string Color { get => _color.Trim(); set => _color = value ?? string.Empty; }
        public string Make { get => _make.Trim(); set => _make = value ?? string.Empty; }
        public string Winner { get => _winner.Trim(); set => _winner = value ?? string.Empty; }
        public string Seller { get => _seller.Trim(); set => _seller = value ?? string.Empty; }
        public int? Year { get; set; }
        public NumericRangeFilter Mileage { get; set; } = new();
        public NumericRangeFilter ReservePrice { get; set; } = new();
        public int? StateFilter { get; set; }
        public string StartDate { get => _startDate.Trim(); set => _startDate = value ?? string.Empty; }
        public string EndDate { get => _endDate.Trim(); set => _endDate = value ?? string.Empty; }
        public bool OnlyGetEndingSoonAuctions { get; set; }

    }
}
