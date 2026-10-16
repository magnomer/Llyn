using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LDepotClerk
{
    private const string LDepotClerkId = "ID";

    private const string LDepotClerkLead = "Text 1";

    private const int LDepotClerkSides = 5;

    private const int LDepotClerkCategories = 4;

    private static readonly IReadOnlyList<string> LDepotClerkOrder =
    [
        .. Enumerable.Range(1, LDepotClerkSides).SelectMany(static side => new[]
        {
            "Text " + side.ToString(CultureInfo.InvariantCulture),
            "Picture " + side.ToString(CultureInfo.InvariantCulture),
            "Sound " + side.ToString(CultureInfo.InvariantCulture),
        }),
        "Notes",
        "Extra Info",
        .. Enumerable.Range(1, LDepotClerkCategories)
            .Select(static index => "Category " + index.ToString(CultureInfo.InvariantCulture)),
    ];

    private readonly LDeckVault _lDepotClerkDecks;
    private readonly LDepot _lDepotClerkDepot;
    private readonly LDeckManifestVault _lDepotClerkManifest;
    private readonly LClock _lDepotClerkClock;
    private readonly LStencilClerk _lDepotClerkStencil;
    private readonly LTrailClerk _lDepotClerkTrail;
    private readonly LDepotLatch _lDepotClerkLatch;
    private readonly object _lDepotClerkGate;
    private readonly Func<LSettings> _lDepotClerkSettings;
    private readonly Action<Exception> _lDepotClerkFault;

    public LDepotClerk(
        LRig rig,
        LStencilClerk stencil,
        LTrailClerk trail,
        LDepotLatch latch,
        object gate,
        Func<LSettings> settings,
        Action<Exception> fault)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(stencil);
        ArgumentNullException.ThrowIfNull(trail);
        ArgumentNullException.ThrowIfNull(latch);
        ArgumentNullException.ThrowIfNull(gate);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(fault);
        _lDepotClerkDecks = rig.LRigRehearsal.LRigRehearsalDecks;
        _lDepotClerkDepot = rig.LRigRehearsal.LRigRehearsalDepot;
        _lDepotClerkManifest = rig.LRigRehearsal.LRigRehearsalManifest;
        _lDepotClerkClock = rig.LRigClock;
        _lDepotClerkStencil = stencil;
        _lDepotClerkTrail = trail;
        _lDepotClerkLatch = latch;
        _lDepotClerkGate = gate;
        _lDepotClerkSettings = settings;
        _lDepotClerkFault = fault;
    }

    public async Task<LDeckReceipt> LDepotClerkExport(long deckId, LPortraitLabel label, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(label);
        if (!_lDepotClerkLatch.LDepotLatchStart())
        {
            throw new LRefusal(LRefusal.LRefusalBusy);
        }

        try
        {
            return await LDepotDeckExport(deckId, label, cancellation).ConfigureAwait(false);
        }
        finally
        {
            _lDepotClerkLatch.LDepotLatchFinish();
        }
    }

    public static IReadOnlyList<string> LDepotHeaderResolve(IReadOnlyList<LCueFace> faces)
    {
        ArgumentNullException.ThrowIfNull(faces);

        HashSet<string> used = new(StringComparer.Ordinal) { LDepotClerkLead };
        foreach (LCueCell cell in faces.SelectMany(static face => face.LCueFaceCells))
        {
            if (cell.LCueCellValue.Length > 0 && LDepotColumnFormat(cell) is string name)
            {
                used.Add(name);
            }
        }

        return [.. LDepotClerkOrder.Where(used.Contains), LDepotClerkId];
    }

    private async Task<LDeckReceipt> LDepotDeckExport(long deckId, LPortraitLabel label, CancellationToken cancellation)
    {
        string setting = _lDepotClerkSettings().LSettingsDepot.Trim();
        string folder = (setting.Length > 0
                ? _lDepotClerkTrail.LTrailPathResolve(_lDepotClerkTrail.LTrailPathValidate(setting))
                : null)
            ?? throw new LRefusal(LRefusal.LRefusalDepot);

        LDeck deck;
        string? written;
        IReadOnlyList<string> owned;
        List<long> ids = [];
        List<LCueFace> faces = [];
        lock (_lDepotClerkGate)
        {
            deck = _lDepotClerkDecks.LDeckRead(deckId) ?? throw new LRefusal(LRefusal.LRefusalDeck);
            LDeckManifest manifest = _lDepotClerkManifest.LDeckManifestRead();
            bool known = manifest.LDeckManifestDecks.TryGetValue(deckId, out var entry);
            written = known ? entry.LDeckManifestDigest : null;
            owned = known ? entry.LDeckManifestMedia : [];
            string? digest = _lDepotClerkDepot.LDepotDigestRead(folder, deck.LDeckCode);
            if (digest is not null && !string.Equals(digest, written, StringComparison.Ordinal))
            {
                throw new LRefusal(LRefusal.LRefusalPhone);
            }

            foreach (var face in _lDepotClerkStencil.LStencilFaceRead(deckId, label))
            {
                ids.Add(face.LCueStored.LCueId);
                faces.Add(face.LCueShown);
            }
        }

        string code = deck.LDeckCode;
        HashSet<string> named = new(StringComparer.Ordinal);
        HashSet<string> wrote = new(StringComparer.Ordinal);
        HashSet<string> held = new(owned, StringComparer.Ordinal);
        List<string> failed = [];
        int removed = 0;
        try
        {
            List<LCueFace> resolved = [];
            foreach (LCueFace face in faces)
            {
                cancellation.ThrowIfCancellationRequested();
                List<LCueCell> cells = [];
                foreach (LCueCell cell in face.LCueFaceCells)
                {
                    string value = LDepotCellResolve(cell, folder, code, named, wrote, failed);
                    cells.Add(cell with { LCueCellValue = value });
                }

                resolved.Add(new LCueFace(cells));
            }

            IReadOnlyList<string> header = LDepotHeaderResolve(resolved);
            List<IReadOnlyList<string>> rows =
                [.. resolved.Select((face, index) => LDepotRowFormat(ids[index], face, header))];
            LDeckSheet sheet = new([], header, rows);
            await _lDepotClerkDepot.LDepotSave(folder, code, sheet, deck.LDeckBasic, cancellation)
                .ConfigureAwait(false);
            written = _lDepotClerkDepot.LDepotDigestRead(folder, code);

            foreach (string name in owned.Where(name => !named.Contains(name)))
            {
                try
                {
                    _lDepotClerkDepot.LDepotMediaDelete(folder, code, name);
                    held.Remove(name);
                    removed++;
                }
                catch (Exception exception) when (exception is not LRefusal and not OperationCanceledException)
                {
                    _lDepotClerkFault(exception);
                    failed.Add(name);
                }
            }

            return new LDeckReceipt(rows.Count, wrote.Count, removed, failed);
        }
        finally
        {
            held.UnionWith(wrote);
            string stamp = _lDepotClerkClock.LClockRead().UtcDateTime
                .ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            lock (_lDepotClerkGate)
            {
                Dictionary<long, (string, IReadOnlyList<string>, string)> decks =
                    new(_lDepotClerkManifest.LDeckManifestRead().LDeckManifestDecks)
                    {
                        [deckId] = (written ?? string.Empty, held.Order(StringComparer.Ordinal).ToList(), stamp),
                    };
                _lDepotClerkManifest.LDeckManifestSave(new LDeckManifest(decks));
            }
        }
    }

    private string LDepotCellResolve(
        LCueCell cell, string folder, string code, HashSet<string> named, HashSet<string> wrote, List<string> failed)
    {
        string location = cell.LCueCellValue.Trim();
        if (cell.LCueCellColumn is not (LCueColumn.LCueColumnPicture or LCueColumn.LCueColumnSound)
            || location.Length == 0)
        {
            return cell.LCueCellValue;
        }

        if (_lDepotClerkTrail.LTrailClerkResolve(location) is not Uri address)
        {
            failed.Add(location);
            return string.Empty;
        }

        if (!address.IsFile)
        {
            return cell.LCueCellColumn == LCueColumn.LCueColumnPicture && LVideo.LVideoFilmRead(address) is null
                ? address.OriginalString
                : string.Empty;
        }

        try
        {
            if (_lDepotClerkDepot.LDepotMediaResolve(address.LocalPath) is not string name)
            {
                failed.Add(location);
                return string.Empty;
            }

            if (!named.Contains(name) && !_lDepotClerkDepot.LDepotMediaCheck(folder, code, name))
            {
                _lDepotClerkDepot.LDepotMediaCopy(address.LocalPath, folder, code, name);
                wrote.Add(name);
            }

            named.Add(name);
            return name;
        }
        catch (Exception exception) when (exception is not LRefusal and not OperationCanceledException)
        {
            _lDepotClerkFault(exception);
            failed.Add(location);
            return string.Empty;
        }
    }

    private static List<string> LDepotRowFormat(long cue, LCueFace face, IReadOnlyList<string> header)
    {
        Dictionary<string, string> values = new(StringComparer.Ordinal);
        foreach (LCueCell cell in face.LCueFaceCells)
        {
            if (cell.LCueCellValue.Length > 0 && LDepotColumnFormat(cell) is string name)
            {
                values.TryAdd(name, cell.LCueCellValue);
            }
        }

        values[LDepotClerkId] = cue.ToString(CultureInfo.InvariantCulture);
        return [.. header.Select(name => values.GetValueOrDefault(name, string.Empty))];
    }

    private static string? LDepotColumnFormat(LCueCell cell)
    {
        string index = cell.LCueCellIndex.ToString(CultureInfo.InvariantCulture);
        string? name = cell.LCueCellColumn switch
        {
            LCueColumn.LCueColumnText => "Text " + index,
            LCueColumn.LCueColumnPicture => "Picture " + index,
            LCueColumn.LCueColumnSound => "Sound " + index,
            LCueColumn.LCueColumnNotes => "Notes",
            LCueColumn.LCueColumnExtra => "Extra Info",
            LCueColumn.LCueColumnCategory => "Category " + index,
            _ => null,
        };

        return name is not null && LDepotClerkOrder.Contains(name) ? name : null;
    }
}
