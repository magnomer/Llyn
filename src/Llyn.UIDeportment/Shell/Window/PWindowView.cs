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
        _qReference.QReferenceVistaRestore();
        _qCorpus.QCorpusVistaRestore();
        _qGuild.QGuildVistaRestore();
        PXiesheng.PXieshengVistaRestore();
        PYunjing.PYunjingVistaRestore();

        _qDuplex.QDuplexRestore(state);

        PNavigationRestore();
    }
}
