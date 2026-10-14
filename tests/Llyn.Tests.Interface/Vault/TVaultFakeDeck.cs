using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Tests;

internal sealed class TVaultFakeDeck : LDeckVault
{
    private readonly TVaultFakeCue _tVaultFakeCues;
    private readonly TVaultFakeStencil _tVaultFakeStencils;
    private readonly List<LDeck> _tVaultFakeDecks = [];
    private readonly Dictionary<long, IReadOnlyList<LBearing>> _tVaultFakeBearings = [];
    private long _tVaultFakeNext = 100;

    public TVaultFakeDeck(TVaultFakeCue cues, TVaultFakeStencil stencils)
    {
        _tVaultFakeCues = cues;
        _tVaultFakeStencils = stencils;
    }

    public LDeck LDeckCreate(LDeck deck)
    {
        LDeck created = deck with
        {
            LDeckId = ++_tVaultFakeNext,
            LDeckPosition = TVaultFakeScan(deck.LDeckFolderId).Count,
        };
        _tVaultFakeDecks.Add(created);
        return created;
    }

    public IReadOnlyList<LDeck> LDeckRead() =>
        _tVaultFakeDecks
            .OrderBy(static deck => deck.LDeckFolderId ?? 0)
            .ThenBy(static deck => deck.LDeckPosition)
            .ToList();

    public LDeck? LDeckRead(long deckId) =>
        _tVaultFakeDecks.Find(deck => deck.LDeckId == deckId);

    public void LDeckUpdate(LDeck deck)
    {
        int index = _tVaultFakeDecks.FindIndex(held => held.LDeckId == deck.LDeckId);
        if (index < 0)
        {
            throw new InvalidOperationException($"No deck carries the id '{deck.LDeckId}'.");
        }

        LDeck held = _tVaultFakeDecks[index];
        _tVaultFakeDecks[index] = deck with
        {
            LDeckGrain = held.LDeckGrain,
            LDeckStencilId = held.LDeckStencilId,
            LDeckAddedUtc = held.LDeckAddedUtc,
        };
    }

    public void LDeckOrderSet(long? folderId, IReadOnlyList<long> order)
    {
        for (int position = 0; position < order.Count; position++)
        {
            int index = _tVaultFakeDecks.FindIndex(
                deck => deck.LDeckId == order[position] && deck.LDeckFolderId == folderId);
            if (index >= 0)
            {
                _tVaultFakeDecks[index] = _tVaultFakeDecks[index] with { LDeckPosition = position };
            }
        }
    }

    public void LDeckDelete(long deckId)
    {
        LDeck? held = LDeckRead(deckId);
        if (held is null)
        {
            return;
        }

        _tVaultFakeDecks.Remove(held);
        _tVaultFakeBearings.Remove(deckId);
        _tVaultFakeCues.LCueDelete(_tVaultFakeCues.LCueRead(deckId).Select(static cue => cue.LCueId).ToList());
        if (_tVaultFakeStencils.LStencilRead(held.LDeckStencilId) is { LStencilSaved: false })
        {
            _tVaultFakeStencils.TVaultFakeDelete(held.LDeckStencilId);
        }

        LDeckOrderSet(held.LDeckFolderId, TVaultFakeScan(held.LDeckFolderId));
    }

    public IReadOnlyList<LBearing> LBearingRead(long deckId) =>
        _tVaultFakeBearings.TryGetValue(deckId, out IReadOnlyList<LBearing>? held) ? held : [];

    public void LBearingSave(long deckId, IReadOnlyList<LBearing> bearings)
    {
        _tVaultFakeBearings[deckId] = bearings.OrderBy(static bearing => bearing.LBearingDirection).ToList();
    }

    private List<long> TVaultFakeScan(long? folderId) =>
        _tVaultFakeDecks
            .Where(deck => deck.LDeckFolderId == folderId)
            .OrderBy(static deck => deck.LDeckPosition)
            .Select(static deck => deck.LDeckId)
            .ToList();
}
