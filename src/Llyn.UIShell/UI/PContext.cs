using System;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

internal sealed class PContext
{
    private readonly string _pContextText;
    private readonly bool _pContextUnreadable;

    internal PContext(string text)
        : this(LStateValue.LStateValueRead(text), LEngine.LEngineIdentityCreate())
    {
    }

    internal PContext(LStateValue text, string id)
    {
        ArgumentNullException.ThrowIfNull(text);

        _pContextText = text.LStateValueShow();
        _pContextUnreadable = text.LStateValueState == LState.LStateUnknown;
        PContextId = id;
    }

    public string PContextId { get; }

    public string PContextText => _pContextText;

    public bool PContextUnreadable => _pContextUnreadable;

    internal LStateValue PContextTextRead()
    {
        return LStateValue.LStateValueResolve(_pContextText, _pContextUnreadable);
    }
}
