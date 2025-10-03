namespace Application.DTOs;

public class UpdateAuctionDto
{
    private string _make = string.Empty;
    private string _model = string.Empty;
    private string _color = string.Empty;
    public string Make { get => _make.Trim(); set => _make = value ?? string.Empty; }
    public string Model { get => _model.Trim(); set => _model = value ?? string.Empty; }
    public int? Year { get; set; }
    public string Color { get => _color.Trim(); set => _color = value ?? string.Empty; }
    public int? Mileage { get; set; }
}
