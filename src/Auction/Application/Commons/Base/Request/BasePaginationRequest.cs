namespace Application.Commons.Base.Request;

public class BasePaginationRequest
{
    public int NumPage { get; set; } = 0;
    private int NumRecordsPage { get; set; } = 10;
    private readonly int NumMaxRecordsPage = 50;
    public string Order { get; set; } = "asc";
    public string Sort { get; set; } = null;

    public int Records
    {
        get => NumRecordsPage;
        set
        {
            NumRecordsPage = (value > NumMaxRecordsPage) ? NumMaxRecordsPage : value;
        }
    }
}
