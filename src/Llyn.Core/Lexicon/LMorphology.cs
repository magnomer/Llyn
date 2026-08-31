namespace Llyn.Core;

/// <summary>
/// One row of the language-controlled morphology display vocabulary: it maps a stable
/// <c>(part_of_speech id, feature id, value id)</c> triple to the feature and value display names shown
/// for a given language. Inflections store only the ids (<see cref="LFeature"/>); the names live here
/// and are resolved by <c>(language, part_of_speech_id, feature_id, value_id)</c>, never copied onto an
/// entry's lexical rows.
/// </summary>
/// <param name="LMorphologyLanguage">Language the display names are governed by.</param>
/// <param name="LMorphologySpeechId">Stable part-of-speech id the feature applies to.</param>
/// <param name="LMorphologyFeatureId">Stable grammatical feature id (for example <c>number</c>).</param>
/// <param name="LMorphologyFeatureName">Feature display name for the language (for example <c>Number</c>).</param>
/// <param name="LMorphologyValueId">Stable grammatical value id (for example <c>plural</c>).</param>
/// <param name="LMorphologyValueName">Value display name for the language (for example <c>Plural</c>).</param>
/// <param name="LMorphologyPosition">Display order within the language's vocabulary.</param>
public sealed record LMorphology(
    string LMorphologyLanguage,
    string LMorphologySpeechId,
    string LMorphologyFeatureId,
    string LMorphologyFeatureName,
    string LMorphologyValueId,
    string LMorphologyValueName,
    int LMorphologyPosition);
