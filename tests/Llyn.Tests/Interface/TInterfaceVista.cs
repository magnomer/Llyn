using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LVista TEngineVistaStart(
        this LEngine engine, string tab, LCatalogOrder fallback, bool blank = false) =>
        engine.LEngineVistaStart(tab, fallback, blank);

    internal static IReadOnlyList<LVistaRow> TEngineEntryFind(this LEngine engine, LVista vista) =>
        engine.LEngineEntryFind(vista);

    internal static IReadOnlyList<LCatalogPronunciation> TEnginePronunciationFind(this LEngine engine, LVista vista) =>
        engine.LEnginePronunciationFind(vista);

    internal static IReadOnlyList<LVistaRow> TEngineFavoriteFind(this LEngine engine, LVista vista) =>
        engine.LEngineFavoriteFind(vista);

    internal static IReadOnlyList<LTag> TEngineTagFind(this LEngine engine, LVista vista) =>
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
}
