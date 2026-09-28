using System;
using System.Windows;

namespace Llyn.UIDeportment;

public sealed record LTab(
    string LTabMode,
    FrameworkElement LTabButton,
    FrameworkElement LTabPanel)
{
    public Action<bool, bool>? LTabVoyage { get; init; }
}
