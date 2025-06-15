namespace Infraestructure.Commons.Bases.Response
{
    public class BaseResponse<T>
    {
        public bool IsSucces { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }

    }
}
