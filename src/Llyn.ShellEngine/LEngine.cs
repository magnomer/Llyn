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
    /// A recording the downloader saved is written as the pronunciation's audio row, in the same
    /// transaction, with its path made relative to the workspace so a moved workspace keeps its audio.
    /// Because the row hangs off the pronunciation, a recording with no typed IPA still creates the
    /// pronunciation to hang from.
    /// </para>
    /// <para>
    /// The Example, Situation and Tag text a card carries is written as independent data: each non-empty
    /// field becomes a new row of its own entity, which the card's sense or collocation then references.
    /// The entry owns none of them, so clearing a card would only detach what it points at.
    /// </para>
    /// <para>
    /// Text is never matched against an existing Example, Situation or Tag: every non-empty field
    /// creates a new row, even when the same words were saved before. Matching needs a picker that
    /// resolves typed text to a chosen row, and none exists yet.
    /// </para>
    /// <para>
    /// Parts of speech are not written: <c>part_of_speech_value</c> is unseeded and the input panel has
    /// no control for them. <b>Synonyms are not written either.</b> An <c>LSynonym</c> targets an Entry
    /// or a Meaning by id, and a sense-card synonym is an <c>LRelation</c> with the same requirement;
    /// the card's field holds free text, which is neither, and no picker exists to resolve it. Writing
    /// one would mean fabricating a target and corrupting the relation model. The collocation card has
    /// no Synonym control at all, so nothing feeds <c>LCollocationDraft.LCollocationDraftSynonym</c>.
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
            LSense sense = senses.LSenseCreate(new LSense(
                string.Empty,
                entry.LEntryId,
                null,
                0,
                null,
                null,
                card.LSenseDraftDefinition,
                string.Empty));

            LEngineSenseAttach(sense.LSenseId, card, draft.LEntryDraftLanguage);
        }

        LCollocationArchive collocations = new(_lEngineDatabase);
        foreach (LCollocationDraft card in draft.LEntryDraftCollocations)
        {
            LCollocation collocation = collocations.LCollocationCreate(new LCollocation(
                string.Empty,
                entry.LEntryId,
                0,
                card.LCollocationDraftExpression,
                card.LCollocationDraftMeaning));

            LEngineCollocationAttach(collocation.LCollocationId, card, draft.LEntryDraftLanguage);
        }

        if (!string.IsNullOrWhiteSpace(draft.LEntryDraftNote))
        {
            new LNoteArchive(_lEngineDatabase).LNoteSave(new LNote(entry.LEntryId, draft.LEntryDraftNote));
        }

        // The pronunciation row is what a recording hangs from, so a downloaded recording creates one
        // even when no IPA was typed; without it the audio would have nothing to reference.
        if (!string.IsNullOrWhiteSpace(draft.LEntryDraftPronunciation) ||
            !string.IsNullOrWhiteSpace(draft.LEntryDraftAudio))
        {
            LPronunciationArchive pronunciations = new(_lEngineDatabase);
            LPronunciation pronunciation = pronunciations.LPronunciationCreate(new LPronunciation(
                string.Empty,
                entry.LEntryId,
                null,
                draft.LEntryDraftPronunciation,
                [],
                []));

            if (!string.IsNullOrWhiteSpace(draft.LEntryDraftAudio))
            {
                pronunciations.LPronunciationAudioSave(
                    pronunciation.LPronunciationId,
                    LEngineRecordingFormat(draft.LEntryDraftAudio),
                    draft.LEntryDraftSource);
            }
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
    /// Example, Situation and Tag each card references into one value the shell can put back on screen,
    /// read as a single consistent snapshot. Synonyms come back empty, because the save writes none.
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

    /// <summary>
    /// Writes the Example, Situation and Tag a sense card typed and points the stored sense at them.
    /// Each non-empty field creates a row of its own — independent data the sense references rather
    /// than owns — and an empty field writes nothing at all.
    /// <para>
    /// A card holds one of each field, so each reference set the card fills holds a single row and its
    /// position is 0. The card's own order is already carried by the sense row it produced.
    /// </para>
    /// </summary>
    private void LEngineSenseAttach(string senseId, LSenseDraft card, string language)
    {
        if (!string.IsNullOrWhiteSpace(card.LSenseDraftExample))
        {
            LExample example = new LExampleArchive(_lEngineDatabase).LExampleCreate(
                new LExample(string.Empty, language, card.LSenseDraftExample, null, null, []));
            new LExampleLink(_lEngineDatabase).LExampleSenseAttach(senseId, example.LExampleId, 0);
        }

        if (!string.IsNullOrWhiteSpace(card.LSenseDraftSituation))
        {
            LSituationArchive situations = new(_lEngineDatabase);
            LSituation situation = situations.LSituationCreate(
                new LSituation(string.Empty, card.LSenseDraftSituation, null, null));
            situations.LSituationSenseAttach(senseId, situation.LSituationId, 0);
        }

        if (!string.IsNullOrWhiteSpace(card.LSenseDraftTag))
        {
            LTagArchive tags = new(_lEngineDatabase);
            LTag tag = tags.LTagCreate(new LTag(string.Empty, card.LSenseDraftTag));
            tags.LTagSenseAttach(senseId, tag.LTagId, 0);
        }
    }

    /// <summary>
    /// Writes the Example, Situation and Tag a collocation card typed and points the stored collocation
    /// at them, on the same terms as a sense card: independent rows, referenced and never owned, and
    /// nothing written for an empty field.
    /// </summary>
    private void LEngineCollocationAttach(string collocationId, LCollocationDraft card, string language)
    {
        if (!string.IsNullOrWhiteSpace(card.LCollocationDraftExample))
        {
            LExample example = new LExampleArchive(_lEngineDatabase).LExampleCreate(
                new LExample(string.Empty, language, card.LCollocationDraftExample, null, null, []));
            new LExampleLink(_lEngineDatabase).LExampleCollocationAttach(
                collocationId, example.LExampleId, 0);
        }

        if (!string.IsNullOrWhiteSpace(card.LCollocationDraftSituation))
        {
            LSituationArchive situations = new(_lEngineDatabase);
            LSituation situation = situations.LSituationCreate(
                new LSituation(string.Empty, card.LCollocationDraftSituation, null, null));
            situations.LSituationCollocationAttach(collocationId, situation.LSituationId, 0);
        }

        if (!string.IsNullOrWhiteSpace(card.LCollocationDraftTag))
        {
            LTagArchive tags = new(_lEngineDatabase);
            LTag tag = tags.LTagCreate(new LTag(string.Empty, card.LCollocationDraftTag));
            tags.LTagCollocationAttach(collocationId, tag.LTagId, 0);
        }
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
