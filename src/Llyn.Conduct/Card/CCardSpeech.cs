using System;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CCardSpeech
{
    private readonly CDesk _cCardSpeechDesk;

    private string _cCardSpeechTyped = string.Empty;

    internal CCardSpeech(CDesk desk)
    {
        ArgumentNullException.ThrowIfNull(desk);

        _cCardSpeechDesk = desk;
    }

    private LTenure? CCardSpeechTenure => _cCardSpeechDesk.CDeskFilling ? null : _cCardSpeechDesk.CDeskTenure;

    public CCategory CCardSpeechSet(string typed)
    {
        ArgumentNullException.ThrowIfNull(typed);

        if (CCardSpeechTenure is not LTenure held)
        {
            return new CCategory([], false, false, false);
        }

        _cCardSpeechTyped = typed;
        return LCategoryRead(held.LTenureSpeechSet(typed));
    }

    public void CCardSpeechAdd(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        if (CCardSpeechTenure is not LTenure held)
        {
            return;
        }

        _cCardSpeechTyped = string.Empty;
        held.LTenureSpeechAdd(name);
    }

    public void CCardSpeechRemove(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        CCardSpeechTenure?.LTenureSpeechRemove(name, _cCardSpeechTyped);
    }

    public CMarker CCardSpeechRead()
    {
        if (_cCardSpeechDesk.CDeskTenure is not LTenure held)
        {
            return new CMarker([], _cCardSpeechTyped, new CCategory([], false, false, false));
        }

        (var names, _cCardSpeechTyped, LSpeechOffer found) = held.LTenureSpeechRead(_cCardSpeechTyped);
        return new CMarker(names, _cCardSpeechTyped, LCategoryRead(found));
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
