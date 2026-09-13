using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine : IDisposable
{
    private readonly object _lEngineGate = new();
    private readonly HttpClient _lEngineClient;
    private readonly Dictionary<string, IReadOnlyList<LSource>> _lEngineLookupSources = new(StringComparer.Ordinal);
    private readonly Dictionary<string, IReadOnlyList<LSource>> _lEngineHarvestSources = new(StringComparer.Ordinal);
    private readonly Dictionary<(string LEngineLanguage, string LEngineScheme), IReadOnlyList<LSource>> _lEngineSchemeSources = [];
    private readonly Dictionary<string, IReadOnlyList<LSource>> _lEngineFrequencySources = new(StringComparer.Ordinal);
    private readonly Dictionary<long, CancellationTokenSource> _lEngineFrequencyPending = [];
    private readonly HashSet<long> _lEngineFrequencyMissed = [];
    private readonly Dictionary<string, LLanguage> _lEngineLanguages = new(StringComparer.Ordinal);
    private readonly LTrove _lEngineTrove = new();
    private string _lEngineWorkspace;
    private LSettings _lEngineSettings;
    private LDatabase _lEngineDatabase;
    private LDoctorRescue _lEngineRescue;
    private LRealm _lEngineRealm;
    private LIdentity _lEngineIdentity;

    public LEngine()
        : this(LWorkspaceRoot.LWorkspaceRootRead())
    {
    }

    public LEngine(string workspace)
        : this(workspace, null)
    {
    }

    internal LEngine(string workspace, HttpClient? client)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspace);

        _lEngineWorkspace = workspace;
        _lEngineSettings = LSettingsLoader.LSettingsLoaderLoad(_lEngineWorkspace);

        _lEngineDatabase = new LDatabase(_lEngineWorkspace);
        _lEngineRescue = LDoctor.LDoctorDatabaseCreate(_lEngineDatabase);
        _lEngineRealm = new LRealmArchive(_lEngineDatabase).LRealmRead();
        _lEngineIdentity = new LIdentity(_lEngineDatabase);

        LEngineLanguageImport();

        _lEngineClient = client ?? new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(10)
        };
        _lEngineClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) Llyn/0.0 (pronunciation lookup)");
    }

    private long LEngineIdentityCreate()
    {
        lock (_lEngineGate)
        {
            return _lEngineIdentity.LIdentityCreate();
        }
    }

    public LRealm LEngineRealmRead()
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

    public string? LEngineAuditRecord(Exception exception)
    {
        lock (_lEngineGate)
        {
            return LAuditWriter.LAuditWriterRecord(_lEngineWorkspace, exception);
        }
    }

    public static string LEngineWorkspaceResolve()
    {
        return LWorkspaceRoot.LWorkspaceRootRead();
    }

    public void LEngineWorkspaceChange(string path)
    {
        lock (_lEngineGate)
        {
            LWorkspaceRoot.LWorkspaceRootChange(path);
            _lEngineWorkspace = path;

            foreach (long held in _lEngineDraftHeld)
            {
                _lEngineDraftStale.Add(held);
            }

            _lEngineDraftHeld.Clear();
            _lEngineLookupSources.Clear();
            _lEngineHarvestSources.Clear();
            _lEngineFrequencySources.Clear();
            LEngineFrequencyClear();
            _lEngineLanguages.Clear();
            _lEngineTrove.LTroveClear();
            LSettingsLoader.LSettingsLoaderSave(_lEngineWorkspace, _lEngineSettings);

            _lEngineDatabase = new LDatabase(_lEngineWorkspace);
            _lEngineRescue = LDoctor.LDoctorDatabaseCreate(_lEngineDatabase);
            _lEngineRealm = new LRealmArchive(_lEngineDatabase).LRealmRead();
            _lEngineIdentity = new LIdentity(_lEngineDatabase);
            LEngineLanguageImport();
        }

        LEngineBulletinRaise(LSubject.LSubjectWorkspace, 0);
    }

    private string LEngineRecordingFormat(string path)
    {
        string relative = Path.GetRelativePath(_lEngineWorkspace, path);
        return Path.IsPathRooted(relative) || relative.StartsWith("..", StringComparison.Ordinal)
            ? path
            : relative;
    }

    private string LEngineRecordingResolve(string file)
    {
        return Path.IsPathRooted(file) ? file : Path.Combine(_lEngineWorkspace, file);
    }

    public Task LEnginePronunciationFind(
        long session,
        string word,
        string language,
        LReceiver receiver,
        CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(receiver);

        IReadOnlyList<LCandidate>? held;
        IReadOnlyList<LSource> sources;
        LLanguage pack;
        lock (_lEngineGate)
        {
            held = _lEngineTrove.LTroveCandidateRead(session, word, language);
            sources = held is null ? LEngineLookupRead(language) : [];
            pack = LEngineLanguageLoad(language);
            if (_lEngineSettings.LSettingsRespelled && pack.LLanguageRespellings.Count > 0)
            {
                receiver = new LReceiverRespelling(receiver, pack.LLanguageRespellings);
            }
        }

        return held is null
            ? LEngineCandidateScan(session, word, language, sources, pack, receiver, cancellation)
            : LEngineCandidatePublish(held, receiver);
    }

    public Task LEngineRecordingFind(
        long session,
        string word,
        string language,
        long target,
        LListener listener,
        CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(listener);

        IReadOnlyList<LRecording>? held;
        IReadOnlyList<LSource> sources;
        LLanguage pack;
        string variety;
        lock (_lEngineGate)
        {
            variety = LEngineVarietyResolve(session, target);
            held = _lEngineTrove.LTroveRecordingRead(session, word, language);
            sources = held is null ? LEngineHarvestRead(language) : [];
            pack = LEngineLanguageLoad(language);
        }

        return held is null
            ? LEngineRecordingScan(session, word, language, variety, sources, pack, listener, cancellation)
            : LEngineRecordingPublish(LHarvest.LHarvestRecordingScan(held, variety), listener);
    }

    private string LEngineVarietyResolve(long session, long target)
    {
        if (session == 0)
        {
            return string.Empty;
        }

        IReadOnlyList<LPronunciationDraft>? rows =
            LEngineDraftRead(session)?.LDraftContent.LEntryDraftPronunciations;
        if (rows is null)
        {
            return string.Empty;
        }

        if (target == 0)
        {
            return rows.Count == 0 ? string.Empty : rows[0].LPronunciationDraftVariety.Trim();
        }

        foreach (LPronunciationDraft row in rows)
        {
            if (row.LPronunciationDraftId == target)
            {
                return row.LPronunciationDraftVariety.Trim();
            }
        }

        return string.Empty;
    }

    private async Task LEngineCandidateScan(
        long session,
        string word,
        string language,
        IReadOnlyList<LSource> sources,
        LLanguage pack,
        LReceiver receiver,
        CancellationToken cancellation)
    {
        IReadOnlyList<LCandidate> found =
            await new LLookup(sources, pack.LLanguageVarieties, pack.LLanguageCleanups)
                .LSeekerStart(word, receiver, cancellation)
                .ConfigureAwait(false);

        lock (_lEngineGate)
        {
            _lEngineTrove.LTroveCandidateSave(session, word, language, found);
        }
    }

    private async Task LEngineRecordingScan(
        long session,
        string word,
        string language,
        string variety,
        IReadOnlyList<LSource> sources,
        LLanguage pack,
        LListener listener,
        CancellationToken cancellation)
    {
        IReadOnlyList<LRecording> found =
            await new LHarvest(sources, pack.LLanguageVarieties)
                .LHarvestStart(word, variety, listener, cancellation)
                .ConfigureAwait(false);

        lock (_lEngineGate)
        {
            _lEngineTrove.LTroveRecordingSave(session, word, language, found);
        }
    }

    private static Task LEngineCandidatePublish(IReadOnlyList<LCandidate> held, LReceiver receiver)
    {
        foreach (LCandidate candidate in held)
        {
            receiver.LReceiverCandidateAdd(candidate);
        }

        receiver.LReceiverLookupFinish();
        return Task.CompletedTask;
    }

    private static Task LEngineRecordingPublish(IReadOnlyList<LRecording> held, LListener listener)
    {
        foreach (LRecording recording in held)
        {
            listener.LListenerRecordingAdd(recording);
        }

        listener.LListenerFinish();
        return Task.CompletedTask;
    }

    public Task<string> LEngineRecordingSave(LRecording recording, string word, string language, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(recording);

        string workspace;
        lock (_lEngineGate)
        {
            workspace = _lEngineWorkspace;
        }

        return LWorkspace.LWorkspaceRecordingSave(recording, word, language, workspace, _lEngineClient, cancellation);
    }

    public Task<string> LEngineRecordingPrepare(LRecording recording, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(recording);

        string workspace;
        lock (_lEngineGate)
        {
            workspace = _lEngineWorkspace;
        }

        return LWorkspace.LWorkspaceRecordingPrepare(recording, workspace, _lEngineClient, cancellation);
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

    private IReadOnlyList<LSource> LEngineLookupRead(string language)
    {
        if (!_lEngineLookupSources.TryGetValue(language, out IReadOnlyList<LSource>? sources))
        {
            LLanguage pack = LEngineLanguageLoad(language);
            sources = LSourceFactory.LSourceFactoryCreate(pack.LLanguageLookupSources, _lEngineClient);
            _lEngineLookupSources[language] = sources;
        }

        return sources;
    }

    private IReadOnlyList<LSource> LEngineHarvestRead(string language)
    {
        if (!_lEngineHarvestSources.TryGetValue(language, out IReadOnlyList<LSource>? sources))
        {
            LLanguage pack = LEngineLanguageLoad(language);
            sources = LSourceFactory.LSourceFactoryCreate(pack.LLanguageHarvestSources, _lEngineClient);
            _lEngineHarvestSources[language] = sources;
        }

        return sources;
    }

    public void Dispose()
    {
        lock (_lEngineGate)
        {
            LEngineFrequencyClear();
            _lEngineClient.Dispose();
        }
    }
}
