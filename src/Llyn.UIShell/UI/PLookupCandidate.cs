using Llyn.Core;

namespace Llyn.UIShell;

/// <summary>
/// Presentation item for one pronunciation candidate shown in <c>PLookupMenu</c>. Wraps the domain
/// <see cref="PronunciationCandidate"/> with the localized source label the menu row displays.
/// </summary>
internal sealed class PLookupCandidate
{
    internal PLookupCandidate(PronunciationCandidate candidate, string sourceLabel)
    {
        SourceLabel = sourceLabel;
        Phonetic = candidate.Phonetic;
    }

    public string SourceLabel { get; }

    public string Phonetic { get; }
}
