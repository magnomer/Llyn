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

    private const string LCourierStyle = "Llyn style";

    private const string LCourierTagPrefix = "llyn/";

    private const int LCourierStall = 3;

    private const int LCourierPatience = 120;

    private static readonly TimeSpan LCourierInterval = TimeSpan.FromSeconds(1);

    private readonly LOutpost _lCourierClerkOutpost;
    private readonly LLivery _lCourierClerkLivery;
    private readonly LManifestVault _lCourierClerkManifest;
    private readonly LWarrant _lCourierClerkWarrant;
    private readonly LWorkspaceVault _lCourierClerkWorkspaces;
    private readonly LPortraitClerk _lCourierClerkPortrait;
    private readonly LEntryClerk _lCourierClerkEntry;
    private readonly object _lCourierClerkGate;
    private readonly Func<LSettings> _lCourierClerkSettings;
    private readonly Action<Exception> _lCourierClerkFault;
    private static int _lCourierClerkBusy;

    public LCourierClerk(
        LRig rig,
        LPortraitClerk portrait,
        LEntryClerk entry,
        object gate,
        Func<LSettings> settings,
        Action<Exception> fault)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(portrait);
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(gate);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(fault);
        _lCourierClerkOutpost = rig.LRigOutpost;
        _lCourierClerkLivery = rig.LRigLivery;
        _lCourierClerkManifest = rig.LRigManifest;
        _lCourierClerkWarrant = rig.LRigWarrant;
        _lCourierClerkWorkspaces = rig.LRigWorkspaces;
        _lCourierClerkPortrait = portrait;
        _lCourierClerkEntry = entry;
        _lCourierClerkGate = gate;
        _lCourierClerkSettings = settings;
        _lCourierClerkFault = fault;
    }

    public async Task<LReceipt> LCourierClerkSend(LPortraitLabel label, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(label);
        if (Interlocked.CompareExchange(ref _lCourierClerkBusy, 1, 0) != 0)
        {
            throw new LRefusal(LRefusal.LRefusalCourier);
        }

        try
        {
            return await LCourierBatchSend(label, cancellation).ConfigureAwait(false);
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

    private async Task<LReceipt> LCourierBatchSend(LPortraitLabel label, CancellationToken cancellation)
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

        string stamp = realm.ToString("N");
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
                    .LOutpostFolderSave(port, token, notebook, LCourierNotebook, cancellation)
                    .ConfigureAwait(false);
                string sheet = _lCourierClerkLivery.LLiveryRead();
                LOutpostNote styleNote = new(style, notebook, LCourierStyle, sheet);
                await LCourierNoteSend(port, token, styleNote, [], [], digests, cancellation)
                    .ConfigureAwait(false);
            }
            catch (TimeoutException exception)
            {
                _lCourierClerkFault(exception);
                throw new LRefusal(LRefusal.LRefusalOutpost);
            }

            foreach (LEntry entry in entries)
            {
                cancellation.ThrowIfCancellationRequested();
                string id = _lCourierClerkLivery.LLiveryIdFormat(
                    "llyn:entry:" + stamp + ":"
                    + entry.LEntryId.ToString(CultureInfo.InvariantCulture));
                current.Add(id);
                try
                {
                    bool sent = await LCourierEntrySend(
                        port, token, id, notebook, style, entry, label, digests, cancellation)
                        .ConfigureAwait(false);
                    stalled = 0;
                    if (sent)
                    {
                        saved++;
                    }
                    else
                    {
                        kept++;
                    }
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
            foreach (string id in digests.Keys.Where(id => !current.Contains(id)).ToList())
            {
                cancellation.ThrowIfCancellationRequested();
                try
                {
                    bool trashed = await _lCourierClerkOutpost
                        .LOutpostNoteRemove(port, token, id, notebook, mark, cancellation)
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
        string notebook,
        string style,
        LEntry entry,
        LPortraitLabel label,
        Dictionary<string, string> digests,
        CancellationToken cancellation)
    {
        LPortraitPage page;
        LEntryDraft draft;
        lock (_lCourierClerkGate)
        {
            page = _lCourierClerkPortrait.LPortraitClerkRead(entry.LEntryId, label, false);
            draft = _lCourierClerkEntry.LEntryClerkLoad(entry.LEntryId)
                ?? throw new InvalidOperationException("The entry no longer stands in the workspace.");
        }

        LLiveryNote rendered = _lCourierClerkLivery.LLiveryFormat(page, style);
        LOutpostNote note = new(id, notebook, entry.LEntryHeadword, rendered.LLiveryNoteBody);
        return await LCourierNoteSend(
            port, token, note, LCourierTagRead(draft), rendered.LLiveryNoteParcel, digests, cancellation)
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
