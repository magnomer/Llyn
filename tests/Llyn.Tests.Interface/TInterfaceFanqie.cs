using Llyn.Application;
using Llyn.Core;

namespace Llyn.Tests;

internal static class TInterfaceFanqie
{
    internal static IReadOnlyList<LAnchorRow> TAnchorRowScan(
        IReadOnlyList<LFanqieRow> rows, IReadOnlyList<long> anchors, IReadOnlyList<string> classes) =>
        LAnchor.LAnchorRowScan(rows, anchors, classes);

    internal static string TAnchorTextFormat(
        IReadOnlyList<LFanqieRow> rows, IReadOnlyList<long> anchors, string headword) =>
        LAnchor.LAnchorTextFormat(rows, anchors, headword, " · ");

    internal static IReadOnlyList<LAnchorRow> TReflexAnchorScan(
        IReadOnlyList<LFanqieRow> rows,
        IReadOnlyList<long> anchors,
        IReadOnlyList<LDescent> tones,
        string reflex,
        string tone) =>
        LReflexClerk.LReflexAnchorScan(rows, anchors, tones, reflex, tone);

    internal static string TDiweiRimeFormat(string rime, string division, bool rounded) =>
        LDiwei.LDiweiRimeFormat(rime, division, rounded);

    internal static string TDiweiChongniuRead(string rime) =>
        LDiwei.LDiweiChongniuRead(rime);

    internal static int TDiweiRankRead(string kind, string heading, LHypothesis? hypothesis) =>
        LDiwei.LDiweiRankRead(kind, heading, hypothesis);

    internal static int TDiweiRankNormalize(int rank) =>
        LDiwei.LDiweiRankNormalize(rank);

    internal static LDiweiSection TDiweiSectionScan(bool respelled, Func<string, string?> localize) =>
        LDiweiSection.LDiweiSectionScan(
            LDiwei.LDiweiInitial,
            [new LFanqieRow("爛", "book", 0, "text", "來", "寒", "寒", "一", "平")],
            null,
            [
                new LTally(
                    "一",
                    [
                        new LTallyLine(
                            "Cantonese", "literary", [new LTallyMark("l", ["爛"])], [new LTallyMark("L", ["蘭"])]),
                        new LTallyLine("Korean", string.Empty, [new LTallyMark("r", ["爛"])], []),
                    ]),
            ],
            true,
            respelled,
            localize)[0];

    internal static IReadOnlyList<LDiweiSection> TDiweiSectionScan(IReadOnlyList<LFanqieRow> rows) =>
        LDiweiSection.LDiweiSectionScan(LDiwei.LDiweiInitial, rows, null, [], false, false, static _ => null);

    internal static LHypothesis THypothesisCreate(
        IReadOnlyDictionary<string, string> initials,
        IReadOnlyDictionary<string, string> finals,
        IReadOnlyDictionary<string, IReadOnlyList<LHypothesisTone>> tones,
        IReadOnlyList<LHypothesisLocus>? places = null) =>
        new(initials, finals, tones, places);

    internal static LHypothesisLocus THypothesisPlaceCreate(string name, IReadOnlyList<string> initials) =>
        new(name, initials);

    internal static string? THypothesisInitialFind(this LHypothesis hypothesis, LFanqieRow row) =>
        hypothesis.LHypothesisInitialFind(row);

    internal static LHypothesisLocus? THypothesisPlaceFind(this LHypothesis hypothesis, string initial) =>
        hypothesis.LHypothesisLocusFind(initial);

    internal static int THypothesisRankRead(this LHypothesis hypothesis, string initial) =>
        hypothesis.LHypothesisRankRead(initial);

    internal static LHypothesisTone THypothesisToneCreate(
        string onset, IReadOnlyList<LRespellingRule> rules, string label) =>
        new(onset, rules, label);

    internal static LHypothesisSound? THypothesisResolve(this LHypothesis hypothesis, LFanqieRow row) =>
        hypothesis.LHypothesisResolve(row);

    internal static IReadOnlyList<LTallyMark> TTallyMarkScan(IReadOnlyDictionary<string, List<string>> parts) =>
        LTallyMark.LTallyMarkScan(parts);
}
