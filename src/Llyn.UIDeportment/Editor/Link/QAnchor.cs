using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QAnchor
{
    private const string QAnchorMenuStyle = "Theme.Choice.Filter";

    private const string QAnchorMenuBrush = "Theme.Accent";

    private const string QAnchorMenuMark = " ≈";

    private readonly FrameworkElement _qAnchorSurface;

    private CSoundingAnchor _cSoundingAnchor = null!;

    internal QAnchor(FrameworkElement surface)
    {
        _qAnchorSurface = surface;
        QAnchorPopup.Closed += QAnchorClosedObserve;
        CommandManager.AddPreviewExecutedHandler(
            QContract.QContractFind<StackPanel>(_qAnchorSurface, "PEditorSound"), QAnchorShutRefine);
    }

    private Popup QAnchorPopup => QContract.QContractFind<Popup>(_qAnchorSurface, "PAnchor");

    private TextBlock QAnchorEmpty => QContract.QContractFind<TextBlock>(_qAnchorSurface, "PAnchorEmpty");

    private StackPanel QAnchorList => QContract.QContractFind<StackPanel>(_qAnchorSurface, "PAnchorList");

    internal void QAnchorIntroduce(CSoundingAnchor anchor)
    {
        _cSoundingAnchor = anchor;
    }

    private void QAnchorShutRefine(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Command == QReflexCommand.QReflexCommandAnchor)
        {
            QAnchorPopup.IsOpen = false;
        }
    }

    internal void QAnchorReflexObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is not QReflexItem row || e.OriginalSource is not UIElement anchor)
        {
            return;
        }

        QAnchorRefine(_cSoundingAnchor.CSoundingAnchorOpen(row.QReflexItemId), anchor);
    }

    private void QAnchorRefine(CAnchor menu, UIElement anchor)
    {
        QAnchorList.Children.Clear();
        QAnchorEmpty.Visibility = QLook.QLookVisibleRead(menu.CAnchorEmpty);
        foreach (CAnchorRow item in menu.CAnchorRows)
        {
            CheckBox box = new()
            {
                Style = (Style)QAnchorList.FindResource(QAnchorMenuStyle),
                Tag = item.CAnchorRowId,
                IsChecked = item.CAnchorRowHeld,
                Content = QAnchorLabelBuild(item),
            };
            box.Click += QAnchorTickObserve;
            QAnchorList.Children.Add(box);
        }

        QAnchorPopup.PlacementTarget = anchor;
        QAnchorPopup.IsOpen = true;
    }

    private TextBlock QAnchorLabelBuild(CAnchorRow item)
    {
        TextBlock label = new() { VerticalAlignment = VerticalAlignment.Center };
        label.Inlines.Add(new Run(item.CAnchorRowSummary));
        if (item.CAnchorRowEstimated)
        {
            label.Inlines.Add(new Run(QAnchorMenuMark)
            {
                Foreground = (Brush)QAnchorList.FindResource(QAnchorMenuBrush),
                FontWeight = FontWeights.SemiBold,
            });
        }

        return label;
    }

    private void QAnchorTickObserve(object sender, RoutedEventArgs e)
    {
        if (sender is not CheckBox { Tag: long fanqieId } box)
        {
            return;
        }

        _cSoundingAnchor.CSoundingAnchorSet(fanqieId, QLook.QLookCheckedRead(box.IsChecked));
    }

    private void QAnchorClosedObserve(object? sender, EventArgs e)
    {
        _cSoundingAnchor.CSoundingAnchorClose();
    }
}
