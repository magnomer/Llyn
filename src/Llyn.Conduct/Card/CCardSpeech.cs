using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CCardSpeech
{
    private readonly CDesk _cCardSpeechDesk;

    private readonly CLedgerNoticed _cCardSpeechNoticed;

    private string _cCardSpeechTyped = string.Empty;

    internal CCardSpeech(CDesk desk, CLedgerNoticed noticed)
    {
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(noticed);

        _cCardSpeechDesk = desk;
        _cCardSpeechNoticed = noticed;
    }

    private LTenure? CCardSpeechTenure =>
        _cCardSpeechDesk.CDeskDraft.CDeskDraftFilling ? null : _cCardSpeechDesk.CDeskDraft.CDeskDraftTenure;

    public CCategory CCardSpeechSet(string typed)
    {
        ArgumentNullException.ThrowIfNull(typed);

        if (CCardSpeechTenure is null || _cCardSpeechDesk.CDeskDraft.CDeskDraftSpeech is not LQuillSpeech speech)
        {
            return new CCategory([], false, false, false);
        }

        _cCardSpeechTyped = typed;
        return LCategoryRead(_cCardSpeechDesk.LDeskRepaintRead(
            _cCardSpeechNoticed,
            () => speech.LQuillSpeechSet(typed),
            LSpeechEmpty,
            ".FindFailed"));
    }

    public void CCardSpeechAdd(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        if (CCardSpeechTenure is null || _cCardSpeechDesk.CDeskDraft.CDeskDraftSpeech is not LQuillSpeech speech)
        {
            return;
        }

        _cCardSpeechTyped = string.Empty;
        try
        {
            speech.LQuillSpeechAdd(name);
        }
        catch (Exception exception)
        {
            _cCardSpeechDesk.LDeskFailureShow(".SpeechFailed", exception);
        }
    }

    public void CCardSpeechRemove(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        if (CCardSpeechTenure is not null)
        {
            _cCardSpeechDesk.CDeskDraft.CDeskDraftSpeech?.LQuillSpeechRemove(name, _cCardSpeechTyped);
        }
    }

    public CMarker CCardSpeechRead()
    {
        if (_cCardSpeechDesk.CDeskDraft.CDeskDraftTenure is not LTenure held
            || _cCardSpeechDesk.CDeskDraft.CDeskDraftSpeech is not LQuillSpeech speech)
        {
            return new CMarker([], _cCardSpeechTyped, new CCategory([], false, false, false), [], []);
        }

        string typed = _cCardSpeechTyped;
        (var names, _cCardSpeechTyped, LSpeechOffer found) = _cCardSpeechDesk.LDeskRepaintRead(
            _cCardSpeechNoticed,
            () => speech.LQuillSpeechRead(typed),
            (Array.Empty<string>(), typed, LSpeechEmpty),
            ".FindFailed");
        LQuillEntry entry = new(held);
        return new CMarker(
            names,
            _cCardSpeechTyped,
            LCategoryRead(found),
            LUnitRowScan(entry.LQuillUnitScan(), entry.LQuillUnitRead()),
            LFreedomRowScan(entry.LQuillFreedomScan(), entry.LQuillFreedomRead()));
    }

    internal static LUnit LUnitRowParse(string key)
    {
        return key switch
        {
            "Unit.Morpheme" => LUnit.LUnitMorpheme,
            "Unit.Word" => LUnit.LUnitWord,
            "Unit.Expression" => LUnit.LUnitExpression,
            _ => throw new ArgumentOutOfRangeException(nameof(key), key, null),
        };
    }

    internal static LFreedom LFreedomRowParse(string key)
    {
        return key switch
        {
            "Freedom.Dependent" => LFreedom.LFreedomDependent,
            "Freedom.Standalone" => LFreedom.LFreedomStandalone,
            _ => throw new ArgumentOutOfRangeException(nameof(key), key, null),
        };
    }

    private static IReadOnlyList<(string, bool)> LUnitRowScan(IReadOnlyList<LUnit> units, LUnit taken)
    {
        return units.Select(unit => (LEntryPort.LEngineUnitFormat(unit), unit == taken)).ToList();
    }

    private static IReadOnlyList<(string, bool)> LFreedomRowScan(IReadOnlyList<LFreedom> freedoms, LFreedom taken)
    {
        return freedoms
            .Select(freedom => (LEntryPort.LEngineFreedomFormat(freedom), freedom == taken))
            .ToList();
    }

    private static readonly LSpeechOffer LSpeechEmpty = new([], false, false, false);

    private static CCategory LCategoryRead(LSpeechOffer offer)
    {
        return new CCategory(
            offer.LSpeechOfferRows
                .Select(static row => new CCategoryRow(row.LSpeechRowName, row.LSpeechRowTaken))
                .ToList(),
            offer.LSpeechOfferDeclared,
            offer.LSpeechOfferMatched,
            offer.LSpeechOfferShown);
    }
}
