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
        LDeckOutlet port = new(engine);
        long stored = port.LEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        long first = port.LEngineCueCreate(stored, []);
        long second = port.LEngineCueCreate(stored, []);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);

        deck.CDeckCue.CCueRemove([first, second]);

        Assert.Equal(["Cue.RemoveConfirm"], asked);
        Assert.Equal([first, second], port.LEngineCueRead(stored).Select(static cue => cue.LCueId));
    }

    [Fact]
    public void CueAdd_ThreeEntries_ReadsNewRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        List<long> entries = new[] { "bank", "river", "money" }
            .Select(headword => TInterface.TEntryClerkCreate(rig).TEntryClerkSave(TInterface.TEntryDraftCreate(
                headword,
                "English",
                string.Empty,
                string.Empty,
                [TInterface.TCardDraftCreate(string.Empty, string.Empty, headword, [], [], [], [], [], 1)],
                [])).LEntryId)
            .ToList();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = new(engine);
        long stored = port.LEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);
        int changed = 0;
        deck.CDeckCue.CCueChanged += () => changed++;

        deck.CDeckCue.CCueAdd(entries);

        Assert.Equal(entries, port.LEngineCueRead(stored).Select(static cue => cue.LCueEntryId!.Value));
        Assert.Equal(1, changed);
        Assert.Empty(asked);
    }

    [Fact]
    public void CueCellSet_FreeCue_ReadsCell()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = new(engine);
        long stored = port.LEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        long cue = port.LEngineCueCreate(stored, []);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));

        deck.CDeckCue.CCueCellSet(cue, (int)LCueColumn.LCueColumnText, 2, "river");

        Assert.Equal(
            new CCueCell((int)LCueColumn.LCueColumnText, 2, "river"),
            Assert.Single(deck.CDeckCue.CCueCellRead(cue)));
        Assert.Empty(asked);
    }

    [Fact]
    public void CueCellOpen_Declined_KeepsCell()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = new(engine);
        long stored = port.LEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        long cue = port.LEngineCueCreate(stored, [new LCueCell(LCueColumn.LCueColumnPicture, 1, "old.png")]);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyMediaCreate(null, asked));

        deck.CDeckCue.CCueCellOpen(cue, (int)LCueColumn.LCueColumnPicture, 1);

        Assert.Equal(["Media:Stencil.Picture"], asked);
        Assert.Equal(
            new CCueCell((int)LCueColumn.LCueColumnPicture, 1, "old.png"),
            Assert.Single(deck.CDeckCue.CCueCellRead(cue)));
    }
}
