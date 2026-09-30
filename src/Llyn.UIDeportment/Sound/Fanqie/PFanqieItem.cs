using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class PFanqieItem
{
    private PFanqieItem(
        string character,
        string book,
        string source,
        IReadOnlyList<string> stems,
        IReadOnlyList<PFanqieLine> lines)
    {
        PFanqieItemCharacter = character;
        PFanqieItemBook = book;
        PFanqieItemSource = source;
        PFanqieItemStems = stems;
        PFanqieItemLines = lines;
    }

    public string PFanqieItemCharacter { get; }

    public string PFanqieItemBook { get; }

    public string PFanqieItemSource { get; }

    public IReadOnlyList<string> PFanqieItemStems { get; }

    public IReadOnlyList<PFanqieLine> PFanqieItemLines { get; }

    internal static void PFanqieItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not PFanqieItem row)
        {
            return;
        }

        if (QLook.QLookPartFind<Grid>(container, "PFanqieStem") is Grid stem)
        {
            stem.Visibility = QLook.QLookVisibleRead(row.PFanqieItemStems.Count > 0);
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PFanqieStems") is ItemsControl stems)
        {
            stems.ItemsSource = row.PFanqieItemStems;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PFanqieCharacter") is TextBlock character)
        {
            character.Text = row.PFanqieItemCharacter;
        }

        if (QLook.QLookPartFind<Border>(container, "PFanqieChip") is Border chip)
        {
            chip.Visibility = row.PFanqieItemBook.Length > 0 ? Visibility.Visible : Visibility.Hidden;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PFanqieBook") is TextBlock book)
        {
            book.Text = row.PFanqieItemBook;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PFanqieSource") is TextBlock source)
        {
            source.Text = row.PFanqieItemSource;
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PFanqieLines") is ItemsControl lines)
        {
            lines.ItemsSource = row.PFanqieItemLines;
            QLookItem.QLookItemAttach(lines, PFanqieLine.PFanqieRowRefine);
        }
    }

    internal static IReadOnlyList<PFanqieItem> PFanqieItemScan(IReadOnlyList<CFanqieGroup> groups)
    {
        ArgumentNullException.ThrowIfNull(groups);

        List<PFanqieItem> items = new(groups.Count);
        foreach (CFanqieGroup group in groups)
        {
            List<PFanqieLine> lines = new(group.CFanqieGroupRows.Count);
            foreach (CFanqieRow row in group.CFanqieGroupRows)
            {
                lines.Add(new PFanqieLine(row));
            }

            items.Add(new PFanqieItem(
                group.CFanqieGroupHeading,
                group.CFanqieGroupLabel,
                group.CFanqieGroupSource,
                group.CFanqieGroupStems,
                lines));
        }

        return items;
    }
}
