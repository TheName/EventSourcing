using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.Serialization.Abstractions
{
    /// <summary>
    /// The serializer
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Serializes provided object
        /// </summary>
        /// <param name="object">
        /// The object to serialize
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>.
        /// </param>
        /// <returns>
        /// A serialized representation of <paramref name="@object"/> in the form of string.
        /// </returns>
        Task<string> SerializeAsync(object @object, CancellationToken cancellationToken);
    }
}