using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIDeportment;

public sealed class QParadigm : Decorator
{
    public static readonly DependencyProperty QParadigmItemsProperty = DependencyProperty.Register(
        nameof(QParadigmItems),
        typeof(IReadOnlyList<QParadigmItem>),
        typeof(QParadigm),
        new FrameworkPropertyMetadata(null, QParadigmItemsRefine));

    private readonly ItemsControl _qParadigmList = new();

    public QParadigm()
    {
        Visibility = Visibility.Collapsed;
        HorizontalAlignment = HorizontalAlignment.Left;

        Grid.SetIsSharedSizeScope(_qParadigmList, true);
        _qParadigmList.SetResourceReference(ItemsControl.ItemTemplateProperty, "Theme.Paradigm.Row");
        QLookItem.QLookItemAttach(_qParadigmList, QParadigmItemRefine);

        Border box = new() { Child = _qParadigmList };
        box.SetResourceReference(StyleProperty, "Theme.Paradigm.Box");
        Child = box;
    }

    public IReadOnlyList<QParadigmItem>? QParadigmItems
    {
        get => (IReadOnlyList<QParadigmItem>?)GetValue(QParadigmItemsProperty);
        set => SetValue(QParadigmItemsProperty, value);
    }

    private static void QParadigmItemRefine(FrameworkElement container, object item, string? _)
    {
        if (container is not ContentPresenter presenter || item is not QParadigmItem row)
        {
            return;
        }

        presenter.ApplyTemplate();
        if (presenter.ContentTemplate?.FindName("PParadigmPart", presenter) is not TextBlock part
            || presenter.ContentTemplate.FindName("PParadigmName", presenter) is not TextBlock name
            || presenter.ContentTemplate.FindName("PParadigmText", presenter) is not TextBlock text)
        {
            return;
        }

        part.Text = row.QParadigmItemPart;
        name.Text = row.QParadigmItemName;
        text.Text = row.QParadigmItemText;
        if (row.QParadigmItemTip is not string tip)
        {
            text.ClearValue(TextBlock.ForegroundProperty);
            text.ClearValue(ToolTipProperty);
            return;
        }

        text.SetResourceReference(TextBlock.ForegroundProperty, "Theme.Muted");
        text.SetResourceReference(ToolTipProperty, tip);
    }

    private static void QParadigmItemsRefine(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        QParadigm box = (QParadigm)sender;
        IReadOnlyList<QParadigmItem>? items = e.NewValue as IReadOnlyList<QParadigmItem>;
        box._qParadigmList.ItemsSource = items;
        box.Visibility = items is null || items.Count == 0
            ? Visibility.Collapsed
            : Visibility.Visible;
    }
}
