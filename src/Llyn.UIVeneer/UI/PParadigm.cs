using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIVeneer;

public sealed class PParadigm : ContentControl
{
    public static readonly DependencyProperty PParadigmItemsProperty = DependencyProperty.Register(
        nameof(PParadigmItems),
        typeof(IReadOnlyList<PParadigmItem>),
        typeof(PParadigm),
        new FrameworkPropertyMetadata(null, PParadigmItemsHandle));

    private readonly ItemsControl _pParadigmList = new();

    public PParadigm()
    {
        Focusable = false;
        IsTabStop = false;
        Visibility = Visibility.Collapsed;
        HorizontalAlignment = HorizontalAlignment.Left;

        Grid.SetIsSharedSizeScope(_pParadigmList, true);
        _pParadigmList.SetResourceReference(ItemsControl.ItemTemplateProperty, "Theme.Paradigm.Row");

        Border box = new() { Child = _pParadigmList };
        box.SetResourceReference(StyleProperty, "Theme.Paradigm.Box");
        Content = box;
    }

    public IReadOnlyList<PParadigmItem>? PParadigmItems
    {
        get => (IReadOnlyList<PParadigmItem>?)GetValue(PParadigmItemsProperty);
        set => SetValue(PParadigmItemsProperty, value);
    }

    private static void PParadigmItemsHandle(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        PParadigm box = (PParadigm)sender;
        IReadOnlyList<PParadigmItem>? items = e.NewValue as IReadOnlyList<PParadigmItem>;
        box._pParadigmList.ItemsSource = items;
        box.Visibility = items is null || items.Count == 0
            ? Visibility.Collapsed
            : Visibility.Visible;
    }
}
