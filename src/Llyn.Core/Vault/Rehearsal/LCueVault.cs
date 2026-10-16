using System.Collections.Generic;

namespace Llyn.Core;

public interface LCueVault
{
    IReadOnlyList<LCue> LCueCreate(long deckId, IReadOnlyList<LCue> cues);

    IReadOnlyList<LCue> LCueRead();

    IReadOnlyList<LCue> LCueRead(long deckId);

    long? LCueHolderRead(LCue cue);

    void LCueUpdate(LCue cue);

    void LCueOrderSet(long deckId, IReadOnlyList<long> order);

    void LCueDelete(IReadOnlyList<long> cueIds);

    IReadOnlyList<LCueCell> LCueCellRead(long cueId);

    void LCueCellSave(long cueId, IReadOnlyList<LCueCell> cells);
}
