using Application.Messaging.Interfaces;
using MassTransit;

namespace Infraestructure.Messaging.Publishers
{
    public class MessagerPublisher : IMessagerPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public MessagerPublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
        {
            await _publishEndpoint.Publish(message, cancellationToken);
        }
    }
}
