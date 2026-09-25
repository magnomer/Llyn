using System;
using System.Windows;

namespace Llyn.UIDeportment;

public sealed record LTab(
    string LTabMode,
    FrameworkElement LTabButton,
    FrameworkElement LTabPanel)
{
    public Func<bool>? LTabAllowed { get; init; }

    public Func<bool>? LTabLeave { get; init; }

    public Action<bool>? LTabScribe { get; init; }

    public Func<long>? LTabStation { get; init; }

    public Action<long>? LTabArrival { get; init; }

    public Action<bool, bool>? LTabVoyage { get; init; }
}
