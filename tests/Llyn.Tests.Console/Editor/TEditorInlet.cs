using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDemeanor;
using Xunit;
using static Llyn.Tests.TEditorFrame;

namespace Llyn.Tests;

public sealed class TEditorInlet
{
    private const int TEditorInletSide = 40;
    private static readonly ConsoleKeyInfo TEditorInletEscape = new('\u001b', ConsoleKey.Escape, false, false, false);

    private static readonly ConsoleKeyInfo TEditorInletBack = new('\b', ConsoleKey.Backspace, false, false, false);

    private static readonly ConsoleKeyInfo TEditorInletLeft = new('\0', ConsoleKey.LeftArrow, false, false, false);

    private static readonly ConsoleKeyInfo TEditorInletUp = new('\0', ConsoleKey.UpArrow, false, false, false);

    [Fact]
    public void EditorDraw_EmptyChipFields_ShowsDimmedHintsAndLitFoot()
    {
        TEditorInletRun(1, (library, _) =>
        {
            IReadOnlyList<BCanvasText> texts = TEditorFrameDraw(library);

            BCanvasText lit = Assert.Single(texts, static text => text.BCanvasTextRole == "lit"
                && text.BCanvasTextValue.Contains("Where or when is it used?"));
            Assert.StartsWith("   Where", lit.BCanvasTextValue);
            Assert.Contains(texts, static text => text.BCanvasTextRole == "muted"
                && text.BCanvasTextValue == "What register does it carry?");
            Assert.Contains(texts, static text => text.BCanvasTextRole == "muted"
                && text.BCanvasTextValue.StartsWith("Where or when is it used?"));
        });
    }

    [Fact]
    public void EditorKeymap_CommaOnSituationField_MakesBracketedChip()
    {
        TEditorInletRun(1, (library, find) =>
        {
            Assert.Equal("Editor.Open", find(ConsoleKey.Enter)?.BKeymapRowLabel);

            TEditorInletSend(library, "sport,");

            IReadOnlyList<BCanvasText> texts = TEditorFrameDraw(library);
            BCanvasText chip = Assert.Single(texts, static text => text.BCanvasTextValue == "[sport]");
            Assert.Equal("situation", chip.BCanvasTextRole);
            BCanvasText entry = Assert.Single(texts, TEditorInletMatch);
            Assert.Equal(chip.BCanvasTextRow, entry.BCanvasTextRow);
            Assert.True(entry.BCanvasTextColumn > chip.BCanvasTextColumn);
            Assert.Equal("Card.ChipAdd", find(ConsoleKey.Enter)?.BKeymapRowLabel);
            Assert.DoesNotContain(texts, static text => text.BCanvasTextRole == "muted"
                && text.BCanvasTextValue.StartsWith("Where or when"));
        });
    }

    [Fact]
    public void EditorKeymap_LitStoredRowOnSituationList_BecomesChip()
    {
        TEditorInletRun(1, (library, find) =>
        {
            TEditorInletSend(library, "sum");

            IReadOnlyList<BCanvasText> texts = TEditorFrameDraw(library);
            Assert.Contains(texts, static text => text.BCanvasTextRole == "menu"
                && text.BCanvasTextValue.StartsWith(" summer"));
            Assert.Contains(texts, static text => text.BCanvasTextRole == "menu-hotkey"
                && text.BCanvasTextValue == "sum");
            Assert.Equal("Card.ChipAdd", find(ConsoleKey.Enter)?.BKeymapRowLabel);

            TEditorFrameSend(library, TLibrary.TLibraryDown);

            Assert.Equal("Editor.Pick", find(ConsoleKey.Enter)?.BKeymapRowLabel);
            Assert.Contains(TEditorFrameDraw(library), static text => text.BCanvasTextRole == "lit"
                && text.BCanvasTextValue.StartsWith(" summer"));

            TEditorFrameSend(library, TLibrary.TLibraryEnter);

            texts = TEditorFrameDraw(library);
            Assert.Contains(texts, static text => text.BCanvasTextValue == "[summer]");
            Assert.DoesNotContain(texts, static text => text.BCanvasTextRole == "shadow");
            Assert.StartsWith(" ", Assert.Single(texts, TEditorInletMatch).BCanvasTextValue);
        });
    }

