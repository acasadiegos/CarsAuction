using FluentValidation.Results;
using System.Net;

namespace Application.Commons.Base
{
    public class BaseResponse<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public T Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public IEnumerable<ValidationFailure> Errors { get; set; }
    }
}
