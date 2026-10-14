using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LDeckClerk
{
    private readonly LVault _lDeckClerkVault;
    private readonly LDeckVault _lDeckClerkDecks;
    private readonly LDeckFolderVault _lDeckClerkFolders;
    private readonly LStencilVault _lDeckClerkStencils;
    private readonly LTrailClerk _lDeckClerkTrail;

    public LDeckClerk(LRig rig, LTrailClerk trail)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(trail);
        _lDeckClerkVault = rig.LRigVault;
        _lDeckClerkDecks = rig.LRigRehearsal.LRigRehearsalDecks;
        _lDeckClerkFolders = rig.LRigRehearsal.LRigRehearsalFolders;
        _lDeckClerkStencils = rig.LRigRehearsal.LRigRehearsalStencils;
        _lDeckClerkTrail = trail;
    }

    public IReadOnlyList<LDeck> LDeckClerkRead()
    {
        return _lDeckClerkDecks.LDeckRead();
    }

    public (LDeck LDeckStored, IReadOnlyList<LBearing> LDeckBearings)? LDeckClerkRead(long deckId)
    {
        LDeck? stored = _lDeckClerkDecks.LDeckRead(deckId);
        return stored is null ? null : (stored, _lDeckClerkDecks.LBearingRead(deckId));
    }

    public LDeck LDeckClerkCreate(string name, long? folderId, LGrain grain)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        string trimmed = name.Trim();
        string code = LDeckCode.LDeckCodeResolve(
            _lDeckClerkTrail.LTrailNameNormalize(trimmed),
            _lDeckClerkDecks.LDeckRead().Select(static deck => deck.LDeckCode).ToList());

        using LVaultSession session = _lDeckClerkVault.LVaultSessionStart();

        LStencil stencil = _lDeckClerkStencils.LStencilCreate(
            new LStencil(0, trimmed, false, LStencilDefault.LStencilDefaultRead(grain)));
        LDeck deck = _lDeckClerkDecks.LDeckCreate(new LDeck(
            0, folderId, 0, trimmed, grain, stencil.LStencilId, LBearingLead.LBearingLeadOne,
            new LDeckSchedule(), code, false, null, string.Empty, string.Empty));
        _lDeckClerkDecks.LBearingSave(deck.LDeckId, [new LBearing(1, 1, 2, 1), new LBearing(2, 2, 1, 2)]);

        session.LVaultSessionCommit();
        return deck;
    }

    public void LDeckNameSet(long deckId, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        LDeck stored = _lDeckClerkDecks.LDeckRead(deckId) ?? throw new LRefusal(LRefusal.LRefusalDeck);
        _lDeckClerkDecks.LDeckUpdate(stored with { LDeckName = name.Trim() });
    }

    public void LDeckClerkUpdate(LDeck deck, IReadOnlyList<LBearing> bearings)
    {
        ArgumentNullException.ThrowIfNull(deck);
        ArgumentNullException.ThrowIfNull(bearings);
        ArgumentException.ThrowIfNullOrWhiteSpace(deck.LDeckName);
        ArgumentException.ThrowIfNullOrWhiteSpace(deck.LDeckCode);

        string code = _lDeckClerkTrail.LTrailNameNormalize(deck.LDeckCode.Trim());
        List<string> taken = _lDeckClerkDecks.LDeckRead()
            .Where(other => other.LDeckId != deck.LDeckId)
            .Select(static other => other.LDeckCode)
            .ToList();
        if (!LDeckCode.LDeckCodeCheck(code, taken))
        {
            throw new LRefusal(LRefusal.LRefusalCode);
        }

        using LVaultSession session = _lDeckClerkVault.LVaultSessionStart();

        LDeck stored = _lDeckClerkDecks.LDeckRead(deck.LDeckId) ?? throw new LRefusal(LRefusal.LRefusalDeck);
        _lDeckClerkDecks.LDeckUpdate(stored with
        {
            LDeckName = deck.LDeckName.Trim(),
            LDeckLead = deck.LDeckLead,
            LDeckSchedule = deck.LDeckSchedule,
            LDeckCode = code,
            LDeckBasic = deck.LDeckBasic,
        });
        _lDeckClerkDecks.LBearingSave(stored.LDeckId, bearings);

        session.LVaultSessionCommit();
    }

    public void LDeckClerkMove(long deckId, long? folderId)
    {
        using LVaultSession session = _lDeckClerkVault.LVaultSessionStart();

        IReadOnlyList<LDeck> decks = _lDeckClerkDecks.LDeckRead();
        LDeck stored = decks.FirstOrDefault(deck => deck.LDeckId == deckId)
            ?? throw new LRefusal(LRefusal.LRefusalDeck);
        if (stored.LDeckFolderId == folderId)
        {
            return;
        }

        _lDeckClerkDecks.LDeckUpdate(stored with
        {
            LDeckFolderId = folderId,
            LDeckPosition = LDeckPlaceRead(decks, folderId),
        });
        _lDeckClerkDecks.LDeckOrderSet(
            stored.LDeckFolderId,
            decks.Where(deck => deck.LDeckFolderId == stored.LDeckFolderId && deck.LDeckId != deckId)
                .OrderBy(static deck => deck.LDeckPosition)
                .Select(static deck => deck.LDeckId)
                .ToList());

        session.LVaultSessionCommit();
    }

    public void LDeckOrderSet(long? folderId, IReadOnlyList<long> order)
    {
        ArgumentNullException.ThrowIfNull(order);
        _lDeckClerkDecks.LDeckOrderSet(folderId, order);
    }

    public void LDeckClerkDelete(long deckId)
    {
        _lDeckClerkDecks.LDeckDelete(deckId);
    }

    public IReadOnlyList<LDeckFolder> LDeckFolderRead()
    {
        return _lDeckClerkFolders.LDeckFolderRead();
    }

    public LDeckFolder LDeckFolderCreate(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return _lDeckClerkFolders.LDeckFolderCreate(name.Trim());
    }

    public void LDeckFolderUpdate(long folderId, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        LDeckFolder? stored = _lDeckClerkFolders.LDeckFolderRead()
            .FirstOrDefault(folder => folder.LDeckFolderId == folderId);
        if (stored is not null)
        {
            _lDeckClerkFolders.LDeckFolderUpdate(stored with { LDeckFolderName = name.Trim() });
        }
    }

    public void LDeckFolderDelete(long folderId)
    {
        using LVaultSession session = _lDeckClerkVault.LVaultSessionStart();

        IReadOnlyList<LDeck> decks = _lDeckClerkDecks.LDeckRead();
        int place = LDeckPlaceRead(decks, null);
        foreach (LDeck deck in decks.Where(deck => deck.LDeckFolderId == folderId)
            .OrderBy(static deck => deck.LDeckPosition))
        {
            _lDeckClerkDecks.LDeckUpdate(deck with { LDeckFolderId = null, LDeckPosition = place++ });
        }

        _lDeckClerkFolders.LDeckFolderDelete(folderId);

        session.LVaultSessionCommit();
    }

    private static int LDeckPlaceRead(IReadOnlyList<LDeck> decks, long? folderId)
    {
        return decks.Where(deck => deck.LDeckFolderId == folderId)
            .Select(static deck => deck.LDeckPosition + 1)
            .DefaultIfEmpty(0)
            .Max();
    }
}
