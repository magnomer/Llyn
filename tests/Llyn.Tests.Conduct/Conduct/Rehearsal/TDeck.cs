using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TDeck
{
    [Fact]
    public void DeckCreate_Coined_SelectsNewDeck()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCoinageCreate("Verbs", null, asked));

        deck.CDeckCreate(null, (int)LGrain.LGrainEntry);

        CDeckRow row = Assert.Single(deck.CDeckRead().CDeckRollDecks);
        Assert.Equal("Verbs", row.CDeckRowName);
        Assert.Equal((int)LGrain.LGrainEntry, row.CDeckRowGrain);
        Assert.True(row.CDeckRowChosen);
        Assert.Equal(["Coinage:Coinage.Deck"], asked);
    }

    [Fact]
    public void DeckCreate_CoinageDeclined_CreatesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCoinageCreate(null, null, asked));

        deck.CDeckCreate(null, (int)LGrain.LGrainEntry);

        Assert.True(deck.CDeckRead().CDeckRollEmpty);
        Assert.Equal(["Coinage:Coinage.Deck"], asked);
    }

    [Fact]
    public void DeckDelete_HoldsCues_AsksWithCount()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = new(engine);
        long stored = port.LEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        port.LEngineCueCreate(stored, []);
        port.LEngineCueCreate(stored, []);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyTallyCreate(true, asked));

        deck.CDeckDelete(stored);

        Assert.Equal(["Deck.DeleteConfirm>Deck.DeleteCount>2"], asked);
        Assert.True(deck.CDeckRead().CDeckRollEmpty);
    }

    [Fact]
    public void DeckDelete_Declined_KeepsDeck()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long stored = new LDeckOutlet(engine).LEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyTallyCreate(false, asked));

        deck.CDeckDelete(stored);

        Assert.Equal(["Deck.DeleteConfirm"], asked);
        Assert.Equal(stored, Assert.Single(deck.CDeckRead().CDeckRollDecks).CDeckRowId);
    }

    [Fact]
    public void DeckFolderUpdate_NewName_ReadsRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long folder = new LDeckOutlet(engine).LEngineFolderCreate("Grammar");
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));

        deck.CDeckFolderUpdate(folder, "Syntax");

        CDeckFolder row = Assert.Single(deck.CDeckRead().CDeckRollFolders);
        Assert.Equal(folder, row.CDeckFolderId);
        Assert.Equal("Syntax", row.CDeckFolderName);
        Assert.Empty(asked);
    }

    [Fact]
    public void CharterLeadSet_Alternate_SavesLead()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = new(engine);
        long stored = port.LEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);

        deck.CDeckCharter.CCharterLeadSet((int)LBearingLead.LBearingLeadAlternate);
        bool saved = deck.CDeckCharter.CCharterSave();

        Assert.True(saved);
        Assert.Equal(LBearingLead.LBearingLeadAlternate, port.LEngineDeckRead(stored)!.Value.LDeckStored.LDeckLead);
        Assert.Empty(asked);
    }

    [Fact]
    public void CharterBasicSet_On_SavesBasic()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = new(engine);
        long stored = port.LEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);

        deck.CDeckCharter.CCharterBasicSet(true);
        bool saved = deck.CDeckCharter.CCharterSave();

        Assert.True(saved);
        Assert.True(port.LEngineDeckRead(stored)!.Value.LDeckStored.LDeckBasic);
        Assert.Empty(asked);
    }
}
