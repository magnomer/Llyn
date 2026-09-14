using System;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PRegister
{
    private readonly string _pRegisterText;
    private readonly bool _pRegisterUnknown;

    internal PRegister(LStateValue text, long id)
    {
        ArgumentNullException.ThrowIfNull(text);

        _pRegisterText = text.LStateValueShow();
        _pRegisterUnknown = text.LStateValueState == LState.LStateUnknown;
        PRegisterId = id;
    }

    public long PRegisterId { get; }

    public string PRegisterText => _pRegisterText;

    public bool PRegisterUnknown => _pRegisterUnknown;

    internal LStateWritten PRegisterTextRead()
    {
        return new LStateWritten(_pRegisterText, _pRegisterUnknown);
    }
}
