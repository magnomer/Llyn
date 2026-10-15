using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TDepot
{
    private const string TDepotFolder = "Phone";

    [Fact]
    public async Task DepotExport_Running_RefusesSecondRun()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long deck = TDepotDeckCreate(atelier, engine);
        CDepot depot = new(atelier);
        Assert.True(engine.TDepotLatchRead().TDepotLatchStart());
        List<string> asked = [];

        await depot.CDepotExport(deck, TEnvoyFake.TEnvoyFolderCreate(null, asked));

        Assert.Equal(["Deck.ExportFailed>Refusal.DepotBusy"], asked);
        Assert.True(depot.CDepotRead().CDepotStateBusy);
        Assert.Empty(Directory.GetFiles(Path.Combine(workspace.TWorkspaceFolder, TDepotFolder)));
    }

    [Fact]
    public async Task DepotExport_Throws_ShowsExportFailed()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierFaultCreate(engine, "LDeckPort.LEngineDeckExport", false);
        CDepot depot = new(atelier);
        List<CDepotState> shown = [];
        depot.CDepotChanged += shown.Add;
        List<string> asked = [];

        await depot.CDepotExport(1, TEnvoyFake.TEnvoyCreate(false, asked));

        Assert.Equal(["Deck.ExportFailed"], asked);
        Assert.Equal(
            [new CDepotState(false, engine.TEngineTextRead("Deck.Exporting")), new CDepotState(false, string.Empty)],
            shown);
    }

    [Fact]
    public async Task DepotExport_PhonePending_ShowsRefusal()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long deck = TDepotDeckCreate(atelier, engine);
        CDepot depot = new(atelier);
        List<string> asked = [];
        await depot.CDepotExport(deck, TEnvoyFake.TEnvoyFolderCreate(null, asked));
        Assert.Empty(asked);
        Assert.NotEmpty(depot.CDepotRead().CDepotStateLine);
        File.AppendAllText(
            Assert.Single(Directory.GetFiles(Path.Combine(workspace.TWorkspaceFolder, TDepotFolder))), "shore");

        await depot.CDepotExport(deck, TEnvoyFake.TEnvoyFolderCreate(null, asked));

        Assert.Equal(["Deck.ExportFailed>Refusal.DeckPhonePending"], asked);
        Assert.Equal(new CDepotState(false, string.Empty), depot.CDepotRead());
    }

    private static long TDepotDeckCreate(CAtelier atelier, LEngine engine)
    {
        Directory.CreateDirectory(Path.Combine(atelier.CAtelierPathRead(), TDepotFolder));
        Assert.Equal(
            TDepotFolder,
            atelier.CAtelierLedger.CLedgerDepotSave(TDepotFolder, TEnvoyFake.TEnvoyCreate(false, [])));
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long deck = port.TEngineDeckCreate("Banks", null, LGrain.LGrainEntry);
        port.TEngineCueCreate(deck, [TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 1, "bank")]);
        return deck;
    }
}
