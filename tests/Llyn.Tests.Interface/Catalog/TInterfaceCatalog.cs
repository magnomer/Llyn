using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static IReadOnlyList<LEntry> TEngineEntryFind(
        this LEngine engine,
        string query,
        LCatalogOrder order) =>
        engine.LEngineHearth.LEngineStaffHeld.LEngineStaffEntry.LEntryStaffQuery.LEntryFind(query, order);

    internal static IReadOnlyList<LCatalogFavorite> TEngineFavoriteFind(
        this LEngine engine,
        string query,
        LCatalogOrder order) =>
        engine.LEngineHearth.LEngineStaffHeld.LEngineStaffCatalog.LCatalogStaffFavorite
            .LFavoriteClerkFind(query, order);

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
        engine.LEngineCatalog.LEngineRegisterFind(query, order);

    internal static IReadOnlyList<LRegister> TEngineRegisterFind(
        this LEngine engine,
        string query,
        string language) =>
        engine.LEngineHearth.LEngineStaffHeld.LEngineStaffCatalog.LCatalogStaffRegister
            .LRegisterClerkFind(query, language);

    internal static LRegisterOffer TEngineRegisterFind(
        this LEngine engine,
        string text,
        string language,
        long card) =>
        engine.LEngineHearth.LEngineStaffHeld.LEngineStaffCatalog.LCatalogStaffRegister
            .LRegisterClerkFind(text, language, null, card);

    internal static IReadOnlyList<LTag> TEngineTagFind(
        this LEngine engine,
        string query,
        LCatalogOrder order) =>
        engine.LEngineCatalog.LEngineTagFind(query, order);

    internal static IReadOnlyDictionary<long, string> TEngineCitationRead(this LEngine engine) =>
        engine.LEngineHearth.LEngineStaffHeld.LEngineStaffCatalog.LCatalogStaffReference.LCitationRead();

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
        LReference stored = engine.LEngineHearth.LEngineStaffHeld.LEngineStaffCatalog.LCatalogStaffReference
            .LReferenceClerkCreate(title);
        engine.LEngineHearth.LEngineBulletinRaise(LSubject.LSubjectReference, stored.LReferenceId);
        return stored;
    }

    internal static long TEngineCitationResolve(
        this LEngine engine, long draftId, long cardId, long sentenceId, string title) =>
        engine.LEngineReference.LEngineCitationResolve(draftId, cardId, sentenceId, title);

    internal static (string, string, string) TCatalogMarkFind(string text, string word) =>
        LCatalog.LCatalogMarkFind(text, word);

    internal static string TCatalogUsageFormat(int usage) =>
        LCatalog.LCatalogUsageFormat(usage);

    internal static string TCatalogTallyFormat(int count, string realm) =>
        LCatalog.LCatalogTallyFormat(count, realm, static key => "text:" + key);

    internal static string TCatalogOrderFormat(LCatalogOrder order) =>
        LCatalog.LCatalogOrderFormat(order);

    internal static LCatalogOrder TCatalogOrderParse(string? text, LCatalogOrder fallback) =>
        LCatalog.LCatalogOrderParse(text, fallback);

    internal static LCatalogRegister TCatalogRegisterCreate(LRegister register, int usage) =>
        LCatalogRegister.LCatalogRegisterCreate(register, usage);

    internal static IReadOnlyList<LEntry> TCatalogEntrySort(IReadOnlyList<LEntry> entries, LCatalogOrder order) =>
        LCatalogEntry.LCatalogEntrySort(entries, order);

    internal static IReadOnlyList<LCatalogPronunciation> TCatalogPronunciationSort(
        IReadOnlyList<LCatalogPronunciation> rows,
        LCatalogOrder order) =>
        LCatalogPronunciation.LCatalogPronunciationSort(rows, order);

    internal static IReadOnlyList<LCatalogFavorite> TCatalogFavoriteSort(
        IReadOnlyList<LCatalogFavorite> favorites,
        LCatalogOrder order) =>
        LCatalogFavorite.LCatalogFavoriteSort(favorites, order);

    internal static IReadOnlyList<LCatalogExample> TCatalogExampleSort(
        IReadOnlyList<LCatalogExample> rows,
        LCatalogOrder order) =>
        LCatalogExample.LCatalogExampleSort(rows, order);

    internal static IReadOnlyList<LCatalogSituation> TCatalogSituationSort(
        IReadOnlyList<LCatalogSituation> rows,
        LCatalogOrder order) =>
        LCatalogSituation.LCatalogSituationSort(rows, order);

    internal static IReadOnlyList<LCatalogReference> TCatalogReferenceSort(
        IReadOnlyList<LCatalogReference> rows,
        LCatalogOrder order) =>
        LCatalogReference.LCatalogReferenceSort(rows, order);

    internal static IReadOnlyList<LCatalogRegister> TCatalogRegisterSort(
        IReadOnlyList<LCatalogRegister> rows,
        LCatalogOrder order) =>
        LCatalogRegister.LCatalogRegisterSort(rows, order);

    internal static LCatalogExample TCatalogExampleCreate(LExample stored, string source, int usage) =>
        new(stored, source, usage);

    internal static LCatalogFavorite TCatalogFavoriteCreate(LEntry entry, string marked) =>
        new(entry, marked);

    internal static LCatalogPronunciation TCatalogPronunciationCreate(LEntry entry, string sound) =>
        new(entry, sound);

    internal static LCatalogReference TCatalogReferenceCreate(
        LReference stored, string name, string byline, int usage) =>
        new(stored, name, byline, [], usage);

    internal static LCatalogSituation TCatalogSituationCreate(LSituation stored, int usage) =>
        new(stored, usage);
}
