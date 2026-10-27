using System.Text.RegularExpressions;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TInterfaceEmbassyFailure
{
    [Fact]
    public async Task CourierSend_ThreeTimeouts_ThrowsRefusal()
    {
        TEmbassyFake joplin = new();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TInterfaceEmbassy.TInterfaceEmbassyStart(workspace, joplin);
        TEmbassyPrepare(engine, joplin, ["瀧", "江", "河"]);

        LRefusal refusal = await Assert.ThrowsAsync<LRefusal>(
            () => TInterfaceEmbassy.TInterfaceEmbassySend(engine, false));

        Assert.Equal(LRefusal.LRefusalOutpost, refusal.LRefusalReason);
    }

    [Fact]
    public async Task CourierSend_TwoTimeouts_ListsBothFailedTitles()
    {
        TEmbassyFake joplin = new();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TInterfaceEmbassy.TInterfaceEmbassyStart(workspace, joplin);
        TEmbassyPrepare(engine, joplin, ["江", "河"]);

        LReceipt receipt = await TInterfaceEmbassy.TInterfaceEmbassySend(engine, false);

        Assert.Equal(["江", "河"], receipt.LReceiptFailed.Order(StringComparer.Ordinal));
        Assert.Equal((1, 0, 0), (receipt.LReceiptSaved, receipt.LReceiptKept, receipt.LReceiptRemoved));
    }

    [Fact]
    public async Task CourierSend_SendRefused_SavesManifest()
    {
        TEmbassyFake joplin = new();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TInterfaceEmbassy.TInterfaceEmbassyStart(workspace, joplin);
        TEmbassyPrepare(engine, joplin, ["瀧", "江", "河"]);

        await Assert.ThrowsAsync<LRefusal>(() => TInterfaceEmbassy.TInterfaceEmbassySend(engine, false));

        string style = TInterfaceEmbassy.TInterfaceEmbassyRead(joplin, "Llyn style").LOutpostNoteId;
        string manifest = File.ReadAllText(Path.Combine(workspace.TWorkspaceFolder, "joplin.json"));
        Assert.Contains("\"" + style + "\"", manifest);
    }

    [Fact]
    public async Task CourierSend_ForeignRealm_ResendsEveryNote()
    {
        TEmbassyFake joplin = new();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TInterfaceEmbassy.TInterfaceEmbassyStart(workspace, joplin);
        await TInterfaceEmbassy.TInterfaceEmbassySend(engine, true);
        string path = Path.Combine(workspace.TWorkspaceFolder, "joplin.json");
        string realm = Regex.Match(File.ReadAllText(path), "\"realm\": \"([^\"]*)\"").Groups[1].Value;
        File.WriteAllText(path, File.ReadAllText(path).Replace(realm, "foreign", StringComparison.Ordinal));
        joplin.TEmbassyFakeSaved.Clear();

        LReceipt receipt = await TInterfaceEmbassy.TInterfaceEmbassySend(engine, true);

        Assert.Equal((2, 0, 0), (receipt.LReceiptSaved, receipt.LReceiptKept, receipt.LReceiptRemoved));
        Assert.Equal(receipt.LReceiptSaved + 1, joplin.TEmbassyFakeSaved.Count);
        Assert.Contains("\"realm\": \"" + realm + "\"", File.ReadAllText(path));
    }

    [Fact]
    public async Task CourierSend_DigestHitBodyDiffers_ResavesNote()
    {
        TEmbassyFake joplin = new();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TInterfaceEmbassy.TInterfaceEmbassyStart(workspace, joplin);
        await TInterfaceEmbassy.TInterfaceEmbassySend(engine, true);
        LOutpostNote entry = TInterfaceEmbassy.TInterfaceEmbassyRead(joplin, "瀧");
        joplin.TEmbassyFakeNote[entry.LOutpostNoteId] = entry with { LOutpostNoteBody = "edited in Joplin" };
        joplin.TEmbassyFakeSaved.Clear();

        LReceipt receipt = await TInterfaceEmbassy.TInterfaceEmbassySend(engine, true);

        Assert.Equal((1, 1, 0), (receipt.LReceiptSaved, receipt.LReceiptKept, receipt.LReceiptRemoved));
        Assert.Equal([entry.LOutpostNoteId], joplin.TEmbassyFakeSaved);
        Assert.Equal(entry.LOutpostNoteBody, joplin.TEmbassyFakeNote[entry.LOutpostNoteId].LOutpostNoteBody);
    }

    [Fact]
    public async Task CourierAttach_AcceptedWarrant_ReturnsHiddenToken()
    {
        TEmbassyFake joplin = new();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TInterfaceEmbassy.TInterfaceEmbassyStart(workspace, joplin);

        string hidden = await TInterface.TCourierAttach(engine);

        Assert.Equal("fake:kot", hidden);
    }

    [Fact]
    public async Task CourierAttach_RejectedWarrant_ThrowsRefusal()
    {
        TEmbassyFake joplin = new() { TEmbassyFakeAnswer = "rejected" };
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TInterfaceEmbassy.TInterfaceEmbassyStart(workspace, joplin);

        LRefusal refusal = await Assert.ThrowsAsync<LRefusal>(() => TInterface.TCourierAttach(engine));

        Assert.Equal(LRefusal.LRefusalWarrant, refusal.LRefusalReason);
    }

    private static void TEmbassyPrepare(LEngine engine, TEmbassyFake joplin, string[] silent)
    {
        foreach (string headword in silent.Where(static word => word != "瀧"))
        {
            TInterfaceEmbassy.TInterfaceEmbassySave(engine, headword, TInterfaceEmbassy.TInterfaceEmbassyLanguage);
        }

        joplin.TEmbassyFakeSilence.UnionWith(silent);
    }
}
