using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
    : IDisposable, LDraftPort, LEntryPort, LPhonologyPort, LSettingsPort, LMediaPort, LPortraitPort
{
    private readonly object _lEngineGate = new();
    private readonly LTrove _lEngineTrove = new();
    private LEnsign _lEngineEnsign;
    private string _lEngineWorkspace;
    private LSettings _lEngineSettings;
    private LDoctorRescue _lEngineRescue;
    private LRealm _lEngineRealm;
    private LIdentity _lEngineIdentity;
    private LLanguageCache _lEngineLanguageCache;
    private LDraftClerk _lEngineDraftClerk;
    private LChronicleClerk _lEngineChronicleClerk;
    private LCourtClerk _lEngineCourtClerk;
    private LClaimClerk _lEngineClaimClerk;
    private LTagClerk _lEngineTagClerk;
    private LRegisterClerk _lEngineRegisterClerk;
    private LTranslationClerk _lEngineTranslationClerk;
    private LReferenceClerk _lEngineReferenceClerk;
    private LExampleClerk _lEngineExampleClerk;
    private LSituationClerk _lEngineSituationClerk;
    private LCardClerk _lEngineCardClerk;
    private LMeaningClerk _lEngineMeaningClerk;
    private LMentionClerk _lEngineMentionClerk;
    private LUsageClerk _lEngineUsageClerk;
    private LVocabularyClerk _lEngineVocabularyClerk;
    private LParadigmClerk _lEngineParadigmClerk;
    private LInflectionClerk _lEngineInflectionClerk;
    private LPronunciationClerk _lEnginePronunciationClerk;
    private LTrailClerk _lEngineTrailClerk;
    private LLanguageClerk _lEngineLanguageClerk;
    private LRecordingClerk _lEngineRecordingClerk;
    private LTranscriptionClerk _lEngineTranscriptionClerk;
    private LReflexClerk _lEngineReflexClerk;
    private LEntryClerk _lEngineEntryClerk;
    private LLacunaClerk _lEngineLacunaClerk;
    private LFrequencyClerk _lEngineFrequencyClerk;
    private LOutcomeClerk _lEngineOutcomeClerk;
    private LAuthorClerk _lEngineAuthorClerk;
    private LFavoriteClerk _lEngineFavoriteClerk;
    private LCitationClerk _lEngineCitationClerk;
    private LFanqieClerk _lEngineFanqieClerk;
    private LShengfuClerk _lEngineShengfuClerk;
    private LStemClerk _lEngineStemClerk;
    private LDiweiClerk _lEngineDiweiClerk;
    private LScriptClerk _lEngineScriptClerk;
    private LWorkspaceClerk _lEngineWorkspaceClerk;
    private LMarkupClerk _lEngineMarkupClerk;
    private LMarkupClerkIntake _lEngineMarkupIntake;
    private LPortraitClerk _lEnginePortraitClerk;

    public LEngine(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);

        LEngineRigSet(rig);
        _lEngineSettings = _lEngineWorkspaceClerk.LWorkspaceSettingsRead();
        _lEngineRescue = LWorkspaceClerk.LWorkspaceRescueCreate(rig);
        _lEngineRealm = LWorkspaceClerk.LWorkspaceRealmRead(rig);
        LEngineWorkspaceOpen();
    }

    [MemberNotNull(
        nameof(_lEngineWorkspace),
        nameof(_lEngineIdentity),
        nameof(_lEngineLanguageCache),
        nameof(_lEngineDraftClerk),
        nameof(_lEngineChronicleClerk),
        nameof(_lEngineCourtClerk),
        nameof(_lEngineClaimClerk),
        nameof(_lEngineTagClerk),
        nameof(_lEngineRegisterClerk),
        nameof(_lEngineTranslationClerk),
        nameof(_lEngineReferenceClerk),
        nameof(_lEngineExampleClerk),
        nameof(_lEngineSituationClerk),
        nameof(_lEngineCardClerk),
        nameof(_lEngineMeaningClerk),
        nameof(_lEngineMentionClerk),
        nameof(_lEngineUsageClerk),
        nameof(_lEngineVocabularyClerk),
        nameof(_lEngineParadigmClerk),
        nameof(_lEngineInflectionClerk),
        nameof(_lEnginePronunciationClerk),
        nameof(_lEngineTrailClerk),
        nameof(_lEngineLanguageClerk),
        nameof(_lEngineRecordingClerk),
        nameof(_lEngineTranscriptionClerk),
        nameof(_lEngineReflexClerk),
        nameof(_lEngineEntryClerk),
        nameof(_lEngineLacunaClerk),
        nameof(_lEngineFrequencyClerk),
        nameof(_lEngineOutcomeClerk),
        nameof(_lEngineAuthorClerk),
        nameof(_lEngineFavoriteClerk),
        nameof(_lEngineCitationClerk),
        nameof(_lEngineFanqieClerk),
        nameof(_lEngineShengfuClerk),
        nameof(_lEngineStemClerk),
        nameof(_lEngineDiweiClerk),
        nameof(_lEngineScriptClerk),
        nameof(_lEngineWorkspaceClerk),
        nameof(_lEngineMarkupClerk),
        nameof(_lEngineMarkupIntake),
        nameof(_lEnginePortraitClerk),
        nameof(_lEngineEnsign))]
    private void LEngineRigSet(LRig rig)
    {
        object gate = _lEngineGate;
        Action<LSubject, long> raise = LEngineBulletinRaise;
        Func<LSettings> settings = LEngineSettingsRead;

        _lEngineWorkspace = rig.LRigWorkspace;
        _lEngineIdentity = new LIdentity(rig.LRigWorkspaces);
        _lEngineLanguageCache = new LLanguageCache(rig.LRigLanguages);
        _lEngineDraftClerk = new LDraftClerk(rig, _lEngineIdentity, _lEngineLanguageCache);
        _lEngineChronicleClerk = new LChronicleClerk(rig);
        _lEngineTagClerk = new LTagClerk(rig);
        _lEngineRegisterClerk = new LRegisterClerk(rig);
        _lEngineTranslationClerk = new LTranslationClerk(rig);
        _lEngineCourtClerk = new LCourtClerk(rig, _lEngineIdentity, _lEngineChronicleClerk, _lEngineTranslationClerk);
        _lEngineClaimClerk = new LClaimClerk(rig, _lEngineIdentity, _lEngineChronicleClerk, _lEngineCourtClerk);
        _lEngineReferenceClerk = new LReferenceClerk(rig);
        _lEngineExampleClerk = new LExampleClerk(rig, _lEngineReferenceClerk);
        _lEngineSituationClerk = new LSituationClerk(rig);
        _lEngineCardClerk = new LCardClerk(
            rig,
            _lEngineTagClerk,
            _lEngineRegisterClerk,
            _lEngineTranslationClerk,
            _lEngineExampleClerk,
            _lEngineSituationClerk);
        _lEngineMeaningClerk = new LMeaningClerk(rig, _lEngineCardClerk);
        _lEngineMentionClerk = new LMentionClerk(rig, _lEngineLanguageCache);
        _lEngineUsageClerk = new LUsageClerk(rig);
        _lEngineVocabularyClerk = new LVocabularyClerk(rig);
        _lEngineParadigmClerk = new LParadigmClerk(rig);
        _lEngineInflectionClerk = new LInflectionClerk(rig, _lEngineParadigmClerk);
        _lEnginePronunciationClerk = new LPronunciationClerk(rig);
        _lEngineTrailClerk = new LTrailClerk(rig);
        _lEngineLanguageClerk = new LLanguageClerk(rig, _lEngineLanguageCache, _lEngineTrailClerk);
        _lEngineRecordingClerk = new LRecordingClerk(
            rig, _lEngineLanguageCache, _lEngineTrailClerk, _lEngineClaimClerk);
        _lEngineTranscriptionClerk = new LTranscriptionClerk(rig, _lEngineLanguageCache);
        _lEngineReflexClerk = new LReflexClerk(rig, _lEngineLanguageCache, _lEngineClaimClerk, gate, raise);
        _lEngineEntryClerk = new LEntryClerk(
            rig,
            _lEngineCardClerk,
            _lEngineMeaningClerk,
            _lEngineVocabularyClerk,
            _lEngineInflectionClerk,
            _lEngineParadigmClerk,
            _lEnginePronunciationClerk,
            _lEngineTranscriptionClerk,
            _lEngineReflexClerk,
            _lEngineRecordingClerk);
        _lEngineLacunaClerk = new LLacunaClerk(
            rig, _lEngineLanguageCache, _lEngineParadigmClerk, _lEngineClaimClerk, gate, settings, raise);
        _lEngineFrequencyClerk = new LFrequencyClerk(rig, _lEngineLanguageCache, gate, settings, raise);
        _lEngineOutcomeClerk = new LOutcomeClerk(
            rig,
            _lEngineLanguageCache,
            _lEngineDraftClerk,
            _lEngineClaimClerk,
            _lEngineCourtClerk,
            _lEngineEntryClerk,
            _lEngineLacunaClerk,
            _lEngineFrequencyClerk);
        _lEngineAuthorClerk = new LAuthorClerk(rig);
        _lEngineFavoriteClerk = new LFavoriteClerk(rig);
        _lEngineCitationClerk = new LCitationClerk(
            rig,
            _lEngineIdentity,
            _lEngineClaimClerk,
            _lEngineAuthorClerk,
            _lEngineExampleClerk,
            _lEngineReferenceClerk,
            _lEngineSituationClerk,
            _lEngineEntryClerk);
        _lEngineFanqieClerk = new LFanqieClerk(rig, _lEngineLanguageCache, gate, raise);
        _lEngineShengfuClerk = new LShengfuClerk(rig, _lEngineLanguageCache, gate, raise);
        _lEngineStemClerk = new LStemClerk(rig);
        _lEngineDiweiClerk = new LDiweiClerk(rig, _lEngineLanguageCache);
        _lEngineScriptClerk = new LScriptClerk(rig, _lEngineLanguageCache, gate, raise);
        _lEngineWorkspaceClerk = new LWorkspaceClerk(
            rig, _lEngineLanguageCache, _lEngineReflexClerk, _lEngineParadigmClerk);
        _lEngineMarkupClerk = new LMarkupClerk(rig);
        LMarkupClerkLink link = new(rig, _lEngineReferenceClerk, _lEngineAuthorClerk, _lEngineTrailClerk);
        _lEngineMarkupIntake = new LMarkupClerkIntake(
            rig,
            _lEngineClaimClerk,
            _lEngineEntryClerk,
            _lEngineLacunaClerk,
            _lEngineFrequencyClerk,
            link,
            new LMarkupClerkDraft(rig, link));
        _lEnginePortraitClerk = new LPortraitClerk(
            rig,
            _lEngineLanguageClerk,
            _lEngineEntryClerk,
            _lEngineVocabularyClerk,
            _lEngineTranslationClerk,
            _lEngineReferenceClerk,
            _lEngineFavoriteClerk,
            _lEngineFanqieClerk,
            _lEngineFrequencyClerk,
            _lEngineParadigmClerk,
            _lEngineScriptClerk,
            _lEngineMarkupClerk,
            settings);
        _lEngineEnsign = new LEnsign(rig.LRigUsher);
    }

    private void LEngineWorkspaceOpen()
    {
        LEngineLanguageImport();
        _lEngineFanqieClerk.LDiweiApply();
        _lEngineShengfuClerk.LStemApply();
        if (_lEngineWorkspaceClerk.LWorkspaceClerkMigrated)
        {
            _lEngineWorkspaceClerk.LWorkspaceClerkUpdate();
        }
    }

    private long LEngineIdentityCreate()
    {
        lock (_lEngineGate)
        {
            return _lEngineIdentity.LIdentityCreate();
        }
    }

    internal LRealm LEngineRealmRead()
    {
        lock (_lEngineGate)
        {
            return _lEngineRealm;
        }
    }

    public LDoctorRescue LEngineRescueRead()
    {
        lock (_lEngineGate)
        {
            return _lEngineRescue;
        }
    }

    public string LEngineWorkspaceRead()
    {
        lock (_lEngineGate)
        {
            return _lEngineWorkspace;
        }
    }

    public string LEngineWorkspaceFormat()
    {
        lock (_lEngineGate)
        {
            return _lEngineTrailClerk.LWorkspaceFormat();
        }
    }

    public string? LEngineAuditRecord(Exception exception)
    {
        lock (_lEngineGate)
        {
            return _lEngineWorkspaceClerk.LWorkspaceAuditRecord(exception);
        }
    }

    public string? LEngineNoticeRead(Exception exception)
    {
        return LWorkspaceClerk.LWorkspaceNoticeRead(exception);
    }

    public void LEngineRigApply(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);

        lock (_lEngineGate)
        {
            LDoctorRescue rescue = LWorkspaceClerk.LWorkspaceRescueCreate(rig);
            LRealm realm = LWorkspaceClerk.LWorkspaceRealmRead(rig);
            LSettings settings = LWorkspaceClerk.LWorkspaceSettingsRead(rig, _lEngineSettings, out bool settled);

            foreach (long held in _lEngineClaimClerk.LClaimClerkHeld)
            {
                _lEngineDraftStale.Add(held);
            }

            LEngineFetchClear();
            _lEngineEnsign.LEnsignClear();
            _lEngineTrove.LTroveClear();

            _lEngineSettings = settings;
            _lEngineRescue = rescue;
            _lEngineRealm = realm;
            LEngineRigSet(rig);
            if (!settled)
            {
                _lEngineWorkspaceClerk.LWorkspaceSettingsSave(_lEngineSettings);
            }

            LEngineWorkspaceOpen();
        }

        LEngineBulletinRaise(LSubject.LSubjectWorkspace, 0);
    }

    private void LEngineFetchClear()
    {
        _lEngineFrequencyClerk.LFrequencyClerkClear();
        _lEngineLacunaClerk.LLacunaClerkClear();
        _lEngineScriptClerk.LScriptClerkClear();
        _lEngineFanqieClerk.LFanqieClerkClear();
        _lEngineShengfuClerk.LShengfuClerkClear();
        _lEngineReflexClerk.LReflexClerkClear();
    }

    private static bool LEngineOwnerCheck(LOwner owner)
    {
        return LCardClerk.LCardOwnerCheck(owner);
    }

    private static ArgumentOutOfRangeException LEngineOwnerRaise(LOwner owner)
    {
        return new ArgumentOutOfRangeException(
            nameof(owner), owner, "This entity has no reference from that kind of row.");
    }

    public void Dispose()
    {
        lock (_lEngineGate)
        {
            LEngineFetchClear();
        }
    }
}
