using System;
using Llyn.Core;

namespace Llyn.UIDeportment;

internal sealed class PContext
{
    internal PContext(LStateValue text, long id)
    {
        ArgumentNullException.ThrowIfNull(text);

        PContextText = text;
        PContextId = id;
    }

    public long PContextId { get; }

    public LStateValue PContextText { get; }
}
