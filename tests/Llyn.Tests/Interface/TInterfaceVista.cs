using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LVista TEngineVistaStart(
        this LEngine engine, string tab, LCatalogOrder fallback, bool blank = false) =>
        engine.LEngineVistaStart(tab, TVistaSubjectRead(tab), fallback, blank);

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
        engine.LEngineEntryFind(vista);

    internal static IReadOnlyList<LCatalogPronunciation> TEnginePronunciationFind(this LEngine engine, LVista vista) =>
        engine.LEnginePronunciationFind(vista);

    internal static IReadOnlyList<LVistaRow> TEngineFavoriteFind(this LEngine engine, LVista vista) =>
        engine.LEngineFavoriteFind(vista);

    internal static IReadOnlyList<LCatalogTag> TEngineTagFind(this LEngine engine, LVista vista) =>
        engine.LEngineTagFind(vista);

    internal static IReadOnlyList<LDiwei> TEngineDiweiFind(
        this LEngine engine, LVista vista, string language, string kind) =>
        engine.LEngineDiweiFind(vista, language, kind);

    internal static IReadOnlyList<LVistaRow> TEngineXiaoyunFind(
        this LEngine engine, string language, IReadOnlyList<long> diweiIds, string query) =>
        engine.LEngineXiaoyunFind(language, diweiIds, query);

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

    internal static LRevision? TVistaDelete(this LVista vista) => vista.LVistaDelete();

    internal static IReadOnlyList<LVistaRow> TEngineEntryFind(this LEngine engine, LVista parent, LVista child) =>
        engine.LEngineEntryFind(parent, child);

    internal static IReadOnlyList<LCatalogExample> TEngineExampleFind(this LEngine engine, LVista vista,
        string unknown = "", string unwritten = "") => engine.LEngineExampleFind(vista, unknown, unwritten);

    internal static IReadOnlyList<LCatalogSituation> TEngineSituationFind(this LEngine engine, LVista vista) =>
        engine.LEngineSituationFind(vista);

    internal static IReadOnlyList<LCatalogReference> TEngineReferenceFind(this LEngine engine, LVista vista) =>
        engine.LEngineReferenceFind(vista);

    internal static IReadOnlyList<LCatalogAuthor> TEngineAuthorFind(this LEngine engine, LVista vista) =>
        engine.LEngineAuthorFind(vista);

    internal static IReadOnlyList<LCatalogRegister> TEngineRegisterFind(this LEngine engine, LVista vista) =>
        engine.LEngineRegisterFind(vista);

    internal static bool TTenureFlaggedCheck(this LTenure tenure) => tenure.LTenureFlaggedCheck();

    internal static IReadOnlyList<LVariety> TTenureVarietyRead(this LTenure tenure) => tenure.LTenureVarietyRead();

    internal static bool TEngineFlaggedCheck(this LEngine engine, LEntryDraft draft) => engine.LEngineFlaggedCheck(draft);

    internal static Task TEnginePortraitPrint(this LEngine engine, LVista vista, LPortraitLabel label, LPressTicket ticket) =>
        engine.LEnginePortraitPrint(vista, label, ticket);

    internal static Task TEnginePortraitPrint(this LEngine engine, LVista vista, LPortraitLegend legend, LPressTicket ticket) =>
        engine.LEnginePortraitPrint(vista, legend, ticket);

    internal static Task TEnginePortraitExport(this LEngine engine, LVista vista, string path,
        LPortraitFormat format, LPortraitLabel label) => engine.LEnginePortraitExport(vista, path, format, label);

    internal static IReadOnlyList<LVistaRow> TEngineProspectFind(this LEngine engine, string query) =>
        engine.LEngineProspectFind(query);
}
