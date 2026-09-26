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
        return found as QContractPart ?? throw new InvalidOperationException(
            $"The contract element '{id}' is missing or is not a {typeof(QContractPart).Name}.");
    }

    public static QContractSheet QContractSheetFind<QContractSheet>(string id)
        where QContractSheet : class
    {
        ArgumentException.ThrowIfNullOrEmpty(id);

        object? found = System.Windows.Application.Current?.TryFindResource(id);
        return found as QContractSheet ?? throw new InvalidOperationException(
            $"The contract resource '{id}' is missing or is not a {typeof(QContractSheet).Name}.");
    }
}
