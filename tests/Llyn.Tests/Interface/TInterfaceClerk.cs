using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LRig TRigClerkCreate(LEntryVault entries) =>
        TRigFake.TRigFakeBuild(entries) with
        {
            LRigWorkspaces = new TVaultFakeWorkspace(),
            LRigRevisions = new TVaultFakeRevision(),
            LRigTombstones = new TVaultFakeTombstone(),
            LRigPronunciations = new TVaultFakePronunciation(),
        };

    internal static LDraftClerk TDraftClerkCreate(LRig rig) =>
        new(rig, new LIdentity(rig.LRigWorkspaces), new LLanguageCache(rig.LRigLanguages));

    internal static LDraft TDraftClerkApply(this LDraftClerk clerk, LDraft draft, LRequest request) =>
        clerk.LDraftClerkApply(draft, request);

    internal static LRequest TRequestStrayCreate(long draftId) => new TRequestStray(draftId);

    private sealed record TRequestStray(long LRequestDraftId) : LRequest(LRequestDraftId)
    {
        public override string LRequestKey => nameof(TRequestStray);
    }

    internal static LEntryClerk TEntryClerkCreate(LRig rig)
    {
        LTagClerk tags = new(rig);
        LRegisterClerk registers = new(rig);
        LTranslationClerk translations = new(rig);
        LCardClerk cards = new(rig, tags, registers, translations, new LExampleClerk(rig));
        LParadigmClerk paradigms = new(rig);
        return new LEntryClerk(
            rig,
            cards,
            new LMeaningClerk(rig, cards),
            new LVocabularyClerk(rig),
            new LInflectionClerk(rig, paradigms),
            paradigms,
            new LPronunciationClerk(rig));
    }

    internal static LEntry TEntryClerkSave(this LEntryClerk clerk, LEntryDraft draft) =>
        clerk.LEntryClerkSave(draft, []);

    internal static LTranslationClerk TTranslationClerkCreate(LRig rig) => new(rig);

    internal static IReadOnlyList<LEntry> TTranslationClerkFind(
        this LTranslationClerk clerk, string query, long? entryId) =>
        clerk.LTranslationClerkFind(query, entryId);

    internal static LEntry? TTranslationClerkResolve(this LTranslationClerk clerk, string word, long? entryId) =>
        clerk.LTranslationClerkResolve(word, entryId);

    internal static LEntry TEntryClerkAdd(this LEntryClerk clerk, LEntry entry) =>
        clerk.LEntryClerkCreate(entry, [], []);

    internal static LEntry? TEntryClerkRead(this LEntryClerk clerk, long id) => clerk.LEntryClerkRead(id);

    internal static IReadOnlyList<LEntry> TEntryClerkFind(this LEntryClerk clerk, string query, LCatalogOrder order) =>
        clerk.LEntryClerkFind(query, order);

    internal static LRevision TEntryClerkDelete(this LEntryClerk clerk, long id) => clerk.LEntryClerkDelete(id);

    internal static LTombstone? TEntryTombstoneRead(this LEntryClerk clerk, long entryId) =>
        clerk.LTombstoneRead(entryId);

    internal static LRevision? TEntryRevisionRead(this LEntryClerk clerk) => clerk.LRevisionRead();

    internal static IReadOnlyList<LRevisionChange> TEntryChangeRead(this LEntryClerk clerk, long revisionId) =>
        clerk.LRevisionChangeRead(revisionId);
}
