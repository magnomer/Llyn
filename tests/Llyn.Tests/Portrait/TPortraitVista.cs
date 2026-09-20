using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TPortraitVista
{
    [Fact]
    public async Task Export_Vista_UsesChosenEntryAndDoesNotCreateAFileForEmptySelection()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate("water", "English", "", "", [], []));
        LVista vista = engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderName);
        string path = Path.Combine(workspace.TWorkspaceFolder, "portrait.html");
        await engine.TEnginePortraitExport(
            vista, path, LPortraitFormat.LPortraitFormatHtml, TInterface.TPortraitLabelRead());
        Assert.False(File.Exists(path));
        vista.TVistaSelect(entry.LEntryId);
        await engine.TEnginePortraitExport(
            vista, path, LPortraitFormat.LPortraitFormatHtml, TInterface.TPortraitLabelRead());
        Assert.Contains("water", File.ReadAllText(path));
    }

    [Fact]
    public async Task Print_Vista_UsesChosenEntryAndIgnoresClearedSelection()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TPress press = new();
        engine.TEnginePressApply(press);
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate("water", "English", "", "", [], []));
        LVista vista = engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderName);
        LPressTicket ticket = TInterface.TPressTicketCreate("Printer", false, 1);
        await engine.TEnginePortraitPrint(vista, TInterface.TPortraitLabelRead(), ticket);
        Assert.Null(press.TPressHtml);
        vista.TVistaSelect(entry.LEntryId);
        await engine.TEnginePortraitPrint(vista, TInterface.TPortraitLabelRead(), ticket);
        Assert.Contains("water", press.TPressHtml);
        Assert.Same(ticket, press.TPressTicket);
    }

    [Fact]
    public async Task Print_CatalogVista_ResolvesItsSubject()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TPress press = new();
        engine.TEnginePressApply(press);
        LExample example = engine.TEngineExampleCreate(
            TInterface.TExampleCreate(0, "English", "Water flows.", null, LStateAnchor.LStateAnchorUnspecified));
        LVista vista = engine.TEngineVistaStart("corpus", LCatalogOrder.LCatalogOrderText);
        vista.TVistaSelect(example.LExampleId);
        await engine.TEnginePortraitPrint(
            vista, TInterface.TPortraitLegendRead(), TInterface.TPressTicketCreate("Printer", false, 1));
        Assert.Contains("Water flows.", press.TPressHtml);
    }
}
