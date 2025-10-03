namespace Application.Commons.Base.Response
{
    public class BaseCreatedResponse<T> : BaseResponse<T>
    {
        public Guid? Id { get; set; }
    }
}
