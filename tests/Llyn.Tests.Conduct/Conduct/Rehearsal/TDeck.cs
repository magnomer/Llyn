using System.Collections.Generic;
using System.Linq;
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

        deck.CDeckCreate(null);

        CDeckRow row = Assert.Single(deck.CDeckRead().CDeckRollDecks);
        Assert.Equal("Verbs", row.CDeckRowName);
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

        deck.CDeckCreate(null);

        Assert.True(deck.CDeckRead().CDeckRollEmpty);
        Assert.Equal(["Coinage:Coinage.Deck"], asked);
    }

    [Fact]
    public void DeckDelete_HoldsCues_AsksWithCount()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long stored = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        port.TEngineCueCreate(stored, []);
        port.TEngineCueCreate(stored, []);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyTallyCreate(true, asked));

        deck.CDeckDelete(stored);

        Assert.Equal(["Deck.DeleteConfirm>Deck.DeleteCount>2"], asked);
        Assert.True(deck.CDeckRead().CDeckRollEmpty);
    }

    [Fact]
    public void DeckRead_HoldsCues_CountsThem()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long stored = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        port.TEngineCueCreate(stored, []);
        port.TEngineCueCreate(stored, []);
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyTallyCreate(true, []));

        CDeckRow row = Assert.Single(deck.CDeckRead().CDeckRollDecks);

        Assert.Equal(2, row.CDeckRowCount);
        Assert.Equal("Deck.Count", row.CDeckRowKey);
    }

    [Fact]
    public void DeckRead_OneCue_UsesSingular()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long stored = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        port.TEngineCueCreate(stored, []);
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyTallyCreate(true, []));

        CDeckRow row = Assert.Single(deck.CDeckRead().CDeckRollDecks);

        Assert.Equal(1, row.CDeckRowCount);
        Assert.Equal("Deck.CountOne", row.CDeckRowKey);
    }

    [Fact]
    public void DeckDelete_Declined_KeepsDeck()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long stored = TInterface.TDeckOutletCreate(engine).TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
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
        long folder = TInterface.TDeckOutletCreate(engine).TEngineFolderCreate("Grammar");
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));

        deck.CDeckFolderUpdate(folder, "Syntax");

        CDeckFolder row = Assert.Single(deck.CDeckRead().CDeckRollFolders);
        Assert.Equal(folder, row.CDeckFolderId);
        Assert.Equal("Syntax", row.CDeckFolderName);
        Assert.Empty(asked);
    }

    [Fact]
    public void DeckOrderMove_Down_SwapsWithNext()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long first = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        long second = port.TEngineDeckCreate("Nouns", null, LGrain.LGrainEntry);
        long third = port.TEngineDeckCreate("Words", null, LGrain.LGrainEntry);
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyTallyCreate(true, []));

        deck.CDeckOrderMove(first, 1);

        Assert.Equal(
            [second, first, third], deck.CDeckRead().CDeckRollDecks.Select(static row => row.CDeckRowId));
    }

    [Fact]
    public void DeckOrderMove_Up_SwapsWithPrevious()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long first = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        long second = port.TEngineDeckCreate("Nouns", null, LGrain.LGrainEntry);
        long third = port.TEngineDeckCreate("Words", null, LGrain.LGrainEntry);
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyTallyCreate(true, []));

        deck.CDeckOrderMove(third, -1);

        Assert.Equal(
            [first, third, second], deck.CDeckRead().CDeckRollDecks.Select(static row => row.CDeckRowId));
    }

    [Fact]
    public void DeckOrderMove_DeckInFolder_SwapsWithinFolder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long grammar = port.TEngineFolderCreate("Grammar");
        long top = port.TEngineDeckCreate("Words", null, LGrain.LGrainEntry);
        long first = port.TEngineDeckCreate("Verbs", grammar, LGrain.LGrainEntry);
        long second = port.TEngineDeckCreate("Nouns", grammar, LGrain.LGrainEntry);
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyTallyCreate(true, []));

        deck.CDeckOrderMove(second, -1);

        IReadOnlyList<CDeckRow> rows = deck.CDeckRead().CDeckRollDecks;
        Assert.Equal(
            [second, first], rows.Where(row => row.CDeckRowFolder == grammar).Select(static row => row.CDeckRowId));
        Assert.Equal(
            [top], rows.Where(static row => row.CDeckRowFolder is null).Select(static row => row.CDeckRowId));
    }

    [Fact]
    public void DeckOrderMove_UnknownDeck_KeepsOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long first = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        long second = port.TEngineDeckCreate("Nouns", null, LGrain.LGrainEntry);
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyTallyCreate(true, []));

        deck.CDeckOrderMove(second + 100, 1);

        Assert.Equal(
            [first, second], deck.CDeckRead().CDeckRollDecks.Select(static row => row.CDeckRowId));
    }

    [Fact]
    public void DeckOrderMove_LastDown_KeepsOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long first = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        long second = port.TEngineDeckCreate("Nouns", null, LGrain.LGrainEntry);
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyTallyCreate(true, []));

        deck.CDeckOrderMove(second, 1);

        Assert.Equal(
            [first, second], deck.CDeckRead().CDeckRollDecks.Select(static row => row.CDeckRowId));
    }

    [Fact]
    public void DeckGrainRead_Creatable_ListsFiveGrains()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyTallyCreate(true, []));

        IReadOnlyList<CDeckGrain> grains = deck.CDeckGrainRead();

        Assert.Equal(
            ["Grain.Entry", "Grain.Meaning", "Grain.Collocation", "Grain.Example", "Grain.Pair"],
            grains.Select(static grain => grain.CDeckGrainKey));
        Assert.Equal([true, false, false, false, false], grains.Select(static grain => grain.CDeckGrainChosen));
    }

    [Fact]
    public void DeckGrainSelect_InRange_KeepsChoice()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyTallyCreate(true, []));

        deck.CDeckGrainSelect(2);

        Assert.Equal(
            [false, false, true, false, false], deck.CDeckGrainRead().Select(static grain => grain.CDeckGrainChosen));
        Assert.Equal(deck.CDeckGrainRead(), deck.CDeckRead().CDeckRollGrains);
    }

    [Fact]
    public void DeckGrainSelect_OutOfRange_KeepsChoice()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyTallyCreate(true, []));
        deck.CDeckGrainSelect(1);

        deck.CDeckGrainSelect(-1);
        deck.CDeckGrainSelect(5);

        Assert.Equal(
            [false, true, false, false, false], deck.CDeckGrainRead().Select(static grain => grain.CDeckGrainChosen));
    }

    [Fact]
    public void DeckCreate_GrainSelected_CreatesWithKeptGrain()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCoinageCreate("Verbs", null, []));
        deck.CDeckGrainSelect(4);

        deck.CDeckCreate(null);

        long created = Assert.Single(deck.CDeckRead().CDeckRollDecks).CDeckRowId;
        Assert.Equal(
            LGrain.LGrainPair,
            TInterface.TDeckOutletCreate(engine).TEngineDeckRead(created)!.Value.LDeckStored.LDeckGrain);
    }

    [Fact]
    public void DeckDestinationRead_DeckInFolder_ListsTopFirstThenOtherFolders()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long grammar = port.TEngineFolderCreate("Grammar");
        long idioms = port.TEngineFolderCreate("Idioms");
        long stored = port.TEngineDeckCreate("Verbs", grammar, LGrain.LGrainEntry);
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyTallyCreate(true, []));

        IReadOnlyList<CDeckDestination> destinations = deck.CDeckDestinationRead(stored);

        Assert.Equal(
            [(long?)null, idioms], destinations.Select(static place => place.CDeckDestinationFolder));
        Assert.Equal(["Deck.MoveTop", null], destinations.Select(static place => place.CDeckDestinationKey));
        Assert.Equal("Idioms", destinations[1].CDeckDestinationName);
        Assert.All(destinations, place => Assert.Equal(stored, place.CDeckDestinationDeck));
    }

    [Fact]
    public void DeckDestinationRead_DeckAtTopLevel_ListsFoldersOnly()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long grammar = port.TEngineFolderCreate("Grammar");
        long stored = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyTallyCreate(true, []));

        CDeckDestination place = Assert.Single(deck.CDeckDestinationRead(stored));

        Assert.Equal(grammar, place.CDeckDestinationFolder);
        Assert.Null(place.CDeckDestinationKey);
    }

    [Fact]
    public void DeckMove_Destination_MovesDeck()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long grammar = port.TEngineFolderCreate("Grammar");
        long stored = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyTallyCreate(true, []));

        deck.CDeckMove(Assert.Single(deck.CDeckDestinationRead(stored)));

        Assert.Equal(grammar, Assert.Single(deck.CDeckRead().CDeckRollDecks).CDeckRowFolder);
    }

    [Fact]
    public void DeckViewSet_Charter_ReadsCharterView()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyTallyCreate(true, []));
        int changed = 0;
        deck.CDeckChanged += () => changed++;

        deck.CDeckViewSet((int)CDeckView.CDeckViewCharter);
        deck.CDeckViewSet(3);

        Assert.Equal(CDeckView.CDeckViewCharter, deck.CDeckView);
        Assert.Equal(1, changed);
    }

    [Fact]
    public void DeckSelect_OtherDeck_KeepsView()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long first = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        long second = port.TEngineDeckCreate("Nouns", null, LGrain.LGrainEntry);
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyTallyCreate(true, []));
        deck.CDeckSelect(first);
        deck.CDeckViewSet((int)CDeckView.CDeckViewStencil);

        deck.CDeckSelect(second);

        Assert.Equal(CDeckView.CDeckViewStencil, deck.CDeckView);
    }
}
