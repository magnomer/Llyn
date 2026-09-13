using System;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private const string LEngineStateLibrary = "library";
    private const string LEngineStatePhonology = "phonology";
    private const string LEngineStateFavorite = "favorite";
    private const string LEngineStateTaxonomy = "taxonomy";
    private const string LEngineStateTenor = "tenor";
    private const string LEngineStateRepertoire = "repertoire";
    private const string LEngineStateReference = "reference";
    private const string LEngineStateCorpus = "corpus";

    public LWorkspaceState LEngineStateRead()
    {
        lock (_lEngineGate)
        {
            return new LWorkspaceArchive(_lEngineDatabase).LWorkspaceStateRead();
        }
    }

    public void LEngineLeftSave(long? id)
    {
        LEngineStateChange(state => state with { LWorkspaceStateLeft = id });
    }

    public void LEngineRightSave(long? id)
    {
        LEngineStateChange(state => state with { LWorkspaceStateRight = id });
    }

    public void LEngineModeSave(string mode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mode);
        LEngineSettingsChange(settings => settings with { LSettingsMode = mode });
    }

    public void LEngineSplitSave(bool split)
    {
        LEngineSettingsChange(settings => settings with { LSettingsSplit = split });
    }

    public void LEngineOrderSave(LCatalogOrder order)
    {
        LEngineLayoutSave([new LLayout(LEngineStateLibrary, LLayoutOrder: order)]);
    }

    public void LEngineSequenceSave(LCatalogOrder order)
    {
        LEngineLayoutSave([new LLayout(LEngineStatePhonology, LLayoutOrder: order)]);
    }

    public void LEngineSeriesSave(LCatalogOrder order)
    {
        LEngineLayoutSave([new LLayout(LEngineStateFavorite, LLayoutOrder: order)]);
    }

    public void LEngineFunnelSave(LCatalogOrder order)
    {
        LEngineLayoutSave([new LLayout(LEngineStateTaxonomy, LLayoutOrder: order)]);
    }

    public void LEngineDegreeSave(LCatalogOrder order)
    {
        LEngineLayoutSave([new LLayout(LEngineStateTenor, LLayoutOrder: order)]);
    }

    public void LEngineTierSave(LCatalogOrder order)
    {
        LEngineLayoutSave([new LLayout(LEngineStateRepertoire, LLayoutOrder: order)]);
    }

    public void LEngineGradeSave(LCatalogOrder order)
    {
        LEngineLayoutSave([new LLayout(LEngineStateReference, LLayoutOrder: order)]);
    }

    public void LEngineRankSave(LCatalogOrder order)
    {
        LEngineLayoutSave([new LLayout(LEngineStateCorpus, LLayoutOrder: order)]);
    }

    public void LEngineSieveSave(LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        LEngineLayoutSave([new LLayout(LEngineStateLibrary, LLayoutFilter: filter)]);
    }

    public void LEngineLensSave(LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        LEngineLayoutSave([new LLayout(LEngineStatePhonology, LLayoutFilter: filter)]);
    }

    public void LEngineStrainerSave(LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        LEngineLayoutSave([new LLayout(LEngineStateFavorite, LLayoutFilter: filter)]);
    }

    public void LEngineLatticeSave(LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        LEngineLayoutSave([new LLayout(LEngineStateTaxonomy, LLayoutFilter: filter)]);
    }

    public void LEngineGrilleSave(LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        LEngineLayoutSave([new LLayout(LEngineStateTenor, LLayoutFilter: filter)]);
    }

    public void LEngineMeshSave(LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        LEngineLayoutSave([new LLayout(LEngineStateRepertoire, LLayoutFilter: filter)]);
    }

    public void LEngineGauzeSave(LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        LEngineLayoutSave([new LLayout(LEngineStateCorpus, LLayoutFilter: filter)]);
    }

    public void LEngineTrellisSave(LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        LEngineLayoutSave([new LLayout(LEngineStateReference, LLayoutFilter: filter)]);
    }

    private void LEngineStateChange(Func<LWorkspaceState, LWorkspaceState> change)
    {
        lock (_lEngineGate)
        {
            LWorkspaceArchive workspace = new(_lEngineDatabase);
            workspace.LWorkspaceStateSave(change(workspace.LWorkspaceStateRead()));
        }
    }
}
