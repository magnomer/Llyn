using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Llyn.UIDeportment;

internal static class PLook
{
    internal sealed record PLookSetter(
        string PLookSetterStyle,
        string PLookSetterState,
        string? PLookSetterPart,
        DependencyProperty PLookSetterProperty,
        object PLookSetterValue);

    private static readonly Dictionary<Style, string> PLookStyle = [];

    private static readonly ConditionalWeakTable<
        FrameworkElement,
        Dictionary<DependencyProperty, (object PLookHeldSaved, PLookSetter PLookHeldRow)>> PLookHeld = [];

    private static readonly ConditionalWeakTable<FrameworkElement, List<DependencyPropertyDescriptor>> PLookTrack = [];

    internal static Visibility PLookVisibleRead(bool shown)
    {
        return shown ? Visibility.Visible : Visibility.Collapsed;
    }

    internal static bool? PLookCheckedRead(bool chosen)
    {
        return chosen;
    }

    internal static bool PLookCheckedRead(bool? shown)
    {
        return shown == true;
    }

    internal static PLookChoice PLookFirstRead<PLookChoice>(bool first, PLookChoice chosen, PLookChoice other)
    {
        return first ? chosen : other;
    }

    internal static void PLookPromptApply(TextBlock block, string text, string hint, string? ink)
    {
        ArgumentNullException.ThrowIfNull(block);
        ArgumentNullException.ThrowIfNull(text);

        if (text.Length > 0)
        {
            block.Text = text;
            if (ink is null)
            {
                block.ClearValue(TextBlock.ForegroundProperty);
            }
            else
            {
                block.SetResourceReference(TextBlock.ForegroundProperty, ink);
            }

            return;
        }

        block.SetResourceReference(TextBlock.TextProperty, hint);
        block.SetResourceReference(TextBlock.ForegroundProperty, "Theme.Muted");
    }

    internal static void PLookStateAttach()
    {
        PLookStyleScan(
            System.Windows.Application.Current.Resources,
            PLookSheet.PLookSheetState.Select(row => row.PLookSetterStyle).ToHashSet());

        MouseButtonEventHandler press = (sender, _) => Dispatcher.CurrentDispatcher.BeginInvoke(
            DispatcherPriority.Input, () => PLookStateHandle(sender, EventArgs.Empty));
        KeyboardFocusChangedEventHandler focus = (sender, _) => Dispatcher.CurrentDispatcher.BeginInvoke(
            DispatcherPriority.Input, () => PLookStateHandle(sender, EventArgs.Empty));
        MouseEventHandler release = (sender, _) => Dispatcher.CurrentDispatcher.BeginInvoke(
            DispatcherPriority.Input, () => PLookStateHandle(sender, EventArgs.Empty));

        EventManager.RegisterClassHandler(
            typeof(FrameworkElement), FrameworkElement.LoadedEvent, new RoutedEventHandler(PLookStateHandle), true);
        EventManager.RegisterClassHandler(
            typeof(FrameworkElement), FrameworkElement.UnloadedEvent, new RoutedEventHandler(PLookStateDetach), true);
        EventManager.RegisterClassHandler(
            typeof(FrameworkElement), UIElement.MouseEnterEvent, new MouseEventHandler(PLookStateHandle), true);
        EventManager.RegisterClassHandler(
            typeof(FrameworkElement), UIElement.MouseLeaveEvent, new MouseEventHandler(PLookStateHandle), true);
        EventManager.RegisterClassHandler(typeof(ButtonBase), UIElement.MouseDownEvent, press, true);
        EventManager.RegisterClassHandler(typeof(ButtonBase), UIElement.MouseUpEvent, press, true);
        EventManager.RegisterClassHandler(typeof(ButtonBase), UIElement.LostMouseCaptureEvent, release, true);
        EventManager.RegisterClassHandler(typeof(FrameworkElement), Keyboard.GotKeyboardFocusEvent, focus, true);
        EventManager.RegisterClassHandler(typeof(FrameworkElement), Keyboard.LostKeyboardFocusEvent, focus, true);
        EventManager.RegisterClassHandler(
            typeof(ToggleButton), ToggleButton.CheckedEvent, new RoutedEventHandler(PLookStateHandle), true);
        EventManager.RegisterClassHandler(
            typeof(ToggleButton), ToggleButton.UncheckedEvent, new RoutedEventHandler(PLookStateHandle), true);
    }

