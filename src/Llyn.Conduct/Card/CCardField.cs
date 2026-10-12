using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CCardField
{
    private readonly CDesk _cCardFieldDesk;

    private readonly LDraftPort _cCardFieldPort;

    private readonly LSettingsPort _cCardFieldSettings;

    private readonly CEnvoy _cCardFieldEnvoy;

    internal CCardField(CDesk desk, LDraftPort drafts, LSettingsPort settings, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(settings);

        _cCardFieldDesk = desk;
        _cCardFieldPort = drafts;
        _cCardFieldSettings = settings;
        _cCardFieldEnvoy = envoy;
    }

    private LQuillCard? CCardFieldQuill =>
        !_cCardFieldDesk.CDeskDraft.CDeskDraftFilling && _cCardFieldDesk.CDeskDraft.CDeskDraftTenure is LTenure held
            ? new LQuillCard(held)
            : null;

    public void CCardTitleSet(long cardId, string text)
    {
        CCardFieldQuill?.LCardTitleSet(cardId, text);
    }

    public void CCardExpressionSet(long cardId, string text)
    {
        CCardFieldQuill?.LCardExpressionSet(cardId, text);
    }

    public void CCardMeaningSet(long cardId, string text)
    {
        CCardFieldQuill?.LCardMeaningSet(cardId, text);
    }

    public void CCardSenseAttach(long cardId, long senseId)
    {
        CCardFieldQuill?.LCardSenseAttach(cardId, senseId);
    }

    public void CCardSenseDetach(long cardId, long senseId)
    {
        CCardFieldQuill?.LCardSenseDetach(cardId, senseId);
    }

    public IReadOnlyList<CMeaning> CCardSenseRead()
    {
        if (_cCardFieldDesk.CDeskDraft.CDeskDraftTenure is not LTenure held)
        {
            return [];
        }

        try
        {
            return _cCardFieldPort.LEngineMeaningScan(held, "Display.Unknown")
                .Select(static row => new CMeaning(
                    row.LMeaningId, row.LMeaningName, row.LMeaningDepth, row.LMeaningPath))
                .ToList();
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cCardFieldEnvoy, _cCardFieldSettings, "Input.LoadFailed", exception);
            return [];
        }
    }
}
