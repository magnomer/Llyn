using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TLecternEtymology
{
    [Fact]
    public void LecternEtymologyDraw_Map_DrawsTheHeadwordOnTopAndTheSourcesAsATree()
    {
        TLecternCard.TLecternCardRun(
            static (engine, _) => TLecternEtymologySave(engine, "flow"),
            static (_, rows, _, _, _) =>
            {
                List<string> shown = [.. rows()];
                int heading = shown.IndexOf("Etymology");

                Assert.True(heading >= 0);
                Assert.Equal(
                    [
                        "brook",
                        "├─ Old English  brōc  flow  Derived",
                        "│  └─ Proto-Germanic  *brōkaz",
                        "└─ Old English  wæter",
                        "└─ Latin  *wódr̥  (borrowed)",
                    ],
                    shown.Skip(heading + 1).Take(5));
            });
    }

    [Fact]
    public void LecternEtymologyOpen_EnterOnTheLinkedBox_OpensItsEntry()
    {
        TLecternCard.TLecternCardRun(
            static (engine, _) => TLecternEtymologySave(engine, "stream"),
            static (send, rows, lit, _, _) =>
            {
                for (int step = 0; step < 20 && lit()?.Item1 != "wæter"; step++)
                {
                    send(ConsoleKey.DownArrow);
                }

                Assert.Equal(("wæter", "lit"), lit());

                send(ConsoleKey.Enter);

                Assert.StartsWith("wæter", rows()[0], StringComparison.Ordinal);
            });
    }

    [Fact]
    public void LecternEtymologyDraw_LongGloss_WrapsTheTextAndKeepsTheTrunk()
    {
        const string gloss = "a small natural stream of fresh water that runs through the low meadows to the sea";
        TLecternCard.TLecternCardRun(
            static (engine, _) => TLecternEtymologySave(engine, gloss),
            static (_, rows, _, _, _) =>
            {
                List<string> shown = [.. rows()];
                int box = shown.FindIndex(static row =>
                    row.StartsWith("├─ Old English  brōc", StringComparison.Ordinal));

                Assert.True(box >= 0);
                Assert.DoesNotContain(shown, static row => row.EndsWith(gloss, StringComparison.Ordinal));
                Assert.StartsWith("│    ", shown[box + 1], StringComparison.Ordinal);
                Assert.NotEqual(' ', shown[box + 1][5]);
                Assert.Contains(shown.Skip(box + 1).TakeWhile(static row => !row.Contains('└')), static row =>
                    row.EndsWith("Derived", StringComparison.Ordinal));
                Assert.Contains("│  └─ Proto-Germanic  *brōkaz", shown);
            });
    }

    [Fact]
    public void LecternEtymologyDraw_NoMap_DrawsNoHeading()
    {
        TLecternCard.TLecternCardRun(
            static (_, save) => save("brook", [TInterface.TCardCreate("a small stream", 1)]),
            static (_, rows, _, _, _) => Assert.DoesNotContain("Etymology", rows()));
    }

    [Fact]
    public void LecternDraw_PageOrder_FollowsTheCompass()
    {
        Dictionary<CCompassPart, string> headings = new()
        {
            [CCompassPart.CCompassPartMeaning] = "Meanings",
            [CCompassPart.CCompassPartIncoming] = "Links here",
            [CCompassPart.CCompassPartEtymology] = "Etymology",
            [CCompassPart.CCompassPartNote] = "Note",
        };
        TLecternCard.TLecternCardRun(
            static (engine, save) =>
            {
                long brook = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
                    "brook", "English", string.Empty, "Last words.", [TInterface.TCardCreate("a small stream", 1)], [])
                    with
                    {
                        LEntryDraftAncestors = [TInterface.TAncestorDraftCreate(-1, null, null, "brōc", "Old English")],
                    }).LEntryId;
                _ = save(
                    "zebra", [TInterface.TCardCreate("a striped horse", 1) with { LCardDraftTranslation = [brook] }]);
            },
            (_, rows, _, _, _) =>
            {
                List<string> shown = [.. rows()];

                Assert.Equal(
                    CCompass.CCompassOrder.Where(headings.ContainsKey).Select(part => headings[part]),
                    shown.Where(headings.ContainsValue));
                Assert.True(
                    shown.FindIndex(static row => row.StartsWith("Created", StringComparison.Ordinal))
                    > shown.IndexOf("Note"));
            },
            height: 80);
    }

    private static void TLecternEtymologySave(LEngine engine, string gloss)
    {
        LEntry source = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "wæter", "Old English", string.Empty, string.Empty, [TInterface.TCardCreate("water", 1)], []) with
        {
            LEntryDraftAncestors = [TInterface.TAncestorDraftCreate(-1, null, null, "*wódr̥", "Latin")],
        });
        long derived = engine.TEngineRelationRead().First(static relation =>
            relation.LAncestorRelationName == "derived").LAncestorRelationId;
        _ = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "brook", "English", string.Empty, string.Empty, [TInterface.TCardCreate("a small stream", 1)], []) with
        {
            LEntryDraftAncestors =
            [
                TInterface.TAncestorDraftCreate(-1, null, null, "brōc", "Old English", null, gloss) with
                {
                    LAncestorDraftRelation = derived,
                },
                TInterface.TAncestorDraftCreate(-2, -1, null, "*brōkaz", "Proto-Germanic"),
                TInterface.TAncestorDraftCreate(-3, null, source.LEntryId, string.Empty),
            ],
        });
    }
}
