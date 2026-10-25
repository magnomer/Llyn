using Llyn.Conduct;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TLecternIncoming
{
    [Fact]
    public void LecternIncomingDraw_EntryLinkedFrom_ShowsTheRowAndEnterOpensTheSource()
    {
        TLecternCard.TLecternCardRun(
            (engine, save) =>
            {
                long apple = save("apple", [TInterface.TCardCreate("a round fruit", 1)]);
                _ = save(
                    "zebra", [TInterface.TCardCreate("a striped horse", 1) with { LCardDraftTranslation = [apple] }]);
            },
            (send, rows, lit, _, _) =>
            {
                IReadOnlyList<string> shown = rows();
                Assert.Contains("Links here", shown);
                Assert.Contains(shown, static row => row.StartsWith("← zebra", StringComparison.Ordinal));

                for (int step = 0; step < 10 && lit()?.Item1.StartsWith("←", StringComparison.Ordinal) != true; step++)
                {
                    send(ConsoleKey.DownArrow);
                }

                Assert.Equal("lit", lit()?.Item2);
                Assert.StartsWith("← zebra", lit()?.Item1, StringComparison.Ordinal);

                send(ConsoleKey.Enter);

                Assert.StartsWith("zebra", rows()[0], StringComparison.Ordinal);
            });
    }

    [Fact]
    public void LecternIncomingDraw_NoUsage_DrawsNoHeading()
    {
        TLecternCard.TLecternCardRun(
            (_, save) => save("apple", [TInterface.TCardCreate("a round fruit", 1)]),
            (_, rows, _, _, _) => Assert.DoesNotContain("Links here", rows()));
    }

    [Fact]
    public void LecternCardDraw_EmptyPictureAndVideo_DrawNoRow()
    {
        TLecternCard.TLecternCardRun(
            static (_, save) => save(
                "apple",
                [
                    TInterface.TCardCreate("a round fruit", 1) with
                    {
                        LCardDraftImage = [new LImageDraft(string.Empty)],
                        LCardDraftVideo =
                            [new LVideoDraft(LStateValue.LStateValueUnspecified, LStateValue.LStateValueUnspecified)],
                    },
                ]),
            static (_, rows, _, _, _) =>
            {
                IReadOnlyList<string> shown = rows();

                Assert.Contains("a round fruit", shown);
                Assert.DoesNotContain(shown, static row => row.StartsWith('▣') || row.StartsWith('▶'));
            });
    }

    [Fact]
    public void LecternCardDraw_Video_ShowsTheSpanAndMissing()
    {
        TLecternCard.TLecternCardRun(
            static (_, save) => save(
                "apple",
                [
                    TInterface.TCardCreate("a round fruit", 1) with
                    {
                        LCardDraftVideo =
                        [
                            new LVideoDraft(
                                TInterfaceState.TStateValueCreate("C:/nowhere/clip.mp4"),
                                TInterfaceState.TStateValueCreate("0:05-0:10")),
                        ],
                    },
                ]),
            static (_, rows, _, _, console) =>
            {
                Assert.Contains("▶ clip.mp4 (missing)  0:05-0:10", rows());
                Assert.Equal("muted", TLectern.TLecternRoleRead(console, "▶ clip.mp4"));
            });
    }

    [Fact]
    public void LecternSeeObserve_EnterOnSee_LightsTheTarget()
    {
        TLecternCard.TLecternCardRun(
            static (engine, save) =>
            {
                LCardDraft first = TInterface.TCardCreate("to place firmly", 1) with
                {
                    LCardDraftId = TInterface.TIdentityCreate(),
                };
                LCardDraft other = TInterface.TCardCreate("to harden", 2) with
                {
                    LCardDraftId = TInterface.TIdentityCreate(),
                };
                LCardDraft linked = TInterface.TCardDraftCreate(
                    LStateValue.LStateValueUnspecified,
                    TInterfaceState.TStateValueCreate("set off"),
                    LStateValue.LStateValueUnspecified,
                    [], [], [], [], [], 1) with
                {
                    LCardDraftSense = [first.LCardDraftId, other.LCardDraftId],
                };
                _ = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
                    "set", "English", string.Empty, string.Empty, [first, other], [linked]));
            },
            static (send, _, lit, _, _) =>
            {
                for (int step = 0;
                     step < 20 && lit()?.Item1.StartsWith("→ set", StringComparison.Ordinal) != true;
                     step++)
                {
                    send(ConsoleKey.DownArrow);
                }

                Assert.Equal(("→ set off see 1", "lit"), lit());

                send(ConsoleKey.Enter);

                Assert.Equal(("▾ set off", "lit"), lit());
            });
    }

    [Fact]
    public void LecternChipObserve_EnterOnTagAndSituation_KeepsTheEntryWithoutTheirTab()
    {
        foreach (string chip in (string[])["#nature", "[at home]"])
        {
            TLecternCard.TLecternCardRun(
                static (_, save) => save(
                    "apple",
                    [
                        TInterface.TCardCreate("a round fruit", 1) with
                        {
                            LCardDraftSituation = [new LSituationDraft("at home", 0, "home", "place")],
                            LCardDraftTag = [LTagDraft.LTagDraftCreate("nature")],
                        },
                    ]),
                (send, rows, lit, atelier, _) =>
                {
                    List<string?> tabs = [];
                    atelier.CAtelierNavigation.CNavigationChanged += state => tabs.Add(state.CNavigationStateTab);
                    for (int step = 0; step < 20 && lit()?.Item1 != chip; step++)
                    {
                        send(ConsoleKey.DownArrow);
                    }

                    Assert.Equal((chip, "lit"), lit());

                    send(ConsoleKey.Enter);

                    Assert.Empty(tabs);
                    Assert.StartsWith("apple", rows()[0], StringComparison.Ordinal);
                    Assert.Equal((chip, "lit"), lit());
                });
        }
    }
}
