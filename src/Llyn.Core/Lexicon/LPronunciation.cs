using System.Collections.Generic;

namespace Llyn.Core;

/// <summary>
/// The single pronunciation an entry owns: at most one per entry, with no order and no
/// primary/default distinction. <see cref="LPronunciationId"/> is the identity — an opaque,
/// program-generated stable id — and <see cref="LPronunciationEntryId"/> names the owning entry,
/// unique across pronunciations so an entry never carries two. It carries an optional transcription
/// <see cref="LPronunciationIpa"/> and <see cref="LPronunciationLevel"/>, and owns its ordered
/// <see cref="LPronunciationSyllables"/> and <see cref="LPronunciationRepresentations"/>.
/// </summary>
/// <param name="LPronunciationId">Opaque, program-generated stable id.</param>
/// <param name="LPronunciationEntryId">Owning entry id; unique — one pronunciation per entry.</param>
/// <param name="LPronunciationLevel">Optional transcription level (for example phonemic or phonetic).</param>
/// <param name="LPronunciationIpa">Optional IPA transcription of the whole pronunciation.</param>
/// <param name="LPronunciationSyllables">Ordered syllables owned by this pronunciation.</param>
/// <param name="LPronunciationRepresentations">Ordered notational representations owned by this pronunciation.</param>
public sealed record LPronunciation(
    string LPronunciationId,
    string LPronunciationEntryId,
    string? LPronunciationLevel,
    string? LPronunciationIpa,
    IReadOnlyList<LSyllable> LPronunciationSyllables,
    IReadOnlyList<LRepresentation> LPronunciationRepresentations);
