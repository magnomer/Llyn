using System;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PRegister
{
    private readonly string _pRegisterText;
    private readonly bool _pRegisterUnreadable;

    internal PRegister(LStateValue text, long id)
        : this(text, id, string.Empty)
    {
    }

    internal PRegister(LStateValue text, long id, string language)
    {
        ArgumentNullException.ThrowIfNull(text);

        _pRegisterText = text.LStateValueShow();
        _pRegisterUnreadable = text.LStateValueState == LState.LStateUnknown;
        PRegisterId = id;
        PRegisterLanguage = language ?? string.Empty;
    }

    public long PRegisterId { get; internal set; }

    internal string PRegisterLanguage { get; }

    public string PRegisterText => _pRegisterText;

    public bool PRegisterUnreadable => _pRegisterUnreadable;

    internal LStateValue PRegisterTextRead()
    {
        return LStateValue.LStateValueResolve(_pRegisterText, _pRegisterUnreadable);
    }
}
