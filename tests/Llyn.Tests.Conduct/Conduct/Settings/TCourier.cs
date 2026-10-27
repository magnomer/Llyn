using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCourier
{
    private const string TCourierLanguage = "Classical Chinese";

    [Fact]
    public void CourierRead_StoredToken_ShowsConnectedAfterOpen()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TCourierWarrantSave(workspace, "fake:token");
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CCourier courier = CCourier.CCourierCreate(atelier);
        Assert.False(courier.CCourierRead().CCourierStateAttached);

        atelier.TAtelierStubOpen();

        Assert.True(courier.CCourierRead().CCourierStateAttached);
    }

    [Fact]
    public void CourierRead_WorkspaceWithoutToken_ShowsUnconnected()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspacePrepare();
        TCourierWarrantSave(first, "fake:token");
        TCourierWarrantSave(second, string.Empty);
        using LEngine engine = new(first.TWorkspaceRigCreate(), _ => second.TWorkspaceRigCreate(), _ => { });
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CCourier courier = CCourier.CCourierCreate(atelier);
        atelier.TAtelierStubOpen();

        atelier.CAtelierWorkspace.CWorkspaceChange(second.TWorkspaceFolder, TEnvoyFake.TEnvoyCreate(false, []));

        Assert.False(courier.CCourierRead().CCourierStateAttached);
    }

    [Fact]
    public async Task CourierSend_RefusedToken_DropsTokenAndShowsConnect()
    {
        List<string> asked = [];
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TCourierWarrantSave(workspace, "unreadable");
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CCourier courier = CCourier.CCourierCreate(atelier);
        atelier.TAtelierStubOpen();
        List<CCourierState> shown = [];
        courier.CCourierChanged += shown.Add;

        await courier.CCourierSend(TEnvoyFake.TEnvoyCreate(false, asked));

        Assert.Equal(["Courier.SendFailed"], asked);
        Assert.Empty(engine.TEngineSettingsRead().LSettingsWarrant);
        Assert.Contains(shown, static state => state.CCourierStateBusy && !state.CCourierStateAttached);
        Assert.Equal(new CCourierState(false, true, false, string.Empty), courier.CCourierRead());
    }

    [Fact]
    public async Task CourierSend_StoredToken_HandsOutpostOneCargo()
    {
        TOutpostFake outpost = new();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TCourierWarrantSave(workspace, "fake:nekot");
        using LEngine engine = TCourierEngineStart(workspace, outpost);
        engine.TEngineNotebookSave("Lexicon");

        LReceipt receipt = await TCourierSend(engine);

        LOutpostCargo cargo = Assert.Single(outpost.TOutpostFakeCargo);
        Assert.Same(outpost.TOutpostFakeReceipt, receipt);
        Assert.Equal(
            (41184, "token", "Lexicon"),
            (cargo.LOutpostCargoPort, cargo.LOutpostCargoToken, cargo.LOutpostCargoNotebook));
        Assert.Equal(32, cargo.LOutpostCargoRealm.Length);
        Assert.Equal(["瀧"], cargo.LOutpostCargoEntries.Select(static entry => entry.LEntryHeadword));
        Assert.Equal("瀧", TInterface.TOutpostLookupRead(cargo, "瀧"));
    }

    [Fact]
    public async Task CourierSend_MissingToken_RefusesWithoutCargo()
    {
        TOutpostFake outpost = new();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TCourierWarrantSave(workspace, string.Empty);
        using LEngine engine = TCourierEngineStart(workspace, outpost);

        LRefusal refusal = await Assert.ThrowsAsync<LRefusal>(() => TCourierSend(engine));

        Assert.Equal(LRefusal.LRefusalMissing, refusal.LRefusalReason);
        Assert.Empty(outpost.TOutpostFakeCargo);
    }

    [Fact]
    public async Task CourierAttach_RawToken_AnswersHiddenToken()
    {
        TOutpostFake outpost = new();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TCourierWarrantSave(workspace, string.Empty);
        using LEngine engine = TCourierEngineStart(workspace, outpost);

        string hidden = await TInterface.TCourierAttach(engine);

        Assert.Equal("fake:nekot:war", hidden);
        Assert.Equal([41184], outpost.TOutpostFakePort);
    }

    private static LEngine TCourierEngineStart(TWorkspace workspace, TOutpostFake outpost)
    {
        LEngine engine = new(
            workspace.TWorkspaceRigCreate() with { LRigOutpost = outpost },
            _ => workspace.TWorkspaceRigCreate(),
            _ => { });
        engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "瀧",
            TCourierLanguage,
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a state", [], [], [], [], [], 1)],
            []));
        return engine;
    }

    private static Task<LReceipt> TCourierSend(LEngine engine)
    {
        return TInterface.TCourierSend(
            engine,
            engine.TLiveryRead,
            language => TInterface.TLiveryLanguageBuild(language, [], []));
    }

    private static void TCourierWarrantSave(TWorkspace workspace, string warrant)
    {
        File.WriteAllText(
            Path.Combine(workspace.TWorkspaceFolder, "settings.json"),
            "{ \"localization\": \"en\", \"warrant\": \"" + warrant + "\" }");
    }
}
