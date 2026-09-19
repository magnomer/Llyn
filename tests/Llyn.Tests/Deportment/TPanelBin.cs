using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TPanelBin
{
    [Fact]
    public void Delete_SeamDeclined_KeepsChosen()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TPanelEntrySave(engine);
        LPhonology panel = TPanelPrepare(engine, entry.LEntryId, () => false);

        panel.LPhonologyPanel.TPanelDelete();

        Assert.Equal(entry.LEntryId, panel.LPhonologyPanel.LPanelVista?.LVistaChosen);
        Assert.NotNull(engine.TEngineEntryLoad(entry.LEntryId));
    }

    [Fact]
    public void Delete_SeamAccepted_DropsChosenAndEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TPanelEntrySave(engine);
        LPhonology panel = TPanelPrepare(engine, entry.LEntryId, () => true);

        panel.LPhonologyPanel.TPanelDelete();

        Assert.Null(panel.LPhonologyPanel.LPanelVista?.LVistaChosen);
        Assert.False(panel.LPhonologyPanel.LPanelBinEnabled);
        Assert.Null(engine.TEngineEntryLoad(entry.LEntryId));
    }

    private static LEntry TPanelEntrySave(LEngine engine)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", "ˈwɔːtə", string.Empty, [TInterface.TCardCreate("a liquid", 1)], []));
    }

    private static LPhonology TPanelPrepare(LEngine engine, long id, Func<bool> deleteSeam)
    {
        LPhonology panel = TInterfaceDeportment.TPhonologyCreate(engine, () => false, () => true, deleteSeam);
        panel.TPhonologyVistaRestore(engine.TEngineVistaStart("phonology", LCatalogOrder.LCatalogOrderHeadword));
        panel.LPhonologyPanel.TPanelRowSelect(id);
        return panel;
    }
}
