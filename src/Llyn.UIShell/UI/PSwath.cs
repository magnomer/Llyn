using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Llyn.UIShell;

internal static class PSwath
{
    private const BindingFlags PSwathHidden = BindingFlags.NonPublic | BindingFlags.Instance;

    private static readonly DependencyProperty PSwathHeldProperty = DependencyProperty.RegisterAttached(
        "PSwathHeld",
        typeof(object),
        typeof(PSwath));

    private static readonly Type? PSwathEditorType =
        typeof(TextBlock).Assembly.GetType("System.Windows.Documents.TextEditor");

    private static readonly Type? PSwathContainerType =
        typeof(TextBlock).Assembly.GetType("System.Windows.Documents.ITextContainer");

    private static readonly PropertyInfo? PSwathContainer =
        typeof(TextBlock).GetProperty("TextContainer", PSwathHidden);

    private static readonly PropertyInfo? PSwathContainerView =
        PSwathContainerType?.GetProperty("TextView");

    private static readonly PropertyInfo? PSwathEditorView =
        PSwathEditorType?.GetProperty("TextView", PSwathHidden);

    private static readonly PropertyInfo? PSwathEditorLock =
        PSwathEditorType?.GetProperty("IsReadOnly", PSwathHidden);

    private static readonly MethodInfo? PSwathRegister = PSwathEditorType?.GetMethod(
        "RegisterCommandHandlers",
        BindingFlags.NonPublic | BindingFlags.Static,
        [typeof(Type), typeof(bool), typeof(bool), typeof(bool)]);

    private static readonly bool PSwathReady =
        PSwathEditorType is not null
        && PSwathContainer is not null
        && PSwathContainerView is not null
        && PSwathEditorView is not null
        && PSwathEditorLock is not null
        && PSwathRegister is not null;

    internal static void PSwathHook()
    {
        if (!PSwathReady)
        {
            return;
        }

        PSwathRegister!.Invoke(null, [typeof(TextBlock), true, true, true]);
        EventManager.RegisterClassHandler(
            typeof(TextBlock),
            FrameworkElement.LoadedEvent,
            new RoutedEventHandler(PSwathHandle),
            true);
    }

    private static void PSwathHandle(object sender, RoutedEventArgs e)
    {
        if (sender is TextBlock block
            && block.GetValue(PSwathHeldProperty) is null
            && PSwathDisplayCheck(block))
        {
            PSwathAttach(block);
        }
    }

    private static bool PSwathDisplayCheck(DependencyObject block)
    {
        DependencyObject? node = VisualTreeHelper.GetParent(block);
        while (node is not null)
        {
            switch (node)
            {
                case ButtonBase:
                    return false;
                case PDisplay:
                    return true;
            }

            node = VisualTreeHelper.GetParent(node);
        }

        return false;
    }

    private static void PSwathAttach(TextBlock block)
    {
        object? container = PSwathContainer!.GetValue(block);
        if (container is null)
        {
            return;
        }

        object? editor = Activator.CreateInstance(
            PSwathEditorType!,
            PSwathHidden | BindingFlags.CreateInstance,
            null,
            [container, block, false],
            null);
        if (editor is null)
        {
            return;
        }

        PSwathEditorLock!.SetValue(editor, true);
        PSwathEditorView!.SetValue(editor, PSwathContainerView!.GetValue(container));

        block.Focusable = true;
        block.FocusVisualStyle = null;
        block.SetValue(PSwathHeldProperty, editor);
    }
}
