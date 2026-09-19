using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Shapes;
using Llyn.Core;

namespace Llyn.UIVeneer;

public sealed class PFanqie : ContentControl
{
    public static readonly DependencyProperty PFanqieItemsProperty = DependencyProperty.Register(
        nameof(PFanqieItems),
        typeof(IReadOnlyList<PFanqieItem>),
        typeof(PFanqie),
        new FrameworkPropertyMetadata(null, PFanqieStateHandle));

    public static readonly DependencyProperty PFanqiePendingProperty = DependencyProperty.Register(
        nameof(PFanqiePending),
        typeof(bool),
        typeof(PFanqie),
        new FrameworkPropertyMetadata(false, PFanqieStateHandle));

    public static readonly DependencyProperty PFanqieFoldedProperty = DependencyProperty.Register(
        nameof(PFanqieFolded),
        typeof(bool),
        typeof(PFanqie),
        new FrameworkPropertyMetadata(false, PFanqieStateHandle));

    private readonly Grid _pFanqieHead = new();
    private readonly ToggleButton _pFanqieSwitch = new();
    private readonly StackPanel _pFanqieBody = new();
    private readonly ItemsControl _pFanqieList = new();
    private readonly TextBlock _pFanqieLoading = new();
    private readonly Button _pFanqieRefresh = new();

    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new PSurfacePeer(this);
    }

    public PFanqie()
    {
        Focusable = false;
        IsTabStop = false;
        Visibility = Visibility.Collapsed;

        TextBlock label = new();
        label.SetResourceReference(StyleProperty, "Theme.Fanqie.Head");
        label.SetResourceReference(TextBlock.TextProperty, "Display.Fanqie");
        Path chevron = new() { Width = 12, Height = 12, Stretch = System.Windows.Media.Stretch.None };
        chevron.Data = PIcon.PIconResolve("expand", 12);
        chevron.SetBinding(
            Shape.FillProperty,
            new System.Windows.Data.Binding(nameof(Foreground)) { Source = _pFanqieSwitch });
        _pFanqieSwitch.Content = chevron;
        _pFanqieSwitch.SetResourceReference(StyleProperty, "Theme.Marker.Switch");
        _pFanqieSwitch.Checked += (_, _) => PFanqieStateApply();
        _pFanqieSwitch.Unchecked += (_, _) => PFanqieStateApply();
        _pFanqieHead.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        _pFanqieHead.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        Grid.SetColumn(_pFanqieSwitch, 1);
        _pFanqieHead.Children.Add(label);
        _pFanqieHead.Children.Add(_pFanqieSwitch);

        Grid.SetIsSharedSizeScope(_pFanqieList, true);
        _pFanqieList.CommandBindings.Add(
            new CommandBinding(PFanqieCommand.PFanqieCommandInitial, PFanqieDiweiHandle));
        _pFanqieList.CommandBindings.Add(
            new CommandBinding(PFanqieCommand.PFanqieCommandRime, PFanqieDiweiHandle));
        _pFanqieList.SetResourceReference(ItemsControl.ItemTemplateProperty, "Theme.Fanqie.Row");
        _pFanqieLoading.SetResourceReference(StyleProperty, "Theme.Fanqie.Loading");
        _pFanqieLoading.SetResourceReference(TextBlock.TextProperty, "Display.FanqieLoading");
        _pFanqieRefresh.SetResourceReference(StyleProperty, "Theme.Fanqie.Rebuild");
        _pFanqieRefresh.SetResourceReference(ContentProperty, "Display.FanqieRebuild");
        _pFanqieRefresh.Click += (_, _) => PFanqieRebuildNotice?.Invoke();
        _pFanqieBody.Children.Add(_pFanqieList);
        _pFanqieBody.Children.Add(_pFanqieLoading);
        _pFanqieBody.Children.Add(_pFanqieRefresh);

        StackPanel stack = new();
        stack.Children.Add(_pFanqieHead);
        stack.Children.Add(_pFanqieBody);
        Border box = new() { Child = stack };
        box.SetResourceReference(StyleProperty, "Theme.Fanqie.Box");
        Content = box;
        PFanqieStateApply();
    }

    internal IReadOnlyList<PFanqieItem>? PFanqieItems
    {
        get => (IReadOnlyList<PFanqieItem>?)GetValue(PFanqieItemsProperty);
        set => SetValue(PFanqieItemsProperty, value);
    }

    public bool PFanqiePending
    {
        get => (bool)GetValue(PFanqiePendingProperty);
        set => SetValue(PFanqiePendingProperty, value);
    }

    public bool PFanqieFolded
    {
        get => (bool)GetValue(PFanqieFoldedProperty);
        set => SetValue(PFanqieFoldedProperty, value);
    }

    internal Action? PFanqieRebuildNotice { get; set; }

    internal Action<string, string>? PFanqieDiweiNotice { get; set; }

    private void PFanqieDiweiHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is not PFanqieLine line)
        {
            return;
        }

        bool initial = e.Command == PFanqieCommand.PFanqieCommandInitial;
        string key = initial ? line.PFanqieLineInitial : line.PFanqieLineYunmu;
        if (key.Length > 0)
        {
            PFanqieDiweiNotice?.Invoke(initial ? LDiwei.LDiweiInitial : LDiwei.LDiweiRime, key);
        }
    }

    private static void PFanqieStateHandle(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        ((PFanqie)sender).PFanqieStateApply();
    }

    private void PFanqieStateApply()
    {
        IReadOnlyList<PFanqieItem>? items = PFanqieItems;
        bool filled = items is not null && items.Count > 0;
        _pFanqieList.ItemsSource = filled ? items : null;
        _pFanqieLoading.Visibility = PFanqiePending ? Visibility.Visible : Visibility.Collapsed;
        _pFanqieRefresh.Visibility = PFanqieRebuildNotice is not null && !PFanqiePending
            ? Visibility.Visible
            : Visibility.Collapsed;
        _pFanqieHead.Visibility = PFanqieFolded ? Visibility.Visible : Visibility.Collapsed;
        _pFanqieBody.Visibility = !PFanqieFolded || _pFanqieSwitch.IsChecked == true
            ? Visibility.Visible
            : Visibility.Collapsed;
        Visibility = filled || PFanqiePending || PFanqieRebuildNotice is not null
            ? Visibility.Visible
            : Visibility.Collapsed;
    }
}
