using Llyn.Conduct;

namespace Llyn.Tests;

internal static class TInterfaceGate
{
    internal static void TQuillAuthorSet(this CDesk desk, string name) => desk.CDeskQuill!.LQuillAuthorSet(name);
}
