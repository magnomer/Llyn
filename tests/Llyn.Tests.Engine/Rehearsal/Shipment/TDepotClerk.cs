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
    public async Task DepotClerkExport_LocalImage_ShipsFullPath()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TDepotFake depot = new();
        LRig rig = workspace.TWorkspaceRigCreate().TDepotRigCreate(depot);
        string image = Path.Combine(workspace.TWorkspaceFolder, "bank.png");
        LDeck deck = TDepotDeckCreate(rig, [TDepotCellCreate("bank", image)]);

        await TInterface.TDepotClerkCreate(rig, TInterface.TDepotLatchCreate()).TDepotClerkExport(deck.LDeckId);

        LDepotShipment shipment = Assert.Single(depot.TDepotFakeShipments);
        Assert.Equal(deck.LDeckId, shipment.LDepotShipmentDeck);
        Assert.Equal(deck.LDeckCode, shipment.LDepotShipmentCode);
        Assert.Equal([image], TDepotColumnRead(shipment, LCueColumn.LCueColumnPicture));
    }

    [Fact]
    public async Task DepotClerkExport_HttpImage_ShipsAddress()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TDepotFake depot = new();
        LRig rig = workspace.TWorkspaceRigCreate().TDepotRigCreate(depot);
        LDeck deck = TDepotDeckCreate(rig, [TDepotCellCreate("bank", "https://example.com/bank.png")]);

        await TInterface.TDepotClerkCreate(rig, TInterface.TDepotLatchCreate()).TDepotClerkExport(deck.LDeckId);

        LDepotShipment shipment = Assert.Single(depot.TDepotFakeShipments);
        Assert.Equal(["https://example.com/bank.png"], TDepotColumnRead(shipment, LCueColumn.LCueColumnPicture));
    }

    [Fact]
    public async Task DepotClerkExport_TrailUnresolved_ShipsTypedTextWithEmptyValue()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TDepotFake depot = new();
        LRig rig = workspace.TWorkspaceRigCreate().TDepotRigCreate(depot);
        LDeck deck = TDepotDeckCreate(rig, [TDepotCellCreate("bank", "/bank.png")]);

        await TInterface.TDepotClerkCreate(rig, TInterface.TDepotLatchCreate()).TDepotClerkExport(deck.LDeckId);

        LDepotShipment shipment = Assert.Single(depot.TDepotFakeShipments);
        Assert.Equal([string.Empty], TDepotColumnRead(shipment, LCueColumn.LCueColumnPicture));
        Assert.Equal(["/bank.png"], TDepotTypedRead(shipment, LCueColumn.LCueColumnPicture));
    }

    [Fact]
    public async Task DepotClerkExport_LocalImage_ShipsTypedText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TDepotFake depot = new();
        LRig rig = workspace.TWorkspaceRigCreate().TDepotRigCreate(depot);
        string image = Path.Combine(workspace.TWorkspaceFolder, "bank.png");
        LDeck deck = TDepotDeckCreate(rig, [TDepotCellCreate("bank", image)]);

        await TInterface.TDepotClerkCreate(rig, TInterface.TDepotLatchCreate()).TDepotClerkExport(deck.LDeckId);

        Assert.Equal([image], TDepotTypedRead(Assert.Single(depot.TDepotFakeShipments), LCueColumn.LCueColumnPicture));
    }

    [Fact]
    public async Task DepotClerkExport_HttpImage_ShipsNoTypedText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TDepotFake depot = new();
        LRig rig = workspace.TWorkspaceRigCreate().TDepotRigCreate(depot);
        LDeck deck = TDepotDeckCreate(rig, [TDepotCellCreate("bank", "https://example.com/bank.png")]);

        await TInterface.TDepotClerkCreate(rig, TInterface.TDepotLatchCreate()).TDepotClerkExport(deck.LDeckId);

        Assert.Equal([null], TDepotTypedRead(Assert.Single(depot.TDepotFakeShipments), LCueColumn.LCueColumnPicture));
    }

    [Fact]
    public async Task DepotClerkExport_Busy_RefusesSecondRun()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TDepotFake depot = new();
        LRig rig = workspace.TWorkspaceRigCreate().TDepotRigCreate(depot);
        LDeck deck = TDepotDeckCreate(rig, [[TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 1, "bank")]]);
        LDepotLatch latch = TInterface.TDepotLatchCreate();
        Assert.True(latch.TDepotLatchStart());

        LRefusal refusal = await Assert.ThrowsAsync<LRefusal>(
            () => TInterface.TDepotClerkCreate(rig, latch).TDepotClerkExport(deck.LDeckId));

        Assert.Equal(LRefusal.LRefusalBusy, refusal.LRefusalReason);
        Assert.Empty(depot.TDepotFakeShipments);
        Assert.True(latch.TDepotLatchCheck());
    }

    [Fact]
    public async Task DepotClerkExport_FolderUnset_RefusesDepotMissing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TDepotFake depot = new();
        LRig rig = workspace.TWorkspaceRigCreate().TDepotRigCreate(depot);
        LDeck deck = TDepotDeckCreate(rig, [[TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 1, "bank")]]);
        LDepotLatch latch = TInterface.TDepotLatchCreate();

        LRefusal refusal = await Assert.ThrowsAsync<LRefusal>(
            () => TInterface.TDepotClerkCreate(rig, latch, " ").TDepotClerkExport(deck.LDeckId));

        Assert.Equal(LRefusal.LRefusalDepot, refusal.LRefusalReason);
        Assert.Empty(depot.TDepotFakeShipments);
        Assert.False(latch.TDepotLatchCheck());
    }

    [Fact]
    public async Task DepotClerkExport_OtherWorkspaceBusy_Runs()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TDepotFake depot = new();
        LRig rig = workspace.TWorkspaceRigCreate().TDepotRigCreate(depot);
        LDeck deck = TDepotDeckCreate(rig, [[TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 1, "bank")]]);
        LDepotLatch other = TInterface.TDepotLatchCreate();
        Assert.True(other.TDepotLatchStart());
        LDepotLatch latch = TInterface.TDepotLatchCreate();

        await TInterface.TDepotClerkCreate(rig, latch).TDepotClerkExport(deck.LDeckId);

        Assert.Single(Assert.Single(depot.TDepotFakeShipments).LDepotShipmentFaces);
        Assert.False(latch.TDepotLatchCheck());
    }

    [Fact]
    public async Task DepotClerkExport_OrphanedCue_LeavesCueOut()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TDepotFake depot = new();
        LRig rig = workspace.TWorkspaceRigCreate().TDepotRigCreate(depot);
        LDeck deck = TDepotDeckCreate(rig, [[TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 1, "bank")]]);
        LCue orphan = TInterface.TCueCreate(LGrain.LGrainMeaning, null, null, null, null, null)
            with { LCueDeckId = deck.LDeckId };
        rig.LRigRehearsal.LRigRehearsalCues.TCueCreate(deck.LDeckId, [orphan]);
        long kept = rig.LRigRehearsal.LRigRehearsalCues.TCueRead(deck.LDeckId)
            .Single(static cue => cue.LCueGrain == LGrain.LGrainFree).LCueId;
        LDepotClerk clerk = TInterface.TDepotClerkCreate(rig, TInterface.TDepotLatchCreate());

        await clerk.TDepotClerkExport(deck.LDeckId);

        Assert.Equal([kept], Assert.Single(depot.TDepotFakeShipments).LDepotShipmentCues);
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

    private static IReadOnlyList<string?> TDepotTypedRead(LDepotShipment shipment, LCueColumn column) =>
        [
            .. shipment.LDepotShipmentFaces.Select((face, index) => shipment.LDepotShipmentTyped[index][
                face.LCueFaceCells.ToList().FindIndex(cell =>
                    cell.LCueCellColumn == column && cell.LCueCellIndex == 1)]),
        ];

    private static IReadOnlyList<string> TDepotColumnRead(LDepotShipment shipment, LCueColumn column) =>
        [
            .. shipment.LDepotShipmentFaces.Select(face => face.LCueFaceCells
                .First(cell => cell.LCueCellColumn == column && cell.LCueCellIndex == 1).LCueCellValue),
        ];
}
