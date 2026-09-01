using System.Collections.Generic;

namespace Llyn.Core;

/// <summary>
/// The whole input form as one immutable value. The shell reads the visual tree once, builds this, and
/// hands it to the engine; the engine never learns that controls exist and the save never walks the
/// tree. Both card lists are lists of the one card shape, and arrive in the order they are shown:
/// a card's position is its place in its list, not a value it carries.
/// </summary>
/// <param name="LEntryDraftHeadword">Headword text as typed.</param>
/// <param name="LEntryDraftLanguage">Language chosen in the language selector.</param>
/// <param name="LEntryDraftPronunciation">Pronunciation text as typed or filled by a lookup.</param>
/// <param name="LEntryDraftNote">Plain text of the note editor.</param>
/// <param name="LEntryDraftSenses">Meaning cards in list order; their Expression is always empty.</param>
/// <param name="LEntryDraftCollocations">Collocation cards in list order.</param>
/// <param name="LEntryDraftAudio">
/// Full path to the recording downloaded for this entry, or empty when none was chosen. The shell
/// deals in full paths; the engine stores the path relative to the workspace and resolves it back on
/// the way out, so the draft is the same value in both directions.
/// </param>
/// <param name="LEntryDraftSource">Label of the source the recording came from, when there is one.</param>
public sealed record LEntryDraft(
    string LEntryDraftHeadword,
    string LEntryDraftLanguage,
    string LEntryDraftPronunciation,
    string LEntryDraftNote,
    IReadOnlyList<LCardDraft> LEntryDraftSenses,
    IReadOnlyList<LCardDraft> LEntryDraftCollocations,
    string LEntryDraftAudio = "",
    string? LEntryDraftSource = null);
