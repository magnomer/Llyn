using System;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class PContext
{
    internal PContext(CStateWording text, long id)
    {
        ArgumentNullException.ThrowIfNull(text);

        PContextText = text;
        PContextId = id;
    }

    public long PContextId { get; }

    public CStateWording PContextText { get; }
}
