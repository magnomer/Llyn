using Llyn.Conduct;

namespace Llyn.UIDeportment;

public partial class PWindow
{
    internal void PWindowViewRestore(CWorkspaceState state)
    {
        PInput.PInputVistaRestore();
        _qLibrary.QLibraryVistaRestore();
        PPhonology.PPhonologyVistaRestore();
        _qFavorite.QFavoriteVistaRestore();
        _qTaxonomy.QTaxonomyVistaRestore();
        _qTenor.QTenorVistaRestore();
        _qRepertoire.QRepertoireVistaRestore();
        PReference.PReferenceVistaRestore();
        PCorpus.PCorpusVistaRestore();
        PGuild.PGuildVistaRestore();
        PXiesheng.PXieshengVistaRestore();
        PYunjing.PYunjingVistaRestore();

        _qDuplex.QDuplexRestore(state);

        PNavigationRestore();
    }
}
