using System.Threading;
using System.Threading.Tasks;

namespace Llyn.Core;

/// <summary>
/// A single pronunciation or audio source. The provider is language-agnostic: what it searches and
/// how it extracts a value are supplied by a language pack (see <see cref="LSourceSpec"/>), never
/// hardcoded here. It returns the bare extracted value — a phonetic form for a pronunciation
/// source, an audio URL for an audio source — as a string, or <c>null</c> when the source has none.
/// </summary>
public interface LSource
{
    /// <summary>The source's declared name, for example <c>"Cambridge"</c>.</summary>
    string LSourceName { get; }

    /// <summary>The kind of value this source yields: <c>"pronunciation"</c> or <c>"audio"</c>.</summary>
    string LSourceKind { get; }

    /// <summary>
    /// Finds a value for <paramref name="word"/>, returning <c>null</c> when the source has none.
    /// Implementations must not throw for an ordinary "not found" or network failure; they return
    /// <c>null</c> so one failing source never fails the whole search.
    /// </summary>
    Task<string?> LSourceFind(string word, CancellationToken cancellation);
}
