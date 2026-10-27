using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LDepotClerk
{
    private readonly LDeckVault _lDepotClerkDecks;
    private readonly LDepot _lDepotClerkDepot;
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

    private async Task<LDeckReceipt> LDepotDeckExport(long deckId, LPortraitLabel label, CancellationToken cancellation)
    {
        string setting = _lDepotClerkSettings().LSettingsDepot.Trim();
        string folder = (setting.Length > 0
                ? _lDepotClerkTrail.LTrailPathResolve(_lDepotClerkTrail.LTrailPathValidate(setting))
                : null)
            ?? throw new LRefusal(LRefusal.LRefusalDepot);

        LDeck deck;
        List<long> ids = [];
        List<LCueFace> faces = [];
        lock (_lDepotClerkGate)
        {
            deck = _lDepotClerkDecks.LDeckRead(deckId) ?? throw new LRefusal(LRefusal.LRefusalDeck);
            foreach (var face in _lDepotClerkStencil.LStencilFaceRead(deckId, label))
            {
                ids.Add(face.LCueStored.LCueId);
                faces.Add(face.LCueShown);
            }
        }

        List<LCueFace> resolved = [];
        List<IReadOnlyList<string?>> typed = [];
        foreach (LCueFace face in faces)
        {
            cancellation.ThrowIfCancellationRequested();
            List<LCueCell> cells = [];
            List<string?> texts = [];
            foreach (LCueCell cell in face.LCueFaceCells)
            {
                string value = LDepotCellResolve(cell, out string? text);
                cells.Add(cell with { LCueCellValue = value });
                texts.Add(text);
            }

            resolved.Add(new LCueFace(cells));
            typed.Add(texts);
        }

        LDepotShipment shipment = new(folder, deckId, deck.LDeckCode, deck.LDeckBasic, ids, resolved, typed);
        return await _lDepotClerkDepot
            .LDepotExport(shipment, _lDepotClerkFault, cancellation)
            .ConfigureAwait(false);
    }

    private string LDepotCellResolve(LCueCell cell, out string? typed)
    {
        string location = cell.LCueCellValue.Trim();
        typed = null;
        if (cell.LCueCellColumn is not (LCueColumn.LCueColumnPicture or LCueColumn.LCueColumnSound)
            || location.Length == 0)
        {
            return cell.LCueCellValue;
        }

        if (_lDepotClerkTrail.LTrailClerkResolve(location) is not Uri address)
        {
            typed = location;
            return string.Empty;
        }

        if (!address.IsFile)
        {
            return cell.LCueCellColumn == LCueColumn.LCueColumnPicture && LVideo.LVideoFilmRead(address) is null
                ? address.OriginalString
                : string.Empty;
        }

        typed = location;
        return address.LocalPath;
    }
}
