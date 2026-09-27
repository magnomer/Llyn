using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

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

        if (QLook.QLookPartFind<TextBlock>(container, "PScriptCharacter") is TextBlock character)
        {
            character.Text = row.PScriptItemCharacter;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PScriptStyle") is TextBlock style)
        {
            style.Text = row.PScriptItemStyle;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PScriptGloss") is TextBlock gloss)
        {
            gloss.Text = row.PScriptItemGloss;
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PScriptPictures") is ItemsControl pictures)
        {
            pictures.ItemsSource = row.PScriptItemImages;
            QLookItem.QLookItemAttach(pictures, PScriptImage.PScriptImageApply);
        }
    }

    internal static IReadOnlyList<PScriptItem> PScriptItemScan(IReadOnlyList<CScriptGroup> groups)
    {
        ArgumentNullException.ThrowIfNull(groups);

        List<PScriptItem> items = new(groups.Count);
        foreach (CScriptGroup group in groups)
        {
            if (PScriptItemCreate(group) is PScriptItem item)
            {
                items.Add(item);
            }
        }

        return items;
    }

    private static PScriptItem? PScriptItemCreate(CScriptGroup group)
    {
        List<PScriptImage> pictures = [];
        foreach (CScriptImage image in group.CScriptGroupImages)
        {
            if (PScriptImage.PScriptImageCreate(image) is PScriptImage picture)
            {
                pictures.Add(picture);
            }
        }

        return pictures.Count == 0
            ? null
            : new PScriptItem(group.CScriptGroupHeading, group.CScriptGroupStyle, group.CScriptGroupGloss, pictures);
    }
}
