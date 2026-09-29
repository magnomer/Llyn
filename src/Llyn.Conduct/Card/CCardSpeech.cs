using System;
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

    public bool CCardSpeechSet(string typed)
    {
        ArgumentNullException.ThrowIfNull(typed);

        if (CCardSpeechTenure is not LTenure held)
        {
            return false;
        }

        _cCardSpeechTyped = typed;
        return held.LTenureSpeechSet(typed);
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
            return new CMarker([], _cCardSpeechTyped);
        }

        (var names, _cCardSpeechTyped) = held.LTenureSpeechRead(_cCardSpeechTyped);
        return new CMarker(names, _cCardSpeechTyped);
    }
}
