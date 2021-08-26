using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.Serialization.Abstractions
{
    public interface ISerializer
    {
        Task<string> SerializeAsync(object @object, CancellationToken cancellationToken);
    }
}