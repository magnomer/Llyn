using System.Threading;
using System.Threading.Tasks;

namespace Llyn.Core;

/// <summary>
/// Pronunciation lookup entry point. A requester starts a lookup and subscribes to its results
/// through an <see cref="IPronunciationReceiver"/>. All lookup logic lives behind this contract,
/// never in the UI.
/// </summary>
public interface IPronunciationLookup
{
    /// <summary>
    /// Starts a lookup for <paramref name="word"/>, streaming results to <paramref name="receiver"/>.
    /// The returned task completes when every source has finished (or the lookup is cancelled).
    /// </summary>
    Task StartAsync(string word, IPronunciationReceiver receiver, CancellationToken cancellation);
}
