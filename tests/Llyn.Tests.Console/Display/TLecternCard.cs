using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDemeanor;
using Llyn.UIDemeanor.Capsule;
using Llyn.UITerminal;
using Xunit;

namespace Llyn.Tests;

public sealed class TLecternCard
{
    [Fact]
    public void LecternCardDraw_StoredMeaning_ShowsHeaderBodyAndMissingPicture()
    {
        TLecternCardRun(
            (engine, save) =>
            {
                long zebra = save("zebra", []);
                _ = save(
                    "apple",
                    [
                        new LCardDraft(
                            "fruit",
                            LStateValue.LStateValueUnspecified,
                            "a round fruit",
                            [
                                new LSentenceDraft(
                                    TInterfaceExample.TExampleDraftCreate("Apples are sweet."), "with", "of"),
                            ],
                            [new LSituationDraft("at home", 0, "home", "place")],
                            [TInterface.TRegisterDraftCreate("everyday")],
                            [zebra],
                            [LTagDraft.LTagDraftCreate("nature")],
                            [new LImageDraft("C:/nowhere/cat.png")],
                            [],
                            1),
                    ]);
            },
            (_, rows, _, _, _) =>
            {
                IReadOnlyList<string> shown = rows();

                Assert.Contains("Meanings", shown);
                Assert.Contains("1 ▾ fruit", shown);
                Assert.Contains("a round fruit", shown);
                Assert.Contains("[at home]  {everyday}", shown);
                Assert.Contains("→ zebra English", shown);
                Assert.Contains(shown, static row => row.StartsWith("· ",StringComparison.Ordinal)
                    && row.Contains("Apples are sweet.", StringComparison.Ordinal));
                Assert.Contains("#nature", shown);
                Assert.Contains("▣ cat.png (missing)", shown);
            });
    }

    [Fact]
    public void LecternFoldObserve_SpaceThenEnterOnHeader_FoldsToThePeekAndOpens()
    {
        TLecternCardRun(
            (engine, save) => save("apple", [TInterface.TCardCreate("a round fruit", 1)]),
            (send, rows, lit, _, _) =>
            {
                send(ConsoleKey.DownArrow);
                send(ConsoleKey.DownArrow);
                send(ConsoleKey.DownArrow);
                Assert.Equal(("▾ Meaning", "lit"), lit());

                send(ConsoleKey.Spacebar);

                Assert.Contains("1 ▸ a round fruit", rows());
                Assert.DoesNotContain("a round fruit", rows());
                Assert.Equal(("▸ a round fruit", "lit"), lit());

                send(ConsoleKey.Enter);

                Assert.Contains("1 ▾ Meaning", rows());
                Assert.Contains("a round fruit", rows());
            });
    }

    [Fact]
    public void LecternFoldObserve_FoldRefused_KeepsTheMarkAndNotices()
    {
        TLecternCardRun(
            (engine, save) => save("apple", [TInterface.TCardCreate("a round fruit", 1)]),
            (send, rows, lit, _, console) =>
            {
                send(ConsoleKey.DownArrow);
                send(ConsoleKey.DownArrow);
                send(ConsoleKey.DownArrow);

                send(ConsoleKey.Spacebar);

                Assert.Equal(1, TLectern.TLecternNoticeRead(console, "The card fold could not be written."));
                Assert.Contains("1 ▾ Meaning", rows());
                Assert.Equal(("▾ Meaning", "lit"), lit());
            },
            "ALTER TABLE sense_fold RENAME TO sense_fold_gone;",
            notice: "could not be");
    }

    [Fact]
    public void LecternSeeObserve_FoldRefused_KeepsTheSeeLit()
    {
        TLecternCardRun(
            (engine, save) =>
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
            (send, _, lit, _, console) =>
            {
                for (int step = 0; step < 20 && lit()?.Item1.StartsWith('→') != true; step++)
                {
                    send(ConsoleKey.DownArrow);
                }

                Assert.Equal(("→ set off see 1", "lit"), lit());

                send(ConsoleKey.Enter);

                Assert.Equal(1, TLectern.TLecternNoticeRead(console, "The card fold could not be written."));
                Assert.Equal(("→ set off see 1", "lit"), lit());
            },
            "ALTER TABLE collocation_fold RENAME TO collocation_fold_gone;",
            notice: "could not be");
    }

    [Fact]
    public void LecternSeeObserve_RowUnderFoldedCard_KeepsTheSeeLit()
    {
        TLecternCardRun(
            (engine, save) =>
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
            (send, rows, lit, _, _) =>
            {
                send(ConsoleKey.D1);
                send(ConsoleKey.Spacebar);
                Assert.DoesNotContain("▾ set off", rows());
                for (int step = 0; step < 20 && lit()?.Item1.StartsWith('→') != true; step++)
                {
                    send(ConsoleKey.DownArrow);
                }

                Assert.Equal(("→ set off see 1", "lit"), lit());

                send(ConsoleKey.Enter);

                Assert.DoesNotContain("▾ set off", rows());
                Assert.Equal(("→ set off see 1", "lit"), lit());
            });
    }

    [Fact]
    public void LecternMeaningObserve_DigitTwo_LightsMeaningTwo()
    {
        TLecternCardRun(
            (_, save) => save(
                "apple",
                [
                    TInterface.TCardCreate("a round fruit", 1) with { LCardDraftTitle = "fruit" },
                    TInterface.TCardCreate("a tall plant", 2) with { LCardDraftTitle = "tree" },
                ]),
            (send, _, lit, _, _) =>
            {
                send(ConsoleKey.D2);
                Assert.Equal(("▾ tree", "lit"), lit());

                send(ConsoleKey.D1);
                Assert.Equal(("▾ fruit", "lit"), lit());

                send(ConsoleKey.D3);
                Assert.Equal(("▾ fruit", "lit"), lit());
            });
    }

