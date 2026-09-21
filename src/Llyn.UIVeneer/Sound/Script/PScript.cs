using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Llyn.UIVeneer;

public sealed class PScript : ContentControl
{
    public static readonly DependencyProperty PScriptItemsProperty = DependencyProperty.Register(
        nameof(PScriptItems),
        typeof(IReadOnlyList<PScriptItem>),
        typeof(PScript),
        new FrameworkPropertyMetadata(null, PScriptStateHandle));

    public static readonly DependencyProperty PScriptPendingProperty = DependencyProperty.Register(
        nameof(PScriptPending),
        typeof(bool),
        typeof(PScript),
        new FrameworkPropertyMetadata(false, PScriptStateHandle));

    public static readonly DependencyProperty PScriptFoldedProperty = DependencyProperty.Register(
        nameof(PScriptFolded),
        typeof(bool),
        typeof(PScript),
        new FrameworkPropertyMetadata(false, PScriptStateHandle));

    private readonly Grid _pScriptHead = new();
    private readonly ToggleButton _pScriptSwitch = new();
    private readonly StackPanel _pScriptBody = new();
    private readonly ItemsControl _pScriptList = new();
    private readonly TextBlock _pScriptLoading = new();

    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new PSurfacePeer(this);
    }

    public PScript()
    {
        Focusable = false;
        IsTabStop = false;
        Visibility = Visibility.Collapsed;

        TextBlock label = new();
        label.SetResourceReference(StyleProperty, "Theme.Script.Head");
        label.SetResourceReference(TextBlock.TextProperty, "Display.Script");
        PIconImage chevron = new() { Width = 12, Height = 12, PIconSource = PIcon.PIconResolve("expand", 12) };
        _pScriptSwitch.Content = chevron;
        _pScriptSwitch.SetResourceReference(StyleProperty, "Theme.Marker.Switch");
        _pScriptSwitch.Checked += (_, _) => PScriptStateApply();
        _pScriptSwitch.Unchecked += (_, _) => PScriptStateApply();
        _pScriptHead.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        _pScriptHead.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        Grid.SetColumn(_pScriptSwitch, 1);
        _pScriptHead.Children.Add(label);
        _pScriptHead.Children.Add(_pScriptSwitch);

        Grid.SetIsSharedSizeScope(_pScriptList, true);
        _pScriptList.SetResourceReference(ItemsControl.ItemTemplateProperty, "Theme.Script.Row");
        _pScriptLoading.SetResourceReference(StyleProperty, "Theme.Script.Loading");
        _pScriptLoading.SetResourceReference(TextBlock.TextProperty, "Display.ScriptLoading");
        _pScriptBody.Children.Add(_pScriptList);
        _pScriptBody.Children.Add(_pScriptLoading);

        StackPanel stack = new();
        stack.Children.Add(_pScriptHead);
        stack.Children.Add(_pScriptBody);
        Border box = new() { Child = stack };
        box.SetResourceReference(StyleProperty, "Theme.Script.Box");
        Content = box;
        PScriptStateApply();
    }

    internal IReadOnlyList<PScriptItem>? PScriptItems
    {
        get => (IReadOnlyList<PScriptItem>?)GetValue(PScriptItemsProperty);
        set => SetValue(PScriptItemsProperty, value);
    }

    public bool PScriptPending
    {
        get => (bool)GetValue(PScriptPendingProperty);
        set => SetValue(PScriptPendingProperty, value);
    }

    public bool PScriptFolded
    {
        get => (bool)GetValue(PScriptFoldedProperty);
        set => SetValue(PScriptFoldedProperty, value);
    }

    private static void PScriptStateHandle(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        ((PScript)sender).PScriptStateApply();
    }

    private void PScriptStateApply()
    {
        IReadOnlyList<PScriptItem>? items = PScriptItems;
        bool filled = items is not null && items.Count > 0;
        _pScriptList.ItemsSource = filled ? items : null;
        _pScriptLoading.Visibility = PScriptPending ? Visibility.Visible : Visibility.Collapsed;
        _pScriptHead.Visibility = PScriptFolded ? Visibility.Visible : Visibility.Collapsed;
        _pScriptBody.Visibility = !PScriptFolded || _pScriptSwitch.IsChecked == true
            ? Visibility.Visible
            : Visibility.Collapsed;
        Visibility = filled || PScriptPending ? Visibility.Visible : Visibility.Collapsed;
    }
}
