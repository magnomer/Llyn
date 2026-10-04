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

internal static class QLook
{
    internal sealed record QLookSetter(
        string QLookSetterStyle,
        QLookCue QLookSetterCue,
        string? QLookSetterPart,
        DependencyProperty QLookSetterProperty,
        QLookValue QLookSetterValue);

    internal static readonly DependencyProperty QLookCueProperty = DependencyProperty.RegisterAttached(
        "QLookCue",
        typeof(QLookCue),
        typeof(QLook),
        new FrameworkPropertyMetadata(QLookCue.QLookCueBase, (sender, _) => QLookStateRefine(sender, EventArgs.Empty)));

    internal static readonly DependencyProperty QLookIconProperty = DependencyProperty.RegisterAttached(
        "QLookIcon",
        typeof(ImageSource),
        typeof(QLook),
        new FrameworkPropertyMetadata(null, (sender, _) => QLookStateRefine(sender, EventArgs.Empty)));

    private static readonly DependencyProperty QLookReachProperty = DependencyProperty.RegisterAttached(
        "QLookReach",
        typeof(bool),
        typeof(QLook),
        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, QLookReachRefine));

    private static readonly RoutedEventHandler QLookBroadcast = (_, _) => { };

    private static readonly Dictionary<Style, string> QLookStyle = [];

    private static readonly ConditionalWeakTable<FrameworkElement, Dictionary<DependencyProperty, object>> QLookHeld =
        [];

    private static readonly ConditionalWeakTable<FrameworkElement, List<DependencyPropertyDescriptor>> QLookTrack = [];

    internal static Visibility QLookVisibleRead(bool shown)
    {
        return shown ? Visibility.Visible : Visibility.Collapsed;
    }

    internal static bool QLookCheckedRead(bool? shown)
    {
        return shown == true;
    }

    internal static QLookChoice QLookFirstRead<QLookChoice>(bool first, QLookChoice chosen, QLookChoice other)
    {
        return first ? chosen : other;
    }

    internal static string QLookEpithetRead(string epithet)
    {
        return "\u2002" + epithet;
    }

    internal static void QLookPromptApply(TextBlock block, string text, string hint, string? ink)
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

    internal static void QLookStateAttach()
    {
        QLookStyleScan(
            System.Windows.Application.Current.Resources,
            QLookSheet.QLookSheetState.Select(row => row.QLookSetterStyle).ToHashSet());

        MouseButtonEventHandler press = (sender, _) => Dispatcher.CurrentDispatcher.BeginInvoke(
            DispatcherPriority.Input, () => QLookStateRefine(sender, EventArgs.Empty));
        KeyboardFocusChangedEventHandler focus = (sender, _) => Dispatcher.CurrentDispatcher.BeginInvoke(
            DispatcherPriority.Input, () => QLookStateRefine(sender, EventArgs.Empty));
        MouseEventHandler release = (sender, _) => Dispatcher.CurrentDispatcher.BeginInvoke(
            DispatcherPriority.Input, () => QLookStateRefine(sender, EventArgs.Empty));

        EventManager.RegisterClassHandler(
            typeof(FrameworkElement), FrameworkElement.LoadedEvent, new RoutedEventHandler(QLookStateRefine), true);
        EventManager.RegisterClassHandler(
            typeof(FrameworkElement), FrameworkElement.UnloadedEvent, new RoutedEventHandler(QLookStateTeardown), true);
        EventManager.RegisterClassHandler(
            typeof(FrameworkElement), UIElement.MouseEnterEvent, new MouseEventHandler(QLookStateRefine), true);
        EventManager.RegisterClassHandler(
            typeof(FrameworkElement), UIElement.MouseLeaveEvent, new MouseEventHandler(QLookStateRefine), true);
        EventManager.RegisterClassHandler(typeof(ButtonBase), UIElement.MouseDownEvent, press, true);
        EventManager.RegisterClassHandler(typeof(ButtonBase), UIElement.MouseUpEvent, press, true);
        EventManager.RegisterClassHandler(typeof(ButtonBase), UIElement.LostMouseCaptureEvent, release, true);
        EventManager.RegisterClassHandler(typeof(FrameworkElement), Keyboard.GotKeyboardFocusEvent, focus, true);
        EventManager.RegisterClassHandler(typeof(FrameworkElement), Keyboard.LostKeyboardFocusEvent, focus, true);
        EventManager.RegisterClassHandler(
            typeof(ToggleButton), ToggleButton.CheckedEvent, new RoutedEventHandler(QLookStateRefine), true);
        EventManager.RegisterClassHandler(
            typeof(ToggleButton), ToggleButton.UncheckedEvent, new RoutedEventHandler(QLookStateRefine), true);
    }

    internal static void QLookStyleAttach(FrameworkElement surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        QLookStyleScan(surface.Resources, QLookSheet.QLookSheetState.Select(row => row.QLookSetterStyle).ToHashSet());
        surface.SetValue(QLookReachProperty, true);
    }

    internal static QLookPart? QLookPartFind<QLookPart>(FrameworkElement container, string name)
        where QLookPart : class
    {
        ArgumentNullException.ThrowIfNull(container);

        container.ApplyTemplate();
        if (container is Control control && control.Template?.FindName(name, control) is QLookPart part)
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
                return presenter.ContentTemplate.FindName(name, presenter) as QLookPart;
            }

            for (int index = 0; index < VisualTreeHelper.GetChildrenCount(node); index++)
            {
                pending.Enqueue(VisualTreeHelper.GetChild(node, index));
            }
        }

        return null;
    }

    private static void QLookStateRefine(object? sender, EventArgs e)
    {
        if (sender is not FrameworkElement element)
        {
            return;
        }

        List<string> styles = [];
        for (Style? level = element.Style; level is not null; level = level.BasedOn)
        {
            if (QLookStyle.TryGetValue(level, out string? key))
            {
                styles.Insert(0, key);
            }
        }

        if (styles.Count == 0)
        {
            styles.Add(element.GetType().Name);
        }

        List<QLookSetter> rows = styles
            .SelectMany(style => QLookSheet.QLookSheetState.Where(row => row.QLookSetterStyle == style))
            .ToList();
        if (rows.Count == 0)
        {
            return;
        }

        if (!QLookHeld.TryGetValue(element, out _))
        {
            QLookHeld.Add(element, []);
            element.IsEnabledChanged += (target, _) => QLookStateRefine(target, EventArgs.Empty);
        }

        if (element.IsLoaded && !QLookTrack.TryGetValue(element, out _))
        {
            List<DependencyPropertyDescriptor> tracked = [];
            DependencyProperty[] watches =
            [
                TextBlock.TextProperty, ContentControl.ContentProperty,
                ComboBox.TextProperty, ComboBoxItem.IsHighlightedProperty,
                Thumb.IsDraggingProperty, ItemsControl.HasItemsProperty,
                ListBoxItem.IsSelectedProperty, Image.SourceProperty,
                ComboBox.IsDropDownOpenProperty,
            ];
            foreach (DependencyProperty watched in watches)
            {
                if (watched.OwnerType.IsInstanceOfType(element)
                    && DependencyPropertyDescriptor.FromProperty(watched, element.GetType()) is { } descriptor)
                {
                    descriptor.AddValueChanged(element, QLookStateRefine);
                    tracked.Add(descriptor);
                }
            }

            QLookTrack.Add(element, tracked);
        }

        QLookCue active = (QLookCue)element.GetValue(QLookCueProperty);
        active |= QLookCueRead(element.IsMouseOver, QLookCue.QLookCueHover);
        active |= QLookCueRead(element is ButtonBase { IsPressed: true }, QLookCue.QLookCuePress);
        active |= QLookCueRead(element.IsKeyboardFocusWithin, QLookCue.QLookCueFocus);
        active |= QLookCueRead(element is ToggleButton { IsChecked: true }, QLookCue.QLookCueChecked);
        active |= QLookCueRead(!element.IsEnabled, QLookCue.QLookCueDisabled);
        active |= QLookCueRead(
            element is ScrollBar { Orientation: Orientation.Horizontal }, QLookCue.QLookCueHorizontal);
        active |= QLookCueRead(
            element is TextBlock { Text.Length: 0 } or ComboBox { Text.Length: 0 } or Image { Source: null }
                or ContentControl { Content: string { Length: 0 } }
            || element is ItemsControl { HasItems: false } and not ComboBox,
            QLookCue.QLookCueEmpty);
        active |= QLookCueRead(element is ComboBox { IsDropDownOpen: true }, QLookCue.QLookCueOpened);
        active |= QLookCueRead(element is ComboBoxItem { IsHighlighted: true }, QLookCue.QLookCueHighlight);
        active |= QLookCueRead(element is ListBoxItem { IsSelected: true }, QLookCue.QLookCueSelected);
        active |= QLookCueRead(element is Thumb { IsDragging: true }, QLookCue.QLookCueDrag);
        active |= QLookCueRead(element.GetValue(QLookIconProperty) is null, QLookCue.QLookCueBare);
        active |= QLookCueRead(element is ContentControl { Content: null }, QLookCue.QLookCueMute);

        Dictionary<(string?, DependencyProperty), QLookSetter?> winners = [];
        foreach (QLookSetter row in rows)
        {
            (string?, DependencyProperty) slot = (row.QLookSetterPart, row.QLookSetterProperty);
            if ((active & row.QLookSetterCue) == row.QLookSetterCue)
            {
                winners[slot] = row;
            }
            else
            {
                winners.TryAdd(slot, null);
            }
        }

        foreach (((string? part, DependencyProperty property), QLookSetter? winner) in winners)
        {
            FrameworkElement? target = part is null
                ? element
                : (element as Control)?.Template?.FindName(part, (Control)element) as FrameworkElement
                  ?? LogicalTreeHelper.FindLogicalNode(element, part) as FrameworkElement
                  ?? QLookPartFind<FrameworkElement>(element, part);
            if (target is null)
            {
                continue;
            }

            Dictionary<DependencyProperty, object> held = QLookHeld.GetValue(target, _ => []);
            if (winner is null)
            {
                if (held.Remove(property, out object? saved))
                {
                    rows.First(row => row.QLookSetterPart == part && row.QLookSetterProperty == property)
                        .QLookSetterValue.QLookValueClear(target, property, saved);
                }

                continue;
            }

            held.TryAdd(property, target.ReadLocalValue(property));
            winner.QLookSetterValue.QLookValueApply(element, target, property);
        }
    }

    private static QLookCue QLookCueRead(bool shown, QLookCue cue)
    {
        return shown ? cue : QLookCue.QLookCueBase;
    }

    private static void QLookStateTeardown(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement element
            || !QLookTrack.TryGetValue(element, out List<DependencyPropertyDescriptor>? tracked))
        {
            return;
        }

        foreach (DependencyPropertyDescriptor descriptor in tracked)
        {
            descriptor.RemoveValueChanged(element, QLookStateRefine);
        }

        QLookTrack.Remove(element);
    }

    private static void QLookReachRefine(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not FrameworkElement element || e.NewValue is not true)
        {
            return;
        }

        element.Loaded -= QLookBroadcast;
        element.Loaded += QLookBroadcast;
        element.Unloaded -= QLookBroadcast;
        element.Unloaded += QLookBroadcast;
        if (element.IsLoaded)
        {
            QLookStateRefine(element, EventArgs.Empty);
        }
    }

    private static void QLookStyleScan(ResourceDictionary dictionary, HashSet<string> keys)
    {
        foreach (string key in keys)
        {
            if (dictionary.Contains(key) && dictionary[key] is Style style)
            {
                QLookStyle[style] = key;
            }
        }

        foreach (ResourceDictionary merged in dictionary.MergedDictionaries)
        {
            QLookStyleScan(merged, keys);
        }
    }
}
