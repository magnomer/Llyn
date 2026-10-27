using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TPortraitVista
{
    [Fact]
    public async Task PortraitExport_EmptySelection_WritesNoFile()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate("water", "English", "", "", [], []));
        LVista vista = engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderName);
        string path = Path.Combine(workspace.TWorkspaceFolder, "portrait.html");
        await engine.TEnginePortraitExport(
            vista, path, LPortraitMedium.LPortraitMediumHtml, TInterface.TPortraitLabelRead());
        Assert.False(File.Exists(path));
        vista.TVistaSelect(entry.LEntryId);
        await engine.TEnginePortraitExport(
            vista, path, LPortraitMedium.LPortraitMediumHtml, TInterface.TPortraitLabelRead());
        Assert.Contains("water", File.ReadAllText(path));
    }

    [Fact]
    public async Task Print_Vista_UsesChosenEntryAndIgnoresClearedSelection()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TPress press = workspace.TWorkspacePress;
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate("water", "English", "", "", [], []));
        LEntry cleared = engine.TEngineEntrySave(TInterface.TEntryDraftCreate("fire", "English", "", "", [], []));
        LVista vista = engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderName);
        vista.TVistaSelect(cleared.LEntryId);
        vista.TVistaSelect(null);
        await using LGalleyRelay? none = await engine.TEnginePressOpen(vista, TInterface.TPortraitLabelRead());
        Assert.Null(press.TPressHtml);
        vista.TVistaSelect(entry.LEntryId);
        await using LGalleyRelay? galley = await engine.TEnginePressOpen(vista, TInterface.TPortraitLabelRead());
        Assert.Contains("water", press.TPressHtml);
        Assert.DoesNotContain("fire", press.TPressHtml);
        Assert.True(press.TPressOpened);
    }

    [Fact]
    public async Task Print_CatalogVista_ResolvesSubject()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TPress press = workspace.TWorkspacePress;
        LExample example = engine.TEngineExampleCreate(
            TInterfaceExample.TExampleCreate(0, "English", "Water flows.", null, LStateAnchor.LStateAnchorUnspecified));
        LVista vista = engine.TEngineVistaStart("corpus", LCatalogOrder.LCatalogOrderText);
        vista.TVistaSelect(example.LExampleId);
        await using LGalleyRelay? galley = await engine.TEnginePressOpen(
            vista, TInterface.TPortraitLegendRead());
        Assert.Contains("Water flows.", press.TPressHtml);
    }
}