    [Fact]
    public void EditorKeymap_EnterWithNoRowLit_SettlesTypedText()
    {
        TEditorInletRun(1, (library, _) =>
        {
            TEditorInletSend(library, "rain");

            TEditorFrameSend(library, TLibrary.TLibraryEnter);

            Assert.Contains(TEditorFrameDraw(library), static text => text.BCanvasTextValue == "[rain]");
        });
    }

    [Fact]
    public void EditorKeymap_EscapeTwiceOnSituationField_SettlesText()
    {
        TEditorInletRun(1, (library, find) =>
        {
            TEditorInletSend(library, "sum");
            Assert.Equal("Editor.Shut", find(ConsoleKey.Escape)?.BKeymapRowLabel);

            TEditorFrameSend(library, TEditorInletEscape);

            IReadOnlyList<BCanvasText> texts = TEditorFrameDraw(library);
            Assert.DoesNotContain(texts, static text => text.BCanvasTextRole == "shadow");
            Assert.StartsWith("sum", Assert.Single(texts, TEditorInletMatch).BCanvasTextValue);
            Assert.Equal("Editor.Close", find(ConsoleKey.Escape)?.BKeymapRowLabel);

            TEditorFrameSend(library, TEditorInletEscape);

            texts = TEditorFrameDraw(library);
            Assert.DoesNotContain(texts, TEditorInletMatch);
            Assert.Contains(texts, static text => text.BCanvasTextRole == "lit"
                && text.BCanvasTextValue.Contains("[sum]"));
            Assert.Equal("Editor.Open", find(ConsoleKey.Enter)?.BKeymapRowLabel);
        });
    }

    [Fact]
    public void EditorKeymap_BackspaceAndLeftOnEmpty_WalksChipTrail()
    {
        TEditorInletRun(1, (library, _) =>
        {
            TEditorInletSend(library, "hot,cold,dry,");
            TEditorFrameSend(library, TEditorInletBack);

            IReadOnlyList<BCanvasText> texts = TEditorFrameDraw(library);
            Assert.DoesNotContain(texts, static text => text.BCanvasTextValue == "[dry]");

            TEditorFrameSend(library, TEditorInletLeft);

            texts = TEditorFrameDraw(library);
            BCanvasText hot = Assert.Single(texts, static text => text.BCanvasTextValue == "[hot]");
            BCanvasText cold = Assert.Single(texts, static text => text.BCanvasTextValue == "[cold]");
            BCanvasText entry = Assert.Single(texts, TEditorInletMatch);
            Assert.InRange(entry.BCanvasTextColumn, hot.BCanvasTextColumn + 1, cold.BCanvasTextColumn - 1);

            TEditorFrameSend(library, TEditorInletBack);

            Assert.DoesNotContain(TEditorFrameDraw(library), static text => text.BCanvasTextValue == "[hot]");
            Assert.Contains(TEditorFrameDraw(library), static text => text.BCanvasTextValue == "[cold]");
        });
    }

    [Fact]
    public void EditorKeymap_LightLeavingOpenChipField_SettlesText()
    {
        TEditorInletRun(1, (library, find) =>
        {
            TEditorInletSend(library, "zzqq");
            Assert.Equal("Window.Help.Item", find(ConsoleKey.UpArrow)?.BKeymapRowLabel);

            TEditorFrameSend(library, TEditorInletUp);

            IReadOnlyList<BCanvasText> texts = TEditorFrameDraw(library);
            Assert.DoesNotContain(texts, TEditorInletMatch);
            Assert.Contains(texts, static text => text.BCanvasTextValue == "[zzqq]");
            Assert.NotEqual("Card.ChipAdd", find(ConsoleKey.Enter)?.BKeymapRowLabel);
        });
    }

