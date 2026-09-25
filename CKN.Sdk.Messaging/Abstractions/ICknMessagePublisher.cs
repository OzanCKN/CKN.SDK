using System.Threading;
using System.Threading.Tasks;

namespace CKN.Sdk.Messaging.Abstractions;

public interface ICknMessagePublisher
{
    Task PublishAsync<T>(string exchange, string routingKey, T message, CancellationToken cancellationToken = default);
}
