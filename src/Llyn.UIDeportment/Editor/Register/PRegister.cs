using System;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class PRegister
{
    internal PRegister(CStateWording text, long id)
    {
        ArgumentNullException.ThrowIfNull(text);

        PRegisterText = text;
        PRegisterId = id;
    }

    public long PRegisterId { get; }

    public CStateWording PRegisterText { get; }
}
