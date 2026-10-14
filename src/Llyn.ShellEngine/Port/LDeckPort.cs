using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LDeckPort
{
    (IReadOnlyList<LDeckFolder> LDeckFolders, IReadOnlyList<LDeck> LDeckListed) LEngineDeckRead();

    (LDeck LDeckStored, IReadOnlyList<LBearing> LDeckBearings)? LEngineDeckRead(long deckId);

    long LEngineDeckCreate(string name, long? folderId, LGrain grain);

    void LEngineDeckUpdate(long deckId, string name);

    void LEngineDeckUpdate(LDeck deck, IReadOnlyList<LBearing> bearings);

    void LEngineDeckMove(long deckId, long? folderId);

    void LEngineDeckMove(long? folderId, IReadOnlyList<long> order);

    void LEngineDeckDelete(long deckId);

    long LEngineFolderCreate(string name);

    void LEngineFolderUpdate(long folderId, string name);

    void LEngineFolderDelete(long folderId);

    IReadOnlyList<LCue> LEngineCueRead(long deckId);

    int LEngineCueAdd(long deckId, IReadOnlyList<long> entryIds);

    long LEngineCueCreate(long deckId, IReadOnlyList<LCueCell> cells);

    void LEngineCueRemove(IReadOnlyList<long> cueIds);

    void LEngineCueMove(long deckId, long cueId, int position);

    void LEngineCueSet(long cueId, int flag, bool excluded);

    LCueFace LEngineFaceRead(long cueId, LPortraitLabel label);

    Task<LDeckReceipt> LEngineDeckExport(long deckId, LPortraitLabel label, CancellationToken cancellation);

    bool LEngineDepotCheck();

    IReadOnlyList<LCueCell> LEngineCellRead(long cueId);

    void LEngineCellSave(long cueId, IReadOnlyList<LCueCell> cells);

    LStencil LEngineStencilRead(long deckId);

    void LEngineSlotSet(long deckId, LStencilSlot slot);

    void LEngineStencilSave(long deckId, string name);

    void LEngineStencilApply(long deckId, long stencilId);
}
