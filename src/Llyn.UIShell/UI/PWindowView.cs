using Llyn.Core;

namespace Llyn.UIShell;

public partial class PWindow
{
    internal void PWindowViewRestore(LWorkspaceState state)
    {
        LSettings settings = _lEngine.LEngineSettingsRead();

        PLibrary.PLibraryVistaRestore(_lEngine.LEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        PPhonology.PPhonologyVistaRestore(_lEngine.LEngineVistaStart("phonology", LCatalogOrder.LCatalogOrderHeadword));
        PFavorite.PFavoriteVistaRestore(_lEngine.LEngineVistaStart("favorite", LCatalogOrder.LCatalogOrderHeadword));
        PTaxonomy.PTaxonomyVistaRestore(_lEngine.LEngineVistaStart("taxonomy", LCatalogOrder.LCatalogOrderName));
        PTenor.PTenorVistaRestore(_lEngine.LEngineVistaStart("tenor", LCatalogOrder.LCatalogOrderName));
        PRepertoire.PRepertoireVistaRestore(_lEngine.LEngineVistaStart("repertoire", LCatalogOrder.LCatalogOrderName));
        PReference.PReferenceVistaRestore(_lEngine.LEngineVistaStart("reference", LCatalogOrder.LCatalogOrderName));
        PCorpus.PCorpusVistaRestore(_lEngine.LEngineVistaStart("corpus", LCatalogOrder.LCatalogOrderText));
        PGuild.PGuildVistaRestore(_lEngine.LEngineVistaStart("guild", LCatalogOrder.LCatalogOrderName));
        PYunjing.PYunjingVistaRestore(
            _lEngine.LEngineVistaStart("yunjing", LCatalogOrder.LCatalogOrderName),
            _lEngine.LEngineVistaStart("yunmu", LCatalogOrder.LCatalogOrderName));

        PDuplex.PDuplexRestore(state);

        PNavigationRestore(settings);
    }
}
