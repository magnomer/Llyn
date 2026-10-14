using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LDeckOutlet : LDeckPort
{
    private readonly LEngine _lDeckOutletEngine;

    public LDeckOutlet(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);
        _lDeckOutletEngine = engine;
    }

    public (IReadOnlyList<LDeckFolder> LDeckFolders, IReadOnlyList<LDeck> LDeckListed) LEngineDeckRead() =>
        _lDeckOutletEngine.LEngineDeck.LEngineDeckRead();

    public (LDeck LDeckStored, IReadOnlyList<LBearing> LDeckBearings)? LEngineDeckRead(long deckId) =>
        _lDeckOutletEngine.LEngineDeck.LEngineDeckRead(deckId);

    public long LEngineDeckCreate(string name, long? folderId, LGrain grain) =>
        _lDeckOutletEngine.LEngineDeck.LEngineDeckCreate(name, folderId, grain);

    public void LEngineDeckUpdate(long deckId, string name) =>
        _lDeckOutletEngine.LEngineDeck.LEngineDeckUpdate(deckId, name);

    public void LEngineDeckUpdate(LDeck deck, IReadOnlyList<LBearing> bearings) =>
        _lDeckOutletEngine.LEngineDeck.LEngineDeckUpdate(deck, bearings);

    public void LEngineDeckMove(long deckId, long? folderId) =>
        _lDeckOutletEngine.LEngineDeck.LEngineDeckMove(deckId, folderId);

    public void LEngineDeckMove(long? folderId, IReadOnlyList<long> order) =>
        _lDeckOutletEngine.LEngineDeck.LEngineDeckMove(folderId, order);

    public void LEngineDeckDelete(long deckId) =>
        _lDeckOutletEngine.LEngineDeck.LEngineDeckDelete(deckId);

    public long LEngineFolderCreate(string name) =>
        _lDeckOutletEngine.LEngineDeck.LEngineFolderCreate(name);

    public void LEngineFolderUpdate(long folderId, string name) =>
        _lDeckOutletEngine.LEngineDeck.LEngineFolderUpdate(folderId, name);

    public void LEngineFolderDelete(long folderId) =>
        _lDeckOutletEngine.LEngineDeck.LEngineFolderDelete(folderId);

    public IReadOnlyList<LCue> LEngineCueRead(long deckId) =>
        _lDeckOutletEngine.LEngineDeck.LEngineCueRead(deckId);

    public int LEngineCueAdd(long deckId, IReadOnlyList<long> entryIds) =>
        _lDeckOutletEngine.LEngineDeck.LEngineCueAdd(deckId, entryIds);

    public long LEngineCueCreate(long deckId, IReadOnlyList<LCueCell> cells) =>
        _lDeckOutletEngine.LEngineDeck.LEngineCueCreate(deckId, cells);

    public void LEngineCueRemove(IReadOnlyList<long> cueIds) =>
        _lDeckOutletEngine.LEngineDeck.LEngineCueRemove(cueIds);

    public void LEngineCueMove(long deckId, long cueId, int position) =>
        _lDeckOutletEngine.LEngineDeck.LEngineCueMove(deckId, cueId, position);

    public void LEngineCueSet(long cueId, int flag, bool excluded) =>
        _lDeckOutletEngine.LEngineDeck.LEngineCueSet(cueId, flag, excluded);

    public LCueFace LEngineFaceRead(long cueId, LPortraitLabel label) =>
        _lDeckOutletEngine.LEngineDeck.LEngineFaceRead(cueId, label);

    public IReadOnlyList<LCueCell> LEngineCellRead(long cueId) =>
        _lDeckOutletEngine.LEngineDeck.LEngineCellRead(cueId);

    public void LEngineCellSave(long cueId, IReadOnlyList<LCueCell> cells) =>
        _lDeckOutletEngine.LEngineDeck.LEngineCellSave(cueId, cells);

    public LStencil LEngineStencilRead(long deckId) =>
        _lDeckOutletEngine.LEngineDeck.LEngineStencilRead(deckId);

    public void LEngineSlotSet(long deckId, LStencilSlot slot) =>
        _lDeckOutletEngine.LEngineDeck.LEngineSlotSet(deckId, slot);

    public void LEngineStencilSave(long deckId, string name) =>
        _lDeckOutletEngine.LEngineDeck.LEngineStencilSave(deckId, name);

    public void LEngineStencilApply(long deckId, long stencilId) =>
        _lDeckOutletEngine.LEngineDeck.LEngineStencilApply(deckId, stencilId);
}
