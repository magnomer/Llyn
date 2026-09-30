using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class PScript : ContentControl
{
    public static readonly DependencyProperty PScriptItemsProperty = DependencyProperty.Register(
        nameof(PScriptItems),
        typeof(IReadOnlyList<PScriptItem>),
        typeof(PScript),
        new FrameworkPropertyMetadata(null, static (sender, _) => ((PScript)sender).PScriptStateRefine()));

    public static readonly DependencyProperty PScriptPendingProperty = DependencyProperty.Register(
        nameof(PScriptPending),
        typeof(bool),
        typeof(PScript),
        new FrameworkPropertyMetadata(false, static (sender, _) => ((PScript)sender).PScriptStateRefine()));

    public static readonly DependencyProperty PScriptFoldedProperty = DependencyProperty.Register(
        nameof(PScriptFolded),
        typeof(bool),
        typeof(PScript),
        new FrameworkPropertyMetadata(false, static (sender, _) => ((PScript)sender).PScriptStateRefine()));

    public static readonly DependencyProperty PScriptRenewalProperty = DependencyProperty.Register(
        nameof(PScriptRenewal),
        typeof(Action),
        typeof(PScript),
        new FrameworkPropertyMetadata(null, static (sender, _) => ((PScript)sender).PScriptStateRefine()));

    private readonly Grid _pScriptHead = new();
    private readonly ToggleButton _pScriptSwitch = new();
    private readonly StackPanel _pScriptBody = new();
    private readonly ItemsControl _pScriptList = new();
    private readonly TextBlock _pScriptLoading = new();
    private readonly Button _pScriptRefresh = new();

    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new QSurfacePeer(this);
    }

    public PScript()
    {
        Focusable = false;
        IsTabStop = false;
        Visibility = Visibility.Collapsed;

        TextBlock label = new();
        label.SetResourceReference(StyleProperty, "Theme.Script.Head");
        label.SetResourceReference(TextBlock.TextProperty, "Display.Script");
        QIconImage chevron = new() { Width = 12, Height = 12, QIconSource = QIcon.QIconResolve("expand", 12) };
        _pScriptSwitch.Content = chevron;
        _pScriptSwitch.SetResourceReference(StyleProperty, "Theme.Marker.Switch");
        _pScriptSwitch.Checked += (_, _) => PScriptStateRefine();
        _pScriptSwitch.Unchecked += (_, _) => PScriptStateRefine();
        _pScriptRefresh.SetResourceReference(StyleProperty, "Theme.Sound.Rebuild");
        _pScriptRefresh.Click += (_, _) => PScriptRenewal?.Invoke();
        _pScriptHead.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        _pScriptHead.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        _pScriptHead.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        Grid.SetColumn(_pScriptRefresh, 1);
        Grid.SetColumn(_pScriptSwitch, 2);
        _pScriptHead.Children.Add(label);
        _pScriptHead.Children.Add(_pScriptRefresh);
        _pScriptHead.Children.Add(_pScriptSwitch);

        Grid.SetIsSharedSizeScope(_pScriptList, true);
        _pScriptList.SetResourceReference(ItemsControl.ItemTemplateProperty, "Theme.Script.Row");
        QLookItem.QLookItemAttach(_pScriptList, PScriptItem.PScriptItemRefine);
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
        QLocalizationCatalog.QLocalizationCatalogCurrent.PropertyChanged += PScriptLanguageRefine;
        PScriptStateRefine();
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

    internal Action? PScriptRenewal
    {
        get => (Action?)GetValue(PScriptRenewalProperty);
        set => SetValue(PScriptRenewalProperty, value);
    }

    internal void PScriptRefine(IReadOnlyList<CScriptGroup> groups, bool pending)
    {
        SetCurrentValue(PScriptItemsProperty, PScriptItem.PScriptItemScan(groups));
        SetCurrentValue(PScriptPendingProperty, pending);
    }

    private void PScriptLanguageRefine(object? sender, PropertyChangedEventArgs e)
    {
        if (PScriptItems is not IReadOnlyList<PScriptItem> items)
        {
            return;
        }

        foreach (PScriptItem item in items)
        {
            foreach (PScriptImage picture in item.PScriptItemImages)
            {
                picture.PScriptImageRaise();
            }
        }
    }

    private void PScriptStateRefine()
    {
        IReadOnlyList<PScriptItem>? items = PScriptItems;
        bool filled = items is not null && items.Count > 0;
        _pScriptList.ItemsSource = filled ? items : null;
        _pScriptLoading.Visibility = PScriptPending ? Visibility.Visible : Visibility.Collapsed;
        _pScriptRefresh.Visibility = QLook.QLookVisibleRead(PScriptRenewal is not null);
        _pScriptRefresh.SetValue(
            QLook.QLookCueProperty,
            QLook.QLookFirstRead(PScriptPending, QLookCue.QLookCuePending, QLookCue.QLookCueBase));
        _pScriptHead.Visibility = PScriptFolded ? Visibility.Visible : Visibility.Collapsed;
        _pScriptBody.Visibility = !PScriptFolded || _pScriptSwitch.IsChecked == true
            ? Visibility.Visible
            : Visibility.Collapsed;
        Visibility = filled || PScriptPending || PScriptRenewal is not null
            ? Visibility.Visible
            : Visibility.Collapsed;
    }
}
