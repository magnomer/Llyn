using System.Collections.Generic;
using System.Linq;
using Llyn.Application;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TDeckClerk
{
    [Fact]
    public void DeckFolderDelete_PositionTaken_AppendsDecks()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LDeckClerk clerk = TInterface.TDeckClerkCreate(workspace.TWorkspaceRigCreate());
        LDeckFolder folder = clerk.TDeckFolderCreate("Verbs");
        LDeck top = clerk.TDeckClerkCreate("Nouns", null, LGrain.LGrainMeaning);
        LDeck held = clerk.TDeckClerkCreate("Irregular", folder.LDeckFolderId, LGrain.LGrainMeaning);

        clerk.TDeckFolderDelete(folder.LDeckFolderId);

        IReadOnlyList<LDeck> decks = clerk.TDeckClerkRead();
        Assert.Equal([top.LDeckId, held.LDeckId], decks.Select(static deck => deck.LDeckId));
        Assert.Equal([0, 1], decks.Select(static deck => deck.LDeckPosition));
        Assert.All(decks, static deck => Assert.Null(deck.LDeckFolderId));
        Assert.Empty(clerk.TDeckFolderRead());
    }

    [Fact]
    public void DeckFolderUpdate_NewName_ReadsBackName()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LDeckClerk clerk = TInterface.TDeckClerkCreate(workspace.TWorkspaceRigCreate());
        LDeckFolder folder = clerk.TDeckFolderCreate("Verbs");

        clerk.TDeckFolderUpdate(folder.LDeckFolderId, " Phrasal verbs ");

        Assert.Equal("Phrasal verbs", Assert.Single(clerk.TDeckFolderRead()).LDeckFolderName);
    }

    [Fact]
    public void DeckOrderSet_FolderReversed_ReadsNewOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LDeckClerk clerk = TInterface.TDeckClerkCreate(workspace.TWorkspaceRigCreate());
        LDeckFolder folder = clerk.TDeckFolderCreate("Verbs");
        List<long> ids = new[] { "Regular", "Irregular", "Phrasal" }
            .Select(name => clerk.TDeckClerkCreate(name, folder.LDeckFolderId, LGrain.LGrainMeaning).LDeckId)
            .ToList();
        ids.Reverse();

        clerk.TDeckOrderSet(folder.LDeckFolderId, ids);

        Assert.Equal(ids, clerk.TDeckClerkRead().Select(static deck => deck.LDeckId));
    }

    [Fact]
    public void DeckClerkCreate_NameTaken_SuffixesCode()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LDeckClerk clerk = TInterface.TDeckClerkCreate(workspace.TWorkspaceRigCreate());
        clerk.TDeckClerkCreate("Verbs", null, LGrain.LGrainMeaning);

        LDeck second = clerk.TDeckClerkCreate("verbs", null, LGrain.LGrainMeaning);

        Assert.Equal("verbs 2", second.LDeckCode);
    }

    [Fact]
    public void DeckNameSet_Renamed_KeepsCode()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LDeckClerk clerk = TInterface.TDeckClerkCreate(workspace.TWorkspaceRigCreate());
        LDeck deck = clerk.TDeckClerkCreate("Verbs", null, LGrain.LGrainMeaning);

        clerk.TDeckNameSet(deck.LDeckId, "Phrasal verbs");

        LDeck stored = clerk.TDeckClerkRead(deck.LDeckId)!.Value.LDeckStored;
        Assert.Equal("Phrasal verbs", stored.LDeckName);
        Assert.Equal("Verbs", stored.LDeckCode);
    }

    [Fact]
    public void DeckClerkUpdate_CodeTaken_Refuses()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LDeckClerk clerk = TInterface.TDeckClerkCreate(workspace.TWorkspaceRigCreate());
        clerk.TDeckClerkCreate("Verbs", null, LGrain.LGrainMeaning);
        LDeck nouns = clerk.TDeckClerkCreate("Nouns", null, LGrain.LGrainMeaning);

        LRefusal refusal = Assert.Throws<LRefusal>(
            () => clerk.TDeckClerkUpdate(nouns with { LDeckCode = "VERBS" }, []));

        Assert.Equal(LRefusal.LRefusalCode, refusal.LRefusalReason);
        Assert.Equal("Nouns", clerk.TDeckClerkRead(nouns.LDeckId)!.Value.LDeckStored.LDeckCode);
    }

    [Fact]
    public void DeckClerkUpdate_OwnCode_SavesDeck()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LDeckClerk clerk = TInterface.TDeckClerkCreate(workspace.TWorkspaceRigCreate());
        LDeck deck = clerk.TDeckClerkCreate("Verbs", null, LGrain.LGrainMeaning);
        IReadOnlyList<LBearing> bearings = clerk.TDeckClerkRead(deck.LDeckId)!.Value.LDeckBearings;

        clerk.TDeckClerkUpdate(
            deck with { LDeckBasic = true, LDeckLead = LBearingLead.LBearingLeadAlternate },
            bearings);

        LDeck stored = clerk.TDeckClerkRead(deck.LDeckId)!.Value.LDeckStored;
        Assert.True(stored.LDeckBasic);
        Assert.Equal(LBearingLead.LBearingLeadAlternate, stored.LDeckLead);
        Assert.Equal("Verbs", stored.LDeckCode);
    }
}
