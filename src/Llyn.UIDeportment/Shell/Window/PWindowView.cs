using Llyn.Conduct;

namespace Llyn.UIDeportment;

public partial class PWindow
{
    internal void PWindowViewRestore(CWorkspaceState state)
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
