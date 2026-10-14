using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Tests;

internal sealed class TVaultFakeCue : LCueVault
{
    private readonly List<LCue> _tVaultFakeCues = [];
    private readonly Dictionary<long, IReadOnlyList<LCueCell>> _tVaultFakeCells = [];
    private long _tVaultFakeNext = 100;

    public IReadOnlyList<LCue> LCueCreate(long deckId, IReadOnlyList<LCue> cues)
    {
        int next = TVaultFakeScan(deckId).Count;
        List<LCue> created = [];
        foreach (LCue cue in cues)
        {
            created.Add(cue with { LCueId = ++_tVaultFakeNext, LCueDeckId = deckId, LCuePosition = next++ });
        }

        _tVaultFakeCues.AddRange(created);
        return created;
    }

    public IReadOnlyList<LCue> LCueRead(long deckId) =>
        _tVaultFakeCues
            .Where(cue => cue.LCueDeckId == deckId)
            .OrderBy(static cue => cue.LCuePosition)
            .ToList();

    public long? LCueHolderRead(LCue cue) => cue.LCueEntryId;

    public void LCueUpdate(LCue cue)
    {
        int index = _tVaultFakeCues.FindIndex(held => held.LCueId == cue.LCueId);
        if (index < 0)
        {
            throw new InvalidOperationException($"No cue carries the id '{cue.LCueId}'.");
        }

        _tVaultFakeCues[index] = _tVaultFakeCues[index] with
        {
            LCueFlag = cue.LCueFlag,
            LCueExcluded = cue.LCueExcluded,
            LCueText1 = cue.LCueText1,
            LCueText2 = cue.LCueText2,
        };
    }

    public void LCueOrderSet(long deckId, IReadOnlyList<long> order)
    {
        for (int position = 0; position < order.Count; position++)
        {
            int index = _tVaultFakeCues.FindIndex(cue => cue.LCueId == order[position] && cue.LCueDeckId == deckId);
            if (index >= 0)
            {
                _tVaultFakeCues[index] = _tVaultFakeCues[index] with { LCuePosition = position };
            }
        }
    }

    public void LCueDelete(IReadOnlyList<long> cueIds)
    {
        List<LCue> held = _tVaultFakeCues.Where(cue => cueIds.Contains(cue.LCueId)).ToList();
        foreach (LCue cue in held)
        {
            _tVaultFakeCues.Remove(cue);
            _tVaultFakeCells.Remove(cue.LCueId);
        }

        foreach (long deckId in held.Select(static cue => cue.LCueDeckId).Distinct())
        {
            LCueOrderSet(deckId, TVaultFakeScan(deckId));
        }
    }

    public IReadOnlyList<LCueCell> LCueCellRead(long cueId) =>
        _tVaultFakeCells.TryGetValue(cueId, out IReadOnlyList<LCueCell>? held) ? held : [];

    public void LCueCellSave(long cueId, IReadOnlyList<LCueCell> cells)
    {
        _tVaultFakeCells[cueId] = cells
            .OrderBy(static cell => cell.LCueCellColumn)
            .ThenBy(static cell => cell.LCueCellIndex)
            .ToList();
    }

    private List<long> TVaultFakeScan(long deckId) =>
        LCueRead(deckId).Select(static cue => cue.LCueId).ToList();
}
