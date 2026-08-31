using System.Collections.Generic;

namespace Llyn.Core;

/// <summary>
/// One inflected form of an entry, ordered within it. Identity is <c>(entry_id, position)</c>: the
/// inflection is subordinate to its <see cref="LInflectionEntryId"/> parent, and reordering changes
/// <see cref="LInflectionPosition"/> only. It carries its own grammatical features in order
/// (<see cref="LInflectionFeatures"/>) and stores only the stable part-of-speech id
/// (<see cref="LInflectionSpeechId"/>), never a display name.
/// </summary>
/// <param name="LInflectionEntryId">Parent entry id.</param>
/// <param name="LInflectionPosition">Order within the parent entry.</param>
/// <param name="LInflectionText">The inflected form.</param>
/// <param name="LInflectionLocal">Optional local representation.</param>
/// <param name="LInflectionSpeechId">Optional stable part-of-speech id; <c>null</c> when unspecified.</param>
/// <param name="LInflectionFeatures">Ordered grammatical features carried by the inflection.</param>
public sealed record LInflection(
    string LInflectionEntryId,
    int LInflectionPosition,
    string LInflectionText,
    string? LInflectionLocal,
    string? LInflectionSpeechId,
    IReadOnlyList<LFeature> LInflectionFeatures);
