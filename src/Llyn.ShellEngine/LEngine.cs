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
public sealed partial class LEngine : IDisposable
{
    private readonly HttpClient _lEngineClient;
    private readonly Dictionary<string, IReadOnlyList<LSource>> _lEngineSources = new(StringComparer.Ordinal);
    private string _lEngineWorkspace;
    private LSettings _lEngineSettings;
    private LDatabase _lEngineDatabase;

    /// <summary>Binds the engine to the workspace the pointer file records, creating it on first run.</summary>
    public LEngine()
        : this(LWorkspaceRoot.LWorkspaceRootRead())
    {
    }

    /// <summary>
    /// Binds the engine to <paramref name="workspace"/> directly, without reading or rewriting the
    /// recorded workspace pointer — the folder is given, so there is nothing to resolve. Changing the
    /// user's workspace is <see cref="LEngineWorkspaceChange"/>; this only says which folder to open.
    /// </summary>
    public LEngine(string workspace)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspace);

        _lEngineWorkspace = workspace;
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
    /// The workspace folder the pointer file records, creating it on first run — the folder the
    /// parameterless constructor opens. The shell asks for it before building an engine, so a
    /// workspace that fails to open can still be named in the message the user sees.
    /// </summary>
    public static string LEngineWorkspaceResolve()
    {
        return LWorkspaceRoot.LWorkspaceRootRead();
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
    /// Returns the entries whose headword contains <paramref name="query"/>, ordered by headword, or
    /// every entry when <paramref name="query"/> is empty — the list a browsing or searching pane shows.
    /// Matching is a case-insensitive contains.
    /// </summary>
    public IReadOnlyList<LEntry> LEngineEntryFind(string query)
    {
        return new LEntryArchive(_lEngineDatabase).LEntryFind(query);
    }

    /// <summary>
    /// Reads the entry identified by <paramref name="id"/> back into the draft the input form saved, or
    /// <c>null</c> when no entry has that id. This is the inverse of <see cref="LEngineEntrySave"/>: it
    /// composes the entry row, its meanings and collocations, its note and pronunciation, and the
    /// Examples, Situations and Tags each card references — all of them, in stored order — into one value
    /// the shell can put back on screen, read as a single consistent snapshot. Synonyms come back empty, because the save writes none.
    /// </summary>
    public LEntryDraft? LEngineEntryLoad(string id)
    {
        LEntryDraft? draft = new LEntryLoader(_lEngineDatabase).LEntryLoad(id);
        if (draft is null || draft.LEntryDraftAudio.Length == 0)
        {
            return draft;
        }

        // Stored relative, handed out full: the shell plays a file, so it never has to know the
        // workspace folder, and the same entry opened from a moved workspace still resolves.
        return draft with { LEntryDraftAudio = LEngineRecordingResolve(draft.LEntryDraftAudio) };
    }

    // The workspace-relative form of a downloaded recording's path, which is how it is stored. A path
    // outside the workspace has no relative form and is stored as it stands.
    private string LEngineRecordingFormat(string path)
    {
        string relative = Path.GetRelativePath(_lEngineWorkspace, path);
        return Path.IsPathRooted(relative) || relative.StartsWith("..", StringComparison.Ordinal)
            ? path
            : relative;
    }

    // The full path of a stored recording within the workspace in use now. A path that was stored
    // absolute — one saved outside the workspace — is returned unchanged.
    private string LEngineRecordingResolve(string file)
    {
        return Path.IsPathRooted(file) ? file : Path.Combine(_lEngineWorkspace, file);
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
    /// <para>
    /// All four writes share one session, so they are one transaction: the deleted entry, the revision
    /// recording it, the tombstone filed under that revision, and the workspace row moved onto it either
    /// all land or none of them do. Without it a failure part-way would leave an entry deleted with no
    /// tombstone naming it — history that no longer describes the file.
    /// </para>
    /// </summary>
    public LRevision LEngineEntryDelete(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();

        LEntryArchive entries = new(_lEngineDatabase);
        LEntry? deleted = entries.LEntryRead(id);
        entries.LEntryDelete(id);

        LRevisionChange change = new(0, id, "entry", "delete", deleted?.LEntryHeadword);
        LRevision revision = new LRevisionArchive(_lEngineDatabase).LRevisionRecord([change]);
        new LTombstoneArchive(_lEngineDatabase).LTombstoneRecord(id, revision.LRevisionId);

        LWorkspaceArchive workspace = new(_lEngineDatabase);
        LWorkspaceState state = workspace.LWorkspaceStateRead();
        workspace.LWorkspaceStateSave(state with { LWorkspaceStateRevision = revision.LRevisionId });

        session.LDatabaseSessionCommit();
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
