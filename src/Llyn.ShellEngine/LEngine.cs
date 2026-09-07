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
    private string _lEngineWorkspace;
    private LSettings _lEngineSettings;
    private LDatabase _lEngineDatabase;
    private LDoctorRescue _lEngineRescue;

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

        LEngineLanguageImport();

        _lEngineClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(10)
        };
        _lEngineClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) Llyn/0.0 (pronunciation lookup)");
    }

    public static string LEngineIdentityCreate()
    {
        return LIdentity.LIdentityCreate();
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
        ArgumentException.ThrowIfNullOrWhiteSpace(language);

        return LLanguageLoader.LLanguageLoaderLoad(language).LLanguageFont;
    }

    public async Task<string?> LEngineFlagRead(string language, CancellationToken cancellation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);

        string? code = LLanguageLoader.LLanguageLoaderLoad(language).LLanguageFlag;
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

            foreach (string held in _lEngineDraftHeld)
            {
                _lEngineDraftStale.Add(held);
            }

            _lEngineDraftHeld.Clear();
            _lEngineLookupSources.Clear();
            _lEngineHarvestSources.Clear();
            LSettingsLoader.LSettingsLoaderSave(_lEngineWorkspace, _lEngineSettings);

            _lEngineDatabase = new LDatabase(_lEngineWorkspace);
            _lEngineRescue = LDoctor.LDoctorDatabaseCreate(_lEngineDatabase);
            LEngineLanguageImport();
        }

        LEngineBulletinRaise(LSubject.LSubjectWorkspace, string.Empty);
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

    public Task LEnginePronunciationFind(string word, string language, LReceiver receiver, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(receiver);

        IReadOnlyList<LSource> sources;
        lock (_lEngineGate)
        {
            sources = LEngineLookupRead(language);
        }

        return new LLookup(sources).LSeekerStart(word, receiver, cancellation);
    }

    public Task LEngineRecordingFind(string word, string language, LListener listener, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(listener);

        IReadOnlyList<LSource> sources;
        lock (_lEngineGate)
        {
            sources = LEngineHarvestRead(language);
        }

        return new LHarvest(sources).LHarvestStart(word, listener, cancellation);
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
