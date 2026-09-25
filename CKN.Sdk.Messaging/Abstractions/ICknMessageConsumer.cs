using System.Threading;
using System.Threading.Tasks;

namespace CKN.Sdk.Messaging.Abstractions;

public interface ICknMessageConsumer<in T>
{
    Task HandleAsync(T message, CancellationToken cancellationToken);
}
