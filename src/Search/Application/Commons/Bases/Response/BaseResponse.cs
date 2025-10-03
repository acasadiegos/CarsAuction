using System.Net;

namespace Application.Commons.Bases.Response
{
    public class BaseResponse<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
        public IEnumerable<ErrorDetail> Errors { get; set; }

    }
}
