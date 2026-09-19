using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PWindow
{
    internal void PWindowViewRestore(LWorkspaceState state)
    {
        PInput.PInputVistaRestore(
            _lPosture.LPostureVistaStart("input", LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword));
        PLibrary.PLibraryVistaRestore(
            _lPosture.LPostureVistaStart("library", LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword));
        PPhonology.PPhonologyVistaRestore(
            _lPosture.LPostureVistaStart("phonology", LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword));
        PFavorite.PFavoriteVistaRestore(
            _lPosture.LPostureVistaStart("favorite", LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword));
        PTaxonomy.PTaxonomyVistaRestore(
            _lPosture.LPostureVistaStart("taxonomy", LSubject.LSubjectTag, LCatalogOrder.LCatalogOrderName),
            _lPosture.LPostureVistaStart("membership", LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword));
        PTenor.PTenorVistaRestore(
            _lPosture.LPostureVistaStart("tenor", LSubject.LSubjectRegister, LCatalogOrder.LCatalogOrderName),
            _lPosture.LPostureVistaStart("cohort", LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword));
        PRepertoire.PRepertoireVistaRestore(
            _lPosture.LPostureVistaStart("repertoire", LSubject.LSubjectSituation, LCatalogOrder.LCatalogOrderName),
            _lPosture.LPostureVistaStart("occurrence", LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword));
        PReference.PReferenceVistaRestore(
            _lPosture.LPostureVistaStart("reference", LSubject.LSubjectReference, LCatalogOrder.LCatalogOrderName),
            _lPosture.LPostureVistaStart("footnote", LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword));
        PCorpus.PCorpusVistaRestore(
            _lPosture.LPostureVistaStart("corpus", LSubject.LSubjectExample, LCatalogOrder.LCatalogOrderText),
            _lPosture.LPostureVistaStart("quotation", LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword));
        PGuild.PGuildVistaRestore(
            _lPosture.LPostureVistaStart("guild", LSubject.LSubjectAuthor, LCatalogOrder.LCatalogOrderName),
            _lPosture.LPostureVistaStart("oeuvre", LSubject.LSubjectReference, LCatalogOrder.LCatalogOrderName));
        PYunjing.PYunjingVistaRestore(
            _lPosture.LPostureVistaStart("yunjing", null, LCatalogOrder.LCatalogOrderName),
            _lPosture.LPostureVistaStart("yunmu", null, LCatalogOrder.LCatalogOrderName),
            _lPosture.LPostureVistaStart("xiaoyun", LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword));

        PDuplex.PDuplexRestore(state);

        PNavigationRestore();
    }
}
