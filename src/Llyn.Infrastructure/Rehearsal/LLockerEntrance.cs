using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed class LLockerEntrance : LDepot
{
    private const string LLockerEntranceStamp = "yyyy-MM-dd HH:mm:ss";

    private readonly LDepotFolder _lLockerEntranceFolder = new();
    private readonly LDeckManifestFile _lLockerEntranceManifest;
    private readonly LClock _lLockerEntranceClock;
    private readonly object _lLockerEntranceGate = new();

    public LLockerEntrance(LKeep keep, LClock clock)
    {
        ArgumentNullException.ThrowIfNull(keep);
        ArgumentNullException.ThrowIfNull(clock);
        _lLockerEntranceManifest = new LDeckManifestFile(keep);
        _lLockerEntranceClock = clock;
    }

    async Task<LDeckReceipt> LDepot.LDepotExport(
        LDepotShipment shipment, Action<Exception> fault, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(shipment);
        ArgumentNullException.ThrowIfNull(fault);
        if (shipment.LDepotShipmentCues.Count != shipment.LDepotShipmentFaces.Count
            || shipment.LDepotShipmentTyped.Count != shipment.LDepotShipmentFaces.Count)
        {
            throw new ArgumentException(
                "The shipment must hold one face and one typed row for each cue.", nameof(shipment));
        }

        string folder = shipment.LDepotShipmentFolder;
        string code = shipment.LDepotShipmentCode;
        string? written;
        IReadOnlyList<string> owned;
        lock (_lLockerEntranceGate)
        {
            LDeckManifest manifest = _lLockerEntranceManifest.LDeckManifestRead();
            bool known = manifest.LDeckManifestDecks.TryGetValue(shipment.LDepotShipmentDeck, out var entry);
            written = known ? entry.LDeckManifestDigest : null;
            owned = known ? entry.LDeckManifestMedia : [];
            string? digest = _lLockerEntranceFolder.LDepotDigestRead(folder, code);
            if (digest is not null && !string.Equals(digest, written, StringComparison.Ordinal))
            {
                throw new LRefusal(LRefusal.LRefusalPhone);
            }
        }

        HashSet<string> named = new(StringComparer.Ordinal);
        HashSet<string> wrote = new(StringComparer.Ordinal);
        HashSet<string> held = new(owned, StringComparer.Ordinal);
        List<string> failed = [];
        int removed = 0;
        try
        {
            List<LCueFace> resolved = [];
            for (int row = 0; row < shipment.LDepotShipmentFaces.Count; row++)
            {
                cancellation.ThrowIfCancellationRequested();
                LCueFace face = shipment.LDepotShipmentFaces[row];
                IReadOnlyList<string?> texts = shipment.LDepotShipmentTyped[row];
                List<LCueCell> cells = [];
                for (int index = 0; index < face.LCueFaceCells.Count; index++)
                {
                    string value = LLockerCellResolve(
                        face.LCueFaceCells[index], texts[index], folder, code, named, wrote, failed, fault);
                    cells.Add(face.LCueFaceCells[index] with { LCueCellValue = value });
                }

                resolved.Add(new LCueFace(cells));
            }

            IReadOnlyList<string> header = LDepotFolder.LDepotHeaderResolve(resolved);
            List<IReadOnlyList<string>> rows =
            [
                .. resolved.Select((face, index) =>
                    LDepotFolder.LDepotRowFormat(shipment.LDepotShipmentCues[index], face, header)),
            ];
            LDeckSheet sheet = new([], header, rows);
            await _lLockerEntranceFolder.LDepotSave(folder, code, sheet, shipment.LDepotShipmentBasic, cancellation)
                .ConfigureAwait(false);
            written = _lLockerEntranceFolder.LDepotDigestRead(folder, code);

            foreach (string name in owned.Where(name => !named.Contains(name)))
            {
                try
                {
                    _lLockerEntranceFolder.LDepotMediaDelete(folder, code, name);
                    held.Remove(name);
                    removed++;
                }
                catch (Exception exception) when (exception is not LRefusal and not OperationCanceledException)
                {
                    fault(exception);
                    failed.Add(name);
                }
            }

            return new LDeckReceipt(rows.Count, wrote.Count, removed, failed);
        }
        finally
        {
            held.UnionWith(wrote);
            string stamp = _lLockerEntranceClock.LClockRead().UtcDateTime
                .ToString(LLockerEntranceStamp, CultureInfo.InvariantCulture);
            lock (_lLockerEntranceGate)
            {
                Dictionary<long, (string, IReadOnlyList<string>, string)> decks =
                    new(_lLockerEntranceManifest.LDeckManifestRead().LDeckManifestDecks)
                    {
                        [shipment.LDepotShipmentDeck] =
                            (written ?? string.Empty, held.Order(StringComparer.Ordinal).ToList(), stamp),
                    };
                _lLockerEntranceManifest.LDeckManifestSave(new LDeckManifest(decks));
            }
        }
    }

    private string LLockerCellResolve(
        LCueCell cell,
        string? typed,
        string folder,
        string code,
        HashSet<string> named,
        HashSet<string> wrote,
        List<string> failed,
        Action<Exception> fault)
    {
        string source = cell.LCueCellValue.Trim();
        if (typed is null
            || cell.LCueCellColumn is not (LCueColumn.LCueColumnPicture or LCueColumn.LCueColumnSound))
        {
            return cell.LCueCellValue;
        }

        if (!Path.IsPathFullyQualified(source))
        {
            failed.Add(typed);
            return string.Empty;
        }

        try
        {
            if (_lLockerEntranceFolder.LDepotMediaResolve(source) is not string name)
            {
                failed.Add(typed);
                return string.Empty;
            }

            if (!named.Contains(name) && !_lLockerEntranceFolder.LDepotMediaCheck(folder, code, name))
            {
                _lLockerEntranceFolder.LDepotMediaCopy(source, folder, code, name);
                wrote.Add(name);
            }

            named.Add(name);
            return name;
        }
        catch (Exception exception) when (exception is not LRefusal and not OperationCanceledException)
        {
            fault(exception);
            failed.Add(typed);
            return string.Empty;
        }
    }
}
