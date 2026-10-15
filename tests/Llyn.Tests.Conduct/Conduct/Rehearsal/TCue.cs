using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCue
{
    [Fact]
    public void CueRemove_Declined_KeepsCues()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long stored = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        long first = port.TEngineCueCreate(stored, []);
        long second = port.TEngineCueCreate(stored, []);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);

        deck.CDeckCue.CCueRemove([first, second]);

        Assert.Equal(["Cue.RemoveConfirm"], asked);
        Assert.Equal([first, second], port.TEngineCueRead(stored).Select(static cue => cue.LCueId));
    }

    [Fact]
    public void CueRead_TwoCues_ReadsEngineOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long stored = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        long first = port.TEngineCueCreate(stored, []);
        long second = port.TEngineCueCreate(stored, []);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);

        IReadOnlyList<CCueRow> rows = deck.CDeckCue.CCueRead();

        Assert.Equal([first, second], rows.Select(static row => row.CCueRowId));
        Assert.All(rows, static row => Assert.Empty(row.CCueRowLine));
        Assert.Empty(asked);
    }

    [Fact]
    public void CueRead_NoDeckChosen_ReturnsNoRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long stored = TInterface.TDeckOutletCreate(engine).TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        TInterface.TDeckOutletCreate(engine).TEngineCueCreate(stored, []);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));

        IReadOnlyList<CCueRow> rows = deck.CDeckCue.CCueRead();

        Assert.Empty(rows);
        Assert.Empty(asked);
    }

    [Fact]
    public void CueRead_ReadThrows_ShowsLoadFailed()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long stored = TInterface.TDeckOutletCreate(engine).TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        using CAtelier atelier = TInterfaceConduct.TAtelierFaultCreate(engine, "LDeckPort.LEngineLineRead", false);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);

        IReadOnlyList<CCueRow> rows = deck.CDeckCue.CCueRead();

        Assert.Empty(rows);
        Assert.Equal(["Deck.LoadFailed"], asked);
    }

    [Fact]
    public void CueRead_FlaggedCue_ReadsFlagTrue()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long stored = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        long cue = port.TEngineCueCreate(stored, []);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);
        deck.CDeckCue.CCueSet(cue, true, false);

        CCueRow row = Assert.Single(deck.CDeckCue.CCueRead());

        Assert.True(row.CCueRowFlag);
        Assert.Empty(asked);
    }

    [Fact]
    public void CueSet_FlagChecked_StoresFlagOne()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long stored = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        long cue = port.TEngineCueCreate(stored, []);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        int changed = 0;
        deck.CDeckCue.CCueChanged += () => changed++;

        deck.CDeckCue.CCueSet(cue, true, true);

        LCue written = Assert.Single(port.TEngineCueRead(stored));
        Assert.Equal(1, written.LCueFlag);
        Assert.True(written.LCueExcluded);
        Assert.Equal(1, changed);
        Assert.Empty(asked);
    }

    [Fact]
    public void CueSet_FlagCleared_StoresFlagZero()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long stored = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        long cue = port.TEngineCueCreate(stored, []);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckCue.CCueSet(cue, true, true);

        deck.CDeckCue.CCueSet(cue, false, false);

        LCue written = Assert.Single(port.TEngineCueRead(stored));
        Assert.Equal(0, written.LCueFlag);
        Assert.False(written.LCueExcluded);
        Assert.Empty(asked);
    }

    [Fact]
    public void CueCellSet_FreeCue_ReadsCell()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long stored = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        long cue = port.TEngineCueCreate(stored, []);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);

        deck.CDeckCue.CCueCellSet(cue, (int)LCueColumn.LCueColumnText, 2, "river");

        Assert.Equal(
            "river",
            Assert.Single(
                deck.CDeckCue.CCueCellRead([cue])!,
                static cell => cell.CCueCellColumn == (int)LCueColumn.LCueColumnText
                    && cell.CCueCellIndex == 2).CCueCellValue);
        Assert.Empty(asked);
    }

    [Fact]
    public void CueCellOpen_Declined_KeepsCell()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long stored = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        long cue = port.TEngineCueCreate(
            stored, [TInterface.TCueCellCreate(LCueColumn.LCueColumnPicture, 1, "old.png")]);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyMediaCreate(null, asked));
        deck.CDeckSelect(stored);

        deck.CDeckCue.CCueCellOpen(cue, (int)LCueColumn.LCueColumnPicture, 1);

        Assert.Equal(["Media:Stencil.Picture"], asked);
        Assert.Equal(
            "old.png",
            Assert.Single(
                deck.CDeckCue.CCueCellRead([cue])!,
                static cell => cell.CCueCellColumn == (int)LCueColumn.LCueColumnPicture
                    && cell.CCueCellIndex == 1).CCueCellValue);
    }

    [Fact]
    public void CueCellRead_FreshCue_MarksMediaCells()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long stored = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        long cue = port.TEngineCueCreate(stored, []);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);

        IReadOnlyList<CCueCell> cells = deck.CDeckCue.CCueCellRead([cue])!;

        Assert.Equal(5, cells.Count(static cell => cell.CCueCellColumn == (int)LCueColumn.LCueColumnText));
        Assert.All(cells, static cell => Assert.Empty(cell.CCueCellValue));
        Assert.Equal(
            [(int)LCueColumn.LCueColumnPicture, (int)LCueColumn.LCueColumnSound],
            cells.Where(static cell => cell.CCueCellFile).Select(static cell => cell.CCueCellColumn).Distinct());
        Assert.Empty(asked);
    }

    [Fact]
    public void CueCellSet_BoundCue_ShowsSaveFailed()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        long entry = TInterface.TEntryClerkCreate(rig).TEntryClerkSave(TInterface.TEntryDraftCreate(
            "bank",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "bank", [], [], [], [], [], 1)],
            [])).LEntryId;
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long stored = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);
        port.TEngineCueAdd(stored, [entry]);
        CCueRow bound = Assert.Single(deck.CDeckCue.CCueRead());

        deck.CDeckCue.CCueCellSet(bound.CCueRowId, (int)LCueColumn.LCueColumnText, 1, "river");

        Assert.Null(deck.CDeckCue.CCueCellRead([bound.CCueRowId]));
        Assert.Equal(["Deck.SaveFailed"], asked);
    }

    [Fact]
    public void CueCellRead_TwoCues_ReturnsNull()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long stored = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        long first = port.TEngineCueCreate(stored, []);
        long second = port.TEngineCueCreate(stored, []);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);

        IReadOnlyList<CCueCell>? cells = deck.CDeckCue.CCueCellRead([first, second]);

        Assert.Null(cells);
        Assert.Empty(asked);
    }
}
