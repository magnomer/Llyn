using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LVista TEngineVistaStart(
        this LEngine engine, string tab, LCatalogOrder fallback, bool blank = false) =>
        engine.LEngineVista.LEngineVistaStart(
            tab, TVistaSubjectRead(tab), fallback, LCatalogFilter.LCatalogFilterEmpty, false, blank);

    internal static LVista TPostureVistaStart(
        this LPosture posture, string tab, LCatalogOrder fallback, bool blank = false) =>
        posture.LPostureVistaStart(tab, TVistaSubjectRead(tab), fallback, blank);

    private static LSubject? TVistaSubjectRead(string tab) => tab switch
    {
        "corpus" => LSubject.LSubjectExample,
        "reference" or "oeuvre" => LSubject.LSubjectReference,
        "repertoire" => LSubject.LSubjectSituation,
        "guild" => LSubject.LSubjectAuthor,
        "taxonomy" => LSubject.LSubjectTag,
        "tenor" => LSubject.LSubjectRegister,
        "yunjing" or "yunmu" => null,
        _ => LSubject.LSubjectEntry,
    };

    internal static IReadOnlyList<LVistaRow> TEngineEntryFind(this LEngine engine, LVista vista) =>
        engine.LEngineVista.LEngineEntryFind(vista);

    internal static IReadOnlyList<LCatalogPronunciation> TEnginePronunciationFind(this LEngine engine, LVista vista) =>
        engine.LEnginePronunciation.LEnginePronunciationFind(vista);

    internal static IReadOnlyList<LVistaRow> TEngineFavoriteFind(this LEngine engine, LVista vista) =>
        engine.LEngineVista.LEngineFavoriteFind(vista);

    internal static IReadOnlyList<LCatalogTag> TEngineTagFind(this LEngine engine, LVista vista) =>
        engine.LEngineCard.LEngineTagFind(vista);

    internal static IReadOnlyList<LDiwei> TEngineDiweiFind(
        this LEngine engine, LVista vista, string language, string kind) =>
        engine.LEngineFanqie.LEngineDiweiFind(vista, language, kind);

    internal static IReadOnlyList<LVistaRow> TEngineXiaoyunFind(
        this LEngine engine, string language, IReadOnlyList<long> diweiIds, string query) =>
        engine.LEngineFanqie.LEngineXiaoyunFind(language, diweiIds, query);

    internal static void TVistaOrderSet(this LVista vista, LCatalogOrder order)
    {
        vista.LVistaOrderSet(order);
    }

    internal static void TVistaFilterSet(this LVista vista, LCatalogFilter filter)
    {
        vista.LVistaFilterSet(filter);
    }

    internal static void TVistaQuerySet(this LVista vista, string query)
    {
        vista.LVistaQuerySet(query);
    }

    internal static void TVistaSelect(this LVista vista, long? id)
    {
        vista.LVistaSelect(id);
    }

    internal static void TVistaEditingSet(this LVista vista, bool editing) => vista.LVistaEditingSet(editing);

    internal static LDraft? TVistaLoad(this LVista vista) => vista.LVistaLoad();

    internal static string TVistaFileRead(LVista? vista) => LVista.LVistaFileRead(vista);

    internal static LRevision? TVistaDelete(this LVista vista) => vista.LVistaDelete();

    internal static IReadOnlyList<LVistaRow> TEngineEntryFind(this LEngine engine, LVista parent, LVista child) =>
        engine.LEngineVista.LEngineEntryFind(parent, child);

    internal static IReadOnlyList<LCatalogExample> TEngineExampleFind(
        this LEngine engine,
        LVista vista,
        string unknown = "",
        string unwritten = "") => engine.LEngineExample.LEngineExampleFind(vista, unknown, unwritten);

    internal static IReadOnlyList<LCatalogSituation> TEngineSituationFind(this LEngine engine, LVista vista) =>
        engine.LEngineSituation.LEngineSituationFind(vista);

    internal static IReadOnlyList<LCatalogReference> TEngineReferenceFind(this LEngine engine, LVista vista) =>
        engine.LEngineReference.LEngineReferenceFind(vista);

    internal static IReadOnlyList<LCatalogAuthor> TEngineAuthorFind(this LEngine engine, LVista vista) =>
        engine.LEngineAuthor.LEngineAuthorFind(vista);

    internal static IReadOnlyList<LCatalogRegister> TEngineRegisterFind(this LEngine engine, LVista vista) =>
        engine.LEngineCard.LEngineRegisterFind(vista);

    internal static bool TTenureFlaggedCheck(this LTenure tenure) => tenure.LTenureFlaggedCheck();

    internal static IReadOnlyList<LVariety> TTenureVarietyRead(this LTenure tenure) => tenure.LTenureVarietyRead();

    internal static bool TEngineFlaggedCheck(this LEngine engine, LEntryDraft draft) =>
        engine.LEngineLanguage.LEngineFlaggedCheck(draft);

    internal static Task TEnginePortraitPrint(
        this LEngine engine, LVista vista, LPortraitLabel label, LPressTicket ticket) =>
        engine.LEnginePortrait.LEnginePortraitPrint(vista, label, ticket);

    internal static Task TEnginePortraitPrint(
        this LEngine engine, LVista vista, LPortraitLegend legend, LPressTicket ticket) =>
        engine.LEnginePortrait.LEnginePortraitPrint(vista, legend, ticket);

    internal static Task TEnginePortraitExport(this LEngine engine, LVista vista, string path,
        LPortraitFormat format, LPortraitLabel label) =>
        engine.LEnginePortrait.LEnginePortraitExport(vista, path, format, label);

    internal static IReadOnlyList<LVistaRow> TEngineProspectFind(this LEngine engine, string query) =>
        engine.LEngineCard.LEngineProspectFind(query);
}
