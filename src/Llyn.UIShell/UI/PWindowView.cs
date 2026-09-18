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
        PTaxonomy.PTaxonomyVistaRestore(
            _lEngine.LEngineVistaStart("taxonomy", LCatalogOrder.LCatalogOrderName),
            _lEngine.LEngineVistaStart("membership", LCatalogOrder.LCatalogOrderHeadword));
        PTenor.PTenorVistaRestore(
            _lEngine.LEngineVistaStart("tenor", LCatalogOrder.LCatalogOrderName),
            _lEngine.LEngineVistaStart("cohort", LCatalogOrder.LCatalogOrderHeadword));
        PRepertoire.PRepertoireVistaRestore(
            _lEngine.LEngineVistaStart("repertoire", LCatalogOrder.LCatalogOrderName),
            _lEngine.LEngineVistaStart("occurrence", LCatalogOrder.LCatalogOrderHeadword));
        PReference.PReferenceVistaRestore(
            _lEngine.LEngineVistaStart("reference", LCatalogOrder.LCatalogOrderName),
            _lEngine.LEngineVistaStart("footnote", LCatalogOrder.LCatalogOrderHeadword));
        PCorpus.PCorpusVistaRestore(
            _lEngine.LEngineVistaStart("corpus", LCatalogOrder.LCatalogOrderText),
            _lEngine.LEngineVistaStart("quotation", LCatalogOrder.LCatalogOrderHeadword));
        PGuild.PGuildVistaRestore(
            _lEngine.LEngineVistaStart("guild", LCatalogOrder.LCatalogOrderName),
            _lEngine.LEngineVistaStart("oeuvre", LCatalogOrder.LCatalogOrderName));
        PYunjing.PYunjingVistaRestore(
            _lEngine.LEngineVistaStart("yunjing", LCatalogOrder.LCatalogOrderName),
            _lEngine.LEngineVistaStart("yunmu", LCatalogOrder.LCatalogOrderName),
            _lEngine.LEngineVistaStart("xiaoyun", LCatalogOrder.LCatalogOrderHeadword));

        PDuplex.PDuplexRestore(state);

        PNavigationRestore(settings);
    }
}
