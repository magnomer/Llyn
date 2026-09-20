using System;

namespace Llyn.Core;

public sealed class LVaultFault : Exception
{
    public LVaultFault(Exception cause)
        : base(cause?.Message, cause)
    {
        ArgumentNullException.ThrowIfNull(cause);
    }
}
