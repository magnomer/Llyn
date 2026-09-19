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
        LPhonology panel = TPanelPrepare(engine, () => { asked++; return false; });

        panel.LPhonologyPanel.TPanelScribeSet(true);
        panel.LPhonologyEditor.TEditorHeadwordSet("waters");
        panel.LPhonologyPanel.TPanelScribeSet(false);

        Assert.Equal(1, asked);
        Assert.True(panel.LPhonologyPanel.LPanelEditing);
    }

    [Fact]
    public void ScribeSet_UnsavedDraftLeaveAccepted_ShowsViewer()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LPhonology panel = TPanelPrepare(engine, () => true);

        panel.LPhonologyPanel.TPanelScribeSet(true);
        panel.LPhonologyEditor.TEditorHeadwordSet("waters");
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
        LPhonology panel = TPanelPrepare(engine, () => { asked++; return false; });

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
        LPhonology panel = TPanelPrepare(engine, () => true);

        panel.LPhonologyPanel.TPanelFreshStart();

        Assert.Null(panel.LPhonologyPanel.LPanelVista?.LVistaChosen);
        Assert.True(panel.LPhonologyPanel.LPanelEditing);
        Assert.True(panel.LPhonologyPanel.LPanelModeEnabled);
        Assert.False(panel.LPhonologyPanel.LPanelBinEnabled);
    }

    private static LPhonology TPanelPrepare(LEngine engine, Func<bool> leaveSeam)
    {
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", "ˈwɔːtə", string.Empty, [TInterface.TCardCreate("a liquid", 1)], []));
        LPhonology panel = TInterfaceDeportment.TPhonologyCreate(engine, leaveSeam, () => true);
        LVista vista = engine.TEngineVistaStart("phonology", LCatalogOrder.LCatalogOrderHeadword);
        panel.TPhonologyVistaRestore(vista);
        panel.LPhonologyEditor.TEditorVistaRestore(vista);
        panel.LPhonologyPanel.TPanelRowSelect(entry.LEntryId);
        return panel;
    }
}
