using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TDepotFolder
{
    private const string TDepotFolderCode = "Spanish";

    [Fact]
    public async Task DepotExport_CellWithBreak_QuotesAndDoublesQuotes()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        string text = await TDepotFolderExport(workspace, false, [
            TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 1, "speak"),
            TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 2, "hablo\n\"hablas\""),
        ]);

        Assert.Equal("Text 1\tText 2\tID\nspeak\t\"hablo\n\"\"hablas\"\"\"\t1\n", text);
    }

    [Fact]
    public async Task DepotExport_BasicStyle_WritesBarForBreak()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        string text = await TDepotFolderExport(workspace, true, [
            TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 1, "speak"),
            TInterface.TCueCellCreate(LCueColumn.LCueColumnNotes, 1, "a \"verb\"\r\nthird person"),
        ]);

        Assert.Equal("Text 1\tNotes\tID\nspeak\ta \"verb\"|third person\t1\n", text);
    }

    [Fact]
    public async Task DepotExport_ColumnsOutOfOrder_WritesPhoneOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        string text = await TDepotFolderExport(workspace, false, [
            TInterface.TCueCellCreate(LCueColumn.LCueColumnCategory, 1, "verb"),
            TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 2, "libro"),
            TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 1, "book"),
        ]);

        Assert.Equal("Text 1\tText 2\tCategory 1\tID\nbook\tlibro\tverb\t1\n", text);
    }

    [Fact]
    public async Task DepotExport_AnyFile_WritesNoByteOrderMark()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        await TDepotFolderExport(
            workspace, false, [TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 1, "ünïcode")]);

        byte[] content = File.ReadAllBytes(Path.Combine(TDepotPhoneRead(workspace), TDepotFolderCode + ".txt"));
        Assert.Equal((byte)'T', content[0]);
    }

    [Fact]
    public async Task DepotExport_SameImage_CopiesOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        string first = Path.Combine(workspace.TWorkspaceFolder, "bank.png");
        string second = Path.Combine(workspace.TWorkspaceFolder, "shore.png");
        File.WriteAllBytes(first, [1, 2, 3]);
        File.WriteAllBytes(second, [1, 2, 3]);
        LDepot depot = TInterface.TLockerEntranceCreate(workspace.TWorkspaceFolder);

        LDeckReceipt receipt = await depot.TDepotExport(TInterface.TDepotShipmentCreate(
            TDepotPhoneRead(workspace), TDepotFolderCode, false,
            [TInterface.TCueCellCreate(LCueColumn.LCueColumnPicture, 1, first)],
            [TInterface.TCueCellCreate(LCueColumn.LCueColumnPicture, 1, second)]));

        Assert.Equal(1, receipt.LDeckReceiptCopied);
        Assert.Empty(receipt.LDeckReceiptFailed);
        string media = Path.Combine(TDepotPhoneRead(workspace), TDepotFolderCode + " Media");
        string name = Path.GetFileName(Assert.Single(Directory.GetFiles(media)));
        Assert.EndsWith(".png", name, StringComparison.Ordinal);
        Assert.Contains("\n\t" + name + "\t1\n\t" + name + "\t2\n", await TDepotFolderRead(workspace));
    }

    [Fact]
    public async Task DepotExport_ImageMissing_FailsAndLeavesCellEmpty()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        string image = Path.Combine(workspace.TWorkspaceFolder, "missing.png");
        LDepot depot = TInterface.TLockerEntranceCreate(workspace.TWorkspaceFolder);

        LDeckReceipt receipt = await depot.TDepotExport(TInterface.TDepotShipmentCreate(
            TDepotPhoneRead(workspace), TDepotFolderCode, false,
            [
                TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 1, "bank"),
                TInterface.TCueCellCreate(LCueColumn.LCueColumnPicture, 1, image),
            ]));

        Assert.Equal([image], receipt.LDeckReceiptFailed);
        Assert.Equal(0, receipt.LDeckReceiptCopied);
        Assert.Equal("Text 1\tID\nbank\t1\n", await TDepotFolderRead(workspace));
    }

    [Fact]
    public async Task DepotExport_HttpImage_WritesAddress()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        string text = await TDepotFolderExport(workspace, false, [
            TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 1, "bank"),
            TInterface.TCueCellCreate(LCueColumn.LCueColumnPicture, 1, "https://example.com/bank.png"),
        ]);

        Assert.Equal("Text 1\tPicture 1\tID\nbank\thttps://example.com/bank.png\t1\n", text);
    }

    private static string TDepotPhoneRead(TWorkspace workspace) =>
        Path.Combine(workspace.TWorkspaceFolder, "Phone");

    private static Task<string> TDepotFolderRead(TWorkspace workspace) =>
        File.ReadAllTextAsync(Path.Combine(TDepotPhoneRead(workspace), TDepotFolderCode + ".txt"));

    private static async Task<string> TDepotFolderExport(
        TWorkspace workspace, bool basic, IReadOnlyList<LCueCell> cells)
    {
        await TInterface.TLockerEntranceCreate(workspace.TWorkspaceFolder)
            .TDepotExport(TInterface.TDepotShipmentCreate(TDepotPhoneRead(workspace), TDepotFolderCode, basic, cells));
        return await TDepotFolderRead(workspace);
    }
}
