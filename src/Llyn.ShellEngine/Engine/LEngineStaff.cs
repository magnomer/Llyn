using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed record LEngineStaff(
    LDraftClerk LEngineStaffDraft,
    LChronicleClerk LEngineStaffChronicle,
    LCourtClerk LEngineStaffCourt,
    LClaimClerk LEngineStaffClaim,
    LTagClerk LEngineStaffTag,
    LRegisterClerk LEngineStaffRegister,
    LTranslationClerk LEngineStaffTranslation,
    LReferenceClerk LEngineStaffReference,
    LExampleClerk LEngineStaffExample,
    LSituationClerk LEngineStaffSituation,
    LMeaningClerk LEngineStaffMeaning,
    LMentionClerk LEngineStaffMention,
    LUsageClerk LEngineStaffUsage,
    LVocabularyClerk LEngineStaffVocabulary,
    LParadigmClerk LEngineStaffParadigm,
    LPronunciationClerk LEngineStaffPronunciation,
    LTrailClerk LEngineStaffTrail,
    LLanguageClerk LEngineStaffLanguage,
    LRecordingClerk LEngineStaffRecording,
    LTranscriptionClerk LEngineStaffTranscription,
    LReflexClerk LEngineStaffReflex,
    LEntryClerk LEngineStaffEntry,
    LLacunaClerk LEngineStaffLacuna,
    LFrequencyClerk LEngineStaffFrequency,
    LOutcomeClerk LEngineStaffOutcome,
    LAuthorClerk LEngineStaffAuthor,
    LFavoriteClerk LEngineStaffFavorite,
    LCitationClerk LEngineStaffCitation,
    LFanqieClerk LEngineStaffFanqie,
    LShengfuClerk LEngineStaffShengfu,
    LStemClerk LEngineStaffStem,
    LDiweiClerk LEngineStaffDiwei,
    LScriptClerk LEngineStaffScript,
    LWorkspaceClerk LEngineStaffWorkspace,
    LMarkupClerk LEngineStaffMarkup,
    LMarkupClerkIntake LEngineStaffIntake,
    LPortraitClerk LEngineStaffPortrait,
    LEnsign LEngineStaffEnsign)
{
    internal static LEngineStaff LEngineStaffBuild(
        LRig rig,
        object gate,
        Action<LSubject, long> raise,
        Func<LSettings> settings,
        IReadOnlySet<long> retired)
    {
        LIdentity identity = new(rig.LRigWorkspaces, retired);
        LLanguageCache cache = new(rig.LRigLanguages);
        LDraftClerk draft = new(rig, identity, cache);
        LChronicleClerk chronicle = new(rig);
        LTagClerk tag = new(rig);
        LRegisterClerk register = new(rig);
        LTranslationClerk translation = new(rig);
        LCourtClerk court = new(rig, identity, chronicle, translation);
        LClaimClerk claim = new(rig, identity, chronicle, court);
        LReferenceClerk reference = new(rig);
        LExampleClerk example = new(rig, reference);
        LSituationClerk situation = new(rig);
        LCardClerk card = new(rig, tag, register, translation, example);
        LMeaningClerk meaning = new(rig, card);
        LMentionClerk mention = new(rig, cache);
        LUsageClerk usage = new(rig);
        LVocabularyClerk vocabulary = new(rig);
        LParadigmClerk paradigm = new(rig);
        LInflectionClerk inflection = new(rig, paradigm);
        LPronunciationClerk pronunciation = new(rig);
        LTrailClerk trail = new(rig);
        LLanguageClerk language = new(rig, cache, trail);
        LRecordingClerk recording = new(rig, cache, trail, claim);
        LTranscriptionClerk transcription = new(rig, cache);
        LReflexClerk reflex = new(rig, cache, claim, gate, raise);
        LFrequencyClerk frequency = new(rig, cache, gate, settings, raise);
        LEntryClerk entry = new(
            rig, card, meaning, vocabulary, inflection, paradigm, pronunciation, transcription, reflex, recording,
            frequency);
        LLacunaClerk lacuna = new(rig, cache, paradigm, claim, gate, settings, raise);
        LOutcomeClerk outcome = new(
            rig, cache, draft, claim, court, entry, lacuna, frequency);
        LAuthorClerk author = new(rig);
        LFavoriteClerk favorite = new(rig);
        LCitationClerk citation = new(
            rig, identity, claim, author, example, reference, situation, entry);
        LFanqieClerk fanqie = new(rig, cache, gate, raise);
        LShengfuClerk shengfu = new(rig, cache, gate, raise);
        LStemClerk stem = new(rig);
        LDiweiClerk diwei = new(rig, cache);
        LScriptClerk script = new(rig, cache, gate, raise);
        LWorkspaceClerk workspace = new(rig, cache, reflex, paradigm);
        LMarkupClerk markup = new(rig);
        LMarkupClerkLink link = new(rig, reference, author, trail);
        LMarkupClerkIntake intake = new(
            rig, claim, entry, lacuna, frequency, link, new LMarkupClerkDraft(rig, link));
        LPortraitClerk portrait = new(
            rig, language, entry, vocabulary, translation, reference, favorite, fanqie, frequency,
            paradigm, script, markup, settings);
        LEnsign ensign = new(rig.LRigUsher);

        return new LEngineStaff(
            draft, chronicle, court, claim, tag, register, translation, reference, example,
            situation, meaning, mention, usage, vocabulary, paradigm, pronunciation, trail,
            language, recording, transcription, reflex, entry, lacuna, frequency, outcome, author, favorite,
            citation, fanqie, shengfu, stem, diwei, script, workspace, markup, intake, portrait, ensign);
    }
}
