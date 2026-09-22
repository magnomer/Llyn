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
            LRigTranscriptions = new TVaultFakeTranscription(),
            LRigReflexes = new TVaultFakeReflex(),
            LRigEtymologies = new TVaultFakeEtymology(),
        };

    internal static LRig TRigClaimCreate() =>
        TRigClerkCreate(new TVaultFake()) with
        {
            LRigDrafts = new TVaultFakeDraft(),
            LRigClaims = new TVaultFakeClaim(1),
            LRigCourts = new TVaultFakeCourt(),
        };

    internal static LRig TRigProcessSet(this LRig rig, int process)
    {
        ((TVaultFakeClaim)rig.LRigClaims).TClaimProcessSet(process);
        return rig with { LRigProcess = process };
    }

    internal static void TDraftStaleSet(this LRig rig, long id) =>
        ((TVaultFakeDraft)rig.LRigDrafts).TDraftStaleSet(id);

    internal static LClaimClerk TClaimClerkCreate(LRig rig)
    {
        LIdentity identity = new(rig.LRigWorkspaces);
        LChronicleClerk chronicle = new(rig);
        return new LClaimClerk(rig, identity, chronicle, TCourtClerkCreate(rig, identity, chronicle));
    }

    internal static LCourtClerk TCourtClerkCreate(LRig rig) =>
        TCourtClerkCreate(rig, new LIdentity(rig.LRigWorkspaces), new LChronicleClerk(rig));

    private static LCourtClerk TCourtClerkCreate(LRig rig, LIdentity identity, LChronicleClerk chronicle) =>
        new(rig, identity, chronicle, new LTranslationClerk(rig));

    internal static LDraft TClaimClerkStart(this LClaimClerk clerk, string origin, long entryId) =>
        clerk.LClaimClerkStart(clerk.LDraftCreate(origin, entryId, LClaimClerk.LDraftBlank));

    internal static IReadOnlySet<long> TClaimHeldRead(this LClaimClerk clerk) => clerk.LClaimClerkHeld;

    internal static bool TClaimClerkCheck(this LClaimClerk clerk, long id) => clerk.LClaimClerkCheck(id);

    internal static bool TClaimForeignCheck(this LClaimClerk clerk, long id) => clerk.LClaimForeignCheck(id);

    internal static LDraft? TClaimDraftRead(this LClaimClerk clerk, long id) => clerk.LDraftRead(id);

    internal static IReadOnlyList<LDraft> TClaimDraftScan(this LClaimClerk clerk) => clerk.LDraftScan();

    internal static void TClaimClerkFinish(this LClaimClerk clerk, long id) => clerk.LClaimClerkFinish(id);

    internal static void TClaimClerkCancel(this LClaimClerk clerk, long id) => clerk.LClaimClerkCancel(id);

    internal static void TClaimClerkSweep(this LClaimClerk clerk) => clerk.LClaimClerkSweep();

    internal static LCourt TCourtClerkSave(this LCourtClerk clerk, long ownerId, long targetId, string headword) =>
        clerk.LCourtClerkSave(ownerId, targetId, headword, "English");

    internal static IReadOnlyList<LCourt> TCourtClerkScan(this LCourtClerk clerk) => clerk.LCourtClerkScan();

    internal static LPortraitPage TExamplePageRead(
        LExample example, LReference? cited, int count, LPortraitLegend legend) =>
        LExampleClerk.LExamplePageRead(example, cited, count, legend);

    internal static LPortraitPage TReferencePageRead(
        LReference reference, IReadOnlyList<LAuthor> credits, int count, LPortraitLegend legend) =>
        LReferenceClerk.LReferencePageRead(reference, credits, count, legend);

    internal static LPortraitPage TSituationPageRead(LSituation situation, int count, LPortraitLegend legend) =>
        LSituationClerk.LSituationPageRead(situation, count, legend);

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
        LExampleClerk examples = new(rig, new LReferenceClerk(rig));
        LCardClerk cards = new(rig, tags, registers, translations, examples, new LSituationClerk(rig));
        LParadigmClerk paradigms = new(rig);
        LLanguageCache languages = new(rig.LRigLanguages);
        LClaimClerk claims = TClaimClerkCreate(rig);
        return new LEntryClerk(
            rig,
            cards,
            new LMeaningClerk(rig, cards),
            new LVocabularyClerk(rig),
            new LInflectionClerk(rig, paradigms),
            paradigms,
            new LPronunciationClerk(rig),
            new LTranscriptionClerk(rig, languages),
            new LReflexClerk(rig, languages, claims, new object(), static (_, _) => { }),
            new LRecordingClerk(rig, languages, new LTrailClerk(rig), claims));
    }

    internal static LRecordingClerk TRecordingClerkCreate(LRig rig) =>
        new(rig, new LLanguageCache(rig.LRigLanguages), new LTrailClerk(rig), TClaimClerkCreate(rig));

    internal static LMarkupClerkIntake TMarkupIntakeCreate(LRig rig)
    {
        LLanguageCache languages = new(rig.LRigLanguages);
        LClaimClerk claims = TClaimClerkCreate(rig);
        LEntryClerk entries = TEntryClerkCreate(rig);
        LMarkupClerkLink link = new(rig, new LReferenceClerk(rig), new LAuthorClerk(rig), new LTrailClerk(rig));
        return new LMarkupClerkIntake(
            rig,
            claims,
            entries,
            new LLacunaClerk(
                rig, languages, new LParadigmClerk(rig), claims, new object(), TSettingsRead, static (_, _) => { }),
            new LFrequencyClerk(rig, languages, new object(), TSettingsRead, static (_, _) => { }),
            link,
            new LMarkupClerkDraft(rig, link));
    }

    internal static LPortraitClerk TPortraitClerkCreate(LRig rig)
    {
        LLanguageCache languages = new(rig.LRigLanguages);
        object gate = new();
        return new LPortraitClerk(
            rig,
            new LLanguageClerk(rig, languages, new LTrailClerk(rig)),
            TEntryClerkCreate(rig),
            new LVocabularyClerk(rig),
            new LTranslationClerk(rig),
            new LReferenceClerk(rig),
            new LFavoriteClerk(rig),
            new LFanqieClerk(rig, languages, gate, static (_, _) => { }),
            new LFrequencyClerk(rig, languages, gate, TSettingsRead, static (_, _) => { }),
            new LParadigmClerk(rig),
            new LScriptClerk(rig, languages, gate, static (_, _) => { }),
            new LMarkupClerk(rig),
            TSettingsRead);
    }

    private static LSettings TSettingsRead() => TSettingsCreate("en");

    internal static Task<IReadOnlyList<LRecording>> TRecordingClerkFind(
        this LRecordingClerk clerk,
        string word,
        string language,
        string variety,
        LListener listener,
        CancellationToken cancellation) =>
        clerk.LRecordingClerkFind(word, language, variety, listener, cancellation);

    internal static void TRecordingClerkSweep(this LRecordingClerk clerk)
    {
        clerk.LRecordingClerkSweep();
    }

    internal static string TRecordingFormat(LRig rig, string path) => new LTrailClerk(rig).LRecordingFormat(path);

    internal static LPronunciation TPronunciationSave(LRig rig, LPronunciation pronunciation) =>
        rig.LRigPronunciations.LPronunciationCreate(pronunciation);

    internal static void TAudioSave(LRig rig, long pronunciationId, string file, string? source)
    {
        rig.LRigPronunciations.LPronunciationAudioSave(pronunciationId, file, source);
    }

    internal static LDoctorRescue TWorkspaceRescueCreate(LRig rig) => LWorkspaceClerk.LWorkspaceRescueCreate(rig);

    internal static LSettings TWorkspaceSettingsRead(LRig rig, LSettings fallback, out bool settled) =>
        LWorkspaceClerk.LWorkspaceSettingsRead(rig, fallback, out settled);

    internal static string? TWorkspaceNoticeRead(Exception exception) =>
        LWorkspaceClerk.LWorkspaceNoticeRead(exception);

    internal static LRefusal TRefusalCreate(string reason) => new(reason);

    internal static LMarkupClerk TMarkupClerkCreate(LRig rig) => new(rig);

    internal static LMarkupCargo TMarkupClerkRead(this LMarkupClerk clerk, string path) =>
        clerk.LMarkupClerkRead(path);

    internal static LMarkupEntry? TMarkupClerkLoad(this LMarkupClerk clerk, long id) => clerk.LMarkupLoad(id);

    internal static LMarkupOutcome TMarkupClerkImport(
        this LMarkupClerkIntake clerk, LMarkupCargo cargo, IReadOnlyList<LMarkupIntake> intakes) =>
        clerk.LMarkupClerkImport(cargo, intakes);

    internal static LPortraitPage TPortraitClerkRead(this LPortraitClerk clerk, long entryId, LPortraitLabel label) =>
        clerk.LPortraitClerkRead(entryId, label);

    internal static Task TPortraitClerkPrint(this LPortraitClerk clerk, LPortraitPage page, LPressTicket ticket) =>
        clerk.LPortraitClerkPrint(page, ticket);

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
