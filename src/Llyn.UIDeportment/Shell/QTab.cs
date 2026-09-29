using System;
using System.Windows;

namespace Llyn.UIDeportment;

internal sealed record QTab(
    string QTabMode,
    FrameworkElement QTabButton,
    FrameworkElement QTabPanel)
{
    public Action<bool, bool>? QTabVoyage { get; init; }
}
