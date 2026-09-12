using System;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PContext
{
    private readonly string _pContextText;
    private readonly bool _pContextUnknown;

    internal PContext(LStateValue text, long id)
        : this(text, id, LStateValue.LStateValueUnspecified, LStateValue.LStateValueUnspecified)
    {
    }

    internal PContext(LStateValue text, long id, LStateValue description, LStateValue kind)
    {
        ArgumentNullException.ThrowIfNull(text);

        _pContextText = text.LStateValueShow();
        _pContextUnknown = text.LStateValueState == LState.LStateUnknown;
        PContextId = id;
        PContextDescription = description ?? LStateValue.LStateValueUnspecified;
        PContextKind = kind ?? LStateValue.LStateValueUnspecified;
    }

    public long PContextId { get; }

    internal LStateValue PContextDescription { get; }

    internal LStateValue PContextKind { get; }

    public string PContextText => _pContextText;

    public bool PContextUnknown => _pContextUnknown;

    internal LStateValue PContextTextRead()
    {
        return LStateValue.LStateValueResolve(_pContextText, _pContextUnknown);
    }
}
