using System;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

internal sealed class PRegister
{
    private readonly string _pRegisterText;
    private readonly bool _pRegisterUnreadable;

    internal PRegister(string text)
        : this(LStateValue.LStateValueRead(text), LEngine.LEngineIdentityCreate())
    {
    }

    internal PRegister(LStateValue text, string id)
    {
        ArgumentNullException.ThrowIfNull(text);

        _pRegisterText = text.LStateValueShow();
        _pRegisterUnreadable = text.LStateValueState == LState.LStateUnknown;
        PRegisterId = id;
    }

    public string PRegisterId { get; }

    public string PRegisterText => _pRegisterText;

    public bool PRegisterUnreadable => _pRegisterUnreadable;

    internal LStateValue PRegisterTextRead()
    {
        return LStateValue.LStateValueResolve(_pRegisterText, _pRegisterUnreadable);
    }
}
