namespace Infraestructure.Commons.Bases.Request;

public class BaseFiltersRequest : BasePaginationRequest
{
    public List<FilterRequest> Filters { get; set; }
    public int StateFilter { get; set; } = -1;
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public bool Download { get; set; }
    
}