    internal static void PLookStyleAttach(ResourceDictionary dictionary)
    {
        ArgumentNullException.ThrowIfNull(dictionary);

        PLookStyleScan(dictionary, PLookSheet.PLookSheetState.Select(row => row.PLookSetterStyle).ToHashSet());
    }

    internal static PLookPart? PLookPartFind<PLookPart>(FrameworkElement container, string name)
        where PLookPart : class
    {
        ArgumentNullException.ThrowIfNull(container);

        container.ApplyTemplate();
        if (container is Control control && control.Template?.FindName(name, control) is PLookPart part)
        {
            return part;
        }

        Queue<DependencyObject> pending = new([container]);
        while (pending.Count > 0)
        {
            DependencyObject node = pending.Dequeue();
            if (node is ContentPresenter { ContentTemplate: not null } presenter)
            {
                presenter.ApplyTemplate();
                return presenter.ContentTemplate.FindName(name, presenter) as PLookPart;
            }

            for (int index = 0; index < VisualTreeHelper.GetChildrenCount(node); index++)
            {
                pending.Enqueue(VisualTreeHelper.GetChild(node, index));
            }
        }

        return null;
    }

    private static void PLookStateHandle(object? sender, EventArgs e)
    {
        if (sender is not FrameworkElement element)
        {
            return;
        }

        List<string> styles = [];
        for (Style? level = element.Style; level is not null; level = level.BasedOn)
        {
            if (PLookStyle.TryGetValue(level, out string? key))
            {
                styles.Insert(0, key);
            }
        }

        if (styles.Count == 0)
        {
            styles.Add(element.GetType().Name);
        }

        List<PLookSetter> rows = styles
            .SelectMany(style => PLookSheet.PLookSheetState.Where(row => row.PLookSetterStyle == style))
            .ToList();
        if (rows.Count == 0)
        {
            return;
        }

        if (!PLookHeld.TryGetValue(element, out _))
        {
            PLookHeld.Add(element, []);
            element.IsEnabledChanged += (target, _) => PLookStateHandle(target, EventArgs.Empty);
        }

        if (element.IsLoaded && !PLookTrack.TryGetValue(element, out _))
        {
            List<DependencyPropertyDescriptor> tracked = [];
            DependencyProperty[] watches =
            [
                FrameworkElement.TagProperty, TextBlock.TextProperty, ContentControl.ContentProperty,
                PTab.PTabChosenProperty, ComboBox.TextProperty,
                ComboBoxItem.IsHighlightedProperty, Thumb.IsDraggingProperty,
                ItemsControl.HasItemsProperty, ListBoxItem.IsSelectedProperty,
                Image.SourceProperty, ComboBox.IsDropDownOpenProperty,
            ];
            foreach (DependencyProperty watched in watches)
            {
                if (watched.OwnerType.IsInstanceOfType(element)
                    && DependencyPropertyDescriptor.FromProperty(watched, element.GetType()) is { } descriptor)
                {
                    descriptor.AddValueChanged(element, PLookStateHandle);
                    tracked.Add(descriptor);
                }
            }

            PLookTrack.Add(element, tracked);
        }

        HashSet<string> active = ["Base"];
        if (element.IsMouseOver)
        {
            active.Add("Hover");
        }

        if (element is ButtonBase { IsPressed: true })
        {
            active.Add("Press");
        }

        if (element.IsKeyboardFocusWithin)
        {
            active.Add("Focus");
        }

        if (element is ToggleButton { IsChecked: true })
        {
            active.Add("Check");
        }

        if (!element.IsEnabled)
        {
            active.Add("Disabled");
        }

        if (element is ScrollBar { Orientation: Orientation.Horizontal })
        {
            active.Add("Horizontal");
        }

        if (element is TextBlock { Text.Length: 0 } or ComboBox { Text.Length: 0 } or Image { Source: null }
            or ContentControl { Content: string { Length: 0 } }
            || element is ItemsControl { HasItems: false } and not ComboBox)
        {
            active.Add("Empty");
        }

        if (element is ComboBox { IsDropDownOpen: true })
        {
            active.Add("Open");
        }

        if (element is ComboBoxItem { IsHighlighted: true })
        {
            active.Add("Highlight");
        }

        if (element is ListBoxItem { IsSelected: true })
        {
            active.Add("Selected");
        }

        if (element is Thumb { IsDragging: true })
        {
            active.Add("Drag");
        }

        if (element.Tag is null)
        {
            active.Add("Bare");
        }

        if (element.Tag is string tag)
        {
            active.Add(tag);
        }

        if (element is ContentControl { Content: null })
        {
            active.Add("Mute");
        }

        if (element is PTab { PTabChosen: true })
        {
            active.Add("Chosen");
        }

        Dictionary<(string?, DependencyProperty), PLookSetter?> winners = [];
        foreach (PLookSetter row in rows)
        {
            (string?, DependencyProperty) slot = (row.PLookSetterPart, row.PLookSetterProperty);
            if (row.PLookSetterState.Split('+').All(active.Contains))
            {
                winners[slot] = row;
            }
            else
            {
                winners.TryAdd(slot, null);
            }
        }

        foreach (((string? part, DependencyProperty property), PLookSetter? winner) in winners)
        {
            FrameworkElement? target = part is null
                ? element
                : (element as Control)?.Template?.FindName(part, (Control)element) as FrameworkElement
                  ?? LogicalTreeHelper.FindLogicalNode(element, part) as FrameworkElement
                  ?? PLookPartFind<FrameworkElement>(element, part);
            if (target is null)
            {
                continue;
            }

            Dictionary<DependencyProperty, (object PLookHeldSaved, PLookSetter PLookHeldRow)> held =
                PLookHeld.GetValue(target, _ => []);
            if (winner is null)
            {
                if (held.Remove(property, out (object PLookHeldSaved, PLookSetter PLookHeldRow) prior))
                {
                    if (prior.PLookHeldRow.PLookSetterValue is AnimationTimeline)
                    {
                        target.RenderTransform.BeginAnimation(property, null);
                    }
                    else if (prior.PLookHeldSaved is BindingExpressionBase expression)
                    {
                        BindingOperations.SetBinding(target, property, expression.ParentBindingBase);
                    }
                    else if (prior.PLookHeldSaved == DependencyProperty.UnsetValue)
                    {
                        target.ClearValue(property);
                    }
                    else
                    {
                        target.SetValue(property, prior.PLookHeldSaved);
                    }
                }

                continue;
            }

            if (held.TryGetValue(property, out (object PLookHeldSaved, PLookSetter PLookHeldRow) current)
                && current.PLookHeldRow == winner)
            {
                continue;
            }

            object saved = held.TryGetValue(property, out current)
                ? current.PLookHeldSaved
                : target.ReadLocalValue(property);
            held[property] = (saved, winner);
            switch (winner.PLookSetterValue)
            {
                case DependencyProperty source:
                    PLookPartCopy(element, target, source, property);
                    break;
                case string key:
                    target.SetResourceReference(property, key);
                    break;
                case AnimationTimeline motion:
                    Transform shift = target.RenderTransform.CloneCurrentValue();
                    target.RenderTransform = shift;
                    shift.BeginAnimation(property, motion);
                    break;
                default:
                    target.SetValue(property, winner.PLookSetterValue);
                    break;
            }
        }
    }

    private static void PLookStateDetach(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement element
            || !PLookTrack.TryGetValue(element, out List<DependencyPropertyDescriptor>? tracked))
        {
            return;
        }

        foreach (DependencyPropertyDescriptor descriptor in tracked)
        {
            descriptor.RemoveValueChanged(element, PLookStateHandle);
        }

        PLookTrack.Remove(element);
    }

    private static void PLookStyleScan(ResourceDictionary dictionary, HashSet<string> keys)
    {
        foreach (string key in keys)
        {
            if (dictionary.Contains(key) && dictionary[key] is Style style)
            {
                PLookStyle[style] = key;
            }
        }

        foreach (ResourceDictionary merged in dictionary.MergedDictionaries)
        {
            PLookStyleScan(merged, keys);
        }
    }

    private static void PLookPartCopy(
        FrameworkElement control, FrameworkElement part, DependencyProperty source, DependencyProperty property)
    {
        part.SetBinding(property, new Binding { Source = control, Path = new PropertyPath(source) });
    }
}
