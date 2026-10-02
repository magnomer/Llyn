using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Llyn.UIDeportment;

public sealed class QBerth : Panel
{
    public static readonly DependencyProperty QBerthAnchorProperty = DependencyProperty.RegisterAttached(
        "QBerthAnchor",
        typeof(object),
        typeof(QBerth),
        new FrameworkPropertyMetadata(
            null,
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

    public static readonly DependencyProperty QBerthEntryProperty = DependencyProperty.RegisterAttached(
        "QBerthEntry",
        typeof(UIElement),
        typeof(QBerth),
        new FrameworkPropertyMetadata(
            null,
            FrameworkPropertyMetadataOptions.AffectsMeasure,
            QBerthEntryRefine));

    private UIElement? _qBerthEntry;

    private UIElement? _qBerthPending;

    internal static ItemsControl QBerthBuild(ItemsControl list, object caret, string anchor)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(caret);

        if (list.GetValue(QBerthEntryProperty) is not ItemsControl entry)
        {
            entry = new ItemsControl { Focusable = false, IsTabStop = false };
            list.SetValue(QBerthEntryProperty, entry);
        }

        entry.ItemTemplateSelector = list.ItemTemplateSelector;
        if (entry.ItemsSource is not object[] held || !ReferenceEquals(held[0], caret))
        {
            entry.ItemsSource = new[] { caret };
        }

        list.SetBinding(QBerthAnchorProperty, new Binding(anchor) { Source = caret });
        return entry;
    }

    protected override int VisualChildrenCount =>
        base.VisualChildrenCount + (_qBerthEntry is null ? 0 : 1);

    protected override IEnumerator LogicalChildren
    {
        get
        {
            List<object> held = [];
            IEnumerator children = base.LogicalChildren;
            while (children.MoveNext())
            {
                held.Add(children.Current);
            }

            if (_qBerthEntry is not null)
            {
                held.Add(_qBerthEntry);
            }

            return held.GetEnumerator();
        }
    }

    protected override Visual GetVisualChild(int index)
    {
        if (_qBerthEntry is null)
        {
            return base.GetVisualChild(index);
        }

        int seat = QBerthSeatRead();
        if (index == seat)
        {
            return _qBerthEntry;
        }

        return base.GetVisualChild(index < seat ? index : index - 1);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        QBerthOwnerAttach();
        QBerthEntryAttach();
        double line = 0;
        double lineHeight = 0;
        double width = 0;
        double height = 0;
        foreach (UIElement child in QBerthOrderRead())
        {
            child.Measure(availableSize);
            Size wanted = child.DesiredSize;
            if (line > 0 && line + wanted.Width > availableSize.Width)
            {
                width = Math.Max(width, line);
                height += lineHeight;
                line = 0;
                lineHeight = 0;
            }

            line += wanted.Width;
            lineHeight = Math.Max(lineHeight, wanted.Height);
        }

        return new Size(Math.Max(width, line), height + lineHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        List<UIElement> row = [];
        double line = 0;
        double lineHeight = 0;
        double top = 0;
        foreach (UIElement child in QBerthOrderRead())
        {
            Size wanted = child.DesiredSize;
            if (line > 0 && line + wanted.Width > finalSize.Width)
            {
                QBerthLineApply(row, top, lineHeight);
                top += lineHeight;
                row.Clear();
                line = 0;
                lineHeight = 0;
            }

            row.Add(child);
            line += wanted.Width;
            lineHeight = Math.Max(lineHeight, wanted.Height);
        }

        QBerthLineApply(row, top, lineHeight);
        return finalSize;
    }

    private static void QBerthLineApply(List<UIElement> row, double top, double height)
    {
        double left = 0;
        foreach (UIElement child in row)
        {
            child.Arrange(new Rect(left, top, child.DesiredSize.Width, height));
            left += child.DesiredSize.Width;
        }
    }

    private static void QBerthEntryRefine(DependencyObject owner, DependencyPropertyChangedEventArgs e)
    {
        if (owner is not QBerth berth)
        {
            return;
        }

        berth.QBerthEntryDetach();
        berth._qBerthPending = e.NewValue as UIElement;
    }

    private void QBerthOwnerAttach()
    {
        if (BindingOperations.IsDataBound(this, QBerthEntryProperty)
            || ItemsControl.GetItemsOwner(this) is not ItemsControl owner)
        {
            return;
        }

        SetBinding(QBerthAnchorProperty, new Binding { Source = owner, Path = new PropertyPath(QBerthAnchorProperty) });
        SetBinding(QBerthEntryProperty, new Binding { Source = owner, Path = new PropertyPath(QBerthEntryProperty) });
    }

    private void QBerthEntryAttach()
    {
        if (_qBerthPending is null || ReferenceEquals(_qBerthPending, _qBerthEntry))
        {
            return;
        }

        if (VisualTreeHelper.GetParent(_qBerthPending) is QBerth prior)
        {
            prior.QBerthEntryDetach();
            prior.InvalidateMeasure();
        }

        _qBerthEntry = _qBerthPending;
        AddVisualChild(_qBerthEntry);
        AddLogicalChild(_qBerthEntry);
    }

    private void QBerthEntryDetach()
    {
        if (_qBerthEntry is null)
        {
            return;
        }

        RemoveVisualChild(_qBerthEntry);
        RemoveLogicalChild(_qBerthEntry);
        _qBerthEntry = null;
    }

    private int QBerthSeatRead()
    {
        UIElementCollection children = InternalChildren;
        object? anchor = GetValue(QBerthAnchorProperty);
        if (anchor is not null && ItemsControl.GetItemsOwner(this) is ItemsControl owner)
        {
            for (int index = 0; index < children.Count; index++)
            {
                if (ReferenceEquals(owner.ItemContainerGenerator.ItemFromContainer(children[index]), anchor))
                {
                    return index;
                }
            }
        }

        return children.Count;
    }

    private IEnumerable<UIElement> QBerthOrderRead()
    {
        UIElementCollection children = InternalChildren;
        int seat = _qBerthEntry is null ? -1 : QBerthSeatRead();
        for (int index = 0; index <= children.Count; index++)
        {
            if (index == seat)
            {
                yield return _qBerthEntry!;
            }

            if (index < children.Count && children[index] is UIElement child)
            {
                yield return child;
            }
        }
    }
}
