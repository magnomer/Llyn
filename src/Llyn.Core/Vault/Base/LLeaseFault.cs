using System;

namespace Llyn.Core;

public sealed class LLeaseFault : Exception
{
    public LLeaseFault(Exception cause)
        : base(cause?.Message, cause)
    {
        ArgumentNullException.ThrowIfNull(cause);
    }
}
