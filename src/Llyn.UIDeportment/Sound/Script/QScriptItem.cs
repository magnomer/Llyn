using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QScriptItem
{
    private QScriptItem(string character, string style, string gloss, IReadOnlyList<QScriptImage> images)
    {
        QScriptItemCharacter = character;
        QScriptItemStyle = style;
        QScriptItemGloss = gloss;
        QScriptItemImages = images;
    }

    public string QScriptItemCharacter { get; }

    public string QScriptItemStyle { get; }

    public string QScriptItemGloss { get; }

    public IReadOnlyList<QScriptImage> QScriptItemImages { get; }

    internal static void QScriptItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QScriptItem row)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PScriptCharacter") is TextBlock character)
        {
            character.Text = row.QScriptItemCharacter;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PScriptStyle") is TextBlock style)
        {
            style.Text = row.QScriptItemStyle;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PScriptGloss") is TextBlock gloss)
        {
            gloss.Text = row.QScriptItemGloss;
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PScriptPictures") is ItemsControl pictures)
        {
            pictures.ItemsSource = row.QScriptItemImages;
            QLookItem.QLookItemAttach(pictures, QScriptImage.QScriptImageRefine);
        }
    }

    internal static IReadOnlyList<QScriptItem> QScriptItemScan(
        IReadOnlyList<CScriptGroup> groups, Action<Exception> failure)
    {
        ArgumentNullException.ThrowIfNull(groups);
        ArgumentNullException.ThrowIfNull(failure);

        bool shown = false;
        Action<Exception> once = exception =>
        {
            if (shown)
            {
                return;
            }

            shown = true;
            failure(exception);
        };
        List<QScriptItem> items = new(groups.Count);
        foreach (CScriptGroup group in groups)
        {
            if (QScriptItemCreate(group, once) is QScriptItem item)
            {
                items.Add(item);
            }
        }

        return items;
    }

    private static QScriptItem? QScriptItemCreate(CScriptGroup group, Action<Exception> failure)
    {
        List<QScriptImage> pictures = [];
        foreach (CScriptImage image in group.CScriptGroupImages)
        {
            if (QScriptImage.QScriptImageCreate(image, failure) is QScriptImage picture)
            {
                pictures.Add(picture);
            }
        }

        return pictures.Count == 0
            ? null
            : new QScriptItem(group.CScriptGroupHeading, group.CScriptGroupStyle, group.CScriptGroupGloss, pictures);
    }
}
