using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QFanqieItem
{
    private QFanqieItem(
        string character,
        string book,
        string source,
        IReadOnlyList<string> stems,
        IReadOnlyList<QFanqieLine> lines)
    {
        QFanqieItemCharacter = character;
        QFanqieItemBook = book;
        QFanqieItemSource = source;
        QFanqieItemStems = stems;
        QFanqieItemLines = lines;
    }

    public string QFanqieItemCharacter { get; }

    public string QFanqieItemBook { get; }

    public string QFanqieItemSource { get; }

    public IReadOnlyList<string> QFanqieItemStems { get; }

    public IReadOnlyList<QFanqieLine> QFanqieItemLines { get; }

    internal static void QFanqieItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QFanqieItem row)
        {
            return;
        }

        if (QLook.QLookPartFind<Grid>(container, "PFanqieStem") is Grid stem)
        {
            stem.Visibility = QLook.QLookVisibleRead(row.QFanqieItemStems.Count > 0);
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PFanqieStems") is ItemsControl stems)
        {
            stems.ItemsSource = row.QFanqieItemStems;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PFanqieCharacter") is TextBlock character)
        {
            character.Text = row.QFanqieItemCharacter;
        }

        if (QLook.QLookPartFind<Border>(container, "PFanqieChip") is Border chip)
        {
            chip.Visibility = row.QFanqieItemBook.Length > 0 ? Visibility.Visible : Visibility.Hidden;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PFanqieBook") is TextBlock book)
        {
            book.Text = row.QFanqieItemBook;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PFanqieSource") is TextBlock source)
        {
            source.Text = row.QFanqieItemSource;
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "QFanqieLines") is ItemsControl lines)
        {
            lines.ItemsSource = row.QFanqieItemLines;
            QLookItem.QLookItemAttach(lines, QFanqieLine.QFanqieLineRefine);
        }
    }

    internal static IReadOnlyList<QFanqieItem> QFanqieItemScan(IReadOnlyList<CFanqieGroup> groups)
    {
        ArgumentNullException.ThrowIfNull(groups);

        List<QFanqieItem> items = new(groups.Count);
        foreach (CFanqieGroup group in groups)
        {
            List<QFanqieLine> lines = new(group.CFanqieGroupRows.Count);
            foreach (CFanqieRow row in group.CFanqieGroupRows)
            {
                lines.Add(new QFanqieLine(row));
            }

            items.Add(new QFanqieItem(
                group.CFanqieGroupHeading,
                group.CFanqieGroupLabel,
                group.CFanqieGroupSource,
                group.CFanqieGroupStems,
                lines));
        }

        return items;
    }
}
