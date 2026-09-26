using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

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

    internal static void PFanqieItemApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PFanqieItem row)
        {
            return;
        }

        if (PLook.PLookPartFind<Grid>(container, "PFanqieStem") is Grid stem)
        {
            stem.Visibility = PLook.PLookVisibleRead(row.PFanqieItemStems.Count > 0);
        }

        if (PLook.PLookPartFind<ItemsControl>(container, "PFanqieStems") is ItemsControl stems)
        {
            stems.ItemsSource = row.PFanqieItemStems;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PFanqieCharacter") is TextBlock character)
        {
            character.Text = row.PFanqieItemCharacter;
        }

        if (PLook.PLookPartFind<Border>(container, "PFanqieChip") is Border chip)
        {
            chip.Visibility = row.PFanqieItemBook.Length > 0 ? Visibility.Visible : Visibility.Hidden;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PFanqieBook") is TextBlock book)
        {
            book.Text = row.PFanqieItemBook;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PFanqieSource") is TextBlock source)
        {
            source.Text = row.PFanqieItemSource;
        }

        if (PLook.PLookPartFind<ItemsControl>(container, "PFanqieLines") is ItemsControl lines)
        {
            lines.ItemsSource = row.PFanqieItemLines;
            PLookItem.PLookItemAttach(lines, PFanqieLine.PFanqieRowApply);
        }
    }

    internal static IReadOnlyList<PFanqieItem> PFanqieItemScan(IReadOnlyList<LFanqieGroup> groups)
    {
        ArgumentNullException.ThrowIfNull(groups);

        List<PFanqieItem> items = new(groups.Count);
        foreach (LFanqieGroup group in groups)
        {
            List<PFanqieLine> lines = new(group.LFanqieGroupRows.Count);
            foreach (LFanqieRow row in group.LFanqieGroupRows)
            {
                lines.Add(PFanqieLine.PFanqieLineCreate(row));
            }

            items.Add(new PFanqieItem(
                group.LFanqieGroupHeading,
                group.LFanqieGroupLabel,
                group.LFanqieGroupSource,
                group.LFanqieGroupStems,
                lines));
        }

        return items;
    }
}
