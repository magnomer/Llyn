using System;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed record LLanguageStaff(
    LVocabularyClerk LLanguageStaffVocabulary,
    LParadigmClerk LLanguageStaffParadigm,
    LPronunciationClerk LLanguageStaffPronunciation,
    LTrailClerk LLanguageStaffTrail,
    LLanguageClerk LLanguageStaffLanguage,
    LRecordingClerk LLanguageStaffRecording,
    LTranscriptionClerk LLanguageStaffTranscription,
    LReflexClerk LLanguageStaffReflex,
    LLacunaClerk LLanguageStaffLacuna,
    LFrequencyClerk LLanguageStaffFrequency,
    LFanqieClerk LLanguageStaffFanqie,
    LShengfuClerk LLanguageStaffShengfu,
    LStemClerk LLanguageStaffStem,
    LDiweiClerk LLanguageStaffDiwei,
    LScriptClerk LLanguageStaffScript,
    LEnsign LLanguageStaffEnsign)
{
    internal static LLanguageStaff LLanguageStaffBuild(
        LRig rig,
        LLanguageCache cache,
        object gate,
        Action<LSubject, long> raise,
        Func<LSettings> settings,
        LClaimStaff claim)
    {
        LVocabularyClerk vocabulary = new(rig);
        LParadigmClerk paradigm = new(rig, cache);
        LPronunciationClerk pronunciation = new(rig);
        LTrailClerk trail = new(rig);
        LLanguageClerk language = new(rig, cache, trail);
        LRecordingClerk recording = new(rig, cache, trail, claim.LClaimStaffClaim);
        LTranscriptionClerk transcription = new(rig, cache);
        LReflexClerk reflex = new(rig, cache, claim.LClaimStaffClaim, gate, raise);
        LFrequencyClerk frequency = new(rig, cache, gate, settings, raise);
        LLacunaClerk lacuna = new(rig, cache, paradigm, claim.LClaimStaffClaim, gate, settings, raise);
        LFanqieClerk fanqie = new(rig, cache, gate, raise);
        LShengfuClerk shengfu = new(rig, cache, gate, raise);
        LStemClerk stem = new(rig);
        LDiweiClerk diwei = new(rig, cache);
        LScriptClerk script = new(rig, cache, gate, raise);
        LEnsign ensign = new(rig.LRigUsher);

        return new LLanguageStaff(
            vocabulary, paradigm, pronunciation, trail, language, recording, transcription, reflex, lacuna,
            frequency, fanqie, shengfu, stem, diwei, script, ensign);
    }

    internal void LLanguageStaffApply()
    {
        LLanguageStaffFanqie.LDiweiApply();
        LLanguageStaffShengfu.LStemApply();
    }

    internal void LLanguageStaffClear()
    {
        LLanguageStaffFrequency.LFrequencyClerkClear();
        LLanguageStaffLacuna.LLacunaClerkClear();
        LLanguageStaffScript.LScriptClerkClear();
        LLanguageStaffFanqie.LFanqieClerkClear();
        LLanguageStaffShengfu.LShengfuClerkClear();
        LLanguageStaffReflex.LReflexClerkFetch.LReflexFetchClear();
    }
}
