using System.Windows;

namespace Llyn.UIVeneer;

internal static class PLook
{
    internal static Visibility PLookVisibleRead(bool shown)
    {
        return shown ? Visibility.Visible : Visibility.Collapsed;
    }

    internal static Visibility PLookHiddenRead(bool shown)
    {
        return shown ? Visibility.Visible : Visibility.Hidden;
    }

    internal static bool? PLookCheckedRead(bool chosen)
    {
        return chosen;
    }

    internal static bool PLookCheckedRead(bool? shown)
    {
        return shown == true;
    }

    internal static double PLookOpacityRead(bool active, double full, double faded)
    {
        return active ? full : faded;
    }

    internal static PLookChoice PLookFirstRead<PLookChoice>(bool first, PLookChoice chosen, PLookChoice other)
    {
        return first ? chosen : other;
    }

    internal static Thickness PLookThicknessRead(double left)
    {
        return new Thickness(left, 0, 0, 0);
    }
}
