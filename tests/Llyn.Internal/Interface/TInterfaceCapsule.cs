using Llyn.UIDeportment.Capsule;

namespace Llyn.Tests;

internal static class TInterfaceCapsule
{
    internal static LCapsuleContent TCapsuleRead(string root) => new LCapsule().LCapsuleRead(root);

    internal static void TCapsuleSave(string root, LCapsuleContent state)
    {
        new LCapsule().LCapsuleSave(root, state);
    }

    internal static LCapsuleWindow TCapsuleWindowCreate(
        double left, double top, double width, double height, bool maximized) =>
        new(left, top, width, height, maximized);

    internal static LCapsuleColumn TCapsuleColumnCreate(string tab, double? left, double? middle) =>
        new(tab, left, middle);

    internal static LCapsuleContent TCapsuleContentCreate(
        LCapsuleWindow? window, bool linked, params LCapsuleColumn[] columns) =>
        new(window, linked, columns);
}
