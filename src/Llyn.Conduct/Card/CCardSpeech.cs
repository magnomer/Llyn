using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CCardSpeech
{
    private readonly CDesk _cCardSpeechDesk;

    private readonly LEntryPort _cCardSpeechPort;

    private string _cCardSpeechTyped = string.Empty;

    internal CCardSpeech(CDesk desk, LEntryPort entries)
    {
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(entries);

        _cCardSpeechDesk = desk;
        _cCardSpeechPort = entries;
    }

    private LTenure? CCardSpeechTenure => _cCardSpeechDesk.CDeskFilling ? null : _cCardSpeechDesk.CDeskTenure;

    public CCategory CCardSpeechSet(string typed)
    {
        ArgumentNullException.ThrowIfNull(typed);

        if (CCardSpeechTenure is null || _cCardSpeechDesk.CDeskSpeech is not LQuillSpeech speech)
        {
            return new CCategory([], false, false, false);
        }

        _cCardSpeechTyped = typed;
        return LCategoryRead(speech.LQuillSpeechSet(typed));
    }

    public void CCardSpeechAdd(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        if (CCardSpeechTenure is null || _cCardSpeechDesk.CDeskSpeech is not LQuillSpeech speech)
        {
            return;
        }

        _cCardSpeechTyped = string.Empty;
        speech.LQuillSpeechAdd(name);
    }

    public void CCardSpeechRemove(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        if (CCardSpeechTenure is not null)
        {
            _cCardSpeechDesk.CDeskSpeech?.LQuillSpeechRemove(name, _cCardSpeechTyped);
        }
    }

    public CMarker CCardSpeechRead()
    {
        if (_cCardSpeechDesk.CDeskTenure is not LTenure held
            || _cCardSpeechDesk.CDeskSpeech is not LQuillSpeech speech)
        {
            return new CMarker([], _cCardSpeechTyped, new CCategory([], false, false, false), []);
        }

        (var names, _cCardSpeechTyped, LSpeechOffer found) = speech.LQuillSpeechRead(_cCardSpeechTyped);
        LQuillEntry entry = new(held);
        return new CMarker(
            names,
            _cCardSpeechTyped,
            LCategoryRead(found),
            LUnitRowScan(entry.LQuillUnitScan(), entry.LQuillUnitRead()));
    }

    internal static LUnit LUnitRowParse(string key)
    {
        return key switch
        {
            "Unit.Content" => LUnit.LUnitContent,
            "Unit.Function" => LUnit.LUnitFunction,
            "Unit.Morpheme" => LUnit.LUnitMorpheme,
            "Unit.Word" => LUnit.LUnitWord,
            _ => throw new ArgumentOutOfRangeException(nameof(key), key, null),
        };
    }

    private IReadOnlyList<(string, bool)> LUnitRowScan(IReadOnlyList<LUnit> units, LUnit taken)
    {
        return units.Select(unit => (_cCardSpeechPort.LEngineUnitFormat(unit), unit == taken)).ToList();
    }

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
