using System;
using Llyn.Core;

namespace Llyn.UIVeneer;

internal sealed class PContext
{
    internal PContext(LStateValue text, long id)
        : this(text, id, LStateValue.LStateValueUnspecified, LStateValue.LStateValueUnspecified)
    {
    }

    internal PContext(LStateValue text, long id, LStateValue description, LStateValue kind)
    {
        ArgumentNullException.ThrowIfNull(text);

        PContextText = text;
        PContextId = id;
        PContextDescription = description ?? LStateValue.LStateValueUnspecified;
        PContextKind = kind ?? LStateValue.LStateValueUnspecified;
    }

    public long PContextId { get; }

    internal LStateValue PContextDescription { get; }

    internal LStateValue PContextKind { get; }

    public LStateValue PContextText { get; }
}
