using Llyn.Core;

namespace Llyn.Tests;

internal static class TInterfaceInflection
{
    internal static LForm TFormCreate(
        long entryId,
        int position,
        string text,
        string? local,
        string role) =>
        new(entryId, position, text, local, role);

    internal static LInflection TInflectionCreate(
        long entryId,
        int position,
        string text,
        string? local,
        long? speechValueId,
        IReadOnlyList<long> morphology) =>
        new(0, entryId, position, text, local, speechValueId, morphology);

    internal static LParadigm TParadigmCreate(
        long speechCode,
        IReadOnlyList<long> morphology,
        IReadOnlyList<LParadigmRule>? regular = null,
        IReadOnlyList<long>? except = null) =>
        new(speechCode, morphology, regular, except);

    internal static string? TParadigmRuleResolve(LParadigmRule rule, string headword) =>
        rule.LParadigmRuleResolve(headword);

    internal static LFeature TFeatureCreate(
        long speechValueId,
        long packId,
        string name,
        int position) =>
        new(0, speechValueId, packId, name, position);

    internal static LMorphology TMorphologyCreate(
        long featureId,
        long packId,
        string name,
        int position) =>
        new(0, featureId, packId, name, position);
}
