using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TDepotClerk
{
    [Fact]
    public async Task DepotClerkExport_PhoneFilePending_Refuses()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TDepotFake depot = new();
        LRig rig = workspace.TWorkspaceRigCreate().TDepotRigCreate(depot, new TDeckManifestFake());
        LDeck deck = TDepotDeckCreate(rig, [[TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 1, "bank")]]);
        LDeckSheet phone = TInterface.TDeckSheetCreate([], ["Text 1"], [["shore"]]);
        depot.TDepotFakeFiles[deck.LDeckCode] = phone;
        depot.TDepotFakeDigest[deck.LDeckCode] = "phone";

        LRefusal refusal = await Assert.ThrowsAsync<LRefusal>(
            () => TInterface.TDepotClerkCreate(rig, TInterface.TDepotLatchCreate()).TDepotClerkExport(deck.LDeckId));

        Assert.Equal(LRefusal.LRefusalPhone, refusal.LRefusalReason);
        Assert.Same(phone, depot.TDepotFakeFiles[deck.LDeckCode]);
    }

    [Fact]
    public async Task DepotClerkExport_SameImage_CopiesOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TDepotFake depot = new();
        LRig rig = workspace.TWorkspaceRigCreate().TDepotRigCreate(depot, new TDeckManifestFake());
        string first = Path.Combine(workspace.TWorkspaceFolder, "bank.png");
        string second = Path.Combine(workspace.TWorkspaceFolder, "shore.png");
        depot.TDepotFakeNames[first] = "0123456789abcdef.png";
        depot.TDepotFakeNames[second] = "0123456789abcdef.png";
        LDeck deck = TDepotDeckCreate(rig, [TDepotCellCreate("bank", first), TDepotCellCreate("shore", second)]);
        LDepotClerk clerk = TInterface.TDepotClerkCreate(rig, TInterface.TDepotLatchCreate());

        LDeckReceipt receipt = await clerk.TDepotClerkExport(deck.LDeckId);

        Assert.Equal(["0123456789abcdef.png"], depot.TDepotFakeCopied);
        Assert.Equal(1, receipt.LDeckReceiptCopied);
        Assert.Equal(
            ["0123456789abcdef.png", "0123456789abcdef.png"],
            TDepotColumnRead(depot.TDepotFakeFiles[deck.LDeckCode], "Picture 1"));
    }

    [Fact]
    public async Task DepotClerkExport_DroppedImage_DeletesOnlyOwn()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TDepotFake depot = new();
        TDeckManifestFake manifest = new();
        LRig rig = workspace.TWorkspaceRigCreate().TDepotRigCreate(depot, manifest);
        LDeck deck = TDepotDeckCreate(rig, [[TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 1, "bank")]]);
        manifest.TDeckManifestSave(TInterface.TDeckManifestCreate(
            new Dictionary<long, (string, IReadOnlyList<string>, string)>
            {
                [deck.LDeckId] = (string.Empty, ["dropped.png"], "2026-01-01 00:00:00"),
            }));
        depot.TDepotFakeMedia.Add((deck.LDeckCode, "dropped.png"));
        depot.TDepotFakeMedia.Add((deck.LDeckCode, "phone.png"));
        LDepotClerk clerk = TInterface.TDepotClerkCreate(rig, TInterface.TDepotLatchCreate());

        LDeckReceipt receipt = await clerk.TDepotClerkExport(deck.LDeckId);

        Assert.Equal(["dropped.png"], depot.TDepotFakeDeleted);
        Assert.Equal(1, receipt.LDeckReceiptRemoved);
        Assert.Contains((deck.LDeckCode, "phone.png"), depot.TDepotFakeMedia);
        Assert.Empty(manifest.TDeckManifestStored.LDeckManifestDecks[deck.LDeckId].LDeckManifestMedia);
    }

    [Fact]
    public async Task DepotClerkExport_HttpImage_WritesAddress()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TDepotFake depot = new();
        LRig rig = workspace.TWorkspaceRigCreate().TDepotRigCreate(depot, new TDeckManifestFake());
        LDeck deck = TDepotDeckCreate(rig, [TDepotCellCreate("bank", "https://example.com/bank.png")]);

        await TInterface.TDepotClerkCreate(rig, TInterface.TDepotLatchCreate()).TDepotClerkExport(deck.LDeckId);

        Assert.Empty(depot.TDepotFakeCopied);
        Assert.Equal(
            ["https://example.com/bank.png"],
            TDepotColumnRead(depot.TDepotFakeFiles[deck.LDeckCode], "Picture 1"));
    }

    [Fact]
    public async Task DepotClerkExport_Busy_RefusesSecondRun()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TDepotFake depot = new();
        LRig rig = workspace.TWorkspaceRigCreate().TDepotRigCreate(depot, new TDeckManifestFake());
        LDeck deck = TDepotDeckCreate(rig, [[TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 1, "bank")]]);
        LDepotLatch latch = TInterface.TDepotLatchCreate();
        Assert.True(latch.TDepotLatchStart());

        LRefusal refusal = await Assert.ThrowsAsync<LRefusal>(
            () => TInterface.TDepotClerkCreate(rig, latch).TDepotClerkExport(deck.LDeckId));

        Assert.Equal(LRefusal.LRefusalBusy, refusal.LRefusalReason);
        Assert.Empty(depot.TDepotFakeFiles);
        Assert.True(latch.TDepotLatchCheck());
    }

    [Fact]
    public async Task DepotClerkExport_FolderUnset_RefusesDepotMissing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TDepotFake depot = new();
        LRig rig = workspace.TWorkspaceRigCreate().TDepotRigCreate(depot, new TDeckManifestFake());
        LDeck deck = TDepotDeckCreate(rig, [[TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 1, "bank")]]);
        LDepotLatch latch = TInterface.TDepotLatchCreate();

        LRefusal refusal = await Assert.ThrowsAsync<LRefusal>(
            () => TInterface.TDepotClerkCreate(rig, latch, " ").TDepotClerkExport(deck.LDeckId));

        Assert.Equal(LRefusal.LRefusalDepot, refusal.LRefusalReason);
        Assert.Empty(depot.TDepotFakeFiles);
        Assert.False(latch.TDepotLatchCheck());
    }

    [Fact]
    public async Task DepotClerkExport_OtherWorkspaceBusy_Runs()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TDepotFake depot = new();
        LRig rig = workspace.TWorkspaceRigCreate().TDepotRigCreate(depot, new TDeckManifestFake());
        LDeck deck = TDepotDeckCreate(rig, [[TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 1, "bank")]]);
        LDepotLatch other = TInterface.TDepotLatchCreate();
        Assert.True(other.TDepotLatchStart());
        LDepotLatch latch = TInterface.TDepotLatchCreate();

        LDeckReceipt receipt = await TInterface.TDepotClerkCreate(rig, latch).TDepotClerkExport(deck.LDeckId);

        Assert.Equal(1, receipt.LDeckReceiptCards);
        Assert.True(depot.TDepotFakeFiles.ContainsKey(deck.LDeckCode));
        Assert.False(latch.TDepotLatchCheck());
    }

    [Fact]
    public async Task DepotClerkExport_OrphanedCue_LeavesCueOut()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TDepotFake depot = new();
        LRig rig = workspace.TWorkspaceRigCreate().TDepotRigCreate(depot, new TDeckManifestFake());
        LDeck deck = TDepotDeckCreate(rig, [[TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 1, "bank")]]);
        LCue orphan = TInterface.TCueCreate(LGrain.LGrainMeaning, null, null, null, null, null)
            with { LCueDeckId = deck.LDeckId };
        rig.LRigRehearsal.LRigRehearsalCues.TCueCreate(deck.LDeckId, [orphan]);
        long kept = rig.LRigRehearsal.LRigRehearsalCues.TCueRead(deck.LDeckId)
            .Single(static cue => cue.LCueGrain == LGrain.LGrainFree).LCueId;
        LDepotClerk clerk = TInterface.TDepotClerkCreate(rig, TInterface.TDepotLatchCreate());

        LDeckReceipt receipt = await clerk.TDepotClerkExport(deck.LDeckId);

        Assert.Equal(1, receipt.LDeckReceiptCards);
        Assert.Equal([kept.ToString(System.Globalization.CultureInfo.InvariantCulture)],
            TDepotColumnRead(depot.TDepotFakeFiles[deck.LDeckCode], "ID"));
    }

    private static LDeck TDepotDeckCreate(LRig rig, IReadOnlyList<IReadOnlyList<LCueCell>> cues)
    {
        LDeck deck = TInterface.TDeckClerkCreate(rig).TDeckClerkCreate("Banks", null, LGrain.LGrainMeaning);
        LCueClerk clerk = TInterface.TCueClerkCreate(rig);
        foreach (IReadOnlyList<LCueCell> cells in cues)
        {
            clerk.TCueClerkCreate(deck.LDeckId, cells);
        }

        return deck;
    }

    private static IReadOnlyList<LCueCell> TDepotCellCreate(string text, string picture) =>
        [
            TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 1, text),
            TInterface.TCueCellCreate(LCueColumn.LCueColumnPicture, 1, picture),
        ];

    private static IReadOnlyList<string> TDepotColumnRead(LDeckSheet sheet, string column)
    {
        int index = sheet.LDeckSheetHeader.ToList().IndexOf(column);
        return [.. sheet.LDeckSheetRows.Select(row => row[index])];
    }
}
