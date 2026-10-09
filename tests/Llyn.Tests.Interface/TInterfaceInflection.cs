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
        IReadOnlyList<IReadOnlyList<long>> cells,
        IReadOnlyList<LParadigmRule>? regular = null,
        IReadOnlyList<long>? except = null) =>
        new(speechCode, cells, regular, except);

    internal static LParadigmTable TParadigmTableCreate(
        IReadOnlyList<string> headers, IReadOnlyList<LParadigmLine> lines) =>
        new(headers, lines);

    internal static LParadigmLine TParadigmLineCreate(string group, string label, IReadOnlyList<LParadigmForm> forms) =>
        new(group, label, forms);

    internal static LParadigmForm TParadigmFormCreate(
        string text, IReadOnlyList<LInflectionMark> marks, LParadigmStatus status) =>
        new(text, marks, status);

    internal static LInflectionMark TInflectionMarkCreate(int offset, int length) =>
        new(offset, length);

    internal static string TInflectionMarkFormat(IReadOnlyList<LInflectionMark> marks) =>
        LInflectionMark.LInflectionMarkFormat(marks);

    internal static IReadOnlyList<LInflectionMark> TInflectionMarkParse(string text) =>
        LInflectionMark.LInflectionMarkParse(text);

    internal static IReadOnlyList<LInflectionMark> TInflectionDifferenceScan(
        IReadOnlyList<LInflectionRule> folds, string predicted, string actual) =>
        LInflectionDifference.LInflectionDifferenceScan(folds, predicted, actual);

    internal static int TInflectionDifferenceDivide(
        IReadOnlyList<LInflectionRule> folds, string predicted, int root, string actual) =>
        LInflectionDifference.LInflectionDifferenceDivide(folds, predicted, root, actual);

    internal static string? TInflectionBookResolve(LInflectionBook book, string headword, IReadOnlyList<long> codes) =>
        book.LInflectionBookResolve(headword, codes);

    internal static string? TInflectionBookDivide(
        LInflectionBook book, string headword, IReadOnlyList<long> codes, out int? root) =>
        book.LInflectionBookDivide(headword, codes, out root);

    internal static LInflectionBook TInflectionBookCreate(
        IReadOnlyList<LInflectionKind> kinds,
        IReadOnlyList<LInflectionStem> stems,
        IReadOnlyList<LInflectionRule> rules,
        IReadOnlyList<LInflectionRule> folds,
        LInflectionLayout? layout,
        string stamp) =>
        new(kinds, stems, rules, folds, layout, stamp);

    internal static LInflectionStem TInflectionStemCreate(
        IReadOnlyList<long> values,
        IReadOnlyDictionary<string, string> templates,
        IReadOnlyList<LInflectionEnding> endings) =>
        new(values, templates, endings);

    internal static LInflectionEnding TInflectionEndingCreate(IReadOnlyList<long> values, string text) =>
        new(values, text);

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
