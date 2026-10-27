using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal sealed class LOutpostBatch
{
    internal const int LOutpostBatchStall = 3;

    private const string LOutpostBatchSystem = "System";

    private const string LOutpostBatchStyle = "Llyn style";

    private const string LOutpostBatchTag = "llyn/";

    private readonly LOutpostHttp _lOutpostBatchHttp;
    private readonly LEngraverEntrance _lOutpostBatchEngraver;
    private readonly LManifestFile _lOutpostBatchManifest;
    private readonly object _lOutpostBatchGate = new();

    public LOutpostBatch(LOutpostHttp http, LEngraverEntrance engraver, LManifestFile manifest)
    {
        ArgumentNullException.ThrowIfNull(http);
        ArgumentNullException.ThrowIfNull(engraver);
        ArgumentNullException.ThrowIfNull(manifest);
        _lOutpostBatchHttp = http;
        _lOutpostBatchEngraver = engraver;
        _lOutpostBatchManifest = manifest;
    }

    public async Task<LReceipt> LOutpostBatchSend(LOutpostCargo cargo, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(cargo);
        ArgumentException.ThrowIfNullOrWhiteSpace(cargo.LOutpostCargoToken);
        ArgumentNullException.ThrowIfNull(cargo.LOutpostCargoNotebook);
        ArgumentNullException.ThrowIfNull(cargo.LOutpostCargoRealm);
        ArgumentNullException.ThrowIfNull(cargo.LOutpostCargoEntries);
        ArgumentNullException.ThrowIfNull(cargo.LOutpostCargoLanguages);
        ArgumentNullException.ThrowIfNull(cargo.LOutpostCargoPage);
        ArgumentNullException.ThrowIfNull(cargo.LOutpostCargoSound);
        ArgumentNullException.ThrowIfNull(cargo.LOutpostCargoLookup);
        ArgumentNullException.ThrowIfNull(cargo.LOutpostCargoFault);

        string token = cargo.LOutpostCargoToken;
        string stamp = cargo.LOutpostCargoRealm;
        IReadOnlyList<LEntry> entries = cargo.LOutpostCargoEntries;
        Func<string, string> lookup = cargo.LOutpostCargoLookup;
        Action<Exception> fault = cargo.LOutpostCargoFault;
        int port = await _lOutpostBatchHttp.LOutpostFind(cargo.LOutpostCargoPort, cancellation)
            .ConfigureAwait(false) ?? throw new LRefusal(LRefusal.LRefusalOutpost);

        string notebook = LOutpostSeal.LOutpostSealFormat("llyn:notebook");
        string system = LOutpostSeal.LOutpostSealFormat("llyn:system");
        string style = LOutpostSeal.LOutpostSealFormat("llyn:style");
        string mark = _lOutpostBatchEngraver.LEngraverEntranceRead(style);

        LManifest manifest;
        lock (_lOutpostBatchGate)
        {
            manifest = _lOutpostBatchManifest.LManifestRead();
        }

        Dictionary<string, string> shelves = new(StringComparer.Ordinal);
        foreach (string name in entries.Select(static entry => entry.LEntryLanguage.Trim()))
        {
            if (name.Length > 0)
            {
                shelves.TryAdd(name, LOutpostSeal.LOutpostSealFormat("llyn:language:" + name));
            }
        }

        HashSet<string> folders = new(StringComparer.Ordinal) { notebook, system };
        foreach (string name in shelves.Keys
            .Concat(cargo.LOutpostCargoLanguages.Select(static row => row.Trim()))
            .Where(static row => row.Length > 0))
        {
            foreach (string seed in LOutpostShelf.LOutpostShelfFolder.Prepend("language"))
            {
                folders.Add(LOutpostSeal.LOutpostSealFormat("llyn:" + seed + ":" + name));
            }
        }

        List<LLiveryLanguage> languages = [];
        bool unread = false;
        foreach (string name in shelves.Keys)
        {
            try
            {
                languages.Add(cargo.LOutpostCargoSound(name));
            }
            catch (Exception exception) when (exception is not LRefusal and not OperationCanceledException)
            {
                fault(exception);
                unread = true;
            }
        }

        HashSet<long> sending = [.. entries.Select(static entry => entry.LEntryId)];
        Func<long, string> note = entryId => sending.Contains(entryId)
            ? LOutpostSeal.LOutpostSealFormat(
                "llyn:entry:" + stamp + ":" + entryId.ToString(CultureInfo.InvariantCulture))
            : string.Empty;
        Func<string, string, string, string> address = LOutpostShelf.LOutpostShelfBuild(stamp, languages);
        HashSet<string> answered = new(StringComparer.Ordinal);
        LOutpostShelf reconstruction = new(_lOutpostBatchHttp, _lOutpostBatchEngraver, fault);
        bool foreign = manifest.LManifestRealm.Length > 0
            && !string.Equals(manifest.LManifestRealm, stamp, StringComparison.Ordinal);
        Dictionary<string, string> digests = foreign
            ? new(StringComparer.Ordinal)
            : new(manifest.LManifestDigest, StringComparer.Ordinal);
        HashSet<string> current = new(StringComparer.Ordinal) { style };
        int saved = 0;
        int kept = 0;
        int removed = 0;
        int stalled = 0;
        List<string> failed = [];
        try
        {
            try
            {
                await _lOutpostBatchHttp
                    .LOutpostFolderSave(port, token, notebook, string.Empty, cargo.LOutpostCargoNotebook, cancellation)
                    .ConfigureAwait(false);
                await _lOutpostBatchHttp
                    .LOutpostFolderSave(port, token, system, notebook, LOutpostBatchSystem, cancellation)
                    .ConfigureAwait(false);
                foreach ((string name, string shelf) in shelves)
                {
                    await _lOutpostBatchHttp
                        .LOutpostFolderSave(port, token, shelf, notebook, name, cancellation)
                        .ConfigureAwait(false);
                }

                string sheet = _lOutpostBatchEngraver.LEngraverEntranceRead();
                LOutpostNote styleNote = new(style, system, LOutpostBatchStyle, sheet);
                await LOutpostBatchSave(port, token, styleNote, [], [], digests, cancellation)
                    .ConfigureAwait(false);
            }
            catch (TimeoutException exception)
            {
                fault(exception);
                throw new LRefusal(LRefusal.LRefusalOutpost);
            }

            foreach (LLiveryLanguage sound in languages)
            {
                stalled = await reconstruction.LOutpostShelfSend(
                    port, token, shelves[sound.LLiveryLanguageName], mark, sound, note, address, lookup,
                    async (outpost, parcels) =>
                    {
                        bool sent = await LOutpostBatchSave(port, token, outpost, [], parcels, digests, cancellation)
                            .ConfigureAwait(false);
                        answered.Add(outpost.LOutpostNoteId);
                        saved += sent ? 1 : 0;
                        kept += sent ? 0 : 1;

                        return sent;
                    },
                    current, failed, stalled, cancellation)
                    .ConfigureAwait(false);
            }

            Func<string, string, string, string> link = (name, kind, key) =>
                answered.Contains(address(name, kind, key)) ? address(name, kind, key) : string.Empty;

            foreach (LEntry entry in entries)
            {
                cancellation.ThrowIfCancellationRequested();
                string id = note(entry.LEntryId);
                current.Add(id);
                string folder = shelves.GetValueOrDefault(entry.LEntryLanguage.Trim(), notebook);
                try
                {
                    LLiveryPage read = cargo.LOutpostCargoPage(entry.LEntryId)
                        ?? throw new InvalidOperationException("The entry no longer stands in the workspace.");
                    LLiveryNote rendered = _lOutpostBatchEngraver.LEngraverEntranceFormat(
                        read, mark, note, link, lookup);
                    LOutpostNote outpost = new(id, folder, entry.LEntryHeadword, rendered.LLiveryNoteBody);
                    bool sent = await LOutpostBatchSave(
                        port, token, outpost, LOutpostBatchRead(read.LLiveryPageDraft), rendered.LLiveryNoteParcel,
                        digests, cancellation)
                        .ConfigureAwait(false);
                    stalled = 0;
                    saved += sent ? 1 : 0;
                    kept += sent ? 0 : 1;
                }
                catch (Exception exception) when (exception is not LRefusal and not OperationCanceledException)
                {
                    fault(exception);
                    failed.Add(entry.LEntryHeadword);
                    stalled = exception is TimeoutException ? stalled + 1 : 0;
                    if (stalled >= LOutpostBatchStall)
                    {
                        throw new LRefusal(LRefusal.LRefusalOutpost);
                    }
                }
            }

            stalled = 0;
            foreach (string id in digests.Keys.Where(id => !unread && !current.Contains(id)).ToList())
            {
                cancellation.ThrowIfCancellationRequested();
                try
                {
                    bool trashed = await _lOutpostBatchHttp
                        .LOutpostNoteRemove(port, token, id, folders, mark, cancellation)
                        .ConfigureAwait(false);
                    digests.Remove(id);
                    stalled = 0;
                    if (trashed)
                    {
                        removed++;
                    }
                }
                catch (Exception exception) when (exception is not LRefusal and not OperationCanceledException)
                {
                    fault(exception);
                    stalled = exception is TimeoutException ? stalled + 1 : 0;
                    if (stalled >= LOutpostBatchStall)
                    {
                        throw new LRefusal(LRefusal.LRefusalOutpost);
                    }
                }
            }
        }
        finally
        {
            lock (_lOutpostBatchGate)
            {
                _lOutpostBatchManifest.LManifestSave(new LManifest(digests, stamp));
            }
        }

        return new LReceipt(saved, kept, removed, failed);
    }

    private async Task<bool> LOutpostBatchSave(
        int port,
        string token,
        LOutpostNote note,
        IReadOnlyList<string> tags,
        IReadOnlyList<LParcel> parcels,
        Dictionary<string, string> digests,
        CancellationToken cancellation)
    {
        string digest = LOutpostSeal.LOutpostSealFormat(note, tags, parcels);
        if (digests.TryGetValue(note.LOutpostNoteId, out string? held)
            && string.Equals(held, digest, StringComparison.Ordinal))
        {
            string? body = await _lOutpostBatchHttp.LOutpostNoteRead(
                port, token, note.LOutpostNoteId, note.LOutpostNoteFolder, note.LOutpostNoteTitle, cancellation)
                .ConfigureAwait(false);
            if (string.Equals(body, note.LOutpostNoteBody, StringComparison.Ordinal))
            {
                await _lOutpostBatchHttp.LOutpostTagSave(port, token, note.LOutpostNoteId, tags, cancellation)
                    .ConfigureAwait(false);
                return false;
            }
        }

        foreach (LParcel parcel in parcels)
        {
            await _lOutpostBatchHttp.LOutpostParcelSave(port, token, parcel, cancellation)
                .ConfigureAwait(false);
        }

        await _lOutpostBatchHttp.LOutpostNoteSave(port, token, note, cancellation).ConfigureAwait(false);
        await _lOutpostBatchHttp.LOutpostTagSave(port, token, note.LOutpostNoteId, tags, cancellation)
            .ConfigureAwait(false);
        digests[note.LOutpostNoteId] = digest;
        return true;
    }

    private static IReadOnlyList<string> LOutpostBatchRead(LEntryDraft draft)
    {
        SortedSet<string> tags = new(StringComparer.Ordinal);
        if (!string.IsNullOrWhiteSpace(draft.LEntryDraftLanguage))
        {
            tags.Add(LOutpostBatchTag + draft.LEntryDraftLanguage.Trim());
        }

        Stack<LCardDraft> cards = new(draft.LEntryDraftMeanings.Concat(draft.LEntryDraftCollocations));
        while (cards.Count > 0)
        {
            LCardDraft card = cards.Pop();
            foreach (LTagDraft tag in card.LCardDraftTag)
            {
                if (!string.IsNullOrWhiteSpace(tag.LTagDraftText))
                {
                    tags.Add(LOutpostBatchTag + tag.LTagDraftText.Trim());
                }
            }

            foreach (LCardDraft child in card.LCardDraftChild)
            {
                cards.Push(child);
            }
        }

        return [.. tags];
    }
}
