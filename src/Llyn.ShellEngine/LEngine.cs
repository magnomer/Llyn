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
    private readonly HttpClient _lEngineClient;
    private readonly Dictionary<string, IReadOnlyList<LSource>> _lEngineSources = new(StringComparer.Ordinal);
    private string _lEngineWorkspace;
    private LSettings _lEngineSettings;
    private LDatabase _lEngineDatabase;

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
        _lEngineDatabase.LDatabaseCreate();

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
        return LLanguageLoader.LLanguageLoaderScan();
    }

    public async Task<string?> LEngineFlagRead(string language, CancellationToken cancellation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);

        string? code = LLanguageLoader.LLanguageLoaderLoad(language).LLanguageFlag;
        if (string.IsNullOrWhiteSpace(code))
        {
            return null;
        }

        return await LWorkspace
            .LWorkspaceFlagRead(code, _lEngineWorkspace, _lEngineClient, cancellation)
            .ConfigureAwait(false);
    }

    public string LEngineWorkspaceRead()
    {
        return _lEngineWorkspace;
    }

    public static string LEngineWorkspaceResolve()
    {
        return LWorkspaceRoot.LWorkspaceRootRead();
    }

    public void LEngineWorkspaceChange(string path)
    {
        LWorkspaceRoot.LWorkspaceRootChange(path);
        _lEngineWorkspace = path;
        LSettingsLoader.LSettingsLoaderSave(_lEngineWorkspace, _lEngineSettings);

        _lEngineDatabase = new LDatabase(_lEngineWorkspace);
        _lEngineDatabase.LDatabaseCreate();
        LEngineLanguageImport();
    }

    public LSettings LEngineSettingsRead()
    {
        return _lEngineSettings;
    }

    public void LEngineSettingsSave(LSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        _lEngineSettings = settings;
        LSettingsLoader.LSettingsLoaderSave(_lEngineWorkspace, settings);
    }

    public LWorkspaceState LEngineStateRead()
    {
        return new LWorkspaceArchive(_lEngineDatabase).LWorkspaceStateRead();
    }

    public void LEngineStateSave(LWorkspaceState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        new LWorkspaceArchive(_lEngineDatabase).LWorkspaceStateSave(state);
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
        return new LLookup(LEngineSourcesRead(language)).LSeekerStart(word, receiver, cancellation);
    }

    public Task LEngineRecordingFind(string word, string language, LListener listener, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(listener);
        return new LHarvest(LEngineSourcesRead(language)).LHarvestStart(word, listener, cancellation);
    }

    public Task<string> LEngineRecordingSave(LRecording recording, string word, string language, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(recording);
        return LWorkspace.LWorkspaceRecordingSave(recording, word, language, _lEngineWorkspace, _lEngineClient, cancellation);
    }

    public Task<string> LEngineRecordingPrepare(LRecording recording, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(recording);
        return LWorkspace.LWorkspaceRecordingPrepare(recording, _lEngineWorkspace, _lEngineClient, cancellation);
    }

    private static bool LEngineOwnerCheck(LOwner owner)
    {
        return owner switch
        {
            LOwner.LOwnerSense => false,
            LOwner.LOwnerCollocation => true,
            _ => throw LEngineOwnerRaise(owner),
        };
    }

    private static ArgumentOutOfRangeException LEngineOwnerRaise(LOwner owner)
    {
        return new ArgumentOutOfRangeException(
            nameof(owner), owner, "This entity has no reference from that kind of row.");
    }

    private IReadOnlyList<LSource> LEngineSourcesRead(string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);

        if (!_lEngineSources.TryGetValue(language, out IReadOnlyList<LSource>? sources))
        {
            LLanguage pack = LLanguageLoader.LLanguageLoaderLoad(language);
            sources = LSourceFactory.LSourceFactoryCreate(pack, _lEngineClient);
            _lEngineSources[language] = sources;
        }

        return sources;
    }

    public void Dispose()
    {
        _lEngineClient.Dispose();
    }
}
