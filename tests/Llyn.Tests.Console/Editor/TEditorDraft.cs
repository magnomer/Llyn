using Llyn.Conduct;
using Llyn.ShellEngine;
using Llyn.UIDemeanor;
using Llyn.UIDemeanor.Capsule;
using Llyn.UITerminal;
using Xunit;
using static Llyn.Tests.TEditorFrame;

namespace Llyn.Tests;

public sealed class TEditorDraft
{
    [Fact]
    public void WindowKeyObserve_TypedLeaveBatch_RaisesLeaveQuestion()
    {
        TEditorWindowRun(
            (window, atelier, console) =>
            {
                Assert.True(atelier.CAtelierNavigation.CNavigationTabSelect("Input"));

                window.BWindowKeyObserve(new ConsoleKeyInfo('w', ConsoleKey.W, false, false, false));
                console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\u001b', ConsoleKey.Escape, false, false, false));
                int mark = console.TConsoleFakeShots.Count;
                window.BWindowKeyObserve(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

                Assert.Contains(console.TConsoleFakeShots.Skip(mark), static shot =>
                    string.Join('\n', shot).Contains("[ Save ]     [ Discard ]     [ Cancel ]"));
                Assert.False(window.BWindowExit.IsCompleted);
            });
    }

    [Fact]
    public void EditorKeymap_SaveWithOpenField_ClosesFieldStoresText()
    {
        List<string> asked = [];
        TEditorFrameRun(
            ["fire"],
            (library, find) =>
            {
                TEditorFrameSend(library, new ConsoleKeyInfo('s', ConsoleKey.S, false, false, false));
                Assert.Equal("Editor.Close", find(ConsoleKey.Escape, 0)?.BKeymapRowLabel);

                TEditorFrameSend(library, new ConsoleKeyInfo('\0', ConsoleKey.F2, false, false, false));

                IReadOnlyList<BCanvasText> texts = TEditorFrameDraw(library);
                Assert.Empty(asked);
                Assert.Null(find(ConsoleKey.Escape, 0));
                Assert.Contains(texts, static text => text.BCanvasTextValue == " Editing fires ");
                Assert.DoesNotContain(texts, static text => text.BCanvasTextValue.Contains("Unsaved"));
            },
            asked,
            static engine => engine.TEngineDelaySet(0));
    }

    [Fact]
    public void EditorEntrySave_RefusedOnOtherEditor_ShowsRefusalOnce()
    {
        List<string> asked = [];
        CEnvoy envoy = TEnvoyFake.TEnvoyFolderCreate(null, asked);
        TLibrary.TLibraryRun(
            ["fire"],
            true,
            (library, atelier, _) =>
            {
                ConsoleKeyInfo back = new('\b', ConsoleKey.Backspace, false, false, false);
                ConsoleKeyInfo f4 = new('\0', ConsoleKey.F4, false, false, false);
                library.BLibraryObserve(library.BLibraryKeymapFocus, TLibrary.TLibraryEnter);
                library.BLibraryKeymap.BKeymapFind(BKeymapFocus.BKeymapFocusWindow, f4)!.BKeymapRowAction(f4);
                TEditorFrameSend(library, TLibrary.TLibraryEnter);
                for (int typed = 0; typed < "fire".Length; typed++)
                {
                    TEditorFrameSend(library, back);
                }

                TEditorFrameSend(library, TLibrary.TLibraryDown);
                TEditorFrameSend(library, TLibrary.TLibraryDown);
                TEditorFrameSend(library, TLibrary.TLibraryDown);
                TEditorFrameSend(library, TLibrary.TLibraryDown);
                Func<ConsoleKey, BKeymapRow?> find = key => library.BLibraryKeymap.BKeymapFind(
                    library.BLibraryKeymapFocus, new ConsoleKeyInfo('\0', key, false, false, false));
                Assert.Equal("Editor.Stack", find(ConsoleKey.RightArrow)?.BKeymapRowLabel);
                asked.Clear();
                ConsoleKeyInfo f2 = new('\0', ConsoleKey.F2, false, false, false);
                BKeymapRow save = library.BLibraryKeymap.BKeymapFind(BKeymapFocus.BKeymapFocusWindow, f2)!;
                Assert.Equal(BKeymapLive.BKeymapLiveDim, save.BKeymapRowLive());

                save.BKeymapRowAction(f2);
                Action settle = () =>
                {
                    BLoop loop = Assert.IsType<BLoop>(SynchronizationContext.Current);
                    TaskCompletionSource settled = new();
                    loop.BLoopDefer(settled.SetResult);
                    loop.BLoopRun(settled.Task.WaitAsync(TimeSpan.FromSeconds(10)));
                };
                settle();

                Assert.Empty(asked);
                Assert.Contains(TEditorFrameDraw(library), static text => text.BCanvasTextValue == " Editing ");
                Assert.Contains(TEditorFrameDraw(library), static text => text.BCanvasTextValue.Contains("Unsaved"));
                Assert.Equal("Editor.Stack", find(ConsoleKey.RightArrow)?.BKeymapRowLabel);

                CEditor refused = atelier.CAtelierInputCreate(envoy);
                refused.CEditorEntryOpen(null);
                refused.CEditorEntry.CEntryPronunciationSet("a");
                settle();

                Assert.False(refused.CEditorEntrySave());
                Assert.Equal(["Input.SaveFailed>Refusal.HeadwordMissing"], asked);
            },
            asked,
            envoy: envoy);
    }

    [Theory]
    [InlineData("Library", "F4")]
    [InlineData("Library", "Enter")]
    [InlineData("Library", "AltF1")]
    [InlineData("Library", "AltLeft")]
    [InlineData("Library", "AltRight")]
    [InlineData("Library", "F10")]
    [InlineData("Library", "Workspace")]
    [InlineData("Favorite", "F4")]
    [InlineData("Favorite", "Enter")]
    [InlineData("Favorite", "AltF1")]
    [InlineData("Favorite", "AltLeft")]
    [InlineData("Favorite", "AltRight")]
    [InlineData("Favorite", "F10")]
    [InlineData("Favorite", "Workspace")]
    public void WindowKeymap_LeaveKeysOnBrowseTab_AsksOnAlteredDraft(string tab, string trigger)
    {
        Assert.False(TEditorDraftRun(tab, trigger, false));
        Assert.True(TEditorDraftRun(tab, trigger, true));
    }

    [Theory]
    [InlineData("AltF1", false)]
    [InlineData("AltLeft", false)]
    [InlineData("AltRight", false)]
    [InlineData("F10", true)]
    [InlineData("Workspace", true)]
    public void WindowKeymap_InputTabLeaveKeys_AsksOnQuitOrWorkspace(string trigger, bool asks)
    {
        Assert.False(TEditorDraftRun("Input", trigger, false));
        Assert.Equal(asks, TEditorDraftRun("Input", trigger, true));
    }

    [Fact]
    public void LibraryObserve_CancelAfterOtherRow_LightsEditedEntry()
    {
        List<string> asked = [];
        TLibrary.TLibraryRun(
            ["fire", "stone", "water"],
            true,
            (library, _, _) =>
            {
                TEditorFrameSend(library, TLibrary.TLibraryEnter);
                TEditorFrameSend(library, new ConsoleKeyInfo('\0', ConsoleKey.F4, false, false, false));
                TEditorFrameSend(library, new ConsoleKeyInfo('x', ConsoleKey.X, false, false, false));
                TEditorFrameSend(library, new ConsoleKeyInfo('\u001b', ConsoleKey.Escape, false, false, false));
                TEditorFrameSend(library, new ConsoleKeyInfo('\t', ConsoleKey.Tab, false, false, false));
                TEditorFrameSend(library, TLibrary.TLibraryDown);
                TEditorFrameSend(library, TLibrary.TLibraryDown);
                Assert.Contains("3 of 3", TLibrary.TLibraryDraw(library));

                TEditorFrameSend(library, TLibrary.TLibraryEnter);

                Assert.Equal(["Leave"], asked);
                Assert.Contains("1 of 3", TLibrary.TLibraryDraw(library));
                Assert.Contains(TEditorFrameDraw(library), static text => text.BCanvasTextValue == " Editing firex ");
            },
            asked,
            envoy: TEnvoyFake.TEnvoyCreate(null, asked));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public void LibraryKeymap_LeaveAnswers_StoresDropsOrStays(bool? answer)
    {
        List<string> asked = [];
        TLibrary.TLibraryRun(
            ["fire", "stone"],
            true,
            (library, _, _) =>
            {
                ConsoleKeyInfo f4 = new('\0', ConsoleKey.F4, false, false, false);
                TEditorFrameSend(library, TLibrary.TLibraryEnter);
                TEditorFrameSend(library, f4);
                TEditorFrameSend(library, new ConsoleKeyInfo('x', ConsoleKey.X, false, false, false));
                TEditorFrameSend(library, new ConsoleKeyInfo('\u001b', ConsoleKey.Escape, false, false, false));

                TEditorFrameSend(library, f4);

                Assert.Equal(["Leave"], asked);
                string label = library.BLibraryKeymap.BKeymapFind(BKeymapFocus.BKeymapFocusWindow, f4)!.BKeymapRowLabel;
                Assert.Equal(answer is null ? "Scribe.View" : "Scribe.Edit", label);
                Assert.Equal(answer is not false, TLibrary.TLibraryDraw(library).Contains("firex"));
                Assert.Equal(
                    answer is null,
                    TEditorFrameDraw(library).Any(static text => text.BCanvasTextValue.Contains("Unsaved")));
            },
            asked,
            envoy: TEnvoyFake.TEnvoyCreate(answer, asked));
    }

    [Fact]
    public void LibraryKeymap_F2OnStoredEntry_ReopensLitRowFollows()
    {
        TLibrary.TLibraryRun(
            ["fire", "stone", "water"],
            true,
            (library, _, _) =>
            {
                ConsoleKeyInfo back = new('\b', ConsoleKey.Backspace, false, false, false);
                TEditorFrameSend(library, TLibrary.TLibraryDown);
                TEditorFrameSend(library, new ConsoleKeyInfo('\0', ConsoleKey.UpArrow, false, false, false));
                TEditorFrameSend(library, TLibrary.TLibraryEnter);
                TEditorFrameSend(library, new ConsoleKeyInfo('\0', ConsoleKey.F4, false, false, false));
                TEditorFrameSend(library, TLibrary.TLibraryEnter);
                for (int typed = 0; typed < "fire".Length; typed++)
                {
                    TEditorFrameSend(library, back);
                }

                TEditorFrameSend(library, new ConsoleKeyInfo('z', ConsoleKey.Z, false, false, false));
                TEditorFrameSend(library, new ConsoleKeyInfo('\0', ConsoleKey.F2, false, false, false));

                IReadOnlyList<BCanvasText> texts = TEditorFrameDraw(library);
                Assert.Contains(texts, static text => text.BCanvasTextValue == " Editing z ");
                Assert.DoesNotContain(texts, static text => text.BCanvasTextValue.Contains("Unsaved"));
                Assert.Contains("3 of 3", TLibrary.TLibraryDraw(library));
            },
            seed: static engine => engine.TEngineDelaySet(0));
    }

    [Fact]
    public void LibraryKeymap_F2OnFreshEntry_LightsStoredEntry()
    {
        TLibrary.TLibraryRun(
            ["fire", "stone", "water"],
            true,
            (library, _, _) =>
            {
                TEditorFrameSend(library, TLibrary.TLibraryDown);
                TEditorFrameSend(library, TLibrary.TLibraryDown);
                TEditorFrameSend(library, new ConsoleKeyInfo('\0', ConsoleKey.F7, false, false, false));
                TEditorFrameSend(library, new ConsoleKeyInfo('\t', ConsoleKey.Tab, false, false, false));
                TEditorFrameSend(library, new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false));
                Assert.Contains("3 of 3", TLibrary.TLibraryDraw(library));

                TEditorFrameSend(library, new ConsoleKeyInfo('\0', ConsoleKey.F2, false, false, false));

                Assert.Contains("1 of 4", TLibrary.TLibraryDraw(library));
                Assert.Contains(TEditorFrameDraw(library), static text => text.BCanvasTextValue == " Editing a ");
            },
            seed: static engine => engine.TEngineDelaySet(0));
    }

    private static bool TEditorDraftRun(string tab, string trigger, bool altered)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using TWorkspace other = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        foreach (string headword in (string[])["fire", "stone", "water"])
        {
            Llyn.Core.LEntryDraft draft = TInterface.TEntryDraftCreate(
                headword, "English", string.Empty, string.Empty, [TInterface.TCardCreate("a meaning", 1)], []);
            engine.TEngineFavoriteSave(engine.TEngineEntrySave(draft).LEntryId);
        }

        SynchronizationContext? context = SynchronizationContext.Current;
        BLoop loop = new(static _ => { });
        SynchronizationContext.SetSynchronizationContext(loop);
        try
        {
            TConsoleFake console = new() { TConsoleFakeTerminal = true };
            BLocalizationCatalog catalog = new();
            catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
            ABootstrap bootstrap = new();
            BEnvoy envoy = new(console, catalog, bootstrap);
            using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, false);
            BTheme spare = new(BTheme.BThemeSpareRead);
            LCapsuleConsole capsule = new();
            BWindow window = new(
                console, loop, catalog, atelier, envoy, bootstrap, (BTheme.BThemeDefault, spare), _ => spare, capsule);
            envoy.BEnvoyWindowAttach(
                window, atelier, File.Exists, static path => (Directory.GetDirectories(path), Directory.GetFiles(path)),
                Directory.GetLogicalDrives, capsule);
            bool raised = false;
            string? shown = null;
            atelier.CAtelierNavigation.CNavigationChanged += state => shown = state.CNavigationStateTab ?? shown;
            Action<ConsoleKey, bool> press = (key, alt) => window.BWindowKeyObserve(new ConsoleKeyInfo(
                key switch
                {
                    ConsoleKey.Enter => '\r',
                    ConsoleKey.Tab => '\t',
                    ConsoleKey.Escape => '\u001b',
                    ConsoleKey.X => 'x',
                    _ => '\0',
                },
                key,
                false,
                alt,
                false));
            loop.BLoopDefer(window.BWindowOpen);
            loop.BLoopDefer(window.BWindowAtelierOpen);
            loop.BLoopDefer(() =>
            {
                Assert.True(atelier.CAtelierNavigation.CNavigationTabSelect(tab == "Input" ? "Library" : tab));
                press(ConsoleKey.Enter, false);
                if (trigger is "AltLeft" or "AltRight")
                {
                    press(ConsoleKey.Tab, false);
                    press(ConsoleKey.DownArrow, false);
                    press(ConsoleKey.Enter, false);
                }

                if (trigger is "AltRight")
                {
                    press(ConsoleKey.LeftArrow, true);
                }

                if (tab == "Input")
                {
                    Assert.True(atelier.CAtelierNavigation.CNavigationTabSelect("Input"));
                }
                else
                {
                    press(ConsoleKey.F4, false);
                }

                if (altered)
                {
                    press(ConsoleKey.X, false);
                    press(ConsoleKey.Escape, false);
                }

                if (trigger is "AltF1")
                {
                    console.TConsoleFakeKeys.Add(tab == "Library"
                        ? new ConsoleKeyInfo('3', ConsoleKey.D3, false, false, false)
                        : new ConsoleKeyInfo('2', ConsoleKey.D2, false, false, false));
                }

                console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\u001b', ConsoleKey.Escape, false, false, false));
                int mark = console.TConsoleFakeShots.Count;
                switch (trigger)
                {
                    case "Enter":
                        press(ConsoleKey.Tab, false);
                        press(ConsoleKey.DownArrow, false);
                        press(ConsoleKey.Enter, false);
                        break;
                    case "Workspace":
                        _ = atelier.CAtelierWorkspace.CWorkspaceChange(other.TWorkspaceFolder, envoy);
                        break;
                    default:
                        press(
                            trigger switch
                            {
                                "F4" => ConsoleKey.F4,
                                "AltF1" => ConsoleKey.F1,
                                "AltLeft" => ConsoleKey.LeftArrow,
                                "AltRight" => ConsoleKey.RightArrow,
                                _ => ConsoleKey.F10,
                            },
                            trigger.StartsWith("Alt", StringComparison.Ordinal));
                        break;
                }

                raised = console.TConsoleFakeShots.Skip(mark).Any(static shot =>
                    string.Join('\n', shot).Contains("[ Save ]     [ Discard ]     [ Cancel ]"))
                    || (trigger is "AltF1" && shown == tab);
                console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));
                console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.RightArrow, false, false, false));
                console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
            });
            loop.BLoopBatchAttach(window.BWindowBatchObserve);
            loop.BLoopKeyAttach(console, window.BWindowKeyObserve);
            loop.BLoopRun(window.BWindowExit.WaitAsync(TimeSpan.FromSeconds(30)));
            return raised;
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(context);
        }
    }
}
