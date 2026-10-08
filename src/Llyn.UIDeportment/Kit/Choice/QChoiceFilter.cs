using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QChoiceFilter
{
    private readonly UserControl _qChoiceFilterScope;

    private CAperture _qChoiceFilterAperture = null!;

    internal QChoiceFilter(UserControl choice)
    {
        ArgumentNullException.ThrowIfNull(choice);

        _qChoiceFilterScope = choice;

        QChoice.QChoiceDropperAttach(QChoiceFilterDropper, QChoiceFilterDropdown, QChoiceFilterDropper);
        QChoiceFilterIcon.QIconSource = QIcon.QIconResolve("filter", 24);
    }

    private ToggleButton QChoiceFilterDropper =>
        QContract.QContractFind<ToggleButton>(_qChoiceFilterScope, "PChoiceFilterDropper");

    private QIconImage QChoiceFilterIcon =>
        QContract.QContractFind<QIconImage>(_qChoiceFilterScope, "PChoiceFilterIcon");

    private FrameworkElement QChoiceFilterMark =>
        QContract.QContractFind<FrameworkElement>(_qChoiceFilterScope, "PChoiceFilterMark");

    private Popup QChoiceFilterDropdown => QContract.QContractFind<Popup>(_qChoiceFilterScope, "PChoiceFilterDropdown");

    private StackPanel QChoiceFilterList =>
        QContract.QContractFind<StackPanel>(_qChoiceFilterScope, "PChoiceFilterList");

    internal void QChoiceFilterIntroduce(CAperture aperture, string title)
    {
        ArgumentNullException.ThrowIfNull(aperture);
        ArgumentNullException.ThrowIfNull(title);

        _qChoiceFilterAperture = aperture;
        QChoiceFilterDropper.SetResourceReference(FrameworkElement.ToolTipProperty, title + ".Title");
    }

    internal void QChoiceFilterRefine()
    {
        QChoiceFilterMark.Visibility = QLook.QLookVisibleRead(_qChoiceFilterAperture.CApertureFiltered);
    }

    internal void QChoiceFilterBuild(IReadOnlyList<string> languages)
    {
        QChoice.QChoiceFilterBuild(
            QChoiceFilterList, languages, _qChoiceFilterAperture.CApertureFilter, QChoiceFilterObserve);
    }

    internal void QChoiceFilterBuild(IReadOnlyList<CReferenceKind> kinds)
    {
        QChoice.QChoiceKindRefine(
            QChoiceFilterList, _qChoiceFilterAperture.CApertureFilter, QChoiceFilterObserve, kinds);
    }

    internal void QChoiceFilterClose()
    {
        QChoiceFilterDropdown.IsOpen = false;
    }

    private void QChoiceFilterObserve(object sender, RoutedEventArgs e)
    {
        _qChoiceFilterAperture.CApertureFilterSet(QChoice.QChoiceFilterRead(sender));
        QChoiceFilterRefine();
    }
}
