using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static IReadOnlyList<LEntry> TEngineEntryFind(
        this LEngine engine,
        string query,
        LCatalogOrder order) =>
        engine.LEngineStaffHeld.LEngineStaffEntry.LEntryClerkFind(query, order);

    internal static IReadOnlyList<LCatalogFavorite> TEngineFavoriteFind(
        this LEngine engine,
        string query,
        LCatalogOrder order) =>
        engine.LEngineStaffHeld.LEngineStaffFavorite.LFavoriteClerkFind(query, order);

    internal static IReadOnlyList<LCatalogPronunciation> TEnginePronunciationFind(
        this LEngine engine,
        string query,
        LCatalogOrder order) =>
        engine.LEnginePronunciation.LEnginePronunciationFind(query, order);

    internal static IReadOnlyList<LCatalogReference> TEngineReferenceFind(
        this LEngine engine,
        string query,
        LCatalogOrder order) =>
        engine.LEngineReference.LEngineReferenceFind(query, order);

    internal static IReadOnlyList<LCatalogExample> TEngineExampleFind(
        this LEngine engine,
        string query,
        LCatalogOrder order) =>
        engine.LEngineExample.LEngineExampleFind(query, order);

    internal static IReadOnlyList<LCatalogSituation> TEngineSituationFind(
        this LEngine engine,
        string query,
        LCatalogOrder order) =>
        engine.LEngineSituation.LEngineSituationFind(query, order);

    internal static IReadOnlyList<LCatalogRegister> TEngineRegisterFind(
        this LEngine engine,
        string query,
        LCatalogOrder order) =>
        engine.LEngineCard.LEngineRegisterFind(query, order);

    internal static IReadOnlyList<LRegister> TEngineRegisterFind(
        this LEngine engine,
        string query,
        string language) =>
        engine.LEngineStaffHeld.LEngineStaffRegister.LRegisterClerkFind(query, language);

    internal static IReadOnlyList<LTag> TEngineTagFind(
        this LEngine engine,
        string query,
        LCatalogOrder order) =>
        engine.LEngineCard.LEngineTagFind(query, order);

    internal static IReadOnlyDictionary<long, string> TEngineCitationRead(this LEngine engine) =>
        engine.LEngineStaffHeld.LEngineStaffReference.LCitationRead();

    internal static IReadOnlyDictionary<long, string> TEngineCitationRead(this LEngine engine, LEntryDraft shown) =>
        engine.LEngineReference.LEngineCitationRead(shown);

    internal static string TEngineCitationRead(this LEngine engine, LDraft? draft) =>
        engine.LEngineReference.LEngineCitationRead(draft);

    internal static IReadOnlyList<long> TEntryCitationRead(this LEntryDraft draft) => draft.LEntryDraftCitations;

    internal static LCardDraft TCardChildSet(this LCardDraft card, LCardDraft child) =>
        card with { LCardDraftChild = [child] };

    internal static string TReferenceNameRead(this LReference reference) =>
        reference.LReferenceNameRead();

    internal static LReference TEngineCitationCreate(this LEngine engine, string title)
    {
        LReference stored = engine.LEngineStaffHeld.LEngineStaffReference.LReferenceClerkCreate(title);
        engine.LEngineBulletinRaise(LSubject.LSubjectReference, stored.LReferenceId);
        return stored;
    }

    internal static long TEngineCitationResolve(
        this LEngine engine, long draftId, long cardId, long sentenceId, string title) =>
        engine.LEngineReference.LEngineCitationResolve(draftId, cardId, sentenceId, title);

    internal static (string, string, string) TCatalogMarkFind(string text, string word) =>
        LCatalog.LCatalogMarkFind(text, word);

    internal static string TCatalogUsageFormat(int usage) =>
        LCatalog.LCatalogUsageFormat(usage);

    internal static string TCatalogOrderFormat(LCatalogOrder order) =>
        LCatalog.LCatalogOrderFormat(order);

    internal static LCatalogOrder TCatalogOrderParse(string? text, LCatalogOrder fallback) =>
        LCatalog.LCatalogOrderParse(text, fallback);
}
