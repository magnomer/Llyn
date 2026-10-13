using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCardSense
{
    [Fact]
    public void CardSenseRead_NestedMeanings_ReturnsDepthAndPath()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TCardSenseSave(engine);
        LEntryDraft stored = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(entry.LEntryId));
        LCardDraft root = stored.LEntryDraftMeanings[0];
        TEditorFixture editor = TEditorField.TEditorFieldPrepare(engine);
        editor.TEditorFixtureOpen(entry.LEntryId);

        IReadOnlyList<CMeaning> senses = editor.TEditorFixtureField.TCardSenseRead();

        Assert.Equal(
            [
                (root.LCardDraftId, 0, "1"),
                (root.LCardDraftChild[0].LCardDraftId, 1, "1.1"),
                (root.LCardDraftChild[1].LCardDraftId, 1, "1.2"),
                (stored.LEntryDraftMeanings[1].LCardDraftId, 0, "2"),
            ],
            senses.Select(static sense => (sense.CMeaningId, sense.CMeaningDepth, sense.CMeaningPath)));
    }

    [Fact]
    public void DisplayCardRead_LinkedCollocation_HomesUnderFirstLink()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry entry = TCardSenseSave(engine);
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        wing.CWingEntryOpen(entry.LEntryId);

        CLecternCard card = wing.CWingDisplay.CDisplayCard.CDisplayCardRead();

        CLeaf home = Assert.Single(card.CLecternCardMeanings[0].CLeafCollocation);
        Assert.Equal("set off", home.CLeafPhrase.CStateWordingText);
        Assert.Empty(card.CLecternCardMeanings[1].CLeafCollocation);
    }

    [Fact]
    public void DisplayCardRead_FurtherLink_AddsSeeLine()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry entry = TCardSenseSave(engine);
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        wing.CWingEntryOpen(entry.LEntryId);
        string stub = engine.TEngineTextRead("Display.CollocationStub");

        CLecternCard card = wing.CWingDisplay.CDisplayCard.CDisplayCardRead();

        CLeaf home = Assert.Single(card.CLecternCardMeanings[0].CLeafCollocation);
        Assert.Empty(card.CLecternCardMeanings[0].CLeafSee);
        Assert.Equal(
            [(home.CLeafId, string.Format(CultureInfo.CurrentCulture, stub, "set off", "1.1"))],
            card.CLecternCardMeanings[1].CLeafSee);
    }

    [Fact]
    public void DisplayCardRead_UnlinkedCollocation_ListsOther()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry entry = TCardSenseSave(engine);
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        wing.CWingEntryOpen(entry.LEntryId);

        CLecternCard card = wing.CWingDisplay.CDisplayCard.CDisplayCardRead();

        CLeaf other = Assert.Single(card.CLecternCardOther);
        Assert.Equal("set out", other.CLeafPhrase.CStateWordingText);
        Assert.True(card.CLecternCardCollocated);
    }

    [Fact]
    public void DisplayCompassRead_HomedCollocation_AddsMutedCountRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDisplay display = new TEditorFixture(TInterfaceEditor.TEditorCreate(engine)).TEditorFixtureDisplay;
        display.LDisplayRule.LDisplaySound.TDisplaySoundShow(7, TCompassDraftCreate([41], [42], [42]));

        IReadOnlyList<CCompassRow> rows = display.CDisplayCompass.CCompassRead(
            [CCompassPart.CCompassPartMeaning, CCompassPart.CCompassPartCollocation], static key => key + " {0}");

        Assert.Equal([0, 1, 2, 1, 2], rows.Select(static row => row.CCompassRowDepth));
        Assert.Equal([null, 0, 0, 1, 1], rows.Select(static row => row.CCompassRowCard));
        Assert.Equal([false, false, true, false, true], rows.Select(static row => row.CCompassRowMuted));
        Assert.Equal(
            ["Display.CollocationCountOne 1", "Display.CollocationCount 2"],
            rows.Where(static row => row.CCompassRowMuted).Select(static row => row.CCompassRowName));
        Assert.All(rows.Where(static row => row.CCompassRowMuted), static row => Assert.Empty(row.CCompassRowNumber));
        Assert.DoesNotContain(rows, static row => row.CCompassRowPart == CCompassPart.CCompassPartCollocation);
    }

    [Fact]
    public void DisplayCompassRead_EqualCounts_KeepsCountsUnnumbered()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDisplay display = new TEditorFixture(TInterfaceEditor.TEditorCreate(engine)).TEditorFixtureDisplay;
        display.LDisplayRule.LDisplaySound.TDisplaySoundShow(7, TCompassDraftCreate([41], [42]));

        IReadOnlyList<CCompassRow> rows = display.CDisplayCompass.CCompassRead(
            [CCompassPart.CCompassPartMeaning], static key => key + " {0}");

        Assert.Equal(
            ["Display.CollocationCountOne 1", "Display.CollocationCountOne 1"],
            rows.Where(static row => row.CCompassRowMuted).Select(static row => row.CCompassRowName));
    }

    [Fact]
    public void DisplayCardRead_NoStoredFold_ReadsRowOpen()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry entry = TCardSenseSave(engine);
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        wing.CWingEntryOpen(entry.LEntryId);

        CLecternCard card = wing.CWingDisplay.CDisplayCard.CDisplayCardRead();

        Assert.False(Assert.Single(card.CLecternCardMeanings[0].CLeafCollocation).CLeafFolded);
        Assert.False(Assert.Single(card.CLecternCardOther).CLeafFolded);
    }

    [Fact]
    public void DisplayCardRead_HomedCollocation_WordsHeadingCount()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry entry = TCardSenseSave(engine);
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        wing.CWingEntryOpen(entry.LEntryId);
        string heading = engine.TEngineTextRead("Display.CollocationHeading");

        CLecternCard card = wing.CWingDisplay.CDisplayCard.CDisplayCardRead();

        Assert.Equal(string.Format(CultureInfo.CurrentCulture, heading, 1), card.CLecternCardMeanings[0].CLeafHeading);
        Assert.Empty(card.CLecternCardMeanings[1].CLeafHeading);
    }

    [Fact]
    public void DisplayCardRead_HomedRow_MapsMarkers()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry entry = TCardSenseSave(engine);
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        wing.CWingEntryOpen(entry.LEntryId);
        string also = engine.TEngineTextRead("Display.CollocationAlso");

        CLecternCard card = wing.CWingDisplay.CDisplayCard.CDisplayCardRead();

        CLeaf home = Assert.Single(card.CLecternCardMeanings[0].CLeafCollocation);
        Assert.Equal("set off", home.CLeafPhrase.CStateWordingText);
        Assert.Equal(string.Format(CultureInfo.CurrentCulture, also, "2"), home.CLeafAlso);
        Assert.Empty(home.CLeafExample);
        Assert.Empty(home.CLeafFlag);
        Assert.Equal((false, true), (home.CLeafQuoted, home.CLeafShared));
        Assert.Empty(card.CLecternCardMeanings[0].CLeafAlso);
    }

    [Fact]
    public void CollocationLayoutRead_HomedCollocation_FillsMarkers()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntryDraft draft = TInterface.TEntryDraftCreate(
            "set",
            "English",
            string.Empty,
            string.Empty,
            [
                TInterface.TCardDraftCreate("to place", string.Empty, "a state", [], [], [], [], [], 1, 41),
                TInterface.TCardDraftCreate("to harden", string.Empty, "a change", [], [], [], [], [], 2, 42),
            ],
            [
                TInterface.TCardDraftCreate(
                    "set in",
                    string.Empty,
                    string.Empty,
                    [TInterfaceExample.TSentenceDraftCreate("Winter set in."),
                        TInterfaceExample.TSentenceDraftCreate("Doubt set in.")],
                    [], [], [], [], 1, 51) with
                {
                    LCardDraftSense = [41, 42, 41],
                },
            ]);
        Dictionary<long, IReadOnlyList<LTranslationTarget>> targets = new()
        {
            [51] = [new(1, "들다", "Korean"), new(2, "시작되다", "Korean"), new(3, "s'installer", "French")],
        };

        LCollocationLayout layout = engine.TEngineCollocationRead(draft, targets);

        Assert.Equal("set in", layout.LCollocationLayoutPhrase[51].TStateValueShow());
        Assert.Equal(["Korean", "French"], layout.LCollocationLayoutFlag[51]);
        Assert.Equal(2, layout.LCollocationLayoutExample[51]);
        Assert.Equal(["2"], layout.LCollocationLayoutAlso[51]);
    }

    private static LEntryDraft TCompassDraftCreate(params long[][] senses)
    {
        return TInterface.TEntryDraftCreate(
            "set",
            "English",
            string.Empty,
            string.Empty,
            [
                TInterface.TCardDraftCreate("to place", string.Empty, "a state", [], [], [], [], [], 1, 41),
                TInterface.TCardDraftCreate("to harden", string.Empty, "a change", [], [], [], [], [], 2, 42),
            ],
            senses.Select(static (sense, index) => TInterface.TCardDraftCreate(
                string.Empty,
                "set " + index.ToString(CultureInfo.InvariantCulture),
                string.Empty,
                [], [], [], [], [], index + 1, 51 + index) with
            {
                LCardDraftSense = sense,
            }).ToList());
    }

    private static LEntry TCardSenseSave(LEngine engine)
    {
        LCardDraft first = TInterface.TCardCreate("to place firmly", 1) with
        {
            LCardDraftId = TInterface.TIdentityCreate(),
        };
        LCardDraft second = TInterface.TCardCreate("to place loosely", 2) with
        {
            LCardDraftId = TInterface.TIdentityCreate(),
        };
        LCardDraft root = TInterface.TCardCreate("to place", 1) with
        {
            LCardDraftId = TInterface.TIdentityCreate(),
            LCardDraftChild = [first, second],
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
        LCardDraft unlinked = TInterface.TCardDraftCreate(
            LStateValue.LStateValueUnspecified,
            TInterfaceState.TStateValueCreate("set out"),
            LStateValue.LStateValueUnspecified,
            [], [], [], [], [], 2);
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "set", "English", string.Empty, string.Empty, [root, other], [linked, unlinked]));
    }
}
