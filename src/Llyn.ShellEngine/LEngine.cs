using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Threading;
using Llyn.Application;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine : IDisposable
{
    private readonly object _lEngineGate = new();
    private const long LEngineClientCeiling = 8L * 1024 * 1024;

    private readonly HttpClient _lEngineClient;
    private readonly LSourceFactory _lEngineSourceFactory;
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
    private readonly LUsher _lEngineUsher = new LUsherFile();
    private readonly LEnsign _lEngineEnsign;
    private readonly LLanguageVault _lEngineLanguageVault = new LLanguageLoader();
    private readonly LLocalizationVault _lEngineLocalization = new LLocalizationLoader();
    private readonly LMarkupVault _lEngineMarkupVault = new LMarkupFile();
    private readonly LPortraitVault _lEnginePortraitVault = new LPortraitFile();
    private string _lEngineWorkspace;
    private LSettings _lEngineSettings;
    private LDatabase _lEngineDatabase;
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

    public LEngine(string workspace)
        : this(workspace, null)
    {
    }

    internal LEngine(string workspace, HttpClient? client)
        : this(workspace, client, null)
    {
    }

    internal LEngine(string workspace, HttpClient? client, LEntryVault? entries)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspace);

        _lEngineWorkspace = workspace;
        _lEngineEnsign = new LEnsign(_lEngineUsher);
        _lEngineClient = client ?? new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(10),
            MaxResponseContentBufferSize = LEngineClientCeiling,
        };
        _lEngineClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) Llyn/0.0 (pronunciation lookup)");
        _lEngineSourceFactory = new LSourceFactoryHttp(_lEngineClient);

        _lEngineDatabase = new LDatabase(_lEngineWorkspace);
        LEngineVaultSet();
        _lEngineSettings = _lEngineSettingsVault.LSettingsRead();
        _lEngineRescue = _lEngineDoctor.LDoctorDatabaseCreate();
        _lEngineRealm = _lEngineRealmVault.LRealmRead();
        if (entries is not null)
        {
            _lEngineEntries = entries;
        }

        _lEngineIdentity = new LIdentity(_lEngineWorkspaces);

        LEngineLanguageImport();
        LEngineDiweiApply();
        if (_lEngineDatabase.LDatabaseMigrated)
        {
            LEngineWorkspaceUpdate();
        }
    }

    [MemberNotNull(
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
        nameof(_lEngineVideos))]
    private void LEngineVaultSet()
    {
        _lEngineVault = _lEngineDatabase;
        _lEngineDoctor = new LDoctor(_lEngineDatabase);
        _lEngineRealmVault = new LRealmArchive(_lEngineDatabase);
        _lEngineSettingsVault = new LSettingsLoader(_lEngineWorkspace);
        _lEngineAudit = new LAuditWriter(_lEngineWorkspace);
        _lEngineKeep = new LKeepFile(_lEngineWorkspace);
        _lEngineEntries = new LEntryArchive(_lEngineDatabase);
        _lEngineDrafts = new LDraftArchive(_lEngineWorkspace);
        _lEngineClaims = new LClaimArchive(_lEngineWorkspace);
        _lEngineCourts = new LCourtArchive(_lEngineWorkspace);
        _lEngineRevisions = new LRevisionArchive(_lEngineDatabase);
        _lEngineWorkspaces = new LWorkspaceArchive(_lEngineDatabase);
        _lEngineTombstones = new LTombstoneArchive(_lEngineDatabase);
        _lEngineAuthors = new LAuthorArchive(_lEngineDatabase);
        _lEngineCollocations = new LCollocationArchive(_lEngineDatabase);
        _lEngineDiweiVault = new LDiweiArchive(_lEngineDatabase);
        _lEngineExamples = new LExampleArchive(_lEngineDatabase);
        _lEngineFanqieVault = new LFanqieArchive(_lEngineDatabase);
        _lEngineFavorites = new LFavoriteArchive(_lEngineDatabase);
        _lEngineFrequencies = new LFrequencyArchive(_lEngineDatabase);
        _lEngineGlosses = new LGlossArchive(_lEngineDatabase);
        _lEngineImages = new LImageArchive(_lEngineDatabase);
        _lEngineInflections = new LInflectionArchive(_lEngineDatabase);
        _lEngineLacunae = new LLacunaArchive(_lEngineDatabase);
        _lEngineMeanings = new LMeaningArchive(_lEngineDatabase);
        _lEngineMentions = new LMentionArchive(_lEngineDatabase);
        _lEngineMorphologies = new LMorphologyArchive(_lEngineDatabase);
        _lEngineNotes = new LNoteArchive(_lEngineDatabase);
        _lEnginePronunciations = new LPronunciationArchive(_lEngineDatabase);
        _lEngineReferences = new LReferenceArchive(_lEngineDatabase);
        _lEngineReflexes = new LReflexArchive(_lEngineDatabase);
        _lEngineRegisters = new LRegisterArchive(_lEngineDatabase);
        _lEngineScripts = new LScriptArchive(_lEngineDatabase);
        _lEngineSentences = new LSentenceArchive(_lEngineDatabase);
        _lEngineSituations = new LSituationArchive(_lEngineDatabase);
        _lEngineSpeeches = new LSpeechArchive(_lEngineDatabase);
        _lEngineTags = new LTagArchive(_lEngineDatabase);
        _lEngineTranscriptions = new LTranscriptionArchive(_lEngineDatabase);
        _lEngineTranslations = new LTranslationArchive(_lEngineDatabase);
        _lEngineVideos = new LVideoArchive(_lEngineDatabase);
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

    public void LEngineWorkspaceChange(string path)
    {
        LEngineWorkspaceOpen(path);
        LWorkspaceRoot.LWorkspaceRootChange(LEngineWorkspaceRead());
    }

    internal void LEngineWorkspaceOpen(string path)
    {
        if (!Path.IsPathFullyQualified(path))
        {
            throw new ArgumentException("The workspace path must be fully qualified.", nameof(path));
        }

        string root = Path.GetFullPath(path);

        lock (_lEngineGate)
        {
            Directory.CreateDirectory(root);

            LDatabase database = new(root);
            LDoctorRescue rescue = new LDoctor(database).LDoctorDatabaseCreate();
            LRealm realm = new LRealmArchive(database).LRealmRead();
            LSettingsVault settingsVault = new LSettingsLoader(root);
            LSettings settings = settingsVault.LSettingsExist()
                ? settingsVault.LSettingsRead()
                : _lEngineSettings;

            _lEngineWorkspace = root;

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
            _lEngineDatabase = database;
            _lEngineRescue = rescue;
            _lEngineRealm = realm;
            LEngineVaultSet();
            _lEngineIdentity = new LIdentity(_lEngineWorkspaces);
            LEngineSettingsSave();
            LEngineLanguageImport();
            LEngineDiweiApply();
            if (database.LDatabaseMigrated)
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
            _lEngineClient.Dispose();
        }
    }
}
