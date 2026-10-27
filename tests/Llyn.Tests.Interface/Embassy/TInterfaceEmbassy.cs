using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TInterfaceEmbassy
{
    internal const string TInterfaceEmbassyLanguage = "Classical Chinese";

    private const string TInterfaceEmbassyStray = "Stray";

    [Fact]
    public async Task CourierSend_FirstPush_SavesReconstructionNotes()
    {
        TEmbassyFake joplin = new();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TInterfaceEmbassyStart(workspace, joplin);

        LReceipt receipt = await TInterfaceEmbassySend(engine, true);

        Assert.Equal((2, 0, 0), (receipt.LReceiptSaved, receipt.LReceiptKept, receipt.LReceiptRemoved));
        LOutpostNote series = Assert.Single(
            joplin.TEmbassyFakeNote.Values, static note => note.LOutpostNoteTitle == "龍");
        Assert.Equal("Navigation.Xiesheng", joplin.TEmbassyFakeFolder[series.LOutpostNoteFolder].TEmbassyFakeTitle);
        Assert.Contains(series.LOutpostNoteId, joplin.TEmbassyFakeSaved);
        Assert.DoesNotContain(
            joplin.TEmbassyFakeFolder.Values, static folder => folder.TEmbassyFakeTitle == "Navigation.Yunjing");
        string manifest = File.ReadAllText(Path.Combine(workspace.TWorkspaceFolder, "joplin.json"));
        Assert.Contains("\"" + series.LOutpostNoteId + "\"", manifest);
    }

    [Fact]
    public async Task CourierSend_SecondPush_KeepsReconstructionNotes()
    {
        TEmbassyFake joplin = new();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TInterfaceEmbassyStart(workspace, joplin);
        await TInterfaceEmbassySend(engine, true);
        joplin.TEmbassyFakeSaved.Clear();

        LReceipt receipt = await TInterfaceEmbassySend(engine, true);

        Assert.Equal((0, 2, 0), (receipt.LReceiptSaved, receipt.LReceiptKept, receipt.LReceiptRemoved));
        Assert.Empty(joplin.TEmbassyFakeSaved);
        Assert.Contains(joplin.TEmbassyFakeNote.Values, static note => note.LOutpostNoteTitle == "龍");
    }

    [Fact]
    public async Task CourierSend_GoneSeries_TrashesSeriesNote()
    {
        TEmbassyFake joplin = new();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TInterfaceEmbassyStart(workspace, joplin);
        await TInterfaceEmbassySend(engine, true);
        string series = TInterfaceEmbassyRead(joplin, "龍").LOutpostNoteId;

        LReceipt receipt = await TInterfaceEmbassySend(engine, false);

        Assert.Equal([series], joplin.TEmbassyFakeTrash);
        Assert.DoesNotContain(series, joplin.TEmbassyFakeNote.Keys);
        Assert.Equal(1, receipt.LReceiptRemoved);
        string manifest = File.ReadAllText(Path.Combine(workspace.TWorkspaceFolder, "joplin.json"));
        Assert.DoesNotContain(series, manifest);
    }

    [Fact]
    public async Task CourierSend_StoredSeries_LinksEntryChipToNote()
    {
        TEmbassyFake joplin = new();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TInterfaceEmbassyStart(workspace, joplin);

        await TInterfaceEmbassySend(engine, true);

        string series = TInterfaceEmbassyRead(joplin, "龍").LOutpostNoteId;
        string body = TInterfaceEmbassyRead(joplin, "瀧").LOutpostNoteBody;
        Assert.Contains("[<span class=\"llyn-stem\">龍</span>](:/" + series + ")", body);
        Assert.Contains("<span class=\"llyn-stem\">瀧</span>", body);
        Assert.DoesNotContain("[<span class=\"llyn-stem\">瀧</span>]", body);
    }

    [Fact]
    public async Task CourierSend_NotebookRenamed_RenamesSameTopNotebook()
    {
        TEmbassyFake joplin = new();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TInterfaceEmbassyStart(workspace, joplin);
        engine.TEngineNotebookSave("Lexicon");
        await TInterfaceEmbassySend(engine, true);
        string notebook = TInterfaceEmbassyFind(joplin, "Lexicon", string.Empty);
        int folders = joplin.TEmbassyFakeFolder.Count;

        engine.TEngineNotebookSave("Words");
        await TInterfaceEmbassySend(engine, true);

        Assert.Equal((string.Empty, "Words"), joplin.TEmbassyFakeFolder[notebook]);
        Assert.Equal(folders, joplin.TEmbassyFakeFolder.Count);
        Assert.Equal(
            notebook,
            joplin.TEmbassyFakeFolder[TInterfaceEmbassyFind(joplin, TInterfaceEmbassyLanguage)]
                .TEmbassyFakeParent);
    }

    [Fact]
    public async Task CourierSend_LanguageGone_KeepsSeriesNote()
    {
        TEmbassyFake joplin = new();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TInterfaceEmbassyStart(workspace, joplin);
        LEntry stray = TInterfaceEmbassySave(engine, "江", TInterfaceEmbassyStray);
        await TInterfaceEmbassySend(engine, true);
        string shelf = TInterfaceEmbassyFind(
            joplin, TInterfaceEmbassyStray, TInterfaceEmbassyFind(joplin, "Llyn"));
        string series = TInterfaceEmbassyRead(
            joplin, "龍", TInterfaceEmbassyFind(joplin, "Navigation.Xiesheng", shelf)).LOutpostNoteId;
        engine.TEngineEntryDelete(stray.LEntryId);

        LReceipt receipt = await TInterfaceEmbassySend(engine, true);

        Assert.DoesNotContain(series, joplin.TEmbassyFakeTrash);
        Assert.Contains(series, joplin.TEmbassyFakeNote.Keys);
        Assert.Equal(0, receipt.LReceiptRemoved);
    }

    [Fact]
    public async Task CourierSend_FanqieCategories_FilesNotesAndLinksChips()
    {
        TEmbassyFake joplin = new();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TInterfaceEmbassyStart(workspace, joplin);
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

        string shelf = TInterfaceEmbassyFind(joplin, TInterfaceEmbassyLanguage);
        string table = TInterfaceEmbassyFind(joplin, "Navigation.Yunjing");
        string onset = TInterfaceEmbassyFind(joplin, "Yunjing.Shengmu");
        string rimes = TInterfaceEmbassyFind(joplin, "Yunjing.Yunmu");
        Assert.Equal(
            (shelf, table, table),
            (joplin.TEmbassyFakeFolder[table].TEmbassyFakeParent,
                joplin.TEmbassyFakeFolder[onset].TEmbassyFakeParent,
                joplin.TEmbassyFakeFolder[rimes].TEmbassyFakeParent));
        LOutpostNote initial = TInterfaceEmbassyRead(joplin, "來");
        LOutpostNote rime = TInterfaceEmbassyRead(joplin, row.LFanqieRowCell);
        LOutpostNote tone = TInterfaceEmbassyRead(joplin, "Display.FanqieTone");
        Assert.Equal(
            (onset, rimes, table),
            (initial.LOutpostNoteFolder, rime.LOutpostNoteFolder, tone.LOutpostNoteFolder));
        string body = TInterfaceEmbassyRead(joplin, "瀧").LOutpostNoteBody;
        Assert.Contains("[<span class=\"llyn-initial\">來</span>](:/" + initial.LOutpostNoteId + ")", body);
        Assert.Contains(
            "[<span class=\"llyn-rime\">" + row.LFanqieRowCell + "</span>](:/" + rime.LOutpostNoteId + ")", body);
        Assert.Contains(
            "[<span class=\"llyn-tone\">Display.FanqieTone</span>](:/" + tone.LOutpostNoteId + ")", body);
    }

    [Fact]
    public async Task CourierSend_SeriesNoteFailsToSave_ListsFailedTitle()
    {
        TEmbassyFake joplin = new();
        joplin.TEmbassyFakeRefusal.Add("龍");
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TInterfaceEmbassyStart(workspace, joplin);

        LReceipt receipt = await TInterfaceEmbassySend(engine, true);

        Assert.Equal((1, 0, 0), (receipt.LReceiptSaved, receipt.LReceiptKept, receipt.LReceiptRemoved));
        Assert.Equal(["龍"], receipt.LReceiptFailed);
        string body = TInterfaceEmbassyRead(joplin, "瀧").LOutpostNoteBody;
        Assert.Contains("<span class=\"llyn-stem\">龍</span>", body);
        Assert.DoesNotContain("[<span class=\"llyn-stem\">龍</span>]", body);
    }

    [Fact]
    public async Task CourierSend_LanguageFailsToRead_TrashesNothing()
    {
        TEmbassyFake joplin = new();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TInterfaceEmbassyStart(workspace, joplin);
        TInterfaceEmbassySave(engine, "江", TInterfaceEmbassyStray);
        LReceipt first = await TInterfaceEmbassySend(engine, true);
        int sent = joplin.TEmbassyFakeSaved.Count;
        string shelf = TInterfaceEmbassyFind(
            joplin, TInterfaceEmbassyLanguage, TInterfaceEmbassyFind(joplin, "Llyn"));
        string series = TInterfaceEmbassyRead(
            joplin, "龍", TInterfaceEmbassyFind(joplin, "Navigation.Xiesheng", shelf)).LOutpostNoteId;
        string entry = TInterfaceEmbassyRead(joplin, "瀧").LOutpostNoteId;
        joplin.TEmbassyFakeSaved.Clear();
        joplin.TEmbassyFakeFolder.Clear();

        LReceipt unread = await TInterfaceEmbassySend(engine, true, TInterfaceEmbassyLanguage);

        Assert.Equal((4, 5), (first.LReceiptSaved, sent));
        Assert.Equal((1, 2, 0), (unread.LReceiptSaved, unread.LReceiptKept, unread.LReceiptRemoved));
        Assert.Equal([entry], joplin.TEmbassyFakeSaved);
        Assert.DoesNotContain(joplin.TEmbassyFakeFolder.Values, folder => folder.TEmbassyFakeParent == shelf);
        Assert.Empty(joplin.TEmbassyFakeTrash);
        Assert.Contains(series, joplin.TEmbassyFakeNote.Keys);
        joplin.TEmbassyFakeSaved.Clear();
        await TInterfaceEmbassySend(engine, true);
        Assert.DoesNotContain(series, joplin.TEmbassyFakeSaved);
    }

    [Fact]
    public async Task CourierSend_StemEntryOutsideSend_LeavesChipUnlinked()
    {
        TEmbassyFake joplin = new();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TInterfaceEmbassyStart(workspace, joplin);
        LEntry outside = TInterface.TEntryCreate(9999, "龍", TInterfaceEmbassyLanguage, 0, null, null);

        await TInterfaceEmbassySend(engine, true, kin: [outside]);

        string body = TInterfaceEmbassyRead(joplin, "龍").LOutpostNoteBody;
        Assert.Contains("<span class=\"llyn-stem\">龍</span>", body);
        Assert.DoesNotContain("<a href", body);
    }

    internal static LEngine TInterfaceEmbassyStart(TWorkspace workspace, TEmbassyFake joplin)
    {
        File.WriteAllText(
            Path.Combine(workspace.TWorkspaceFolder, "settings.json"),
            "{ \"localization\": \"en\", \"warrant\": \"fake:nekot\" }");
        LEmbassyEntrance embassy = new(
            new LKeepFile(workspace.TWorkspaceFolder),
            new LEngraverEntrance(TInterface.TThemeLoad()),
            new HttpClient(joplin));
        LEngine engine = new(
            workspace.TWorkspaceRigCreate() with { LRigOutpost = embassy },
            _ => workspace.TWorkspaceRigCreate(),
            _ => { });
        TInterfaceEmbassySave(engine, "瀧", TInterfaceEmbassyLanguage);
        return engine;
    }

    internal static LEntry TInterfaceEmbassySave(LEngine engine, string headword, string language)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword,
            language,
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a state", [], [], [], [], [], 1)],
            []));
    }

    internal static Task<LReceipt> TInterfaceEmbassySend(
        LEngine engine, bool series, string unread = "", IReadOnlyList<LEntry>? kin = null)
    {
        LFanqieGroup group = TInterface.TFanqieGroupCreate(string.Empty, "Book", "Wiki", [], ["龍", "瀧"]);
        return TInterface.TCourierSend(
            engine,
            id => engine.TLiveryRead(id) is LLiveryPage page ? page with { LLiveryPageFanqie = [group] } : null,
            language => string.Equals(language, unread, StringComparison.Ordinal)
                ? throw new InvalidOperationException("The language pack is unreadable.")
                : TInterface.TLiveryLanguageBuild(
                    language,
                    series
                        ? [TInterface.TLiveryStemCreate(TInterface.TStemPageCreate(language, "龍", ["龍"]), kin ?? [])]
                        : [],
                    []));
    }

    internal static string TInterfaceEmbassyFind(TEmbassyFake joplin, string title, string? parent = null)
    {
        return joplin.TEmbassyFakeFolder
            .Single(row => row.Value.TEmbassyFakeTitle == title
                && (parent is null || row.Value.TEmbassyFakeParent == parent))
            .Key;
    }

    internal static LOutpostNote TInterfaceEmbassyRead(TEmbassyFake joplin, string title, string? folder = null)
    {
        return joplin.TEmbassyFakeNote.Values
            .Single(note => note.LOutpostNoteTitle == title && (folder is null || note.LOutpostNoteFolder == folder));
    }
}
