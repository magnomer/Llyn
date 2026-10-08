using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QChoiceOrder
{
    private readonly UserControl _qChoiceOrderScope;

    private CAperture _qChoiceOrderAperture = null!;

    internal QChoiceOrder(UserControl choice, FrameworkElement bar)
    {
        ArgumentNullException.ThrowIfNull(choice);
        ArgumentNullException.ThrowIfNull(bar);

        _qChoiceOrderScope = choice;

        QChoice.QChoiceDropperAttach(QChoiceOrderDropper, QChoiceOrderDropdown, bar);
        QChoiceOrderIcon.QIconSource = QIcon.QIconResolve("sort", 24);
    }

    private ToggleButton QChoiceOrderDropper =>
        QContract.QContractFind<ToggleButton>(_qChoiceOrderScope, "PChoiceOrderDropper");

    private QIconImage QChoiceOrderIcon => QContract.QContractFind<QIconImage>(_qChoiceOrderScope, "PChoiceOrderIcon");

    private Popup QChoiceOrderDropdown => QContract.QContractFind<Popup>(_qChoiceOrderScope, "PChoiceOrderDropdown");

    private StackPanel QChoiceOrderList => QContract.QContractFind<StackPanel>(_qChoiceOrderScope, "PChoiceOrderList");

    internal void QChoiceOrderIntroduce(CAperture aperture, string title, IReadOnlyList<CCatalogOrder> orders)
    {
        ArgumentNullException.ThrowIfNull(aperture);
        ArgumentNullException.ThrowIfNull(title);

        _qChoiceOrderAperture = aperture;
        QChoiceOrderDropper.SetResourceReference(FrameworkElement.ToolTipProperty, title + ".Title");
        QChoice.QChoiceOrderBuild(QChoiceOrderList, title, QChoiceOrderObserve, orders);
    }

    internal void QChoiceOrderRefine()
    {
        QChoice.QChoiceOrderApply(QChoiceOrderDropdown, _qChoiceOrderAperture.CApertureOrder);
    }

    internal void QChoiceOrderClose()
    {
        QChoiceOrderDropdown.IsOpen = false;
    }

    private void QChoiceOrderObserve(object sender, RoutedEventArgs e)
    {
        _qChoiceOrderAperture.CApertureOrderSet(QChoice.QChoiceOrderRead(sender));
        QChoiceOrderDropper.IsChecked = false;
    }
}
