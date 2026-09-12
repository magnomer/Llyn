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
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspace);

        _lEngineWorkspace = workspace;
        _lEngineSettings = LSettingsLoader.LSettingsLoaderLoad(_lEngineWorkspace);

        _lEngineDatabase = new LDatabase(_lEngineWorkspace);
        _lEngineRescue = LDoctor.LDoctorDatabaseCreate(_lEngineDatabase);
        _lEngineRealm = new LRealmArchive(_lEngineDatabase).LRealmRead();
        _lEngineIdentity = new LIdentity(_lEngineDatabase);

        LEngineLanguageImport();

        _lEngineClient = new HttpClient
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

    public IReadOnlyList<string> LEngineLanguageRead()
    {
        lock (_lEngineGate)
        {
            return LLanguageLoader.LLanguageLoaderScan();
        }
    }

    public LFont LEngineFontRead(string language)
    {
        return LEngineFontRead(language, LFontRole.LFontRoleHeadword);
    }

    public LFont LEngineFontRead(string language, LFontRole role)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);

        LLanguage pack = LLanguageLoader.LLanguageLoaderLoad(language);

        return role == LFontRole.LFontRoleExample ? pack.LLanguageExample : pack.LLanguageFont;
    }

    public async Task<string?> LEngineFlagRead(string language, CancellationToken cancellation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);

        string? code = LLanguageLoader.LLanguageLoaderLoad(language).LLanguageFlag;
        return await LEngineFlagResolve(code, cancellation).ConfigureAwait(false);
    }

    public IReadOnlyList<LVariety> LEngineVarietyRead(string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);

        return LLanguageLoader.LLanguageLoaderLoad(language).LLanguageVarieties;
    }

    public bool LEngineFlaggedCheck(string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);

        return LLanguageLoader.LLanguageLoaderLoad(language).LLanguageVarietyFlagged;
    }

    public async Task<string?> LEngineVarietyResolve(string language, string variety, CancellationToken cancellation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);

        string? code = null;
        foreach (LVariety declared in LLanguageLoader.LLanguageLoaderLoad(language).LLanguageVarieties)
        {
            if (string.Equals(declared.LVarietyName, variety, StringComparison.Ordinal))
            {
                code = declared.LVarietyFlag;
                break;
            }
        }

        return await LEngineFlagResolve(code, cancellation).ConfigureAwait(false);
    }

    private async Task<string?> LEngineFlagResolve(string? code, CancellationToken cancellation)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return null;
        }

        string workspace;
        lock (_lEngineGate)
        {
            workspace = _lEngineWorkspace;
        }

        return await LWorkspace
            .LWorkspaceFlagRead(code, workspace, _lEngineClient, cancellation)
            .ConfigureAwait(false);
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

    public LSettings LEngineSettingsRead()
    {
        lock (_lEngineGate)
        {
            return _lEngineSettings;
        }
    }

    public void LEngineLocalizationSave(string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        LEngineSettingsChange(settings => settings with { LSettingsLocalization = language });
    }

    public void LEngineWindowSave(LWindowState window)
    {
        ArgumentNullException.ThrowIfNull(window);
        LEngineSettingsChange(settings => settings with { LSettingsWindow = window });
    }

    public void LEngineVolumeSave(double volume)
    {
        double level = Math.Clamp(volume, 0, 1);
        LEngineSettingsChange(settings => settings with { LSettingsVolume = level });
    }

    private void LEngineSettingsChange(Func<LSettings, LSettings> change)
    {
        lock (_lEngineGate)
        {
            _lEngineSettings = change(_lEngineSettings);
            LSettingsLoader.LSettingsLoaderSave(_lEngineWorkspace, _lEngineSettings);
        }
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
        lock (_lEngineGate)
        {
            held = _lEngineTrove.LTroveCandidateRead(session, word, language);
            sources = held is null ? LEngineLookupRead(language) : [];
        }

        return held is null
            ? LEngineCandidateScan(session, word, language, sources, receiver, cancellation)
            : LEngineCandidatePublish(held, receiver);
    }

    public Task LEngineRecordingFind(
        long session,
        string word,
        string language,
        LListener listener,
        CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(listener);

        IReadOnlyList<LRecording>? held;
        IReadOnlyList<LSource> sources;
        lock (_lEngineGate)
        {
            held = _lEngineTrove.LTroveRecordingRead(session, word, language);
            sources = held is null ? LEngineHarvestRead(language) : [];
        }

        return held is null
            ? LEngineRecordingScan(session, word, language, sources, listener, cancellation)
            : LEngineRecordingPublish(held, listener);
    }

    private async Task LEngineCandidateScan(
        long session,
        string word,
        string language,
        IReadOnlyList<LSource> sources,
        LReceiver receiver,
        CancellationToken cancellation)
    {
        IReadOnlyList<LCandidate> found =
            await new LLookup(sources).LSeekerStart(word, receiver, cancellation).ConfigureAwait(false);

        lock (_lEngineGate)
        {
            _lEngineTrove.LTroveCandidateSave(session, word, language, found);
        }
    }

    private async Task LEngineRecordingScan(
        long session,
        string word,
        string language,
        IReadOnlyList<LSource> sources,
        LListener listener,
        CancellationToken cancellation)
    {
        IReadOnlyList<LRecording> found =
            await new LHarvest(sources).LHarvestStart(word, listener, cancellation).ConfigureAwait(false);

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
        ArgumentException.ThrowIfNullOrWhiteSpace(language);

        if (!_lEngineLookupSources.TryGetValue(language, out IReadOnlyList<LSource>? sources))
        {
            LLanguage pack = LLanguageLoader.LLanguageLoaderLoad(language);
            sources = LSourceFactory.LSourceFactoryCreate(pack.LLanguageLookupSources, _lEngineClient);
            _lEngineLookupSources[language] = sources;
        }

        return sources;
    }

    private IReadOnlyList<LSource> LEngineHarvestRead(string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);

        if (!_lEngineHarvestSources.TryGetValue(language, out IReadOnlyList<LSource>? sources))
        {
            LLanguage pack = LLanguageLoader.LLanguageLoaderLoad(language);
            sources = LSourceFactory.LSourceFactoryCreate(pack.LLanguageHarvestSources, _lEngineClient);
            _lEngineHarvestSources[language] = sources;
        }

        return sources;
    }

    public void Dispose()
    {
        lock (_lEngineGate)
        {
            _lEngineClient.Dispose();
        }
    }
}
