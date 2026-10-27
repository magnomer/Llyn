using System.IO;
using System.Threading.Tasks;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TDeckManifestFile
{
    private const string TDeckManifestCode = "Spanish";

    [Fact]
    public async Task DepotExport_PhoneFilePending_Refuses()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        string phone = Path.Combine(workspace.TWorkspaceFolder, "Phone");
        Directory.CreateDirectory(phone);
        string file = Path.Combine(phone, TDeckManifestCode + ".txt");
        File.WriteAllText(file, "Text 1\nshore\n");
        LDepot depot = TInterface.TLockerEntranceCreate(workspace.TWorkspaceFolder);

        LRefusal refusal = await Assert.ThrowsAsync<LRefusal>(() => depot.TDepotExport(
            TInterface.TDepotShipmentCreate(phone, TDeckManifestCode, false, TDeckCellCreate("bank"))));

        Assert.Equal(LRefusal.LRefusalPhone, refusal.LRefusalReason);
        Assert.Equal("Text 1\nshore\n", File.ReadAllText(file));
    }

    [Fact]
    public async Task DepotExport_OwnFileAgain_Writes()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        string phone = Path.Combine(workspace.TWorkspaceFolder, "Phone");
        await TInterface.TLockerEntranceCreate(workspace.TWorkspaceFolder).TDepotExport(
            TInterface.TDepotShipmentCreate(phone, TDeckManifestCode, false, TDeckCellCreate("bank")));

        LDeckReceipt receipt = await TInterface.TLockerEntranceCreate(workspace.TWorkspaceFolder).TDepotExport(
            TInterface.TDepotShipmentCreate(phone, TDeckManifestCode, false, TDeckCellCreate("shore")));

        Assert.Equal(1, receipt.LDeckReceiptCards);
        Assert.Equal("Text 1\tID\nshore\t1\n", File.ReadAllText(Path.Combine(phone, TDeckManifestCode + ".txt")));
    }

    [Fact]
    public async Task DepotExport_DroppedImage_DeletesOnlyOwn()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        string phone = Path.Combine(workspace.TWorkspaceFolder, "Phone");
        string image = Path.Combine(workspace.TWorkspaceFolder, "bank.png");
        File.WriteAllBytes(image, [1, 2, 3]);
        LDepot depot = TInterface.TLockerEntranceCreate(workspace.TWorkspaceFolder);
        await depot.TDepotExport(TInterface.TDepotShipmentCreate(
            phone, TDeckManifestCode, false,
            [TInterface.TCueCellCreate(LCueColumn.LCueColumnPicture, 1, image)]));
        string media = Path.Combine(phone, TDeckManifestCode + " Media");
        string own = Assert.Single(Directory.GetFiles(media));
        string other = Path.Combine(media, "phone.png");
        File.WriteAllBytes(other, [4, 5, 6]);

        LDeckReceipt receipt = await depot.TDepotExport(
            TInterface.TDepotShipmentCreate(phone, TDeckManifestCode, false, TDeckCellCreate("bank")));

        Assert.Equal(1, receipt.LDeckReceiptRemoved);
        Assert.False(File.Exists(own));
        Assert.True(File.Exists(other));
    }

    private static LCueCell[] TDeckCellCreate(string text) =>
        [TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 1, text)];
}
