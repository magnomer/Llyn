using System.Windows;

namespace Llyn.UIVeneer;

public partial class PWindow
{
    internal void PWindowStemShow(string language, string key)
    {
        if (!PXiesheng.PXieshengLeaveConfirm())
        {
            return;
        }

        PNavigationHandle(PNavigationXiesheng, new RoutedEventArgs());
        PXiesheng.PXieshengStemShow(language, key);
    }
}
