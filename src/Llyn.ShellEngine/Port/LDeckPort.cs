using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Application;
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

    IReadOnlyList<(LCue LCueStored, string LCueLine)> LEngineLineRead(long deckId, LPortraitLabel label);

    IReadOnlyList<(LVistaRow LAccessionRow, bool LAccessionBound)> LEngineAccessionFind(LVista vista, long deckId);

    int LEngineCueAdd(long deckId, IReadOnlyList<long> entryIds);

    long LEngineCueCreate(long deckId, IReadOnlyList<LCueCell> cells);

    void LEngineCueRemove(IReadOnlyList<long> cueIds);

    void LEngineCueMove(long deckId, long cueId, int position);

    void LEngineCueSet(long cueId, int flag, bool excluded);

    Task<LDeckReceipt> LEngineDeckExport(long deckId, LPortraitLabel label, CancellationToken cancellation);

    bool LEngineDepotCheck();

    IReadOnlyList<LCueCell> LEngineCellRead(long cueId);

    void LEngineCellSave(long cueId, IReadOnlyList<LCueCell> cells);

    IReadOnlyList<LStencil> LEngineStencilRead();

    LStencil LEngineStencilRead(long deckId);

    void LEngineSlotSet(long deckId, LStencilSlot slot);

    void LEngineFieldAppend(long deckId, int index, LStencilField field);

    void LEngineFieldRemove(long deckId, int index, int place);

    void LEngineJoinSet(long deckId, int index, int place, bool all);

    void LEngineMediaSet(long deckId, LCueColumn column, int index);

    void LEngineStencilSave(long deckId, string name);

    void LEngineStencilApply(long deckId, long stencilId);

    static bool LEngineBodyCheck(string body)
    {
        return LStencilClerk.LStencilClerkCheck(body);
    }

    static IReadOnlyList<LStencilToken> LEngineTokenParse(string body)
    {
        return LStencilClerk.LStencilClerkParse(body);
    }
}
