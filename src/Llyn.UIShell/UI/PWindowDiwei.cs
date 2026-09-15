using System.Windows;

namespace Llyn.UIShell;

public partial class PWindow
{
    internal void PWindowDiweiShow(string language, string kind, string key)
    {
        if (!PYunjing.PYunjingLeaveConfirm())
        {
            return;
        }

        PNavigationHandle(PNavigationYunjing, new RoutedEventArgs());
        PYunjing.PYunjingDiweiShow(language, kind, key);
    }
}
