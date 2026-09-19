using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine : IDisposable
{
    private readonly object _lEngineGate = new();
    private readonly Dictionary<string, IReadOnlyList<LSource>> _lEngineLookupSources = new(StringComparer.Ordinal);
    private readonly Dictionary<string, IReadOnlyList<LSource>> _lEngineHarvestSources = new(StringComparer.Ordinal);
    private readonly Dictionary<(string LEngineLanguage, string LEngineScheme), IReadOnlyList<LSource>>
        _lEngineSchemeSources = [];
    private readonly Dictionary<string, IReadOnlyList<LSource>> _lEngineFrequencySources = new(StringComparer.Ordinal);
    private readonly Dictionary<long, CancellationTokenSource> _lEngineFrequencyPending = [];
    private readonly HashSet<long> _lEngineFrequencyMissed = [];
    private readonly Dictionary<string, IReadOnlyList<LSource>> _lEngineInflectionSources = new(StringComparer.Ordinal);
    private readonly Dictionary<long, CancellationTokenSource> _lEngineInflectionPending = [];
    private readonly Dictionary<string, LLanguage> _lEngineLanguages = new(StringComparer.Ordinal);
    private IReadOnlyList<string>? _lEngineLanguageListed;
    private readonly Dictionary<string, LSpeechPack> _lEngineSpeechPacks = new(StringComparer.Ordinal);
    private readonly LTrove _lEngineTrove = new();
    private readonly LEnsign _lEngineEnsign;
    private string _lEngineWorkspace;
    private LSettings _lEngineSettings;
    private LSourceFactory _lEngineSourceFactory;
    private LFanqieSource _lEngineFanqieSource;
    private LReflexSource _lEngineReflexSource;
    private LScriptSource _lEngineScriptSource;
    private LRecordingVault _lEngineRecordings;
    private LUsher _lEngineUsher;
    private LLanguageVault _lEngineLanguageVault;
    private LLocalizationVault _lEngineLocalization;
    private LMarkupVault _lEngineMarkupVault;
    private LPortraitVault _lEnginePortraitVault;
    private LVault _lEngineVault;
    private LDoctorVault _lEngineDoctor;
    private LRealmVault _lEngineRealmVault;
    private LSettingsVault _lEngineSettingsVault;
    private LAuditVault _lEngineAudit;
    private LKeep _lEngineKeep;
    private LEntryVault _lEngineEntries;
    private LDraftVault _lEngineDrafts;
    private LClaimVault _lEngineClaims;
    private LCourtVault _lEngineCourts;
    private LRevisionVault _lEngineRevisions;
    private LWorkspaceVault _lEngineWorkspaces;
    private LTombstoneVault _lEngineTombstones;
    private LAuthorVault _lEngineAuthors;
    private LCollocationVault _lEngineCollocations;
    private LDiweiVault _lEngineDiweiVault;
    private LExampleVault _lEngineExamples;
    private LFanqieVault _lEngineFanqieVault;
    private LFavoriteVault _lEngineFavorites;
    private LFrequencyVault _lEngineFrequencies;
    private LGlossVault _lEngineGlosses;
    private LImageVault _lEngineImages;
    private LInflectionVault _lEngineInflections;
    private LLacunaVault _lEngineLacunae;
    private LMeaningVault _lEngineMeanings;
    private LMentionVault _lEngineMentions;
    private LMorphologyVault _lEngineMorphologies;
    private LNoteVault _lEngineNotes;
    private LPronunciationVault _lEnginePronunciations;
    private LReferenceVault _lEngineReferences;
    private LReflexVault _lEngineReflexes;
    private LRegisterVault _lEngineRegisters;
    private LScriptVault _lEngineScripts;
    private LSentenceVault _lEngineSentences;
    private LSituationVault _lEngineSituations;
    private LSpeechVault _lEngineSpeeches;
    private LTagVault _lEngineTags;
    private LTranscriptionVault _lEngineTranscriptions;
    private LTranslationVault _lEngineTranslations;
    private LVideoVault _lEngineVideos;
    private LDoctorRescue _lEngineRescue;
    private LRealm _lEngineRealm;
    private LIdentity _lEngineIdentity;

    public LEngine(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);

        LEngineRigSet(rig);
        _lEngineEnsign = new LEnsign(_lEngineUsher);
        _lEngineSettings = _lEngineSettingsVault.LSettingsRead();
        _lEngineRescue = _lEngineDoctor.LDoctorDatabaseCreate();
        _lEngineRealm = _lEngineRealmVault.LRealmRead();
        _lEngineIdentity = new LIdentity(_lEngineWorkspaces);

        LEngineLanguageImport();
        LEngineDiweiApply();
        if (_lEngineVault.LVaultMigrated)
        {
            LEngineWorkspaceUpdate();
        }
    }

    [MemberNotNull(
        nameof(_lEngineSourceFactory),
        nameof(_lEngineFanqieSource),
        nameof(_lEngineReflexSource),
        nameof(_lEngineScriptSource),
        nameof(_lEngineRecordings),
        nameof(_lEngineUsher),
        nameof(_lEngineLanguageVault),
        nameof(_lEngineLocalization),
        nameof(_lEngineMarkupVault),
        nameof(_lEnginePortraitVault),
        nameof(_lEngineVault),
        nameof(_lEngineDoctor),
        nameof(_lEngineRealmVault),
        nameof(_lEngineSettingsVault),
        nameof(_lEngineAudit),
        nameof(_lEngineKeep),
        nameof(_lEngineEntries),
        nameof(_lEngineDrafts),
        nameof(_lEngineClaims),
        nameof(_lEngineCourts),
        nameof(_lEngineRevisions),
        nameof(_lEngineWorkspaces),
        nameof(_lEngineTombstones),
        nameof(_lEngineAuthors),
        nameof(_lEngineCollocations),
        nameof(_lEngineDiweiVault),
        nameof(_lEngineExamples),
        nameof(_lEngineFanqieVault),
        nameof(_lEngineFavorites),
        nameof(_lEngineFrequencies),
        nameof(_lEngineGlosses),
        nameof(_lEngineImages),
        nameof(_lEngineInflections),
        nameof(_lEngineLacunae),
        nameof(_lEngineMeanings),
        nameof(_lEngineMentions),
        nameof(_lEngineMorphologies),
        nameof(_lEngineNotes),
        nameof(_lEnginePronunciations),
        nameof(_lEngineReferences),
        nameof(_lEngineReflexes),
        nameof(_lEngineRegisters),
        nameof(_lEngineScripts),
        nameof(_lEngineSentences),
        nameof(_lEngineSituations),
        nameof(_lEngineSpeeches),
        nameof(_lEngineTags),
        nameof(_lEngineTranscriptions),
        nameof(_lEngineTranslations),
        nameof(_lEngineVideos),
        nameof(_lEngineWorkspace))]
    private void LEngineRigSet(LRig rig)
    {
        _lEngineSourceFactory = rig.LRigSources;
        _lEngineFanqieSource = rig.LRigFanqieSource;
        _lEngineReflexSource = rig.LRigReflexSource;
        _lEngineScriptSource = rig.LRigScriptSource;
        _lEngineRecordings = rig.LRigRecordings;
        _lEngineUsher = rig.LRigUsher;
        _lEngineLanguageVault = rig.LRigLanguages;
        _lEngineLocalization = rig.LRigLocalization;
        _lEngineMarkupVault = rig.LRigMarkup;
        _lEnginePortraitVault = rig.LRigPortrait;
        _lEngineVault = rig.LRigVault;
        _lEngineDoctor = rig.LRigDoctor;
        _lEngineRealmVault = rig.LRigRealm;
        _lEngineSettingsVault = rig.LRigSettings;
        _lEngineAudit = rig.LRigAudit;
        _lEngineKeep = rig.LRigKeep;
        _lEngineEntries = rig.LRigEntries;
        _lEngineDrafts = rig.LRigDrafts;
        _lEngineClaims = rig.LRigClaims;
        _lEngineCourts = rig.LRigCourts;
        _lEngineRevisions = rig.LRigRevisions;
        _lEngineWorkspaces = rig.LRigWorkspaces;
        _lEngineTombstones = rig.LRigTombstones;
        _lEngineAuthors = rig.LRigAuthors;
        _lEngineCollocations = rig.LRigCollocations;
        _lEngineDiweiVault = rig.LRigDiwei;
        _lEngineExamples = rig.LRigExamples;
        _lEngineFanqieVault = rig.LRigFanqie;
        _lEngineFavorites = rig.LRigFavorites;
        _lEngineFrequencies = rig.LRigFrequencies;
        _lEngineGlosses = rig.LRigGlosses;
        _lEngineImages = rig.LRigImages;
        _lEngineInflections = rig.LRigInflections;
        _lEngineLacunae = rig.LRigLacunae;
        _lEngineMeanings = rig.LRigMeanings;
        _lEngineMentions = rig.LRigMentions;
        _lEngineMorphologies = rig.LRigMorphologies;
        _lEngineNotes = rig.LRigNotes;
        _lEnginePronunciations = rig.LRigPronunciations;
        _lEngineReferences = rig.LRigReferences;
        _lEngineReflexes = rig.LRigReflexes;
        _lEngineRegisters = rig.LRigRegisters;
        _lEngineScripts = rig.LRigScripts;
        _lEngineSentences = rig.LRigSentences;
        _lEngineSituations = rig.LRigSituations;
        _lEngineSpeeches = rig.LRigSpeeches;
        _lEngineTags = rig.LRigTags;
        _lEngineTranscriptions = rig.LRigTranscriptions;
        _lEngineTranslations = rig.LRigTranslations;
        _lEngineVideos = rig.LRigVideos;
        _lEngineWorkspace = rig.LRigWorkspace;
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
        string root = LEngineWorkspaceRead();
        string name = Path.GetFileName(Path.TrimEndingDirectorySeparator(root));
        return name.Length > 0 ? name : root;
    }

    public string? LEngineAuditRecord(Exception exception)
    {
        lock (_lEngineGate)
        {
            return _lEngineAudit.LAuditRecord(exception);
        }
    }

    public void LEngineRigApply(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);

        lock (_lEngineGate)
        {
            LDoctorRescue rescue = rig.LRigDoctor.LDoctorDatabaseCreate();
            LRealm realm = rig.LRigRealm.LRealmRead();
            LSettings settings = rig.LRigSettings.LSettingsExist()
                ? rig.LRigSettings.LSettingsRead()
                : _lEngineSettings;

            foreach (long held in _lEngineDraftHeld)
            {
                _lEngineDraftStale.Add(held);
            }

            _lEngineDraftHeld.Clear();
            _lEngineLookupSources.Clear();
            _lEngineHarvestSources.Clear();
            _lEngineFrequencySources.Clear();
            LEngineFrequencyClear();
            _lEngineInflectionSources.Clear();
            LEngineInflectionClear();
            LEngineScriptClear();
            LEngineFanqieClear();
            LEngineReflexClear();
            _lEngineLanguages.Clear();
            _lEngineLanguageListed = null;
            _lEngineEnsign.LEnsignClear();
            _lEngineSpeechPacks.Clear();
            _lEngineTrove.LTroveClear();

            _lEngineSettings = settings;
            _lEngineRescue = rescue;
            _lEngineRealm = realm;
            LEngineRigSet(rig);
            _lEngineIdentity = new LIdentity(_lEngineWorkspaces);
            LEngineSettingsSave();
            LEngineLanguageImport();
            LEngineDiweiApply();
            if (_lEngineVault.LVaultMigrated)
            {
                LEngineWorkspaceUpdate();
            }
        }

        LEngineBulletinRaise(LSubject.LSubjectWorkspace, 0);
    }

    private static bool LEngineOwnerCheck(LOwner owner)
    {
        return owner switch
        {
            LOwner.LOwnerMeaning => false,
            LOwner.LOwnerCollocation => true,
            _ => throw LEngineOwnerRaise(owner),
        };
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
            LEngineFrequencyClear();
            LEngineInflectionClear();
            LEngineScriptClear();
            LEngineFanqieClear();
            LEngineReflexClear();
        }
    }
}
