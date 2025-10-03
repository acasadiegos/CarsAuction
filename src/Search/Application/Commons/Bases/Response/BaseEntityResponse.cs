namespace Application.Commons.Bases.Response
{
    public class BaseEntityResponse<T>
    {
        public long TotalRecords { get; set; }
        public IReadOnlyList<T> Items { get; set; }
    }
}
