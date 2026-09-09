using Llyn.Core;

namespace Llyn.UIShell;

public partial class PWindow
{
    internal void PWindowViewRestore(LWorkspaceState state)
    {
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
