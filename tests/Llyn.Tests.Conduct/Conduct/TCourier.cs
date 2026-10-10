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

    private const string TCourierStray = "Stray";

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
    public async Task CourierSend_RefusedToken_DropsTheTokenAndShowsConnect()
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
    public async Task CourierSend_FirstPush_SavesNotebooksAndReconstructionNotes()
    {
        TOutpostFake outpost = new();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TCourierEngineStart(workspace, outpost);

        LReceipt receipt = await TCourierSeriesSend(engine, true);

        Assert.Equal((2, 0, 0), (receipt.LReceiptSaved, receipt.LReceiptKept, receipt.LReceiptRemoved));
        LOutpostNote series = Assert.Single(
            outpost.TOutpostFakeNote.Values, static note => note.LOutpostNoteTitle == "龍");
        Assert.Equal("Navigation.Xiesheng", outpost.TOutpostFakeFolder[series.LOutpostNoteFolder].TOutpostFakeTitle);
        Assert.Contains(series.LOutpostNoteId, outpost.TOutpostFakeSaved);
        Assert.DoesNotContain(
            outpost.TOutpostFakeFolder.Values, static folder => folder.TOutpostFakeTitle == "Navigation.Yunjing");
    }

    [Fact]
    public async Task CourierSend_SecondPush_KeepsTheReconstructionNotes()
    {
        TOutpostFake outpost = new();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TCourierEngineStart(workspace, outpost);
        await TCourierSeriesSend(engine, true);
        outpost.TOutpostFakeSaved.Clear();

        LReceipt receipt = await TCourierSeriesSend(engine, true);

        Assert.Equal((0, 2, 0), (receipt.LReceiptSaved, receipt.LReceiptKept, receipt.LReceiptRemoved));
        Assert.Empty(outpost.TOutpostFakeSaved);
        Assert.Contains(outpost.TOutpostFakeNote.Values, static note => note.LOutpostNoteTitle == "龍");
    }

    [Fact]
    public async Task CourierSend_GoneSeries_TrashesItsNote()
    {
        TOutpostFake outpost = new();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TCourierEngineStart(workspace, outpost);
        await TCourierSeriesSend(engine, true);
        string series = outpost.TOutpostFakeNote.Values
            .Single(static note => note.LOutpostNoteTitle == "龍").LOutpostNoteId;

        await TCourierSeriesSend(engine, false);

        Assert.Equal([series], outpost.TOutpostFakeTrash);
        Assert.DoesNotContain(series, outpost.TOutpostFakeNote.Keys);
    }

    [Fact]
    public async Task CourierSend_StoredSeries_LinksTheEntryChipToItsNote()
    {
        TOutpostFake outpost = new();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TCourierEngineStart(workspace, outpost);

        await TCourierSeriesSend(engine, true);

        string series = outpost.TOutpostFakeNote.Values
            .Single(static note => note.LOutpostNoteTitle == "龍").LOutpostNoteId;
        string body = outpost.TOutpostFakeNote.Values
            .Single(static note => note.LOutpostNoteTitle == "瀧").LOutpostNoteBody;
        Assert.Contains("[<span class=\"llyn-stem\">龍</span>](:/" + series + ")", body);
    }

    [Fact]
    public async Task CourierSend_UnsentSeries_LeavesTheEntryChipPlain()
    {
        TOutpostFake outpost = new();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TCourierEngineStart(workspace, outpost);

        await TCourierSeriesSend(engine, true);

        string body = outpost.TOutpostFakeNote.Values
            .Single(static note => note.LOutpostNoteTitle == "瀧").LOutpostNoteBody;
        Assert.Contains("<span class=\"llyn-stem\">瀧</span>", body);
        Assert.DoesNotContain("[<span class=\"llyn-stem\">瀧</span>]", body);
    }

    [Fact]
    public async Task CourierSend_LanguageGoneFromEntriesAndScan_GuardRefusesToTrashItsSeriesNote()
    {
        TOutpostFake outpost = new();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TCourierEngineStart(workspace, outpost);
        LEntry stray = TCourierEntrySave(engine, "江", TCourierStray);
        await TCourierSeriesSend(engine, true);
        string shelf = TCourierFolderFind(outpost, TCourierStray, TCourierFolderFind(outpost, "Llyn"));
        string series = TCourierNoteFind(outpost, "龍", TCourierFolderFind(outpost, "Navigation.Xiesheng", shelf))
            .LOutpostNoteId;
        engine.TEngineEntryDelete(stray.LEntryId);

        LReceipt receipt = await TCourierSeriesSend(engine, true);

        Assert.Contains(series, outpost.TOutpostFakeTrash);
        Assert.Contains(series, outpost.TOutpostFakeNote.Keys);
        Assert.Equal(0, receipt.LReceiptRemoved);
    }

    [Fact]
    public async Task CourierSend_InitialRimeAndToneCategories_FilesEachNoteInItsNotebookAndLinksTheEntryChips()
    {
        TOutpostFake outpost = new();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TCourierEngineStart(workspace, outpost);
        LFanqieRow row = TInterface.TFanqieRowCreate(
            "瀧", "Book", "text", initial: "來", rime: "寒", division: "一", toneClass: "1");
        LFanqieGroup group = TInterface.TFanqieGroupCreate(string.Empty, "Book", "Wiki", [row], []);

        await TInterface.TCourierSend(
            engine,
            id => engine.TLiveryRead(id) is LLiveryPage page ? page with { LLiveryPageFanqie = [group] } : null,
            language => TInterface.TLiveryLanguageBuild(
                language,
                [],
                [
                    TInterface.TLiveryDiweiCreate(LDiwei.LDiweiInitial, language, "來", []),
                    TInterface.TLiveryDiweiCreate(LDiwei.LDiweiRime, language, row.LFanqieRowCell, []),
                    TInterface.TLiveryDiweiCreate(LDiwei.LDiweiTone, language, "1", []),
                ]));

        string shelf = TCourierFolderFind(outpost, TCourierLanguage);
        string table = TCourierFolderFind(outpost, "Navigation.Yunjing");
        string onset = TCourierFolderFind(outpost, "Yunjing.Shengmu");
        string rimes = TCourierFolderFind(outpost, "Yunjing.Yunmu");
        Assert.Equal(
            (shelf, table, table),
            (outpost.TOutpostFakeFolder[table].TOutpostFakeParent,
                outpost.TOutpostFakeFolder[onset].TOutpostFakeParent,
                outpost.TOutpostFakeFolder[rimes].TOutpostFakeParent));
        LOutpostNote initial = TCourierNoteFind(outpost, "來");
        LOutpostNote rime = TCourierNoteFind(outpost, row.LFanqieRowCell);
        LOutpostNote tone = TCourierNoteFind(outpost, "Display.FanqieTone");
        Assert.Equal(
            (onset, rimes, table),
            (initial.LOutpostNoteFolder, rime.LOutpostNoteFolder, tone.LOutpostNoteFolder));
        string body = TCourierNoteFind(outpost, "瀧").LOutpostNoteBody;
        Assert.Contains("[<span class=\"llyn-initial\">來</span>](:/" + initial.LOutpostNoteId + ")", body);
        Assert.Contains(
            "[<span class=\"llyn-rime\">" + row.LFanqieRowCell + "</span>](:/" + rime.LOutpostNoteId + ")", body);
        Assert.Contains(
            "[<span class=\"llyn-tone\">Display.FanqieTone</span>](:/" + tone.LOutpostNoteId + ")", body);
    }

    [Fact]
    public async Task CourierSend_SeriesNoteFailsToSave_ListsItsTitleAndLeavesTheEntryChipPlain()
    {
        TOutpostFake outpost = new();
        outpost.TOutpostFakeRefusal.Add("龍");
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TCourierEngineStart(workspace, outpost);

        LReceipt receipt = await TCourierSeriesSend(engine, true);

        Assert.Equal((1, 0, 0), (receipt.LReceiptSaved, receipt.LReceiptKept, receipt.LReceiptRemoved));
        Assert.Equal(["龍"], receipt.LReceiptFailed);
        string body = TCourierNoteFind(outpost, "瀧").LOutpostNoteBody;
        Assert.Contains("<span class=\"llyn-stem\">龍</span>", body);
        Assert.DoesNotContain("[<span class=\"llyn-stem\">龍</span>]", body);
    }

    [Fact]
    public async Task CourierSend_LanguageFailsToRead_SendsItsEntriesWithoutItsNotesAndTrashesNothing()
    {
        TOutpostFake outpost = new();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TCourierEngineStart(workspace, outpost);
        TCourierEntrySave(engine, "江", TCourierStray);
        LReceipt first = await TCourierSeriesSend(engine, true);
        int sent = outpost.TOutpostFakeSaved.Count;
        string shelf = TCourierFolderFind(outpost, TCourierLanguage, TCourierFolderFind(outpost, "Llyn"));
        string series = TCourierNoteFind(outpost, "龍", TCourierFolderFind(outpost, "Navigation.Xiesheng", shelf))
            .LOutpostNoteId;
        string entry = TCourierNoteFind(outpost, "瀧").LOutpostNoteId;
        outpost.TOutpostFakeSaved.Clear();
        outpost.TOutpostFakeFolder.Clear();

        LReceipt unread = await TCourierSeriesSend(engine, true, TCourierLanguage);

        Assert.Equal((4, 5), (first.LReceiptSaved, sent));
        Assert.Equal((1, 2, 0), (unread.LReceiptSaved, unread.LReceiptKept, unread.LReceiptRemoved));
        Assert.Equal([entry], outpost.TOutpostFakeSaved);
        Assert.DoesNotContain(outpost.TOutpostFakeFolder.Values, folder => folder.TOutpostFakeParent == shelf);
        Assert.Empty(outpost.TOutpostFakeTrash);
        Assert.Contains(series, outpost.TOutpostFakeNote.Keys);
        outpost.TOutpostFakeSaved.Clear();
        await TCourierSeriesSend(engine, true);
        Assert.DoesNotContain(series, outpost.TOutpostFakeSaved);
    }

    private static LEngine TCourierEngineStart(TWorkspace workspace, TOutpostFake outpost)
    {
        TCourierWarrantSave(workspace, "fake:nekot");
        LEngine engine = new(
            workspace.TWorkspaceRigCreate() with { LRigOutpost = outpost },
            _ => workspace.TWorkspaceRigCreate(),
            _ => { });
        TCourierEntrySave(engine, "瀧", TCourierLanguage);
        return engine;
    }

    private static LEntry TCourierEntrySave(LEngine engine, string headword, string language)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword,
            language,
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a state", [], [], [], [], [], 1)],
            []));
    }

    private static Task<LReceipt> TCourierSeriesSend(LEngine engine, bool series, string unread = "")
    {
        LFanqieGroup group = TInterface.TFanqieGroupCreate(string.Empty, "Book", "Wiki", [], ["龍", "瀧"]);
        return TInterface.TCourierSend(
            engine,
            id => engine.TLiveryRead(id) is LLiveryPage page ? page with { LLiveryPageFanqie = [group] } : null,
            language => string.Equals(language, unread, StringComparison.Ordinal)
                ? throw new InvalidOperationException("The language pack is unreadable.")
                : TInterface.TLiveryLanguageBuild(
                    language,
                    series ? [TInterface.TLiveryStemCreate(TInterface.TStemPageCreate(language, "龍", ["龍"]), [])] : [],
                    []));
    }

    private static string TCourierFolderFind(TOutpostFake outpost, string title, string? parent = null)
    {
        return outpost.TOutpostFakeFolder
            .Single(folder => folder.Value.TOutpostFakeTitle == title
                && (parent is null || folder.Value.TOutpostFakeParent == parent))
            .Key;
    }

    private static LOutpostNote TCourierNoteFind(TOutpostFake outpost, string title, string? folder = null)
    {
        return outpost.TOutpostFakeNote.Values
            .Single(note => note.LOutpostNoteTitle == title && (folder is null || note.LOutpostNoteFolder == folder));
    }

    private static void TCourierWarrantSave(TWorkspace workspace, string warrant)
    {
        File.WriteAllText(
            Path.Combine(workspace.TWorkspaceFolder, "settings.json"),
            "{ \"localization\": \"en\", \"warrant\": \"" + warrant + "\" }");
    }
}
