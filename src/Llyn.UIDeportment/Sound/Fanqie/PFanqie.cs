using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class PFanqie : ContentControl
{
    public static readonly DependencyProperty PFanqieItemsProperty = DependencyProperty.Register(
        nameof(PFanqieItems),
        typeof(IReadOnlyList<PFanqieItem>),
        typeof(PFanqie),
        new FrameworkPropertyMetadata(null, static (sender, _) => ((PFanqie)sender).PFanqieStateRefine()));

    public static readonly DependencyProperty PFanqiePendingProperty = DependencyProperty.Register(
        nameof(PFanqiePending),
        typeof(bool),
        typeof(PFanqie),
        new FrameworkPropertyMetadata(false, static (sender, _) => ((PFanqie)sender).PFanqieStateRefine()));

    public static readonly DependencyProperty PFanqieFoldedProperty = DependencyProperty.Register(
        nameof(PFanqieFolded),
        typeof(bool),
        typeof(PFanqie),
        new FrameworkPropertyMetadata(false, static (sender, _) => ((PFanqie)sender).PFanqieStateRefine()));

    public static readonly DependencyProperty PFanqieRenewalProperty = DependencyProperty.Register(
        nameof(PFanqieRenewal),
        typeof(Action),
        typeof(PFanqie),
        new FrameworkPropertyMetadata(null, static (sender, _) => ((PFanqie)sender).PFanqieStateRefine()));

    private readonly Grid _pFanqieHead = new();
    private readonly ToggleButton _pFanqieSwitch = new();
    private readonly StackPanel _pFanqieBody = new();
    private readonly ItemsControl _pFanqieList = new();
    private readonly TextBlock _pFanqieLoading = new();
    private readonly Button _pFanqieRefresh = new();

    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new QSurfacePeer(this);
    }

    public PFanqie()
    {
        Focusable = false;
        IsTabStop = false;
        Visibility = Visibility.Collapsed;

        TextBlock label = new();
        label.SetResourceReference(StyleProperty, "Theme.Fanqie.Head");
        label.SetResourceReference(TextBlock.TextProperty, "Display.Fanqie");
        QIconImage chevron = new() { Width = 12, Height = 12, QIconSource = QIcon.QIconResolve("expand", 12) };
        _pFanqieSwitch.Content = chevron;
        _pFanqieSwitch.SetResourceReference(StyleProperty, "Theme.Marker.Switch");
        _pFanqieSwitch.Checked += (_, _) => PFanqieStateRefine();
        _pFanqieSwitch.Unchecked += (_, _) => PFanqieStateRefine();
        _pFanqieRefresh.SetResourceReference(StyleProperty, "Theme.Sound.Rebuild");
        _pFanqieRefresh.Click += (_, _) => PFanqieRenewal?.Invoke();
        _pFanqieHead.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        _pFanqieHead.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        _pFanqieHead.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        Grid.SetColumn(_pFanqieRefresh, 1);
        Grid.SetColumn(_pFanqieSwitch, 2);
        _pFanqieHead.Children.Add(label);
        _pFanqieHead.Children.Add(_pFanqieRefresh);
        _pFanqieHead.Children.Add(_pFanqieSwitch);

        Grid.SetIsSharedSizeScope(_pFanqieList, true);
        _pFanqieList.CommandBindings.Add(
            new CommandBinding(PFanqieCommand.PFanqieCommandInitial, PFanqieDiweiObserve));
        _pFanqieList.CommandBindings.Add(
            new CommandBinding(PFanqieCommand.PFanqieCommandRime, PFanqieDiweiObserve));
        _pFanqieList.CommandBindings.Add(
            new CommandBinding(PFanqieCommand.PFanqieCommandStem, PFanqieStemObserve));
        _pFanqieList.CommandBindings.Add(
            new CommandBinding(PFanqieCommand.PFanqieCommandRepresentative, PFanqieRepresentativeObserve));
        _pFanqieList.SetResourceReference(ItemsControl.ItemTemplateProperty, "Theme.Fanqie.Row");
        QLookItem.QLookItemAttach(_pFanqieList, PFanqieItem.PFanqieItemRefine);
        _pFanqieLoading.SetResourceReference(StyleProperty, "Theme.Fanqie.Loading");
        _pFanqieLoading.SetResourceReference(TextBlock.TextProperty, "Display.FanqieLoading");
        _pFanqieBody.Children.Add(_pFanqieList);
        _pFanqieBody.Children.Add(_pFanqieLoading);

        StackPanel stack = new();
        stack.Children.Add(_pFanqieHead);
        stack.Children.Add(_pFanqieBody);
        Border box = new() { Child = stack };
        box.SetResourceReference(StyleProperty, "Theme.Fanqie.Box");
        Content = box;
        PFanqieStateRefine();
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

    internal Action? PFanqieRenewal
    {
        get => (Action?)GetValue(PFanqieRenewalProperty);
        set => SetValue(PFanqieRenewalProperty, value);
    }

    internal event Action<bool, string>? PFanqieDiweiNotice;

    internal event Action<string?>? PFanqieStemNotice;

    internal event Action<long, int, bool>? PFanqieRepresentativeNotice;

    internal void PFanqieRefine(IReadOnlyList<CFanqieGroup> groups, bool pending)
    {
        SetCurrentValue(PFanqieItemsProperty, PFanqieItem.PFanqieItemScan(groups));
        SetCurrentValue(PFanqiePendingProperty, pending);
    }

    private void PFanqieDiweiObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is not PFanqieLine line)
        {
            return;
        }

        bool initial = e.Command == PFanqieCommand.PFanqieCommandInitial;
        PFanqieDiweiNotice?.Invoke(initial, initial ? line.PFanqieLineInitial : line.PFanqieLineYunmu);
    }

    private void PFanqieStemObserve(object sender, ExecutedRoutedEventArgs e)
    {
        PFanqieStemNotice?.Invoke(QSender.QSenderTextRead(e));
    }

    private void PFanqieRepresentativeObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (QSender.QSenderParameterRead<PFanqieLine>(e) is not PFanqieLine line)
        {
            return;
        }

        PFanqieRepresentativeNotice?.Invoke(
            line.PFanqieLineId, line.PFanqieLineRank, Keyboard.Modifiers.HasFlag(ModifierKeys.Control));
    }

    private void PFanqieStateRefine()
    {
        IReadOnlyList<PFanqieItem>? items = PFanqieItems;
        bool filled = items is not null && items.Count > 0;
        _pFanqieList.ItemsSource = filled ? items : null;
        _pFanqieLoading.Visibility = PFanqiePending ? Visibility.Visible : Visibility.Collapsed;
        _pFanqieRefresh.Visibility = QLook.QLookVisibleRead(PFanqieRenewal is not null);
        _pFanqieRefresh.SetValue(
            QLook.QLookCueProperty,
            QLook.QLookFirstRead(PFanqiePending, QLookCue.QLookCuePending, QLookCue.QLookCueBase));
        _pFanqieHead.Visibility = PFanqieFolded ? Visibility.Visible : Visibility.Collapsed;
        _pFanqieBody.Visibility = !PFanqieFolded || _pFanqieSwitch.IsChecked == true
            ? Visibility.Visible
            : Visibility.Collapsed;
        Visibility = filled || PFanqiePending || PFanqieRenewal is not null
            ? Visibility.Visible
            : Visibility.Collapsed;
    }
}
