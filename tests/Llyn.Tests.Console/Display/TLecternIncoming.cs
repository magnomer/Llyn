using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
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
    public void LecternChipObserve_StoredTagWithoutConsolePanel_KeepsTheEntry()
    {
        TLecternCard.TLecternCardRun(
            static (_, save) => save(
                "apple",
                [
                    TInterface.TCardCreate("a round fruit", 1) with
                    {
                        LCardDraftTag = [LTagDraft.LTagDraftCreate("nature")],
                    },
                ]),
            static (send, rows, lit, atelier, _) =>
            {
                List<string?> tabs = [];
                atelier.CAtelierNavigation.CNavigationChanged += state => tabs.Add(state.CNavigationStateTab);
                TLecternIncomingSeek(send, lit, "#nature");

                send(ConsoleKey.Enter);

                Assert.Empty(tabs);
                Assert.StartsWith("apple", rows()[0], StringComparison.Ordinal);
                Assert.Equal(("#nature", "lit"), lit());
            });
    }

    [Fact]
    public void LecternChipObserve_StoredTagWithTaxonomy_OpensTheTabAndAltLeftComesBack()
    {
        TLecternCard.TLecternCardRun(
            static (_, save) => save(
                "apple",
                [
                    TInterface.TCardCreate("a round fruit", 1) with
                    {
                        LCardDraftTag = [LTagDraft.LTagDraftCreate("nature")],
                    },
                ]),
            static (send, rows, lit, atelier, console) =>
            {
                List<string> asked = [];
                _ = CTaxonomy.CTaxonomyCreate(
                    atelier, static () => true, TEnvoyFake.TEnvoyCreate(true, asked), static run => run());
                List<string?> tabs = [];
                atelier.CAtelierNavigation.CNavigationChanged += state => tabs.Add(state.CNavigationStateTab);
                TLecternIncomingSeek(send, lit, "#nature");

                send(ConsoleKey.Enter);

                Assert.Equal("Taxonomy", tabs[^1]);
                Assert.Empty(asked);

                console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.LeftArrow, false, true, false));

                Assert.True(SpinWait.SpinUntil(
                    () => rows().Count > 0 && tabs[^1] == "Library", TimeSpan.FromSeconds(10)));
                Assert.StartsWith("apple", rows()[0], StringComparison.Ordinal);
            });
    }

    [Fact]
    public void LecternChipOpen_TranslationThenAltLeft_ComesBackToTheEntry()
    {
        TLecternCard.TLecternCardRun(
            static (engine, save) =>
            {
                long zebra = save("zebra", []);
                _ = save(
                    "apple", [TInterface.TCardCreate("a round fruit", 1) with { LCardDraftTranslation = [zebra] }]);
            },
            static (send, rows, lit, atelier, console) =>
            {
                TLecternIncomingSeek(send, lit, "→ zebra English");
                send(ConsoleKey.Enter);
                Assert.StartsWith("zebra", rows()[0], StringComparison.Ordinal);

                console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.LeftArrow, false, true, false));

                Assert.True(SpinWait.SpinUntil(
                    () => rows()[0].StartsWith("apple", StringComparison.Ordinal), TimeSpan.FromSeconds(10)));
            });
    }

    [Fact]
    public void LecternCardDraw_UnstoredSituation_IsNoItem()
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
            static (send, rows, lit, _, _) =>
            {
                Assert.Contains("[at home]", rows());
                List<string?> seen = [];
                for (int step = 0; step < 20; step++)
                {
                    seen.Add(lit()?.Item1);
                    send(ConsoleKey.DownArrow);
                }

                Assert.Contains("#nature", seen);
                Assert.DoesNotContain("[at home]", seen);
            },
            build: static engine =>
            {
                LVistaPort stored = engine.LEngineVista;
                Func<LDraft?, LDraft?> strip = loaded => loaded is null ? null : loaded with
                {
                    LDraftContent = loaded.LDraftContent with
                    {
                        LEntryDraftMeanings =
                        [
                            .. loaded.LDraftContent.LEntryDraftMeanings.Select(static card => card with
                            {
                                LCardDraftSituation =
                                [
                                    .. card.LCardDraftSituation.Select(
                                        static situation => situation with { LSituationDraftId = 0 }),
                                ],
                            }),
                        ],
                    },
                };
                return TInterfaceConduct.TAtelierCreate(
                    engine,
                    TEngineFake.TEngineCreate<LVistaPort>(new Dictionary<string, Func<object?[]?, object?>>
                    {
                        ["LEngineEntryFind"] = args => args!.Length == 1
                            ? stored.LEngineEntryFind((LVista)args[0]!)
                            : stored.LEngineEntryFind((LVista?)args[0], (LVista?)args[1]),
                        ["LEngineNameResolve"] = args => stored.LEngineNameResolve((IReadOnlyList<string>)args![0]!),
                        ["LEngineVistaLoad"] = args => strip(args!.Length == 1
                            ? stored.LEngineVistaLoad((LVista)args[0]!)
                            : stored.LEngineVistaLoad((LVista)args[0]!, (long?)args[1])),
                        ["LEngineFileRead"] = args => stored.LEngineFileRead((LVista?)args![0]),
                        ["LEngineSideSave"] = args =>
                        {
                            stored.LEngineSideSave((LVista)args![0]!);
                            return null;
                        },
                        ["LEngineUsageRead"] = args => stored.LEngineUsageRead((LVista)args![0]!),
                        ["LEngineTallyRead"] = args => stored.LEngineTallyRead((LVista)args![0]!),
                        ["LEngineVistaDelete"] = args => stored.LEngineVistaDelete((LVista)args![0]!),
                    }));
            });
    }

    private static void TLecternIncomingSeek(Action<ConsoleKey> send, Func<(string, string)?> lit, string item)
    {
        for (int step = 0; step < 20 && lit()?.Item1 != item; step++)
        {
            send(ConsoleKey.DownArrow);
        }

        Assert.Equal((item, "lit"), lit());
    }
}
