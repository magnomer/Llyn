using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TStencil
{
    [Fact]
    public void StencilSlotSet_Body_RaisesChanged()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long stored = new LDeckOutlet(engine).LEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);
        int changed = 0;
        deck.CDeckStencil.CStencilChanged += () => changed++;

        deck.CDeckStencil.CStencilSlotSet((int)LCueColumn.LCueColumnNotes, 1, "{tag}");

        Assert.Equal(1, changed);
        Assert.Contains(
            new CCueCell((int)LCueColumn.LCueColumnNotes, 1, "{tag}"),
            deck.CDeckStencil.CStencilRead().CStencilStateSlots);
        Assert.Empty(asked);
    }

    [Fact]
    public void StencilApply_Saved_ReadsAppliedSlots()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = new(engine);
        long source = port.LEngineDeckCreate("Nouns", null, LGrain.LGrainEntry);
        port.LEngineSlotSet(source, new LStencilSlot(LCueColumn.LCueColumnNotes, 1, "{tag}"));
        port.LEngineStencilSave(source, "Tagged");
        long saved = Assert.Single(workspace.TWorkspaceColumnRead("SELECT stencil_id FROM stencil WHERE saved = 1;"));
        long stored = port.LEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);

        deck.CDeckStencil.CStencilApply(saved);

        Assert.Contains(
            new CCueCell((int)LCueColumn.LCueColumnNotes, 1, "{tag}"),
            deck.CDeckStencil.CStencilRead().CStencilStateSlots);
        Assert.Empty(asked);
    }
}
