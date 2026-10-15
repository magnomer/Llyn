using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TAccession
{
    [Fact]
    public void AccessionRead_BoundEntry_ReadsBound()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        List<long> entries = TAccessionEntrySave(workspace, "bank", "river");
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long stored = TInterface.TDeckOutletCreate(engine).TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);
        deck.CDeckAccession.CAccessionAdd(entries[0]);

        IReadOnlyList<CAccessionRow> rows = deck.CDeckAccession.CAccessionRead();

        Assert.Equal(entries, rows.Select(static row => row.CAccessionRowEntry));
        Assert.Equal(["bank", "river"], rows.Select(static row => row.CAccessionRowHeadword));
        Assert.Equal([true, false], rows.Select(static row => row.CAccessionRowBound));
        Assert.False(deck.CDeckAccession.CAccessionAperture.CApertureEmpty);
        Assert.Empty(asked);
    }

    [Fact]
    public void AccessionAdd_Entry_AddsCue()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        List<long> entries = TAccessionEntrySave(workspace, "bank");
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long stored = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);
        int changed = 0;
        deck.CDeckAccession.CAccessionChanged += () => changed++;

        deck.CDeckAccession.CAccessionAdd(entries[0]);

        Assert.Equal(entries, port.TEngineCueRead(stored).Select(static cue => cue.LCueEntryId!.Value));
        Assert.Equal(1, changed);
        Assert.Empty(asked);
    }

    [Fact]
    public void AccessionShownAdd_ReadRows_AddsUnboundRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        List<long> entries = TAccessionEntrySave(workspace, "bank", "money", "river");
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long stored = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);
        deck.CDeckAccession.CAccessionAdd(entries[1]);
        deck.CDeckAccession.CAccessionRead();

        deck.CDeckAccession.CAccessionShownAdd();

        Assert.Equal(
            [entries[1], entries[0], entries[2]],
            port.TEngineCueRead(stored).Select(static cue => cue.LCueEntryId!.Value));
        Assert.Empty(asked);
    }

    [Fact]
    public void AccessionShownAdd_NothingRead_AddsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TAccessionEntrySave(workspace, "bank");
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long stored = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);
        int changed = 0;
        deck.CDeckAccession.CAccessionChanged += () => changed++;

        deck.CDeckAccession.CAccessionShownAdd();

        Assert.Empty(port.TEngineCueRead(stored));
        Assert.Equal(0, changed);
        Assert.Empty(asked);
    }

    [Fact]
    public void AccessionQuerySet_Headword_RaisesRowsChanged()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        List<long> entries = TAccessionEntrySave(workspace, "bank", "river");
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long stored = TInterface.TDeckOutletCreate(engine).TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);
        int changed = 0;
        deck.CDeckAccession.CAccessionAperture.CApertureRowsChanged += () => changed++;

        deck.CDeckAccession.CAccessionAperture.CApertureQuerySet("river");

        Assert.Equal(1, changed);
        Assert.Equal([entries[1]], deck.CDeckAccession.CAccessionRead().Select(static row => row.CAccessionRowEntry));
        Assert.Empty(asked);
    }

    private static List<long> TAccessionEntrySave(TWorkspace workspace, params string[] headwords)
    {
        LRig rig = workspace.TWorkspaceRigCreate();
        return headwords
            .Select(headword => TInterface.TEntryClerkCreate(rig).TEntryClerkSave(TInterface.TEntryDraftCreate(
                headword,
                "English",
                string.Empty,
                string.Empty,
                [TInterface.TCardDraftCreate(string.Empty, string.Empty, headword, [], [], [], [], [], 1)],
                [])).LEntryId)
            .ToList();
    }
}
