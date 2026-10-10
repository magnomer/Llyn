using System;
using System.Collections.Generic;
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

    internal const int LCourierStall = 3;

    private readonly LOutpost _lCourierClerkOutpost;
    private readonly LLivery _lCourierClerkLivery;
    private readonly LManifestVault _lCourierClerkManifest;
    private readonly LWarrant _lCourierClerkWarrant;
    private readonly LWorkspaceVault _lCourierClerkWorkspaces;
    private readonly LLanguageVault _lCourierClerkLanguages;
    private readonly LEntryQueryClerk _lCourierClerkEntry;
    private readonly LCourierWarrant _lCourierClerkPairing;
    private readonly LCourierNote _lCourierClerkNote;
    private readonly object _lCourierClerkGate;
    private readonly Func<LSettings> _lCourierClerkSettings;
    private readonly Action<Exception> _lCourierClerkFault;
    private static int _lCourierClerkBusy;

    public LCourierClerk(
        LRig rig,
        LEntryQueryClerk entry,
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
        _lCourierClerkLivery = rig.LRigAsset.LRigAssetLivery;
        _lCourierClerkManifest = rig.LRigAsset.LRigAssetManifest;
        _lCourierClerkWarrant = rig.LRigWarrant;
        _lCourierClerkWorkspaces = rig.LRigKeeping.LRigKeepingWorkspaces;
        _lCourierClerkLanguages = rig.LRigLanguages;
        _lCourierClerkEntry = entry;
        _lCourierClerkPairing = new LCourierWarrant(_lCourierClerkOutpost, _lCourierClerkWarrant, fault);
        _lCourierClerkNote = new LCourierNote(_lCourierClerkOutpost, _lCourierClerkLivery);
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
            return await _lCourierClerkPairing
                .LCourierWarrantAttach(_lCourierClerkSettings().LSettingsOutpost, cancellation)
                .ConfigureAwait(false);
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
            entries = _lCourierClerkEntry.LEntryFind(string.Empty);
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
        Func<long, string> note = LCourierNote.LCourierNoteBuild(_lCourierClerkLivery, stamp, entries);
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
                await _lCourierClerkNote.LCourierNoteSend(port, token, styleNote, [], [], digests, cancellation)
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
                        bool sent = await _lCourierClerkNote
                            .LCourierNoteSend(port, token, outpost, [], parcels, digests, cancellation)
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
                    bool sent = await _lCourierClerkNote.LCourierEntrySend(
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
}
