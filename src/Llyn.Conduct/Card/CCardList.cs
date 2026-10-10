using System;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CCardList
{
    private readonly CDesk _cCardListDesk;

    private readonly LCardPort _cCardListPort;

    private readonly LSettingsPort _cCardListSettings;

    private readonly CEnvoy _cCardListEnvoy;

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

    public void CCardRemove(long cardId)
    {
        CCardListQuill?.LQuillCardRemove(cardId);
    }

    public bool CCardFoldToggle(long cardId, bool folded)
    {
        if (_cCardListDesk.CDeskStoredRead() is not long id)
        {
            return false;
        }

        try
        {
            if (folded)
            {
                _cCardListPort.LEngineFoldSave(id, cardId);
            }
            else
            {
                _cCardListPort.LEngineFoldDelete(id, cardId);
            }

            return true;
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cCardListEnvoy, _cCardListSettings, "Fold.SaveFailed", exception);
        }

        return false;
    }

    public void CCardMove(long cardId, int place)
    {
        if (CCardListQuill is LQuillCard quill
            && _cCardListDesk.CDeskDraft.CDeskDraftTenure?.LTenureRead() is LDraft held
            && CFolio.CFolioPlaceRead(held.LDraftContent, cardId, place) is int target)
        {
            quill.LQuillCardMove(cardId, target);
        }
    }

    public void CCardMove(long cardId, string ordinal)
    {
        ArgumentNullException.ThrowIfNull(ordinal);

        if (_cCardListDesk.CDeskDraft.CDeskDraftTenure?.LTenureRead() is LDraft held
            && CFolio.CFolioOrdinalRead(held.LDraftContent, cardId, ordinal) is int place)
        {
            CCardMove(cardId, place);
        }
    }
}
