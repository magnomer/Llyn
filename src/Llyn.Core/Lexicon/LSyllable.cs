namespace Llyn.Core;

/// <summary>
/// One syllable of a pronunciation, ordered within it. Identity is <c>(pronunciation_id, position)</c>:
/// the syllable is subordinate to its <see cref="LSyllablePronunciationId"/> parent, and reordering
/// changes <see cref="LSyllablePosition"/> only. Every field but <see cref="LSyllableNucleus"/> is
/// optional; an absent field is stored as NULL, which is distinct from an empty string.
/// </summary>
/// <param name="LSyllablePronunciationId">Parent pronunciation id.</param>
/// <param name="LSyllablePosition">Order within the parent pronunciation.</param>
/// <param name="LSyllableOrthography">Optional written form of the syllable.</param>
/// <param name="LSyllableLocal">Optional local representation of the syllable.</param>
/// <param name="LSyllableOnset">Optional onset segment.</param>
/// <param name="LSyllableMedial">Optional medial segment.</param>
/// <param name="LSyllableNucleus">The nucleus segment; the only required field.</param>
/// <param name="LSyllableCoda">Optional coda segment.</param>
/// <param name="LSyllableToneNumber">Optional tone number.</param>
/// <param name="LSyllableToneLocal">Optional local tone notation.</param>
/// <param name="LSyllableTonePoints">Optional tone-contour points; free-form text (for example <c>"214"</c>).</param>
public sealed record LSyllable(
    string LSyllablePronunciationId,
    int LSyllablePosition,
    string? LSyllableOrthography,
    string? LSyllableLocal,
    string? LSyllableOnset,
    string? LSyllableMedial,
    string LSyllableNucleus,
    string? LSyllableCoda,
    int? LSyllableToneNumber,
    string? LSyllableToneLocal,
    string? LSyllableTonePoints);
