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
    public void PanelRowSelect_UnsavedLeaveKept_KeepsTheRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CPanel panel = TPanelPrepare(engine, null, [], [], "Scribe", () => true);
        long chosen = panel.TPanelChosenRead();
        panel.CPanelScribeToggle(true);

        panel.CPanelRowSelect(chosen + 100);

        Assert.Equal(chosen, panel.TPanelChosenRead());
    }

    [Fact]
    public void PanelRowOpen_StoredRow_ChoosesTheRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CPanel panel = TPanelPrepare(engine, null, [], [], "Scribe", () => false);
        long chosen = panel.TPanelChosenRead();
        panel.CPanelEntryClose();
        int changed = 0;
        panel.CPanelChanged += () => changed++;

        bool opened = panel.CPanelRowOpen(chosen);

        Assert.True(opened);
        Assert.Equal(chosen, panel.TPanelChosenRead());
        Assert.Equal(1, changed);
    }

    [Fact]
    public void PanelRowOpen_MissingRow_ClosesThePanel()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> asked = [];
        CPanel panel = TPanelPrepare(engine, null, asked, [], "Scribe", () => false);
        long chosen = panel.TPanelChosenRead();
        int cleared = 0;
        panel.CPanelCleared += () => cleared++;

        bool opened = panel.CPanelRowOpen(chosen + 100);

        Assert.False(opened);
        Assert.Equal(0, panel.TPanelChosenRead());
        Assert.Equal(1, cleared);
        Assert.Empty(asked);
    }

    [Fact]
    public void PanelRowOpen_EditingPanel_HandsTheRowToTheEditor()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CPanel panel = TPanelPrepare(engine, null, [], [], "Scribe", () => false);
        long chosen = panel.TPanelChosenRead();
        panel.CPanelEntryCreate();
        List<long> edited = [];
        panel.CPanelEdited += edited.Add;

        panel.CPanelRowOpen(chosen);

        Assert.Equal([chosen], edited);
        Assert.True(panel.CPanelEditing);
    }

    [Fact]
    public void PanelDraftResonate_StoredEntry_RepaintsThePanel()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CPanel panel = TPanelPrepare(engine, null, [], [], "Scribe", () => false);
        long chosen = panel.TPanelChosenRead();
        int changed = 0;
        panel.CPanelChanged += () => changed++;

        panel.CPanelDraftResonate();

        Assert.Equal(1, changed);
        Assert.Equal(chosen, panel.TPanelChosenRead());
    }

    [Fact]
    public void PanelDraftResonate_DeletedEntry_ClosesThePanel()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CPanel panel = TPanelPrepare(engine, null, [], [], "Scribe", () => false);
        engine.TEngineEntryDelete(panel.TPanelChosenRead());
        int cleared = 0;
        panel.CPanelCleared += () => cleared++;

        panel.CPanelDraftResonate();

        Assert.Equal(1, cleared);
        Assert.Equal(0, panel.TPanelChosenRead());
    }

    [Fact]
    public void PanelEntryResonate_StoredBulletin_RelistsTheRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CPanel panel = TPanelPrepare(engine, null, [], [], "Scribe", () => false);
        int rows = 0;
        int changed = 0;
        panel.CPanelRowsChanged += () => rows++;
        panel.CPanelChanged += () => changed++;

        panel.CPanelEntryResonate(new CBulletin(panel.TPanelChosenRead(), true));

        Assert.Equal(1, rows);
        Assert.Equal(1, changed);
    }

    [Fact]
    public void PanelEntryDelete_Declined_KeepsChosenAndEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> asked = [];
        CPanel panel = TPanelPrepare(engine, false, asked, [], "Scribe", () => false);
        long chosen = panel.TPanelChosenRead();

        panel.CPanelEntryDelete();

        Assert.Equal(["Scribe.DeleteConfirm"], asked);
        Assert.Equal(chosen, panel.TPanelChosenRead());
        Assert.NotNull(engine.TEngineEntryLoad(chosen));
    }

    [Fact]
    public void PanelEntryDelete_Accepted_DropsChosenAndEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CPanel panel = TPanelPrepare(engine, true, [], [], "Scribe", () => false);
        long chosen = panel.TPanelChosenRead();

        panel.CPanelEntryDelete();

        Assert.Equal(0, panel.TPanelChosenRead());
        Assert.False(panel.CPanelBinEnabled);
        Assert.Null(engine.TEngineEntryLoad(chosen));
    }

    [Fact]
    public void PanelEntryDelete_NoScope_AsksNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> asked = [];
        CPanel panel = TPanelPrepare(engine, true, asked, [], null, () => false);
        long chosen = panel.TPanelChosenRead();

        panel.CPanelEntryDelete();

        Assert.Empty(asked);
        Assert.NotNull(engine.TEngineEntryLoad(chosen));
    }

    [Fact]
    public void PanelEntryDelete_CreditedAuthor_AsksWithTheTally()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LReference book = engine.TEngineCitationCreate("Book");
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        engine.TRequestCreditApply(book.LReferenceId, ada.LAuthorId, 0);
        List<string> asked = [];
        CPanel panel = TInterfaceConduct.TPanelCreate(
            TInterfaceConduct.TEnvoyCreate(false, asked), "Guild", static () => false, static _ => true);
        LVista vista = engine.TEngineVistaStart("guild", LCatalogOrder.LCatalogOrderName);
        panel.TPanelVistaRestore(vista);
        vista.TVistaSelect(ada.LAuthorId);

        panel.CPanelEntryDelete();

        Assert.Equal(["Guild.DetachConfirm"], asked);
        Assert.Equal(ada.LAuthorId, panel.TPanelChosenRead());
    }

    [Fact]
    public void PanelEntrySelect_StoredWhileFreshEditing_AdoptsTheEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CPanel panel = TPanelPrepare(engine, true, [], [], "Scribe", () => false);
        long chosen = panel.TPanelChosenRead();
        panel.CPanelEntryCreate();

        panel.CPanelEntrySelect(new CBulletin(chosen, true));

        Assert.Equal(chosen, panel.TPanelChosenRead());
    }

    [Fact]
    public void PanelEntrySelect_Viewing_KeepsNothingChosen()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CPanel panel = TPanelPrepare(engine, true, [], [], "Scribe", () => false);
        long chosen = panel.TPanelChosenRead();
        panel.CPanelEntryClose();

        panel.CPanelEntrySelect(new CBulletin(chosen, true));

        Assert.Equal(0, panel.TPanelChosenRead());
    }

    [Fact]
    public void PanelScribeRestore_EditingWithoutRow_StaysViewer()
    {
        CPanel panel = TInterfaceConduct.TPanelCreate(
            TInterfaceConduct.TEnvoyCreate(true, []), "Scribe", static () => false, static _ => true);

        panel.TPanelScribeRestore(true);

        Assert.False(panel.CPanelEditing);
        Assert.Equal(0, panel.TPanelChosenRead());
    }

    [Fact]
    public void PanelVistaRestore_EditingVista_SwitchesEditingOff()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CPanel panel = TInterfaceConduct.TPanelCreate(
            TInterfaceConduct.TEnvoyCreate(true, []), "Scribe", static () => false, static _ => true);
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
            CCatalogOrder mirror = TInterfaceConduct.TPanelOrderRead(order);

            Assert.Equal(order.ToString()[1..], mirror.ToString()[1..]);
            Assert.Equal(order, TInterfaceConduct.TPanelOrderRead(mirror));
        }
    }

    [Fact]
    public void PanelOrderRead_NoOrder_ReturnsNone()
    {
        Assert.Null(TInterfaceConduct.TPanelOrderRead(null));
    }

    [Fact]
    public void PanelSubjectRead_EverySubject_ReturnsTheSameNamedEngineSubject()
    {
        CSubject[] subjects = Enum.GetValues<CSubject>();

        Assert.Equal(subjects.Length, Enum.GetValues<LSubject>().Length);
        foreach (CSubject subject in subjects)
        {
            Assert.Equal(subject.ToString()[1..], TInterfaceConduct.TPanelSubjectRead(subject).ToString()[1..]);
        }
    }

    [Fact]
    public void PanelFilterRead_HiddenLanguages_CarriesThemOver()
    {
        CCatalogFilter filter = TInterfaceConduct.TPanelFilterRead(TInterface.TCatalogFilterCreate("Latin", "Greek"));

        Assert.Equal(["Latin", "Greek"], filter.CCatalogFilterHidden);
    }

    private static CPanel TPanelPrepare(
        LEngine engine, bool? answer, List<string> asked, List<bool> finished, string? scope, Func<bool> change)
    {
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", "w", string.Empty, [TInterface.TCardCreate("a liquid", 1)], []));
        CPanel panel = TInterfaceConduct.TPanelCreate(
            TInterfaceConduct.TEnvoyCreate(answer, asked),
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
