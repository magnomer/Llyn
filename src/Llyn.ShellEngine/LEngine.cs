using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

/// <summary>
/// The shell engine: the single boundary the UI shell talks to. The UI sends a request here and
/// subscribes through an <see cref="LReceiver"/> (pronunciation) or <see cref="LListener"/> (audio);
/// all logic — loading language packs, source fan-out, fetching, parsing, saving — lives behind this
/// engine, so none of it sits in the UI shell.
///
/// This class is also the composition root. It owns the shared <see cref="HttpClient"/>, loads each
/// language pack on first use through <see cref="LLanguageLoader"/>, and builds that language's
/// sources through <see cref="LSourceFactory"/>. It holds no source- or language-specific facts of
/// its own: everything language-specific comes from <c>languages/&lt;Lang&gt;/source.json</c>.
/// </summary>
public sealed class LEngine : IDisposable
{
    private readonly HttpClient _lEngineClient;
    private readonly Dictionary<string, IReadOnlyList<LSource>> _lEngineSources = new(StringComparer.Ordinal);
    private string _lEngineWorkspace;
    private LSettings _lEngineSettings;
    private LDatabase _lEngineDatabase;

    public LEngine()
    {
        _lEngineWorkspace = LWorkspaceRoot.LWorkspaceRootRead();
        _lEngineSettings = LSettingsLoader.LSettingsLoaderLoad(_lEngineWorkspace);

        // Initialize the database once the workspace is known, so the store is ready before any UI
        // request and the UI never opens the database itself.
        _lEngineDatabase = new LDatabase(_lEngineWorkspace);
        _lEngineDatabase.LDatabaseCreate();

        _lEngineClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(10)
        };
        // Cambridge (and some Wiktionary edge caches) reject requests without a browser-like agent.
        _lEngineClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) Llyn/0.0 (pronunciation lookup)");
    }

    /// <summary>
    /// Returns the names of the languages that have a pack on disk, for the UI to offer as choices.
    /// </summary>
    public IReadOnlyList<string> LEngineLanguageRead()
    {
        return LLanguageLoader.LLanguageLoaderScan();
    }

    /// <summary>
    /// Returns the local path to the given language's flag image, for the UI to display beside the
    /// language, or <c>null</c> when the pack declares no flag or the download fails. The pack declares
    /// only an ISO country code; the engine downloads the matching flag from the flag-icons set and
    /// caches it in the workspace, so the UI never reaches into the <c>languages/</c> folder itself.
    /// </summary>
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

    /// <summary>
    /// Returns the current workspace folder — where the user's settings and database are stored.
    /// </summary>
    public string LEngineWorkspaceRead()
    {
        return _lEngineWorkspace;
    }

    /// <summary>
    /// Moves the workspace to <paramref name="path"/>: records the new folder and writes the current
    /// settings into it, so settings and database follow the workspace to its new location.
    /// </summary>
    public void LEngineWorkspaceChange(string path)
    {
        LWorkspaceRoot.LWorkspaceRootChange(path);
        _lEngineWorkspace = path;
        LSettingsLoader.LSettingsLoaderSave(_lEngineWorkspace, _lEngineSettings);

        // The database follows the workspace: initialize one in the new folder.
        _lEngineDatabase = new LDatabase(_lEngineWorkspace);
        _lEngineDatabase.LDatabaseCreate();
    }

    /// <summary>Returns the user's persisted settings.</summary>
    public LSettings LEngineSettingsRead()
    {
        return _lEngineSettings;
    }

    /// <summary>Persists <paramref name="settings"/> into the current workspace and keeps them current.</summary>
    public void LEngineSettingsSave(LSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        _lEngineSettings = settings;
        LSettingsLoader.LSettingsLoaderSave(_lEngineWorkspace, settings);
    }

    /// <summary>
    /// Opens the workspace row — which Entry each pane shows, the display mode, the split, and the
    /// current revision — creating it on first use so the UI always receives a state.
    /// </summary>
    public LWorkspaceState LEngineStateRead()
    {
        return new LWorkspaceArchive(_lEngineDatabase).LWorkspaceStateRead();
    }

    /// <summary>Writes <paramref name="state"/> back into the workspace row.</summary>
    public void LEngineStateSave(LWorkspaceState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        new LWorkspaceArchive(_lEngineDatabase).LWorkspaceStateSave(state);
    }

    /// <summary>
    /// Creates <paramref name="entry"/> with <paramref name="forms"/> and <paramref name="speeches"/>
    /// as its ordered child rows, and returns the stored entry with its assigned id and timestamps.
    /// </summary>
    public LEntry LEngineEntryCreate(LEntry entry, IReadOnlyList<LForm> forms, IReadOnlyList<LSpeech> speeches)
    {
        ArgumentNullException.ThrowIfNull(entry);
        return new LEntryArchive(_lEngineDatabase).LEntryCreate(entry, forms, speeches);
    }

    /// <summary>Reads the entry for <paramref name="id"/>, or <c>null</c> when no entry has that id.</summary>
    public LEntry? LEngineEntryRead(string id)
    {
        return new LEntryArchive(_lEngineDatabase).LEntryRead(id);
    }

    /// <summary>
    /// Deletes the entry identified by <paramref name="id"/> with everything it owns, then writes the
    /// history the deletion leaves behind: a revision carrying one delete change for the entry, a
    /// tombstone filed under that revision, and the workspace row moved onto it. Returns the recorded
    /// revision.
    /// <para>
    /// The delete runs first, so a refused delete — an entry another entry still links to — records no
    /// history at all and throws its own message through.
    /// </para>
    /// </summary>
    public LRevision LEngineEntryDelete(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        LEntry? deleted = new LEntryArchive(_lEngineDatabase).LEntryRead(id);
        new LEntryArchive(_lEngineDatabase).LEntryDelete(id);

        LRevisionChange change = new(0, id, "entry", "delete", deleted?.LEntryHeadword);
        LRevision revision = new LRevisionArchive(_lEngineDatabase).LRevisionRecord([change]);
        new LTombstoneArchive(_lEngineDatabase).LTombstoneRecord(id, revision.LRevisionId);

        LWorkspaceArchive workspace = new(_lEngineDatabase);
        LWorkspaceState state = workspace.LWorkspaceStateRead();
        workspace.LWorkspaceStateSave(state with { LWorkspaceStateRevision = revision.LRevisionId });

        return revision;
    }

    /// <summary>
    /// Reads the tombstone left by deleting the Entry identified by <paramref name="entryId"/>, or
    /// <c>null</c> when that Entry has never been deleted.
    /// </summary>
    public LTombstone? LEngineTombstoneRead(string entryId)
    {
        return new LTombstoneArchive(_lEngineDatabase).LTombstoneRead(entryId);
    }

    /// <summary>Reads the most recently opened revision, or <c>null</c> when the workspace has none yet.</summary>
    public LRevision? LEngineRevisionRead()
    {
        return new LRevisionArchive(_lEngineDatabase).LRevisionLatestRead();
    }

    /// <summary>Reads the changes recorded under <paramref name="revisionId"/>, in the order recorded.</summary>
    public IReadOnlyList<LRevisionChange> LEngineChangeRead(string revisionId)
    {
        return new LRevisionArchive(_lEngineDatabase).LRevisionChangeRead(revisionId);
    }

    /// <summary>
    /// Starts a pronunciation lookup for <paramref name="word"/> in <paramref name="language"/> and
    /// streams results to <paramref name="receiver"/>. The task completes when every source finishes.
    /// </summary>
    public Task LEnginePronunciationFind(string word, string language, LReceiver receiver, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(receiver);
        return new LLookup(LEngineSourcesRead(language)).LSeekerStart(word, receiver, cancellation);
    }

    /// <summary>
    /// Starts an audio-recording discovery for <paramref name="word"/> in <paramref name="language"/>
    /// and streams results to <paramref name="listener"/>. The task completes when every source finishes.
    /// </summary>
    public Task LEngineRecordingFind(string word, string language, LListener listener, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(listener);
        return new LHarvest(LEngineSourcesRead(language)).LHarvestStart(word, listener, cancellation);
    }

    /// <summary>
    /// Downloads the chosen <paramref name="recording"/> into the workspace and returns the saved path.
    /// </summary>
    public Task<string> LEngineRecordingSave(LRecording recording, string word, string language, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(recording);
        return LWorkspace.LWorkspaceRecordingSave(recording, word, language, _lEngineWorkspace, _lEngineClient, cancellation);
    }

    /// <summary>
    /// Downloads the <paramref name="recording"/> to a temporary file for playback and returns its path.
    /// </summary>
    public Task<string> LEngineRecordingPrepare(LRecording recording, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(recording);
        return LWorkspace.LWorkspaceRecordingPrepare(recording, _lEngineWorkspace, _lEngineClient, cancellation);
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