    [Fact]
    public void EditorKeymap_RegisterAndTag_ShowsBracedAndHashedChips()
    {
        TEditorInletRun(2, (library, _) =>
        {
            TEditorInletSend(library, "formal,");
            TEditorFrameSend(library, TLibrary.TLibraryDown);
            TEditorInletSend(library, "warm,");

            IReadOnlyList<BCanvasText> texts = TEditorFrameDraw(library);
            BCanvasText formal = Assert.Single(texts, static text => text.BCanvasTextValue == "{Formal}");
            Assert.Equal("register", formal.BCanvasTextRole);
            Assert.Contains(texts, static text => text.BCanvasTextValue == "#warm");
        });
    }

    [Fact]
    public void EditorDraw_ManyChips_WrapsInsideCard()
    {
        TEditorInletRun(1, (library, _) =>
        {
            TEditorInletSend(library, string.Concat(Enumerable.Range(1, 12).Select(static at => $"place{at},")));

            BCanvasText[] chips =
                [.. TEditorFrameDraw(library).Where(static text => text.BCanvasTextRole == "situation")];

            Assert.Equal(12, chips.Length);
            Assert.True(chips.Select(static chip => chip.BCanvasTextRow).Distinct().Count() > 1);
            Assert.All(chips, static chip => Assert.True(chip.BCanvasTextColumn + chip.BCanvasTextSpan <= 80));
        });
    }

    [Fact]
    public void EditorKeymap_SettlePastFailedObserver_LeavesNoKeptText()
    {
        bool failing = false;
        List<string> asked = [];
        TEditorInletRun(
            1,
            (library, find) =>
            {
                TEditorInletSend(library, "rain");
                Assert.Equal("Editor.Close", find(ConsoleKey.Escape)?.BKeymapRowLabel);
                failing = true;

                TEditorFrameSend(library, TEditorInletEscape);
                failing = false;

                Assert.Contains("Input.UpdateFailed", asked);
                Assert.Contains(
                    TEditorFrameDraw(library),
                    static text => text.BCanvasTextValue.Contains("[rain]", StringComparison.Ordinal));
                Assert.Equal("Editor.Open", find(ConsoleKey.Enter)?.BKeymapRowLabel);

                TEditorFrameSend(library, TLibrary.TLibraryEnter);

                IReadOnlyList<BCanvasText> texts = TEditorFrameDraw(library);
                Assert.DoesNotContain(texts, TEditorRestCheck);
                Assert.Contains(texts, static text => text.BCanvasTextValue == "[rain]");
            },
            engine => engine.TEngineObserverAttach(bulletin =>
            {
                if (failing && bulletin.LBulletinSubject == LSubject.LSubjectTenure)
                {
                    throw new InvalidOperationException("The tenure observer failed.");
                }
            }),
            asked);
    }

    [Fact]
    public void EditorDraw_SettlePastFailedObserver_ShowsNoKeptText()
    {
        TEditorInletSettle("rain", (library, _) =>
        {
            IReadOnlyList<BCanvasText> texts = TEditorFrameDraw(library);

            Assert.DoesNotContain(texts, TEditorInletMatch);
            Assert.DoesNotContain(texts, static text => text.BCanvasTextValue
                .Replace("[rain]", string.Empty, StringComparison.Ordinal).Contains("rain", StringComparison.Ordinal));
        });
    }

    [Fact]
    public void EditorKeymap_LeaveCardAfterRefusedSettle_DropsKeptText()
    {
        TEditorInletSettle("rain", (library, find) =>
        {
            TEditorFrameSend(library, new ConsoleKeyInfo('\0', ConsoleKey.Home, false, false, true));
            for (int step = 0; step < 6; step++)
            {
                TEditorFrameSend(library, TLibrary.TLibraryDown);
            }

            Assert.Equal("Editor.Open", find(ConsoleKey.Enter)?.BKeymapRowLabel);
            TEditorFrameSend(library, TLibrary.TLibraryEnter);

            Assert.DoesNotContain(TEditorFrameDraw(library), TEditorRestCheck);
        });
    }

