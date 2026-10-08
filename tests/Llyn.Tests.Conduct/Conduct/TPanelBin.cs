using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TPanelBin
{
    [Fact]
    public void PanelBinDelete_Declined_KeepsChosenAndEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> asked = [];
        CPanel panel = TPanel.TPanelPrepare(engine, false, asked, [], "Scribe", () => false);
        long chosen = panel.TPanelChosenRead();

        panel.CPanelBin.CPanelBinDelete();

        Assert.Equal(["Scribe.DeleteConfirm"], asked);
        Assert.Equal(chosen, panel.TPanelChosenRead());
        Assert.NotNull(engine.TEngineEntryLoad(chosen));
    }

    [Fact]
    public void PanelBinDelete_Accepted_DropsChosenAndEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CPanel panel = TPanel.TPanelPrepare(engine, true, [], [], "Scribe", () => false);
        long chosen = panel.TPanelChosenRead();

        panel.CPanelBin.CPanelBinDelete();

        Assert.Equal(0, panel.TPanelChosenRead());
        Assert.False(panel.CPanelBinEnabled);
        Assert.Null(engine.TEngineEntryLoad(chosen));
    }

    [Fact]
    public void PanelBinDelete_NoScope_AsksNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> asked = [];
        CPanel panel = TPanel.TPanelPrepare(engine, true, asked, [], null, () => false);
        long chosen = panel.TPanelChosenRead();

        panel.CPanelBin.CPanelBinDelete();

        Assert.Empty(asked);
        Assert.NotNull(engine.TEngineEntryLoad(chosen));
    }

    [Fact]
    public void PanelBinDelete_CreditedAuthor_AsksWithTheTally()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LReference book = engine.TEngineCitationCreate("Book");
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        engine.TRequestCreditApply(book.LReferenceId, ada.LAuthorId, 0);
        List<string> asked = [];
        CEnvoy envoy = TEngineFake.TEngineCreate<CEnvoy>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["CEnvoyConfirm"] = args =>
            {
                asked.Add(string.Join(">", args!));
                return false;
            },
        });
        CPanel panel = TInterfaceConductPanel.TPanelCreate(
            engine, envoy, "Guild", static () => false, static _ => true);
        LVista vista = engine.TEngineVistaStart("guild", LCatalogOrder.LCatalogOrderName);
        panel.TPanelVistaRestore(vista);
        vista.TVistaSelect(ada.LAuthorId);

        panel.CPanelBin.CPanelBinDelete();

        Assert.Equal(["Guild.DetachConfirm>Guild.DetachCount>1"], asked);
        Assert.Equal(ada.LAuthorId, panel.TPanelChosenRead());
    }
}
