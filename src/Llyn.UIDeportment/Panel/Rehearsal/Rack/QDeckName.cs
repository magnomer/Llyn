using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Llyn.UIDeportment;

internal sealed class QDeckName
{
    private Action<long, string> _qDeckNameGate = null!;

    internal void QDeckNameRefine(FrameworkElement container)
    {
        if (QLook.QLookPartFind<TextBox>(container, "PDeckName") is TextBox name)
        {
            name.LostFocus -= QDeckLeaveObserve;
            name.Visibility = Visibility.Collapsed;
            name.KeyDown -= QDeckNameObserve;
            name.KeyDown += QDeckNameObserve;
        }
    }

    internal void QDeckNameOpen(object sender, Action<long, string> gate)
    {
        if (sender is not MenuItem { Parent: ContextMenu { PlacementTarget: FrameworkElement container } }
            || QLook.QLookPartFind<TextBox>(container, "PDeckName") is not TextBox name
            || QLook.QLookPartFind<TextBlock>(container, "PDeckText") is not TextBlock text)
        {
            return;
        }

        _qDeckNameGate = gate;
        name.Text = text.Text;
        text.Visibility = Visibility.Collapsed;
        if (QLook.QLookPartFind<TextBlock>(container, "PDeckCount") is TextBlock count)
        {
            count.Visibility = Visibility.Collapsed;
        }

        name.Visibility = Visibility.Visible;
        name.LostFocus -= QDeckLeaveObserve;
        name.LostFocus += QDeckLeaveObserve;
        name.Focus();
        name.SelectAll();
    }

    private void QDeckNameSave(TextBox name)
    {
        QDeckItem? deck = QSender.QSenderItemRead<QDeckItem>(name);
        try
        {
            if (deck is not null)
            {
                _qDeckNameGate(deck.QDeckItemId, name.Text);
            }
        }
        finally
        {
            QDeckNameRestore(name);
        }
    }

    private void QDeckNameRestore(TextBox name)
    {
        name.LostFocus -= QDeckLeaveObserve;
        name.Visibility = Visibility.Collapsed;
        if (name.Parent is not Panel row)
        {
            return;
        }

        foreach (UIElement part in row.Children)
        {
            if (part is TextBlock text)
            {
                if (text.Name == "PDeckText")
                {
                    name.Text = text.Text;
                }

                text.Visibility = Visibility.Visible;
            }
        }
    }

    private void QDeckNameObserve(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox name)
        {
            return;
        }

        if (e.Key == Key.Enter)
        {
            e.Handled = true;
            QDeckNameSave(name);
        }
        else if (e.Key == Key.Escape)
        {
            e.Handled = true;
            QDeckNameRestore(name);
        }
    }

    private void QDeckLeaveObserve(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox name)
        {
            QDeckNameSave(name);
        }
    }
}
