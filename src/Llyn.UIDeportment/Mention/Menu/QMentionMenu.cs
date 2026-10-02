using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public partial class QWindow
{
    private readonly ObservableCollection<PMentionItem> _qMentionItem = [];

    private Action<PMentionItem> _qMentionChosen = static _ => { };

    private Popup QMentionMenu => (Popup)_qWindowSurface.FindName("PMentionMenu");

    private TextBlock QMentionTitle => (TextBlock)_qWindowSurface.FindName("PMentionTitle");

    private ListBox QMentionList => (ListBox)_qWindowSurface.FindName("PMentionList");

    internal void QMentionOfferRefine(FrameworkElement anchor, Rect place, CMentionOffer offer)
    {
        ArgumentNullException.ThrowIfNull(offer);

        QMentionMenuRefine(
            anchor,
            place,
            offer.CMentionOfferKey,
            PMentionItem.PMentionItemCreate(offer.CMentionOfferEntry),
            item => _cNavigation.CNavigationEntryOpen(item.PMentionItemEntry));
    }

    internal void QMentionMeaningRefine(
        FrameworkElement anchor, Rect place, CMentionSense sense, Action<FrameworkElement, long> chosen)
    {
        ArgumentNullException.ThrowIfNull(sense);
        ArgumentNullException.ThrowIfNull(chosen);

        QMentionMenuRefine(
            anchor,
            place,
            sense.CMentionSenseKey,
            PMentionItem.PMentionItemCreate(sense.CMentionSenseRow),
            item => chosen(anchor, item.PMentionItemSense));
    }

    private void QMentionRowRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not PMentionItem mention)
        {
            return;
        }

        if (QLook.QLookPartFind<Grid>(container, "PMentionRow") is Grid row)
        {
            row.Margin = mention.PMentionItemIndent;
            row.MouseLeftButtonUp -= QMentionMenuObserve;
            row.MouseLeftButtonUp += QMentionMenuObserve;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PMentionName") is TextBlock name)
        {
            name.Text = mention.PMentionItemName;
        }

        if (QLook.QLookPartFind<Image>(container, "PMentionFlag") is Image flag)
        {
            flag.Source = mention.PMentionItemFlag;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PMentionLanguage") is TextBlock language)
        {
            language.Text = mention.PMentionItemLanguage;
        }

        if (QLook.QLookPartFind<StackPanel>(container, "PMentionOrigin") is StackPanel origin)
        {
            origin.Visibility = QLook.QLookVisibleRead(mention.PMentionItemLanguage.Length > 0);
        }
    }

    private void QMentionMenuObserve(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PMentionItem item })
        {
            QMentionMenuHide();
            return;
        }

        QMentionMenuSelect(item);
        e.Handled = true;
    }

    internal void QMentionMenuHide()
    {
        QMentionMenu.IsOpen = false;
        QMentionList.SelectedIndex = -1;
        _qMentionItem.Clear();
    }

    private void QMentionMenuRefine(
        FrameworkElement anchor,
        Rect place,
        string key,
        IReadOnlyList<PMentionItem> rows,
        Action<PMentionItem> chosen)
    {
        ArgumentNullException.ThrowIfNull(anchor);

        QMentionMenuHide();
        foreach (PMentionItem row in rows)
        {
            _qMentionItem.Add(row);
        }

        if (_qMentionItem.Count == 0)
        {
            return;
        }

        _qMentionChosen = chosen;
        QMentionTitle.SetResourceReference(TextBlock.TextProperty, key);
        QMentionMenu.PlacementTarget = anchor;
        QMentionMenu.HorizontalOffset = place.X;
        QMentionMenu.VerticalOffset = place.Bottom - anchor.ActualHeight;
        QMentionMenu.IsOpen = true;
        QMentionList.SelectedIndex = 0;
    }

    private void QMentionKeyObserve(object sender, KeyEventArgs e)
    {
        int? lit = e.Key switch
        {
            Key.Down => CLantern.CLanternMove(QMentionList.SelectedIndex, QMentionList.Items.Count, 1),
            Key.Up => CLantern.CLanternMove(QMentionList.SelectedIndex, QMentionList.Items.Count, -1),
            _ => null,
        };
        if (lit is int chosen)
        {
            QMentionList.SelectedIndex = chosen;
            QMentionList.ScrollIntoView(QMentionList.SelectedItem);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Escape && QMentionMenu.IsOpen)
        {
            QMentionMenuHide();
            e.Handled = true;
        }

        if (e.Key != Key.Enter)
        {
            return;
        }

        if (QMentionList.SelectedItem is not PMentionItem item)
        {
            return;
        }

        QMentionMenuSelect(item);
        e.Handled = true;
    }

    private void QMentionMenuSelect(PMentionItem item)
    {
        QMentionMenuHide();
        _qMentionChosen(item);
    }
}
