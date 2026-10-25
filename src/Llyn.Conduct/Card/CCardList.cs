using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CCardList
{
    private readonly CDesk _cCardListDesk;

    private readonly LCardPort _cCardListPort;

    private readonly LSettingsPort _cCardListSettings;

    private readonly CEnvoy _cCardListEnvoy;

    private readonly HashSet<(bool, long)> _cCardListMoving = [];

    internal CCardList(CDesk desk, LCardPort cards, LSettingsPort settings, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(cards);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(envoy);

        _cCardListDesk = desk;
        _cCardListPort = cards;
        _cCardListSettings = settings;
        _cCardListEnvoy = envoy;
    }

    private LQuillCard? CCardListQuill =>
        !_cCardListDesk.CDeskDraft.CDeskDraftFilling && _cCardListDesk.CDeskDraft.CDeskDraftTenure is LTenure held
            ? new LQuillCard(held)
            : null;

    public void CCardMeaningAdd()
    {
        CCardListQuill?.LQuillCardAdd(LCardKind.LCardKindMeaning);
    }

    public void CCardCollocationAdd()
    {
        CCardListQuill?.LQuillCardAdd(LCardKind.LCardKindCollocation);
    }

    public void CCardRemove(bool collocated, long cardId)
    {
        CCardListQuill?.LQuillCardRemove(
            collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning, cardId);
    }

    public bool CCardFoldToggle(bool collocated, long cardId, bool folded)
    {
        if (_cCardListDesk.CDeskStoredRead() is not long id)
        {
            return false;
        }

        LCardKind kind = collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning;
        try
        {
            if (folded)
            {
                _cCardListPort.LEngineFoldSave(id, kind, cardId);
            }
            else
            {
                _cCardListPort.LEngineFoldDelete(id, kind, cardId);
            }

            return true;
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cCardListEnvoy, _cCardListSettings, "Fold.SaveFailed", exception);
        }

        return false;
    }

    public void CCardMove(bool collocated, long cardId, int place)
    {
        if (CCardListQuill is LQuillCard quill
            && _cCardListDesk.CDeskDraft.CDeskDraftTenure?.LTenureRead() is LDraft held
            && CFolio.CFolioPlaceRead(held.LDraftContent, collocated, place) is int target)
        {
            quill.LQuillCardMove(
                collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning, cardId, target);
        }
    }

    public void CCardMove(bool collocated, long cardId, string ordinal)
    {
        ArgumentNullException.ThrowIfNull(ordinal);

        if (_cCardListDesk.CDeskDraft.CDeskDraftTenure?.LTenureRead() is LDraft held
            && CFolio.CFolioOrdinalRead(held.LDraftContent, collocated, ordinal) is int place)
        {
            CCardMove(collocated, cardId, place);
        }
    }

    public bool CCardPositionOpen(bool collocated, long cardId)
    {
        if (_cCardListDesk.CDeskDraft.CDeskDraftTenure?.LTenureRead() is not LDraft held
            || CFolio.CFolioCountRead(held.LDraftContent, collocated) <= 1)
        {
            return false;
        }

        _cCardListMoving.Add((collocated, cardId));
        return true;
    }

    public void CCardPositionClose(bool collocated, long cardId)
    {
        _cCardListMoving.Remove((collocated, cardId));
    }

    public bool CCardPositionSettle(bool collocated, long cardId, string ordinal)
    {
        ArgumentNullException.ThrowIfNull(ordinal);

        if (!_cCardListMoving.Remove((collocated, cardId)))
        {
            return false;
        }

        CCardMove(collocated, cardId, ordinal);
        return true;
    }
}
