namespace Llyn.Core;

/// <summary>
/// One grammatical feature carried by an inflection, ordered within it. It stores only stable
/// grammatical ids — a feature id and its value id — never display names; the names are resolved from
/// the language-controlled morphology vocabulary (<see cref="LMorphology"/>). The feature's position
/// and its parent inflection are given by its order within <see cref="LInflection.LInflectionFeatures"/>.
/// </summary>
/// <param name="LFeatureId">Stable grammatical feature id (for example <c>number</c>).</param>
/// <param name="LFeatureValueId">Stable grammatical value id (for example <c>plural</c>).</param>
public sealed record LFeature(
    string LFeatureId,
    string LFeatureValueId);
