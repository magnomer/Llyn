using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIDeportment;

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
        PLookItem.PLookItemAttach(_pParadigmList, PParadigmItemApply);

        Border box = new() { Child = _pParadigmList };
        box.SetResourceReference(StyleProperty, "Theme.Paradigm.Box");
        Content = box;
    }

    public IReadOnlyList<PParadigmItem>? PParadigmItems
    {
        get => (IReadOnlyList<PParadigmItem>?)GetValue(PParadigmItemsProperty);
        set => SetValue(PParadigmItemsProperty, value);
    }

    internal void PParadigmShow(IReadOnlyList<LParadigmSlot> slots, bool pending, bool enabled)
    {
        SetCurrentValue(PParadigmItemsProperty, PParadigmItem.PParadigmItemScan(slots, pending, enabled, false));
    }

    private static void PParadigmItemApply(FrameworkElement container, object item, string? _)
    {
        if (container is not ContentPresenter presenter || item is not PParadigmItem row)
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

        part.Text = row.PParadigmItemPart;
        name.Text = row.PParadigmItemName;

        string? tip = row.PParadigmItemUnknown ? "Paradigm.Unknown"
            : row.PParadigmItemHeld ? "Paradigm.Held"
            : row.PParadigmItemLost ? "Paradigm.Lost"
            : row.PParadigmItemPending ? "Paradigm.Pending"
            : row.PParadigmItemText.Length == 0 ? "Paradigm.Absent"
            : null;
        if (tip is null)
        {
            text.Text = row.PParadigmItemText;
            text.ClearValue(TextBlock.ForegroundProperty);
            text.ClearValue(ToolTipProperty);
            return;
        }

        text.Text = row.PParadigmItemUnknown ? "—" : "…";
        text.SetResourceReference(TextBlock.ForegroundProperty, "Theme.Muted");
        text.SetResourceReference(ToolTipProperty, tip);
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
