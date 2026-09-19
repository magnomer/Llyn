using System;
using Llyn.Core;

namespace Llyn.UIVeneer;

internal sealed class PRegister
{
    internal PRegister(LStateValue text, long id)
    {
        ArgumentNullException.ThrowIfNull(text);

        PRegisterText = text;
        PRegisterId = id;
    }

    public long PRegisterId { get; }

    public LStateValue PRegisterText { get; }
}
