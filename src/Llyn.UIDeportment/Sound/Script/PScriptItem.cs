using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIDeportment;

internal sealed class PScriptItem
{
    private PScriptItem(string character, string style, string gloss, IReadOnlyList<PScriptImage> images)
    {
        PScriptItemCharacter = character;
        PScriptItemStyle = style;
        PScriptItemGloss = gloss;
        PScriptItemImages = images;
    }

    public string PScriptItemCharacter { get; }

    public string PScriptItemStyle { get; }

    public string PScriptItemGloss { get; }

    public IReadOnlyList<PScriptImage> PScriptItemImages { get; }

    internal static void PScriptItemApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PScriptItem row)
        {
            return;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PScriptCharacter") is TextBlock character)
        {
            character.Text = row.PScriptItemCharacter;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PScriptStyle") is TextBlock style)
        {
            style.Text = row.PScriptItemStyle;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PScriptGloss") is TextBlock gloss)
        {
            gloss.Text = row.PScriptItemGloss;
        }

        if (PLook.PLookPartFind<ItemsControl>(container, "PScriptPictures") is ItemsControl pictures)
        {
            pictures.ItemsSource = row.PScriptItemImages;
            PLookItem.PLookItemAttach(pictures, PScriptImage.PScriptImageApply);
        }
    }

    internal static IReadOnlyList<PScriptItem> PScriptItemScan(IReadOnlyList<LScriptGroup> groups)
    {
        ArgumentNullException.ThrowIfNull(groups);

        List<PScriptItem> items = new(groups.Count);
        foreach (LScriptGroup group in groups)
        {
            if (PScriptItemCreate(group) is PScriptItem item)
            {
                items.Add(item);
            }
        }

        return items;
    }

    private static PScriptItem? PScriptItemCreate(LScriptGroup group)
    {
        List<PScriptImage> pictures = [];
        foreach (LScriptImage image in group.LScriptGroupImages)
        {
            if (PScriptImage.PScriptImageCreate(image) is PScriptImage picture)
            {
                pictures.Add(picture);
            }
        }

        return pictures.Count == 0
            ? null
            : new PScriptItem(group.LScriptGroupHeading, group.LScriptGroupStyle, group.LScriptGroupGloss, pictures);
    }
}
