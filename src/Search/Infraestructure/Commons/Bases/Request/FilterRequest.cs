namespace Infraestructure.Commons.Bases.Request
{
    public class FilterRequest
    {
        public int NumFilter { get; set; }
        public string TextFilter { get; set; }
        public long MinNumberValue { get; set; } = -1;
        public long MaxNumberValue { get; set; } = -1;
    }
}
