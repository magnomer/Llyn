using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TPanel
{
    [Fact]
    public void PanelScribeToggle_UnsavedLeaveKept_KeepsEditing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> asked = [];
        List<bool> finished = [];
        CPanel panel = TPanelPrepare(engine, null, asked, finished, "Scribe", () => true);

        panel.CPanelScribeToggle(true);
        panel.CPanelScribeToggle(false);

        Assert.Equal(["Leave"], asked);
        Assert.Empty(finished);
        Assert.True(panel.CPanelEditing);
    }

    [Fact]
    public void PanelScribeToggle_UnsavedLeaveStored_FinishesAndShowsViewer()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<bool> finished = [];
        CPanel panel = TPanelPrepare(engine, true, [], finished, "Scribe", () => true);

        panel.CPanelScribeToggle(true);
        panel.CPanelScribeToggle(false);

        Assert.Equal([true], finished);
        Assert.False(panel.CPanelEditing);
        Assert.True(panel.CPanelBinEnabled);
    }

    [Fact]
    public void PanelScribeToggle_UnsavedLeaveDiscarded_ShowsViewerUnfinished()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<bool> finished = [];
        CPanel panel = TPanelPrepare(engine, false, [], finished, "Scribe", () => true);

        panel.CPanelScribeToggle(true);
        panel.CPanelScribeToggle(false);

        Assert.Empty(finished);
        Assert.False(panel.CPanelEditing);
    }

    [Fact]
    public void PanelScribeToggle_CleanDraft_AsksNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> asked = [];
        CPanel panel = TPanelPrepare(engine, null, asked, [], "Scribe", () => false);

        panel.CPanelScribeToggle(true);
        panel.CPanelScribeToggle(false);

        Assert.Empty(asked);
        Assert.False(panel.CPanelEditing);
    }

    [Fact]
    public void PanelScribeToggle_ChosenRow_HandsItToTheEditor()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CPanel panel = TPanelPrepare(engine, true, [], [], "Scribe", () => false);
        List<long> edited = [];
        panel.CPanelEdited += edited.Add;

        panel.CPanelScribeToggle(true);

        Assert.Equal([panel.TPanelChosenRead()], edited);
        Assert.True(panel.CPanelScribeChecked);
    }

    [Fact]
    public void PanelEntryCreate_ChosenRow_ClearsChosenAndEdits()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CPanel panel = TPanelPrepare(engine, true, [], [], "Scribe", () => false);

        panel.CPanelEntryCreate();

        Assert.Equal(0, panel.TPanelChosenRead());
        Assert.True(panel.CPanelEditing);
        Assert.True(panel.CPanelModeEnabled);
        Assert.False(panel.CPanelBinEnabled);
    }

    [Fact]
    public void PanelScribeRestore_EditingWithoutRow_StaysViewer()
    {
        CPanel panel = TInterfaceConductPanel.TPanelCreate(
            TEnvoyFake.TEnvoyCreate(true, []), "Scribe", static () => false, static _ => true);

        panel.TPanelScribeRestore(true);

        Assert.False(panel.CPanelEditing);
        Assert.Equal(0, panel.TPanelChosenRead());
    }

    [Fact]
    public void PanelVistaRestore_EditingVista_SwitchesEditingOff()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CPanel panel = TInterfaceConductPanel.TPanelCreate(
            TEnvoyFake.TEnvoyCreate(true, []), "Scribe", static () => false, static _ => true);
        LVista vista = engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword);
        vista.TVistaEditingSet(true);

        panel.TPanelVistaRestore(vista);

        Assert.False(vista.LVistaEditing);
        Assert.Same(vista, panel.TPanelVistaRead());
    }

    [Fact]
    public void PanelOrderRead_EveryEngineOrder_ReturnsTheSameNamedMirror()
    {
        LCatalogOrder[] orders = Enum.GetValues<LCatalogOrder>();

        Assert.Equal(orders.Length, Enum.GetValues<CCatalogOrder>().Length);
        foreach (LCatalogOrder order in orders)
        {
            CCatalogOrder mirror = TInterfaceConductPanel.TPanelOrderRead(order);

            Assert.Equal(order.ToString()[1..], mirror.ToString()[1..]);
            Assert.Equal(order, TInterfaceConductPanel.TPanelOrderRead(mirror));
        }
    }

    [Fact]
    public void PanelOrderRead_NoOrder_ReturnsNone()
    {
        Assert.Null(TInterfaceConductPanel.TPanelOrderRead(null));
    }

    [Fact]
    public void PanelSubjectRead_EverySubject_ReturnsTheSameNamedEngineSubject()
    {
        CSubject[] subjects = Enum.GetValues<CSubject>();

        Assert.Equal(subjects.Length, Enum.GetValues<LSubject>().Length);
        foreach (CSubject subject in subjects)
        {
            Assert.Equal(subject.ToString()[1..], TInterfaceConductPanel.TPanelSubjectRead(subject).ToString()[1..]);
        }
    }

    [Fact]
    public void PanelFilterRead_HiddenLanguages_CarriesThemOver()
    {
        CCatalogFilter filter =
            TInterfaceConductPanel.TPanelFilterRead(TInterface.TCatalogFilterCreate("Latin", "Greek"));

        Assert.Equal(["Latin", "Greek"], filter.CCatalogFilterHidden);
    }

    internal static CPanel TPanelPrepare(
        LEngine engine, bool? answer, List<string> asked, List<bool> finished, string? scope, Func<bool> change)
    {
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", "w", string.Empty, [TInterface.TCardCreate("a liquid", 1)], []));
        CPanel panel = TInterfaceConductPanel.TPanelCreate(
            TEnvoyFake.TEnvoyCreate(answer, asked),
            scope,
            change,
            store =>
            {
                finished.Add(store);
                return true;
            });
        panel.TPanelVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        panel.CPanelRowSelect(entry.LEntryId);
        return panel;
    }
}
