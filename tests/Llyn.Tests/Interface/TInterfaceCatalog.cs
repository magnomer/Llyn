using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static IReadOnlyList<LEntry> TEngineEntryFind(
        this LEngine engine,
        string query,
        LCatalogOrder order) =>
        engine.LEngineEntryFind(query, order);

    internal static IReadOnlyList<LFavorite> TEngineFavoriteFind(
        this LEngine engine,
        string query,
        LCatalogOrder order) =>
        engine.LEngineFavoriteFind(query, order);

    internal static IReadOnlyList<LCatalogPronunciation> TEnginePronunciationFind(
        this LEngine engine,
        string query,
        LCatalogOrder order) =>
        engine.LEnginePronunciationFind(query, order);

    internal static IReadOnlyList<LCatalogReference> TEngineReferenceFind(
        this LEngine engine,
        string query,
        LCatalogOrder order) =>
        engine.LEngineReferenceFind(query, order);

    internal static IReadOnlyList<LCatalogExample> TEngineExampleFind(
        this LEngine engine,
        string query,
        LCatalogOrder order) =>
        engine.LEngineExampleFind(query, order);

    internal static IReadOnlyList<LCatalogSituation> TEngineSituationFind(
        this LEngine engine,
        string query,
        LCatalogOrder order) =>
        engine.LEngineSituationFind(query, order);

    internal static IReadOnlyList<LTag> TEngineTagFind(
        this LEngine engine,
        string query,
        LCatalogOrder order) =>
        engine.LEngineTagFind(query, order);

    internal static IReadOnlyDictionary<string, string> TEngineCitationRead(this LEngine engine) =>
        engine.LEngineCitationRead();

    internal static string TReferenceNameRead(this LReference reference) =>
        reference.LReferenceNameRead();

    internal static string TCatalogOrderFormat(LCatalogOrder order) =>
        LCatalog.LCatalogOrderFormat(order);

    internal static LCatalogOrder TCatalogOrderParse(string? text, LCatalogOrder fallback) =>
        LCatalog.LCatalogOrderParse(text, fallback);
}
