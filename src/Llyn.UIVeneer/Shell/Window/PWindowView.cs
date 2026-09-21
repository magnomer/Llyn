using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PWindow
{
    internal void PWindowViewRestore(LWorkspaceState state)
    {
        PInput.PInputVistaRestore();
        PLibrary.PLibraryVistaRestore();
        PPhonology.PPhonologyVistaRestore();
        PFavorite.PFavoriteVistaRestore();
        PTaxonomy.PTaxonomyVistaRestore();
        PTenor.PTenorVistaRestore();
        PRepertoire.PRepertoireVistaRestore();
        PReference.PReferenceVistaRestore();
        PCorpus.PCorpusVistaRestore();
        PGuild.PGuildVistaRestore();
        PXiesheng.PXieshengVistaRestore();
        PYunjing.PYunjingVistaRestore();

        PDuplex.PDuplexRestore(state);

        PNavigationRestore();
    }
}
