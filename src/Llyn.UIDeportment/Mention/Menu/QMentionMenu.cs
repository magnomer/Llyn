using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QMentionMenu
{
    private readonly CNavigation _qMentionMenuNavigation;

    private readonly ObservableCollection<PMentionItem> _qMentionItem = [];

    private QMentionAsk? _qMentionMeaningAsk;

    internal QMentionMenu(Window surface, CNavigation navigation)
    {
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(navigation);

        QMentionPopup = (Popup)surface.FindName("PMentionMenu");
        QMentionTitle = (TextBlock)surface.FindName("PMentionTitle");
        QMentionList = (ListBox)surface.FindName("PMentionList");
        _qMentionMenuNavigation = navigation;

        surface.SetValue(PMention.PMentionHostProperty, this);
        QMentionList.ItemsSource = _qMentionItem;
        QLookItem.QLookItemAttach(QMentionList, QMentionRowRefine);
        QMentionPopup.Closed += (_, _) => QMentionMenuHide();
        surface.PreviewKeyDown += QMentionKeyObserve;
        surface.Deactivated += QMentionLeaveRefine;
        navigation.CNavigationArrived += QMentionMenuHide;
    }

    private Popup QMentionPopup { get; }

    private TextBlock QMentionTitle { get; }

    private ListBox QMentionList { get; }

    internal void QMentionOfferRefine(PMention anchor, CMentionOffer? offer)
    {
        ArgumentNullException.ThrowIfNull(anchor);

        if (offer is null)
        {
            return;
        }

        QMentionEntryRefine(anchor, anchor.PMentionPlaceRead(offer.CMentionOfferUnit), offer);
    }

    internal QMentionAsk QMentionMeaningRefine(FrameworkElement anchor, Rect place, CMentionSense sense)
    {
        ArgumentNullException.ThrowIfNull(sense);

        QMentionMenuRefine(
            anchor,
            place,
            sense.CMentionSenseKey,
            PMentionItem.PMentionItemCreate(sense.CMentionSenseRow));
        QMentionAsk ask = new(anchor);
        _qMentionMeaningAsk = ask;
        return ask;
    }

    private void QMentionEntryRefine(FrameworkElement anchor, Rect place, CMentionOffer offer)
    {
        QMentionMenuRefine(
            anchor,
            place,
            offer.CMentionOfferKey,
            PMentionItem.PMentionItemCreate(offer.CMentionOfferEntry));
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

    private void QMentionMenuHide()
    {
        QMentionPopup.IsOpen = false;
        QMentionList.SelectedIndex = -1;
        _qMentionItem.Clear();
        _qMentionMeaningAsk = null;
    }

    private void QMentionMenuRefine(FrameworkElement anchor, Rect place, string key, IReadOnlyList<PMentionItem> rows)
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

        QMentionTitle.SetResourceReference(TextBlock.TextProperty, key);
        QMentionPopup.PlacementTarget = anchor;
        QMentionPopup.HorizontalOffset = place.X;
        QMentionPopup.VerticalOffset = place.Bottom - anchor.ActualHeight;
        QMentionPopup.IsOpen = true;
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

        if (e.Key == Key.Escape && QMentionPopup.IsOpen)
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
        QMentionAsk? ask = _qMentionMeaningAsk;
        QMentionMenuHide();

        if (ask is null)
        {
            _qMentionMenuNavigation.CNavigationEntryOpen(item.PMentionItemEntry);
            return;
        }

        ask.QMentionAskSettle(item.PMentionItemSense);
    }

    private void QMentionLeaveRefine(object? sender, EventArgs e)
    {
        QMentionMenuHide();
    }
}
