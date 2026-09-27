using Llyn.Conduct;

namespace Llyn.UIDeportment;

public partial class PWindow
{
    internal void PWindowViewRestore(CWorkspaceState state)
    {
        PInput.PInputVistaRestore();
        _qLibrary.QLibraryVistaRestore();
        _qPhonology.QPhonologyVistaRestore();
        _qFavorite.QFavoriteVistaRestore();
        _qTaxonomy.QTaxonomyVistaRestore();
        _qTenor.QTenorVistaRestore();
        _qRepertoire.QRepertoireVistaRestore();
        _qReference.QReferenceVistaRestore();
        _qCorpus.QCorpusVistaRestore();
        _qGuild.QGuildVistaRestore();
        _qXiesheng.QXieshengVistaRestore();
        PYunjing.PYunjingVistaRestore();

        _qDuplex.QDuplexRestore(state);

        PNavigationRestore();
    }
}
