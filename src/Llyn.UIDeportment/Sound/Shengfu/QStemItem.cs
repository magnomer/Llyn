using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QStemItem
{
    private QStemItem(CStemMember member)
    {
        QStemItemCharacter = member.CStemMemberCharacter;
        QStemItemReading = member.CStemMemberReading;
        QStemItemReflexes = member.CStemMemberReflexes.Select(static row => new QReflexItem(row)).ToList();
        QStemItemOpened = member.CStemMemberOpened;
        QStemItemFoldable = member.CStemMemberFoldable;
    }

    public string QStemItemCharacter { get; }

    public string QStemItemReading { get; }

    public IReadOnlyList<QReflexItem> QStemItemReflexes { get; }

    public bool QStemItemOpened { get; }

    public bool QStemItemFoldable { get; }

    internal static IReadOnlyList<QStemItem> QStemItemBuild(IReadOnlyList<CStemMember> members)
    {
        ArgumentNullException.ThrowIfNull(members);

        return members.Select(static member => new QStemItem(member)).ToList();
    }

    internal static void QStemItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QStemItem member)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PStemMemberGlyph") is Button glyph)
        {
            glyph.DataContext = member.QStemItemCharacter;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PStemMemberReading") is TextBlock reading)
        {
            reading.Text = member.QStemItemReading;
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PStemMemberReflex") is ItemsControl reflex)
        {
            reflex.ItemsSource = member.QStemItemReflexes;
            reflex.Visibility = QLook.QLookVisibleRead(member.QStemItemOpened);
            QLookItem.QLookItemAttach(reflex, QReflexItem.QReflexItemRefine);
        }

        if (QLook.QLookPartFind<ToggleButton>(container, "PStemMemberHinge") is ToggleButton hinge)
        {
            hinge.IsChecked = member.QStemItemOpened;
            hinge.Visibility = QLook.QLookVisibleRead(member.QStemItemFoldable);
        }
    }
}
