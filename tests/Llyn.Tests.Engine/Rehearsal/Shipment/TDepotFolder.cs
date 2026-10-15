using System.IO;
using System.Threading.Tasks;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TDepotFolder
{
    private const string TDepotFolderCode = "Spanish";

    [Fact]
    public async Task DepotSave_CellWithBreak_QuotesAndDoublesQuotes()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LDeckSheet sheet = TInterface.TDeckSheetCreate([], ["Text 1", "Text 2"], ["speak", "hablo\n\"hablas\""]);

        string text = await TDepotFolderSave(workspace, sheet, false);

        Assert.Equal("Text 1\tText 2\nspeak\t\"hablo\n\"\"hablas\"\"\"\n", text);
    }

    [Fact]
    public async Task DepotSave_BasicStyle_WritesBarForBreak()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LDeckSheet sheet = TInterface.TDeckSheetCreate(
            [], ["Text 1", "Notes"],["speak", "a \"verb\"\r\nthird person"]);

        string text = await TDepotFolderSave(workspace, sheet, true);

        Assert.Equal("Text 1\tNotes\nspeak\ta \"verb\"|third person\n", text);
    }

    [Fact]
    public async Task DepotSave_SettingLine_WritesThreeCells()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LDeckSheet sheet = TInterface.TDeckSheetCreate(
            [("tsv", "true"), ("max-new-cards", "10")], ["Text 1"], ["book"]);

        string text = await TDepotFolderSave(workspace, sheet, false);

        Assert.Equal("*\ttsv\ttrue\n*\tmax-new-cards\t10\nText 1\nbook\n", text);
    }

    [Fact]
    public async Task DepotSave_IdColumn_WritesAfterText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LDeckSheet sheet = TInterface.TDeckSheetCreate([], ["ID", "Text 1", "Text 2"], ["7", "book", "libro"]);

        string text = await TDepotFolderSave(workspace, sheet, false);

        Assert.Equal("Text 1\tID\tText 2\nbook\t7\tlibro\n", text);
    }

    [Fact]
    public async Task DepotSave_AnyFile_WritesNoByteOrderMark()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LDeckSheet sheet = TInterface.TDeckSheetCreate([], ["Text 1"], ["ünïcode"]);

        await TDepotFolderSave(workspace, sheet, false);

        byte[] content = File.ReadAllBytes(Path.Combine(workspace.TWorkspaceFolder, TDepotFolderCode + ".txt"));
        Assert.Equal((byte)'T', content[0]);
    }

    [Fact]
    public async Task DepotDigestRead_BackupsFolder_SkipsFolder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        string backups = Path.Combine(workspace.TWorkspaceFolder, "Backups");
        Directory.CreateDirectory(backups);
        File.WriteAllText(Path.Combine(backups, TDepotFolderCode + ".txt"), "Text 1\nbackup\n");
        LDepot depot = TInterface.TDepotFolderCreate();

        Assert.Null(depot.TDepotDigestRead(workspace.TWorkspaceFolder, TDepotFolderCode));
        Assert.Null(depot.TDepotDigestRead(workspace.TWorkspaceFolder, Path.Combine("Backups", TDepotFolderCode)));

        await TDepotFolderSave(workspace, TInterface.TDeckSheetCreate([], ["Text 1"], ["book"]), false);
        Assert.NotNull(depot.TDepotDigestRead(workspace.TWorkspaceFolder, TDepotFolderCode));
    }

    private static async Task<string> TDepotFolderSave(TWorkspace workspace, LDeckSheet sheet, bool basic)
    {
        await TInterface.TDepotFolderCreate().TDepotSave(workspace.TWorkspaceFolder, TDepotFolderCode, sheet, basic);
        return File.ReadAllText(Path.Combine(workspace.TWorkspaceFolder, TDepotFolderCode + ".txt"));
    }
}
