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

    /// <summary>
    /// Saves the whole input form as one new entry: the headword row, its senses and collocations in
    /// card order, its note and pronunciation when they carry text, the revision recording the create,
    /// and the workspace row moved onto that revision and that entry. Returns the stored entry with its
    /// assigned id and timestamps.
    /// <para>
    /// A blank headword is refused before any connection opens, so a save that cannot be made costs
    /// nothing. Everything after that runs inside one session, which is what makes a half-written entry
    /// impossible: a failure at any write rolls back every write before it, leaving no entry row behind.
    /// That is also why this is a single engine call — the shell has no way to compose a partial write
    /// out of several of them.
    /// </para>
    /// <para>
    /// Parts of speech are not written: <c>part_of_speech_value</c> is unseeded and the input panel has
    /// no control for them. The example, situation, synonym, and tag text a card carries is not written
    /// either — each is an independent entity reached through its own association, which is a later job.
    /// </para>
    /// </summary>
    public LEntry LEngineEntrySave(LEntryDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        if (string.IsNullOrWhiteSpace(draft.LEntryDraftHeadword))
        {
            throw new InvalidOperationException("An entry cannot be saved without a headword.");
        }

        using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();

        // The entry's language is the language-pack name, which is the key part_of_speech_value and
        // morphology_value are already written against.
        LEntry entry = new LEntryArchive(_lEngineDatabase).LEntryCreate(
            new LEntry(
                string.Empty,
                draft.LEntryDraftHeadword,
                draft.LEntryDraftLanguage,
                null,
                null,
                null,
                null),
            forms: [],
            speeches: []);

        LSenseArchive senses = new(_lEngineDatabase);
        foreach (LSenseDraft card in draft.LEntryDraftSenses)
        {
            // Each sense is appended, so card order becomes stored position.
            senses.LSenseCreate(new LSense(
                string.Empty,
                entry.LEntryId,
                null,
                0,
                null,
                null,
                card.LSenseDraftDefinition,
                string.Empty));
        }

        LCollocationArchive collocations = new(_lEngineDatabase);
        foreach (LCollocationDraft card in draft.LEntryDraftCollocations)
        {
            collocations.LCollocationCreate(new LCollocation(
                string.Empty,
                entry.LEntryId,
                0,
                card.LCollocationDraftExpression,
                card.LCollocationDraftMeaning));
        }

        if (!string.IsNullOrWhiteSpace(draft.LEntryDraftNote))
        {
            new LNoteArchive(_lEngineDatabase).LNoteSave(new LNote(entry.LEntryId, draft.LEntryDraftNote));
        }

        if (!string.IsNullOrWhiteSpace(draft.LEntryDraftPronunciation))
        {
            new LPronunciationArchive(_lEngineDatabase).LPronunciationCreate(new LPronunciation(
                string.Empty,
                entry.LEntryId,
                null,
                draft.LEntryDraftPronunciation,
                [],
                []));
        }

        LRevisionChange change = new(0, entry.LEntryId, "entry", "create", entry.LEntryHeadword);
        LRevision revision = new LRevisionArchive(_lEngineDatabase).LRevisionRecord([change]);

        LWorkspaceArchive workspace = new(_lEngineDatabase);
        LWorkspaceState state = workspace.LWorkspaceStateRead();
        workspace.LWorkspaceStateSave(state with
        {
            LWorkspaceStateLeft = entry.LEntryId,
            LWorkspaceStateRevision = revision.LRevisionId,
        });

        session.LDatabaseSessionCommit();
        return entry;
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
