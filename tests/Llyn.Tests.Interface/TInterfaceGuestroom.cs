using System;
using Llyn.Guestroom;

namespace Llyn.Tests;

internal static class TInterfaceGuestroom
{
    internal static int TGuestroomRun(string? path, Func<int> console, Func<int> window) =>
        LGuestroom.LGuestroomRun(path, console, window);
}
