namespace Application.Messaging.Interfaces
{
    public interface IMessagerPublisher
    {
        public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default);
    }
}