    [Fact]
    public void EditorKeymap_SaveAfterRefusedSettle_DropsKeptText()
    {
        TEditorInletSettle("dry,rain", (library, find) =>
        {
            TEditorFrameSend(library, new ConsoleKeyInfo('\0', ConsoleKey.F2, false, false, false));

            Assert.Equal("Editor.Open", find(ConsoleKey.Enter)?.BKeymapRowLabel);
            TEditorFrameSend(library, TLibrary.TLibraryEnter);

            Assert.DoesNotContain(TEditorFrameDraw(library), TEditorRestCheck);
        });
    }

    [Fact]
    public void EditorKeymap_HaltWithOpenChipField_ClosesAndIgnoresKeys()
    {
        LEngine? held = null;
        List<long> drafts = [];
        List<string> asked = [];
        TEditorInletRun(
            1,
            (library, find) =>
            {
                TEditorInletSend(library, "w");
                held!.TEngineDraftCancel(drafts[^1]);

                TEditorInletSend(library, ",x");

                IReadOnlyList<BCanvasText> texts = TEditorFrameDraw(library);
                Assert.DoesNotContain(texts, TEditorInletMatch);
                Assert.DoesNotContain(texts, static text => text.BCanvasTextValue == "[w]");
                Assert.NotEqual("Card.ChipAdd", find(ConsoleKey.Enter)?.BKeymapRowLabel);
            },
            engine =>
            {
                held = engine;
                engine.TEngineObserverAttach(bulletin =>
                {
                    if (bulletin.LBulletinSubject == LSubject.LSubjectDraft)
                    {
                        drafts.Add(bulletin.LBulletinId);
                    }
                });
            },
            asked);
    }

    internal static void TEditorInletRun(
        int part,
        Action<BLibrary, Func<ConsoleKey, BKeymapRow?>> check,
        Action<LEngine>? hold = null,
        List<string>? asked = null)
    {
        TEditorFrameRun(
            [],
            (library, find) =>
            {
                for (int step = 0; step < 6 + part; step++)
                {
                    TEditorFrameSend(library, TLibrary.TLibraryDown);
                }

                check(library, key => find(key, 0));
            },
            asked,
            engine =>
            {
                _ = engine.TEngineSituationCreate(TInterface.TSituationCreate(0, "summer", null, null));
                engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
                    "fire", "English", string.Empty, string.Empty, [TInterface.TCardCreate("a meaning", 1)], []));
                hold?.Invoke(engine);
            });
    }

    private static void TEditorInletSettle(string typed, Action<BLibrary, Func<ConsoleKey, BKeymapRow?>> check)
    {
        bool failing = false;
        TEditorInletRun(
            1,
            (library, find) =>
            {
                TEditorInletSend(library, typed);
                failing = true;
                TEditorFrameSend(library, TEditorInletEscape);
                failing = false;
                check(library, find);
            },
            engine => engine.TEngineObserverAttach(bulletin =>
            {
                if (failing && bulletin.LBulletinSubject == LSubject.LSubjectTenure)
                {
                    throw new InvalidOperationException("The tenure observer failed.");
                }
            }),
            []);
    }

    private static bool TEditorRestCheck(BCanvasText text)
    {
        return text.BCanvasTextRole != "situation" && text.BCanvasTextValue.Contains("rain");
    }

    internal static bool TEditorInletMatch(BCanvasText text)
    {
        return text.BCanvasTextRole == "field" && text.BCanvasTextColumn >= TEditorInletSide;
    }

    internal static void TEditorInletSend(BLibrary library, string typed)
    {
        foreach (char letter in typed)
        {
            TEditorFrameSend(library, new ConsoleKeyInfo(
                letter, letter == ',' ? ConsoleKey.OemComma : ConsoleKey.A, false, false, false));
        }
    }
}
