using System.Collections.Generic;
using System.Linq;
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
        long stored = TInterface.TDeckOutletCreate(engine).TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);
        int changed = 0;
        deck.CDeckStencil.CStencilChanged += () => changed++;

        deck.CDeckStencil.CStencilSlotSet((int)LCueColumn.LCueColumnNotes, 1, "{tag}");

        Assert.Equal(1, changed);
        Assert.Contains(
            new CStencilSlot((int)LCueColumn.LCueColumnNotes, 1, "{tag}", "Stencil.Notes", string.Empty),
            deck.CDeckStencil.CStencilRead()?.CStencilStateSlots ?? []);
        Assert.Empty(asked);
    }

    [Fact]
    public void StencilApply_Saved_ReadsAppliedSlots()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long source = port.TEngineDeckCreate("Nouns", null, LGrain.LGrainEntry);
        port.TEngineSlotSet(source, TInterface.TStencilSlotCreate(LCueColumn.LCueColumnNotes, 1, "{tag}"));
        port.TEngineStencilSave(source, "Tagged");
        long saved = Assert.Single(workspace.TWorkspaceColumnRead("SELECT stencil_id FROM stencil WHERE saved = 1;"));
        long stored = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);

        deck.CDeckStencil.CStencilApply(saved);

        Assert.Contains(
            new CStencilSlot((int)LCueColumn.LCueColumnNotes, 1, "{tag}", "Stencil.Notes", string.Empty),
            deck.CDeckStencil.CStencilRead()?.CStencilStateSlots ?? []);
        Assert.Empty(asked);
    }

    [Fact]
    public void StencilRead_SavedStencil_ListsSaved()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long stored = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        port.TEngineStencilSave(stored, "Tagged");
        long saved = Assert.Single(workspace.TWorkspaceColumnRead("SELECT stencil_id FROM stencil WHERE saved = 1;"));
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);

        CStencilState? state = deck.CDeckStencil.CStencilRead();

        Assert.Equal(new CStencilRow(saved, "Tagged"), Assert.Single(state?.CStencilStateSaved ?? []));
        Assert.Empty(asked);
    }

    [Fact]
    public void StencilRead_NewDeck_OffersEverySlot()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long stored = TInterface.TDeckOutletCreate(engine).TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);

        IReadOnlyList<CStencilSlot> slots = deck.CDeckStencil.CStencilRead()?.CStencilStateSlots ?? [];

        Assert.Equal(21, slots.Count);
        Assert.Equal(
            new CStencilSlot((int)LCueColumn.LCueColumnText, 1, "{headword}", "Stencil.Side", "1"),
            slots[0]);
        Assert.Contains(
            new CStencilSlot((int)LCueColumn.LCueColumnCategory, 4, string.Empty, "Stencil.Category", "4"),
            slots);
        Assert.Empty(asked);
    }

    [Fact]
    public void StencilRead_NoDeck_ReadsNull()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, []));

        Assert.Null(deck.CDeckStencil.CStencilRead());
    }

    [Fact]
    public void StencilRead_LiteralBody_ReadsCustomSide()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long stored = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        port.TEngineSlotSet(stored, TInterface.TStencilSlotCreate(LCueColumn.LCueColumnText, 1, "Q: {headword}"));
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        deck.CDeckSelect(stored);

        CStencilSide side = (deck.CDeckStencil.CStencilRead()?.CStencilStateSides ?? [])[0];

        Assert.True(side.CStencilSideCustom);
        Assert.Empty(side.CStencilSideChips);
    }

    [Fact]
    public void StencilRead_FieldTokens_ReadsChips()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long stored = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        port.TEngineSlotSet(
            stored, TInterface.TStencilSlotCreate(LCueColumn.LCueColumnText, 1, "{headword}\n{forms:all}"));
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        deck.CDeckSelect(stored);

        CStencilSide side = (deck.CDeckStencil.CStencilRead()?.CStencilStateSides ?? [])[0];

        Assert.False(side.CStencilSideCustom);
        Assert.Equal(
            [
                new CStencilChip("Stencil.SlotHeadword", false),
                new CStencilChip("Stencil.SlotForms", true),
            ],
            side.CStencilSideChips);
    }

    [Fact]
    public void StencilFieldAppend_SecondSide_ReadsChip()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long stored = TInterface.TDeckOutletCreate(engine).TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);
        int changed = 0;
        deck.CDeckStencil.CStencilChanged += () => changed++;

        int gloss = (deck.CDeckStencil.CStencilRead()?.CStencilStateFields ?? [])
            .Select(static field => field.CStencilFieldKey)
            .ToList()
            .IndexOf("Stencil.SlotGloss");

        deck.CDeckStencil.CStencilFieldAppend(2, gloss);

        CStencilSide side = (deck.CDeckStencil.CStencilRead()?.CStencilStateSides ?? [])[1];
        Assert.Equal(
            new CStencilChip("Stencil.SlotGloss", false),
            side.CStencilSideChips[^1]);
        Assert.Equal(1, changed);
        Assert.Empty(asked);
    }

    [Fact]
    public void StencilFieldRemove_FirstPlace_DropsChip()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long stored = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        port.TEngineSlotSet(stored, TInterface.TStencilSlotCreate(LCueColumn.LCueColumnText, 1, "{headword}\n{forms}"));
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        deck.CDeckSelect(stored);

        deck.CDeckStencil.CStencilFieldRemove(1, 0);

        CStencilSide side = (deck.CDeckStencil.CStencilRead()?.CStencilStateSides ?? [])[0];
        Assert.Equal(
            new CStencilChip("Stencil.SlotForms", false),
            Assert.Single(side.CStencilSideChips));
    }

    [Fact]
    public void StencilJoinSet_All_ReadsAllJoin()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long stored = TInterface.TDeckOutletCreate(engine).TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        deck.CDeckSelect(stored);

        deck.CDeckStencil.CStencilJoinSet(1, 0, 1);

        CStencilSide side = (deck.CDeckStencil.CStencilRead()?.CStencilStateSides ?? [])[0];
        Assert.True(Assert.Single(side.CStencilSideChips).CStencilChipJoin);
    }

    [Fact]
    public void StencilMediaSet_SoundSideTwo_ReadsSoundSide()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long stored = TInterface.TDeckOutletCreate(engine).TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        deck.CDeckSelect(stored);
        deck.CDeckStencil.CStencilMediaSet((int)LCueColumn.LCueColumnSound, 0);
        int unset = deck.CDeckStencil.CStencilRead()?.CStencilStateSound ?? -1;

        deck.CDeckStencil.CStencilMediaSet((int)LCueColumn.LCueColumnSound, 2);

        Assert.Equal(0, unset);
        Assert.Equal(2, deck.CDeckStencil.CStencilRead()?.CStencilStateSound);
    }

    [Fact]
    public void StencilPictureSet_SideTwo_ReadsPictureSide()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long stored = TInterface.TDeckOutletCreate(engine).TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        deck.CDeckSelect(stored);

        deck.CDeckStencil.CStencilPictureSet(2);

        Assert.Equal(2, deck.CDeckStencil.CStencilRead()?.CStencilStatePicture);
    }

    [Fact]
    public void StencilSoundSet_SideTwo_ReadsSoundSide()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long stored = TInterface.TDeckOutletCreate(engine).TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        deck.CDeckSelect(stored);

        deck.CDeckStencil.CStencilSoundSet(2);

        Assert.Equal(2, deck.CDeckStencil.CStencilRead()?.CStencilStateSound);
    }

    [Fact]
    public void StencilRead_MeaningDeck_ReadsGrain()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long stored = TInterface.TDeckOutletCreate(engine).TEngineDeckCreate("Verbs", null, LGrain.LGrainMeaning);
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        deck.CDeckSelect(stored);

        Assert.Equal("Grain.Meaning", deck.CDeckStencil.CStencilRead()?.CStencilStateGrain);
    }

    [Fact]
    public void StencilTextToggle_Twice_ReadsUnchecked()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        int changed = 0;
        deck.CDeckStencil.CStencilChanged += () => changed++;

        deck.CDeckStencil.CStencilTextToggle();
        bool once = deck.CDeckStencil.CStencilTextChecked;
        deck.CDeckStencil.CStencilTextToggle();

        Assert.True(once);
        Assert.False(deck.CDeckStencil.CStencilTextChecked);
        Assert.Equal(2, changed);
    }

    [Fact]
    public void StencilFieldAppend_Throws_ShowsSaveFailed()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierFaultCreate(engine, "LDeckPort.LEngineFieldAppend", true);
        long stored = TInterface.TDeckOutletCreate(engine).TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);
        int changed = 0;
        deck.CDeckStencil.CStencilChanged += () => changed++;

        int gloss = (deck.CDeckStencil.CStencilRead()?.CStencilStateFields ?? [])
            .Select(static field => field.CStencilFieldKey)
            .ToList()
            .IndexOf("Stencil.SlotGloss");

        deck.CDeckStencil.CStencilFieldAppend(1, gloss);

        Assert.Equal(["Stencil.SaveFailed"], asked);
        Assert.Equal(0, changed);
    }
}
