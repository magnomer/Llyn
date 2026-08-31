using System.Threading;
using System.Threading.Tasks;

namespace Llyn.Core;

/// <summary>
/// Pronunciation lookup contract. A requester starts a lookup and subscribes to its results
/// through an <see cref="LReceiver"/>. All lookup logic lives behind this seam,
/// never in the UI.
/// </summary>
public interface LSeeker
{
    /// <summary>
    /// Starts a lookup for <paramref name="word"/>, streaming results to <paramref name="receiver"/>.
    /// The returned task completes when every source has finished (or the lookup is cancelled).
    /// </summary>
    Task LSeekerStart(string word, LReceiver receiver, CancellationToken cancellation);
}
