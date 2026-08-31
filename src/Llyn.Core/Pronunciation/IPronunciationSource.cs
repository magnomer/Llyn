using System.Threading;
using System.Threading.Tasks;

namespace Llyn.Core;

/// <summary>A single verified pronunciation source, such as Wikipedia or Cambridge.</summary>
public interface IPronunciationSource
{
    /// <summary>The source this provider represents.</summary>
    LookupSource Source { get; }

    /// <summary>
    /// Finds a pronunciation for <paramref name="word"/>, returning <c>null</c> when the source has
    /// none. Implementations must not throw for an ordinary "not found" or network failure; they
    /// return <c>null</c> so one failing source never fails the whole lookup.
    /// </summary>
    Task<PronunciationCandidate?> FindAsync(string word, CancellationToken cancellation);
}
