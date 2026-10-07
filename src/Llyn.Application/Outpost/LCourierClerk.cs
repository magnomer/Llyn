using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LCourierClerk
{
    private const string LCourierNotebook = "Llyn";

    private const string LCourierSystem = "System";

    private const string LCourierStyle = "Llyn style";

    private const string LCourierTagPrefix = "llyn/";

    internal const string LCourierPhonology = "phonology";

    internal const int LCourierStall = 3;

    private const int LCourierPatience = 120;

    private static readonly TimeSpan LCourierInterval = TimeSpan.FromSeconds(1);

    private readonly LOutpost _lCourierClerkOutpost;
    private readonly LLivery _lCourierClerkLivery;
    private readonly LManifestVault _lCourierClerkManifest;
    private readonly LWarrant _lCourierClerkWarrant;
    private readonly LWorkspaceVault _lCourierClerkWorkspaces;
    private readonly LLanguageVault _lCourierClerkLanguages;
    private readonly LEntryClerk _lCourierClerkEntry;
    private readonly object _lCourierClerkGate;
    private readonly Func<LSettings> _lCourierClerkSettings;
    private readonly Action<Exception> _lCourierClerkFault;
    private static int _lCourierClerkBusy;

    public LCourierClerk(
        LRig rig,
        LEntryClerk entry,
        object gate,
        Func<LSettings> settings,
        Action<Exception> fault)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(gate);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(fault);
        _lCourierClerkOutpost = rig.LRigOutpost;
        _lCourierClerkLivery = rig.LRigLivery;
        _lCourierClerkManifest = rig.LRigManifest;
        _lCourierClerkWarrant = rig.LRigWarrant;
        _lCourierClerkWorkspaces = rig.LRigWorkspaces;
        _lCourierClerkLanguages = rig.LRigLanguages;
        _lCourierClerkEntry = entry;
        _lCourierClerkGate = gate;
        _lCourierClerkSettings = settings;
        _lCourierClerkFault = fault;
    }

    public async Task<LReceipt> LCourierClerkSend(
        Func<long, LLiveryPage?> page,
        Func<string, LLiveryLanguage> language,
        Func<string, string> lookup,
        CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(page);
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(lookup);
        if (Interlocked.CompareExchange(ref _lCourierClerkBusy, 1, 0) != 0)
        {
            throw new LRefusal(LRefusal.LRefusalCourier);
        }

        try
        {
            return await LCourierBatchSend(page, language, lookup, cancellation).ConfigureAwait(false);
        }
        finally
        {
            Volatile.Write(ref _lCourierClerkBusy, 0);
        }
    }

    public async Task<string> LCourierClerkAttach(CancellationToken cancellation)
    {
        if (Interlocked.CompareExchange(ref _lCourierClerkBusy, 1, 0) != 0)
        {
            throw new LRefusal(LRefusal.LRefusalCourier);
        }

        try
        {
            LSettings settings = _lCourierClerkSettings();
            int port = await _lCourierClerkOutpost.LOutpostFind(settings.LSettingsOutpost, cancellation)
                .ConfigureAwait(false) ?? throw new LRefusal(LRefusal.LRefusalOutpost);
            string ticket;
            try
            {
                ticket = await _lCourierClerkOutpost.LOutpostWarrantStart(port, cancellation)
                    .ConfigureAwait(false);
            }
            catch (Exception exception) when (exception is not LRefusal and not OperationCanceledException)
            {
                _lCourierClerkFault(exception);
                throw new LRefusal(LRefusal.LRefusalOutpost);
            }

            int stalled = 0;
            for (int poll = 0; poll < LCourierPatience; poll++)
            {
                await Task.Delay(LCourierInterval, cancellation).ConfigureAwait(false);
                LWarrantAnswer answer;
                try
                {
                    answer = await _lCourierClerkOutpost
                        .LOutpostWarrantCheck(port, ticket, cancellation).ConfigureAwait(false);
                }
                catch (TimeoutException exception)
                {
                    _lCourierClerkFault(exception);
                    stalled++;
                    if (stalled >= LCourierStall)
                    {
                        throw new LRefusal(LRefusal.LRefusalOutpost);
                    }

                    continue;
                }

                stalled = 0;
                if (answer.LWarrantAnswerState == LWarrantState.LWarrantStateAccepted)
                {
                    string token = answer.LWarrantAnswerToken ?? throw new LRefusal(LRefusal.LRefusalWarrant);
                    return _lCourierClerkWarrant.LWarrantHide(token);
                }

                if (answer.LWarrantAnswerState == LWarrantState.LWarrantStateRejected)
                {
                    throw new LRefusal(LRefusal.LRefusalWarrant);
                }
            }

            throw new LRefusal(LRefusal.LRefusalPending);
        }
        finally
        {
            Volatile.Write(ref _lCourierClerkBusy, 0);
        }
    }

    public bool LCourierClerkCheck()
    {
        return !string.IsNullOrWhiteSpace(_lCourierClerkSettings().LSettingsWarrant);
    }

    public static bool LCourierWarrantCheck(Exception exception)
    {
        return exception is LRefusal { LRefusalReason: LRefusal.LRefusalWarrant };
    }

    public static Func<long, string> LCourierNoteBuild(LLivery livery, string stamp, IReadOnlyList<LEntry> entries)
    {
        ArgumentNullException.ThrowIfNull(livery);
        ArgumentNullException.ThrowIfNull(stamp);
        ArgumentNullException.ThrowIfNull(entries);

        HashSet<long> sending = [.. entries.Select(static entry => entry.LEntryId)];
        return entryId => sending.Contains(entryId)
            ? livery.LLiveryIdFormat("llyn:entry:" + stamp + ":" + entryId.ToString(CultureInfo.InvariantCulture))
            : string.Empty;
    }

    public static Func<string, string, string, string> LCourierLinkBuild(
        LLivery livery, string stamp, IReadOnlyList<LLiveryLanguage> languages)
    {
        ArgumentNullException.ThrowIfNull(livery);
        ArgumentNullException.ThrowIfNull(stamp);
        ArgumentNullException.ThrowIfNull(languages);

        Dictionary<(string, string, string), string> links = [];
        foreach (LLiveryLanguage language in languages)
        {
            string name = language.LLiveryLanguageName;
            List<(string LCourierLinkKind, string LCourierLinkKey)> held =
            [
                .. language.LLiveryLanguageStem
                    .Select(static stem => (LLiveryStem.LLiveryStemKind, stem.LLiveryStemPage.LStemPageKey)),
                .. language.LLiveryLanguageDiwei
                    .Select(static diwei => (diwei.LLiveryDiweiKind, diwei.LLiveryDiweiPage.LDiweiPageKey)),
            ];
            held.RemoveAll(static row => row.LCourierLinkKey.Length == 0);
            if (language.LLiveryLanguagePronunciation.Count > 0)
            {
                held.Add((LCourierPhonology, string.Empty));
            }

            foreach ((string kind, string key) in held)
            {
                links.TryAdd(
                    (name, kind, key), livery.LLiveryIdFormat("llyn:" + kind + ":" + stamp + ":" + name + ":" + key));
            }
        }

        return (language, kind, key) => links.GetValueOrDefault((language, kind, key), string.Empty);
    }

    private async Task<LReceipt> LCourierBatchSend(
        Func<long, LLiveryPage?> page,
        Func<string, LLiveryLanguage> language,
        Func<string, string> lookup,
        CancellationToken cancellation)
    {
        LSettings settings = _lCourierClerkSettings();
        if (string.IsNullOrWhiteSpace(settings.LSettingsWarrant))
        {
            throw new LRefusal(LRefusal.LRefusalMissing);
        }

        string token = _lCourierClerkWarrant.LWarrantRestore(settings.LSettingsWarrant)
            ?? throw new LRefusal(LRefusal.LRefusalWarrant);
        int port = await _lCourierClerkOutpost.LOutpostFind(settings.LSettingsOutpost, cancellation)
            .ConfigureAwait(false) ?? throw new LRefusal(LRefusal.LRefusalOutpost);

        string notebook = _lCourierClerkLivery.LLiveryIdFormat("llyn:notebook");
        string system = _lCourierClerkLivery.LLiveryIdFormat("llyn:system");
        string style = _lCourierClerkLivery.LLiveryIdFormat("llyn:style");
        string mark = _lCourierClerkLivery.LLiveryMarkFormat(style);

        LManifest manifest;
        Guid realm;
        IReadOnlyList<LEntry> entries;
        lock (_lCourierClerkGate)
        {
            manifest = _lCourierClerkManifest.LManifestRead();
            realm = _lCourierClerkWorkspaces.LWorkspaceRealmRead();
            entries = _lCourierClerkEntry.LEntryClerkFind(string.Empty);
        }

        Dictionary<string, string> shelves = new(StringComparer.Ordinal);
        foreach (string name in entries.Select(static entry => entry.LEntryLanguage.Trim()))
        {
            if (name.Length > 0)
            {
                shelves.TryAdd(name, _lCourierClerkLivery.LLiveryIdFormat("llyn:language:" + name));
            }
        }

        HashSet<string> folders = new(StringComparer.Ordinal) { notebook, system };
        foreach (string name in shelves.Keys
            .Concat(_lCourierClerkLanguages.LLanguageScan().Select(static row => row.Trim()))
            .Where(static row => row.Length > 0))
        {
            foreach (string seed in LCourierLanguage.LCourierLanguageFolder.Prepend("language"))
            {
                folders.Add(_lCourierClerkLivery.LLiveryIdFormat("llyn:" + seed + ":" + name));
            }
        }

        List<LLiveryLanguage> languages = [];
        bool unread = false;
        foreach (string name in shelves.Keys)
        {
            try
            {
                languages.Add(language(name));
            }
            catch (Exception exception) when (exception is not LRefusal and not OperationCanceledException)
            {
                _lCourierClerkFault(exception);
                unread = true;
            }
        }

        string stamp = realm.ToString("N");
        Func<long, string> note = LCourierNoteBuild(_lCourierClerkLivery, stamp, entries);
        Func<string, string, string, string> address = LCourierLinkBuild(_lCourierClerkLivery, stamp, languages);
        HashSet<string> answered = new(StringComparer.Ordinal);
        LCourierLanguage reconstruction = new(_lCourierClerkOutpost, _lCourierClerkLivery, _lCourierClerkFault);
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
                await _lCourierClerkOutpost
                    .LOutpostFolderSave(port, token, notebook, string.Empty, LCourierNotebook, cancellation)
                    .ConfigureAwait(false);
                await _lCourierClerkOutpost
                    .LOutpostFolderSave(port, token, system, notebook, LCourierSystem, cancellation)
                    .ConfigureAwait(false);
                foreach ((string name, string shelf) in shelves)
                {
                    await _lCourierClerkOutpost
                        .LOutpostFolderSave(port, token, shelf, notebook, name, cancellation)
                        .ConfigureAwait(false);
                }

                string sheet = _lCourierClerkLivery.LLiveryRead();
                LOutpostNote styleNote = new(style, system, LCourierStyle, sheet);
                await LCourierNoteSend(port, token, styleNote, [], [], digests, cancellation)
                    .ConfigureAwait(false);
            }
            catch (TimeoutException exception)
            {
                _lCourierClerkFault(exception);
                throw new LRefusal(LRefusal.LRefusalOutpost);
            }

            foreach (LLiveryLanguage sound in languages)
            {
                stalled = await reconstruction.LCourierLanguageSend(
                    port, token, shelves[sound.LLiveryLanguageName], style, sound, note, address, lookup,
                    async (outpost, parcels) =>
                    {
                        bool sent = await LCourierNoteSend(port, token, outpost, [], parcels, digests, cancellation)
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
                    bool sent = await LCourierEntrySend(
                        port, token, id, folder, style, entry, page, note, link, lookup, digests, cancellation)
                        .ConfigureAwait(false);
                    stalled = 0;
                    saved += sent ? 1 : 0;
                    kept += sent ? 0 : 1;
                }
                catch (Exception exception) when (exception is not LRefusal and not OperationCanceledException)
                {
                    _lCourierClerkFault(exception);
                    failed.Add(entry.LEntryHeadword);
                    stalled = exception is TimeoutException ? stalled + 1 : 0;
                    if (stalled >= LCourierStall)
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
                    bool trashed = await _lCourierClerkOutpost
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
                    _lCourierClerkFault(exception);
                    stalled = exception is TimeoutException ? stalled + 1 : 0;
                    if (stalled >= LCourierStall)
                    {
                        throw new LRefusal(LRefusal.LRefusalOutpost);
                    }
                }
            }
        }
        finally
        {
            lock (_lCourierClerkGate)
            {
                _lCourierClerkManifest.LManifestSave(new LManifest(digests, stamp));
            }
        }

        return new LReceipt(saved, kept, removed, failed);
    }

    private async Task<bool> LCourierEntrySend(
        int port,
        string token,
        string id,
        string folder,
        string style,
        LEntry entry,
        Func<long, LLiveryPage?> page,
        Func<long, string> note,
        Func<string, string, string, string> link,
        Func<string, string> lookup,
        Dictionary<string, string> digests,
        CancellationToken cancellation)
    {
        LLiveryPage read = page(entry.LEntryId)
            ?? throw new InvalidOperationException("The entry no longer stands in the workspace.");

        LLiveryNote rendered = _lCourierClerkLivery.LLiveryFormat(read, style, note, link, lookup);
        LOutpostNote outpost = new(id, folder, entry.LEntryHeadword, rendered.LLiveryNoteBody);
        return await LCourierNoteSend(
            port, token, outpost, LCourierTagRead(read.LLiveryPageDraft), rendered.LLiveryNoteParcel, digests,
            cancellation)
            .ConfigureAwait(false);
    }

    private async Task<bool> LCourierNoteSend(
        int port,
        string token,
        LOutpostNote note,
        IReadOnlyList<string> tags,
        IReadOnlyList<LParcel> parcels,
        Dictionary<string, string> digests,
        CancellationToken cancellation)
    {
        string digest = _lCourierClerkLivery.LLiveryDigestFormat(note, tags, parcels);
        if (digests.TryGetValue(note.LOutpostNoteId, out string? held)
            && string.Equals(held, digest, StringComparison.Ordinal))
        {
            string? body = await _lCourierClerkOutpost.LOutpostNoteRead(
                port, token, note.LOutpostNoteId, note.LOutpostNoteFolder, note.LOutpostNoteTitle, cancellation)
                .ConfigureAwait(false);
            if (string.Equals(body, note.LOutpostNoteBody, StringComparison.Ordinal))
            {
                await _lCourierClerkOutpost.LOutpostTagSave(port, token, note.LOutpostNoteId, tags, cancellation)
                    .ConfigureAwait(false);
                return false;
            }
        }

        foreach (LParcel parcel in parcels)
        {
            await _lCourierClerkOutpost.LOutpostParcelSave(port, token, parcel, cancellation)
                .ConfigureAwait(false);
        }

        await _lCourierClerkOutpost.LOutpostNoteSave(port, token, note, cancellation).ConfigureAwait(false);
        await _lCourierClerkOutpost.LOutpostTagSave(port, token, note.LOutpostNoteId, tags, cancellation)
            .ConfigureAwait(false);
        digests[note.LOutpostNoteId] = digest;
        return true;
    }

    private static IReadOnlyList<string> LCourierTagRead(LEntryDraft draft)
    {
        SortedSet<string> tags = new(StringComparer.Ordinal);
        if (!string.IsNullOrWhiteSpace(draft.LEntryDraftLanguage))
        {
            tags.Add(LCourierTagPrefix + draft.LEntryDraftLanguage.Trim());
        }

        Stack<LCardDraft> cards = new(draft.LEntryDraftMeanings.Concat(draft.LEntryDraftCollocations));
        while (cards.Count > 0)
        {
            LCardDraft card = cards.Pop();
            foreach (LTagDraft tag in card.LCardDraftTag)
            {
                if (!string.IsNullOrWhiteSpace(tag.LTagDraftText))
                {
                    tags.Add(LCourierTagPrefix + tag.LTagDraftText.Trim());
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
