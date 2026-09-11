using System.Threading;

namespace Llyn.ShellEngine;

public static class LIdentity
{
    private static long _lIdentityIssued;

    public static long LIdentityCreate()
    {
        return Interlocked.Decrement(ref _lIdentityIssued);
    }

    public static bool LIdentityTemporary(long id)
    {
        return id <= 0;
    }
}
