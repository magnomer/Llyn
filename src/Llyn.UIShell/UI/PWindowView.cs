using Llyn.Core;

namespace Llyn.UIShell;

public partial class PWindow
{
    internal void PWindowViewRestore(LWorkspaceState state)
    {
        PLibrary.PSieveRestore(state.LWorkspaceStateSieve ?? LCatalogFilter.LCatalogFilterEmpty);
        PPhonology.PLensRestore(state.LWorkspaceStateLens ?? LCatalogFilter.LCatalogFilterEmpty);
        PFavorite.PStrainerRestore(state.LWorkspaceStateStrainer ?? LCatalogFilter.LCatalogFilterEmpty);
        PTaxonomy.PLatticeRestore(state.LWorkspaceStateLattice ?? LCatalogFilter.LCatalogFilterEmpty);
        PTenor.PGrilleRestore(state.LWorkspaceStateGrille ?? LCatalogFilter.LCatalogFilterEmpty);
        PCorpus.PPrismRestore(state.LWorkspaceStatePrism ?? LCatalogFilter.LCatalogFilterEmpty);

        PLibrary.POrderRestore(state.LWorkspaceStateOrder);
        PPhonology.PSequenceRestore(state.LWorkspaceStateSequence);
        PFavorite.PSeriesRestore(state.LWorkspaceStateSeries);
        PTaxonomy.PFunnelRestore(state.LWorkspaceStateFunnel);
        PTenor.PDegreeRestore(state.LWorkspaceStateDegree);
        PRepertoire.PTierRestore(state.LWorkspaceStateTier);
        PReference.PGradeRestore(state.LWorkspaceStateGrade);
        PCorpus.PRankRestore(state.LWorkspaceStateRank);

        PDuplex.PDuplexRestore(state);

        PNavigationRestore(state);
    }
}
