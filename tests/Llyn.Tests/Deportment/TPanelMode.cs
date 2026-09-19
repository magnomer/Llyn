using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TPanelMode
{
    [Fact]
    public void ScribeSet_UnsavedDraftLeaveDeclined_KeepsEditing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        int asked = 0;
        LPhonology panel = TPanelPrepare(engine, () => true, () => { asked++; return false; });

        panel.LPhonologyPanel.TPanelScribeSet(true);
        panel.LPhonologyPanel.TPanelScribeSet(false);

        Assert.Equal(1, asked);
        Assert.True(panel.LPhonologyPanel.LPanelEditing);
    }

    [Fact]
    public void ScribeSet_UnsavedDraftLeaveAccepted_ShowsViewer()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LPhonology panel = TPanelPrepare(engine, () => true, () => true);

        panel.LPhonologyPanel.TPanelScribeSet(true);
        panel.LPhonologyPanel.TPanelScribeSet(false);

        Assert.False(panel.LPhonologyPanel.LPanelEditing);
        Assert.True(panel.LPhonologyPanel.LPanelBinEnabled);
    }

    [Fact]
    public void ScribeSet_CleanDraft_SkipsLeaveSeam()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        int asked = 0;
        LPhonology panel = TPanelPrepare(engine, () => false, () => { asked++; return false; });

        panel.LPhonologyPanel.TPanelScribeSet(true);
        panel.LPhonologyPanel.TPanelScribeSet(false);

        Assert.Equal(0, asked);
        Assert.False(panel.LPhonologyPanel.LPanelEditing);
    }

    [Fact]
    public void FreshStart_ChosenRow_ClearsChosenAndEdits()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LPhonology panel = TPanelPrepare(engine, () => false, () => true);

        panel.LPhonologyPanel.TPanelFreshStart();

        Assert.Null(panel.LPhonologyPanel.LPanelVista?.LVistaChosen);
        Assert.True(panel.LPhonologyPanel.LPanelEditing);
        Assert.True(panel.LPhonologyPanel.LPanelModeEnabled);
        Assert.False(panel.LPhonologyPanel.LPanelBinEnabled);
    }

    private static LPhonology TPanelPrepare(LEngine engine, Func<bool> changeSeam, Func<bool> leaveSeam)
    {
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", "ˈwɔːtə", string.Empty, [TInterface.TCardCreate("a liquid", 1)], []));
        LPhonology panel = TInterfaceDeportment.TPhonologyCreate(engine, changeSeam, leaveSeam, () => true);
        panel.TPhonologyVistaRestore(engine.TEngineVistaStart("phonology", LCatalogOrder.LCatalogOrderHeadword));
        panel.LPhonologyPanel.TPanelRowSelect(entry.LEntryId);
        return panel;
    }
}
