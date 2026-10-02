using System;
using System.Windows;

namespace Llyn.UIDeportment;

public static class QContract
{
    public static QContractPart QContractFind<QContractPart>(DependencyObject scope, string id)
        where QContractPart : class
    {
        ArgumentNullException.ThrowIfNull(scope);
        ArgumentException.ThrowIfNullOrEmpty(id);

        object? found = scope is FrameworkElement element ? element.FindName(id) : null;
        found ??= LogicalTreeHelper.FindLogicalNode(scope, id);
        return QContractResolve<QContractPart>(found, id, "element");
    }

    public static QContractSheet QContractSheetFind<QContractSheet>(string id)
        where QContractSheet : class
    {
        ArgumentException.ThrowIfNullOrEmpty(id);

        object? found = System.Windows.Application.Current?.TryFindResource(id);
        return QContractResolve<QContractSheet>(found, id, "resource");
    }

    public static QContractSheet QContractSheetFind<QContractSheet>(FrameworkElement scope, string id)
        where QContractSheet : class
    {
        ArgumentNullException.ThrowIfNull(scope);
        ArgumentException.ThrowIfNullOrEmpty(id);

        object? found = scope.TryFindResource(id);
        return QContractResolve<QContractSheet>(found, id, "resource");
    }

    private static QContractPart QContractResolve<QContractPart>(object? found, string id, string kind)
        where QContractPart : class
    {
        return found as QContractPart ?? throw new InvalidOperationException(
            $"The contract {kind} '{id}' is missing or is not a {typeof(QContractPart).Name}.");
    }
}