    [Fact]
    public void LecternFollowObserve_EnterOnTranslation_OpensTheTarget()
    {
        TLecternCardRun(
            (engine, save) =>
            {
                long zebra = save("zebra", []);
                _ = save(
                    "apple", [TInterface.TCardCreate("a round fruit", 1) with { LCardDraftTranslation = [zebra] }]);
            },
            (send, rows, lit, _, _) =>
            {
                send(ConsoleKey.D1);
                send(ConsoleKey.DownArrow);
                Assert.Equal(("→ zebra English", "lit"), lit());

                send(ConsoleKey.Enter);

                Assert.StartsWith("zebra", rows()[0], StringComparison.Ordinal);
            });
    }

    [Fact]
    public void LecternCardDraw_TranslationTargetGone_ShowsNoLink()
    {
        TLecternCardRun(
            (engine, save) =>
            {
                long zebra = save("zebra", []);
                _ = save(
                    "apple", [TInterface.TCardCreate("a round fruit", 1) with { LCardDraftTranslation = [zebra] }]);
                _ = engine.TEngineEntryDelete(zebra);
            },
            (_, rows, _, _, _) =>
            {
                Assert.Contains("a round fruit", rows());
                Assert.DoesNotContain(rows(), static row => row.Contains("zebra", StringComparison.Ordinal));
            });
    }

    [Fact]
    public void LecternCardDraw_OtherCollocation_FoldsToAClosedRow()
    {
        TLecternCardRun(
            (engine, save) => engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
                "apple",
                "English",
                string.Empty,
                string.Empty,
                [],
                [
                    new LCardDraft(
                        LStateValue.LStateValueUnspecified,
                        "eat apples",
                        "to have fruit",
                        [new LSentenceDraft(TInterfaceExample.TExampleDraftCreate("Eat apples daily."), "with", "of")],
                        [], [], [], [], [], [], 1),
                ])),
            (send, rows, lit, _, _) =>
            {
                Assert.Contains("Other collocations", rows());
                Assert.Contains("▾ eat apples", rows());
                Assert.Contains("to have fruit", rows());

                send(ConsoleKey.DownArrow);
                send(ConsoleKey.DownArrow);
                send(ConsoleKey.DownArrow);
                send(ConsoleKey.Spacebar);

                Assert.Contains(rows(), static row => row.StartsWith("▸ eat apples  to", StringComparison.Ordinal)
                    && row.EndsWith(" 1 example", StringComparison.Ordinal));
                Assert.DoesNotContain("to have fruit", rows());
            });
    }

    internal static void TLecternCardRun(
        Action<LEngine, Func<string, IReadOnlyList<LCardDraft>, long>> seed,
        Action<Action<ConsoleKey>, Func<IReadOnlyList<string>>, Func<(string, string)?>, CAtelier, TConsoleFake>
            check,
        string? script = null,
        int height = 60,
        bool open = true,
        string? notice = null)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        seed(
            engine,
            (headword, meanings) => engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
                headword, "English", string.Empty, string.Empty, meanings, [])).LEntryId);
        SynchronizationContext? context = SynchronizationContext.Current;
        BLoop loop = new(static _ => { });
        SynchronizationContext.SetSynchronizationContext(loop);
        try
        {
            TConsoleFake console = new() { TConsoleFakeTerminal = true, TConsoleFakeHeight = height };
            BLocalizationCatalog catalog = new();
            catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
            ABootstrap bootstrap = new();
            BEnvoy envoy = new(console, catalog, bootstrap);
            using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
            LCapsuleConsole capsule = new();
            BWindow window = new(
                console,
                loop,
                catalog,
                atelier,
                envoy,
                bootstrap,
                (BTheme.BThemeDefault, console.TConsoleFakeTheme),
                _ => console.TConsoleFakeTheme,
                capsule);
            envoy.BEnvoyWindowAttach(
                window,
                atelier,
                File.Exists,
                static path => (Directory.GetDirectories(path), Directory.GetFiles(path)),
                Directory.GetLogicalDrives,
                capsule);
            loop.BLoopBatchAttach(window.BWindowBatchObserve);
            loop.BLoopKeyAttach(console, window.BWindowKeyObserve);
            if (notice is not null)
            {
                TLectern.TLecternNoticeClose(console, notice);
            }

            Action<Action> settle = work =>
            {
                TaskCompletionSource settled = new();
                loop.BLoopDefer(() =>
                {
                    work();
                    loop.BLoopDefer(settled.SetResult);
                });
                loop.BLoopRun(settled.Task);
                window.BWindowBatchObserve();
            };
            settle(() =>
            {
                window.BWindowOpen();
                window.BWindowAtelierOpen();
            });
            settle(() => Assert.True(atelier.CAtelierNavigation.CNavigationTabSelect("Library")));
            if (script is not null)
            {
                workspace.TWorkspaceScriptRun(script);
            }

            if (open)
            {
                settle(() => window.BWindowKeyObserve(TLectern.TLecternKeyCreate(ConsoleKey.Enter)));
            }

            check(
                key => settle(() => window.BWindowKeyObserve(TLectern.TLecternKeyCreate(key))),
                () =>
                {
                    settle(static () => { });
                    return TLectern.TLecternRowsRead(console);
                },
                () =>
                {
                    settle(static () => { });
                    return TLectern.TLecternLitRead(console);
                },
                atelier,
                console);
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(context);
        }
    }
}
