using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PWindow
{
    internal void PWindowViewRestore(LWorkspaceState state)
    {
        LSettings settings = _lEngine.LEngineSettingsRead();

        PLibrary.PLibraryVistaRestore(
            _lEngine.LEngineVistaStart("library", LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword));
        PPhonology.PPhonologyVistaRestore(
            _lEngine.LEngineVistaStart("phonology", LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword));
        PFavorite.PFavoriteVistaRestore(
            _lEngine.LEngineVistaStart("favorite", LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword));
        PTaxonomy.PTaxonomyVistaRestore(
            _lEngine.LEngineVistaStart("taxonomy", LSubject.LSubjectTag, LCatalogOrder.LCatalogOrderName),
            _lEngine.LEngineVistaStart("membership", LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword));
        PTenor.PTenorVistaRestore(
            _lEngine.LEngineVistaStart("tenor", LSubject.LSubjectRegister, LCatalogOrder.LCatalogOrderName),
            _lEngine.LEngineVistaStart("cohort", LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword));
        PRepertoire.PRepertoireVistaRestore(
            _lEngine.LEngineVistaStart("repertoire", LSubject.LSubjectSituation, LCatalogOrder.LCatalogOrderName),
            _lEngine.LEngineVistaStart("occurrence", LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword));
        PReference.PReferenceVistaRestore(
            _lEngine.LEngineVistaStart("reference", LSubject.LSubjectReference, LCatalogOrder.LCatalogOrderName),
            _lEngine.LEngineVistaStart("footnote", LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword));
        PCorpus.PCorpusVistaRestore(
            _lEngine.LEngineVistaStart("corpus", LSubject.LSubjectExample, LCatalogOrder.LCatalogOrderText),
            _lEngine.LEngineVistaStart("quotation", LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword));
        PGuild.PGuildVistaRestore(
            _lEngine.LEngineVistaStart("guild", LSubject.LSubjectAuthor, LCatalogOrder.LCatalogOrderName),
            _lEngine.LEngineVistaStart("oeuvre", LSubject.LSubjectReference, LCatalogOrder.LCatalogOrderName));
        PYunjing.PYunjingVistaRestore(
            _lEngine.LEngineVistaStart("yunjing", null, LCatalogOrder.LCatalogOrderName),
            _lEngine.LEngineVistaStart("yunmu", null, LCatalogOrder.LCatalogOrderName),
            _lEngine.LEngineVistaStart("xiaoyun", LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword));

        PDuplex.PDuplexRestore(state);

        PNavigationRestore(settings);
    }
}
