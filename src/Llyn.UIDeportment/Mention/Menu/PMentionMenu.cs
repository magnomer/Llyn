using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIDeportment;

public partial class PWindow
{
    private readonly ObservableCollection<PMentionItem> _pMentionItem = [];

    private readonly PMentionMenuTemplate _pMentionMenuTemplate;

    private Action<PMentionItem>? _pMentionChosen;

    private Popup PMentionMenu => (Popup)_pWindowSurface.FindName(nameof(PMentionMenu));

    private TextBlock PMentionTitle => (TextBlock)_pWindowSurface.FindName(nameof(PMentionTitle));

    private ListBox PMentionList => (ListBox)_pWindowSurface.FindName(nameof(PMentionList));

    internal void PMentionMenuShow(FrameworkElement anchor, Rect place, LMentionResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        PMentionMenuShow(
            anchor,
            place,
            "Mention.Title",
            PMentionItem.PMentionItemCreate(result.LMentionResultEntry),
            item => PWindowEntryShow(item.PMentionItemEntry));
    }

    internal void PMentionMenuShow(
        FrameworkElement anchor, Rect place, long entryId, IReadOnlyList<LMeaning> meanings, Action<long> chosen)
    {
        ArgumentNullException.ThrowIfNull(meanings);
        ArgumentNullException.ThrowIfNull(chosen);

        PMentionMenuShow(
            anchor,
            place,
            "Mention.Sense",
            PMentionItem.PMentionItemCreate(
                entryId,
                LWindow.LWindowMeaningSort(meanings, PLocalizationCatalog.PLocalizationTextRead("Display.Unknown")),
                PLocalizationCatalog.PLocalizationTextRead("Mention.Whole")),
            item => chosen(item.PMentionItemSense));
    }

    private void PMentionRowApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PMentionItem mention)
        {
            return;
        }

        if (PLook.PLookPartFind<Grid>(container, "PMentionRow") is Grid row)
        {
            row.Margin = mention.PMentionItemIndent;
            row.MouseLeftButtonUp -= _pMentionMenuTemplate.PMentionMenuHandle;
            row.MouseLeftButtonUp += _pMentionMenuTemplate.PMentionMenuHandle;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PMentionName") is TextBlock name)
        {
            name.Text = mention.PMentionItemName;
        }

        if (PLook.PLookPartFind<Image>(container, "PMentionFlag") is Image flag)
        {
            flag.Source = mention.PMentionItemFlag;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PMentionLanguage") is TextBlock language)
        {
            language.Text = mention.PMentionItemLanguage;
        }

        if (PLook.PLookPartFind<StackPanel>(container, "PMentionOrigin") is StackPanel origin)
        {
            origin.Visibility = PLook.PLookVisibleRead(mention.PMentionItemLanguage.Length > 0);
        }
    }

    internal void PMentionMenuHandle(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PMentionItem item })
        {
            PMentionMenuHide();
            return;
        }

        PMentionMenuSelect(item);
        e.Handled = true;
    }

    internal void PMentionMenuHide()
    {
        PMentionMenu.IsOpen = false;
        PMentionList.SelectedIndex = -1;
        _pMentionItem.Clear();
        _pMentionChosen = null;
    }

    private void PMentionMenuShow(
        FrameworkElement anchor,
        Rect place,
        string key,
        IReadOnlyList<PMentionItem> rows,
        Action<PMentionItem> chosen)
    {
        ArgumentNullException.ThrowIfNull(anchor);

        PMentionMenuHide();
        foreach (PMentionItem row in rows)
        {
            _pMentionItem.Add(row);
        }

        if (_pMentionItem.Count == 0)
        {
            return;
        }

        _pMentionChosen = chosen;
        PMentionTitle.SetResourceReference(TextBlock.TextProperty, key);
        PMentionMenu.PlacementTarget = anchor;
        PMentionMenu.HorizontalOffset = place.X;
        PMentionMenu.VerticalOffset = place.Bottom - anchor.ActualHeight;
        PMentionMenu.IsOpen = true;
        PMentionList.SelectedIndex = 0;
    }

    private void PMentionKeyHandle(object sender, KeyEventArgs e)
    {
        if (PMentionMenu.IsOpen && PMentionMenuHandle(e.Key))
        {
            e.Handled = true;
        }
    }

    private bool PMentionMenuHandle(Key key)
    {
        if (key == Key.Escape)
        {
            PMentionMenuHide();
            return true;
        }

        if (key == Key.Down || key == Key.Up)
        {
            int count = _pMentionItem.Count;
            if (count == 0)
            {
                return false;
            }

            int step = key == Key.Down ? 1 : count - 1;
            int chosen = PMentionList.SelectedIndex < 0
                ? (key == Key.Down ? count - 1 : 0)
                : PMentionList.SelectedIndex;
            PMentionList.SelectedIndex = (chosen + step) % count;
            PMentionList.ScrollIntoView(PMentionList.SelectedItem);
            return true;
        }

        if (key == Key.Enter && PMentionList.SelectedItem is PMentionItem item)
        {
            PMentionMenuSelect(item);
            return true;
        }

        return false;
    }

    private void PMentionMenuSelect(PMentionItem item)
    {
        Action<PMentionItem>? chosen = _pMentionChosen;
        PMentionMenuHide();
        chosen?.Invoke(item);
    }
}
