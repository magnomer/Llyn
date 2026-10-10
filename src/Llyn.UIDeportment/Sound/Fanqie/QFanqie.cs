using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Llyn.UIDeportment;

public sealed class QFanqie : Decorator
{
    public static readonly DependencyProperty QFanqieItemsProperty = DependencyProperty.Register(
        nameof(QFanqieItems),
        typeof(IReadOnlyList<QFanqieItem>),
        typeof(QFanqie),
        new FrameworkPropertyMetadata(null, static (sender, _) => ((QFanqie)sender).QFanqieStateRefine()));

    public static readonly DependencyProperty QFanqiePendingProperty = DependencyProperty.Register(
        nameof(QFanqiePending),
        typeof(bool),
        typeof(QFanqie),
        new FrameworkPropertyMetadata(false, static (sender, _) => ((QFanqie)sender).QFanqieStateRefine()));

    public static readonly DependencyProperty QFanqieFoldedProperty = DependencyProperty.Register(
        nameof(QFanqieFolded),
        typeof(bool),
        typeof(QFanqie),
        new FrameworkPropertyMetadata(false, static (sender, _) =>
        {
            QFanqie box = (QFanqie)sender;
            box._qFanqieBody.Visibility = QLook.QLookVisibleRead(!box.QFanqieFolded);
            box.QFanqieStateRefine();
        }));

    public static readonly DependencyProperty QFanqieRenewableProperty = DependencyProperty.Register(
        nameof(QFanqieRenewable),
        typeof(bool),
        typeof(QFanqie),
        new FrameworkPropertyMetadata(false, static (sender, _) => ((QFanqie)sender).QFanqieStateRefine()));

    private readonly Grid _qFanqieHead = new();
    private readonly ToggleButton _qFanqieSwitch = new();
    private readonly StackPanel _qFanqieBody = new();
    private readonly ItemsControl _qFanqieList = new();
    private readonly TextBlock _qFanqieLoading = new();
    private readonly Button _qFanqieRefresh = new();

    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new QVeilPeer(this);
    }

    public QFanqie()
    {
        Visibility = Visibility.Collapsed;

        TextBlock label = new();
        label.SetResourceReference(StyleProperty, "Theme.Fanqie.Head");
        label.SetResourceReference(TextBlock.TextProperty, "Display.Fanqie");
        QIconImage chevron = new() { Width = 12, Height = 12, QIconSource = QIcon.QIconResolve("expand", 12) };
        _qFanqieSwitch.Content = chevron;
        _qFanqieSwitch.SetResourceReference(StyleProperty, "Theme.Marker.Switch");
        _qFanqieRefresh.SetResourceReference(StyleProperty, "Theme.Sound.Rebuild");
        _qFanqieRefresh.Click += QFanqieRefreshObserve;
        _qFanqieHead.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        _qFanqieHead.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        _qFanqieHead.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        Grid.SetColumn(_qFanqieRefresh, 1);
        Grid.SetColumn(_qFanqieSwitch, 2);
        _qFanqieHead.Children.Add(label);
        _qFanqieHead.Children.Add(_qFanqieRefresh);
        _qFanqieHead.Children.Add(_qFanqieSwitch);

        Grid.SetIsSharedSizeScope(_qFanqieList, true);
        _qFanqieList.CommandBindings.Add(
            new CommandBinding(QFanqieCommand.QFanqieCommandInitial, QFanqieDiweiObserve));
        _qFanqieList.CommandBindings.Add(
            new CommandBinding(QFanqieCommand.QFanqieCommandRime, QFanqieDiweiObserve));
        _qFanqieList.CommandBindings.Add(
            new CommandBinding(QFanqieCommand.QFanqieCommandStem, QFanqieStemObserve));
        _qFanqieList.CommandBindings.Add(
            new CommandBinding(QFanqieCommand.QFanqieCommandRepresentative, QFanqieRepresentativeObserve));
        _qFanqieList.SetResourceReference(ItemsControl.ItemTemplateProperty, "Theme.Fanqie.Row");
        QLookItem.QLookItemAttach(_qFanqieList, QFanqieItem.QFanqieItemRefine);
        _qFanqieLoading.SetResourceReference(StyleProperty, "Theme.Fanqie.Loading");
        _qFanqieLoading.SetResourceReference(TextBlock.TextProperty, "Display.FanqieLoading");
        _qFanqieBody.Children.Add(_qFanqieList);
        _qFanqieBody.Children.Add(_qFanqieLoading);

        StackPanel stack = new();
        stack.Children.Add(_qFanqieHead);
        stack.Children.Add(_qFanqieBody);
        Border box = new() { Child = stack };
        box.SetResourceReference(StyleProperty, "Theme.Fanqie.Box");
        Child = box;
        QFanqieStateRefine();
    }

    internal IReadOnlyList<QFanqieItem>? QFanqieItems
    {
        get => (IReadOnlyList<QFanqieItem>?)GetValue(QFanqieItemsProperty);
        set => SetValue(QFanqieItemsProperty, value);
    }

    public bool QFanqiePending
    {
        get => (bool)GetValue(QFanqiePendingProperty);
        set => SetValue(QFanqiePendingProperty, value);
    }

    public bool QFanqieFolded
    {
        get => (bool)GetValue(QFanqieFoldedProperty);
        set => SetValue(QFanqieFoldedProperty, value);
    }

    internal bool QFanqieRenewable
    {
        get => (bool)GetValue(QFanqieRenewableProperty);
        set => SetValue(QFanqieRenewableProperty, value);
    }

    internal event Action? QFanqieRenewalNotice;

    internal event Action<bool, string>? QFanqieDiweiNotice;

    internal event Action<string?>? QFanqieStemNotice;

    internal event Action<long, int, bool>? QFanqieRepresentativeNotice;

    private void QFanqieDiweiObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is not QFanqieLine line)
        {
            return;
        }

        bool initial = e.Command == QFanqieCommand.QFanqieCommandInitial;
        QFanqieDiweiNotice?.Invoke(initial, initial ? line.QFanqieLineInitial : line.QFanqieLineYunmu);
    }

    private void QFanqieStemObserve(object sender, ExecutedRoutedEventArgs e)
    {
        QFanqieStemNotice?.Invoke(QSender.QSenderTextRead(e));
    }

    private void QFanqieRepresentativeObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (QSender.QSenderParameterRead<QFanqieLine>(e) is not QFanqieLine line)
        {
            return;
        }

        QFanqieRepresentativeNotice?.Invoke(
            line.QFanqieLineId, line.QFanqieLineRank, Keyboard.Modifiers.HasFlag(ModifierKeys.Control));
    }

    private void QFanqieRefreshObserve(object sender, RoutedEventArgs e)
    {
        QFanqieRenewalNotice?.Invoke();
    }

    internal ToggleButton QFanqieSwitch => _qFanqieSwitch;

    internal void QFanqieFoldRefine(bool opened)
    {
        _qFanqieSwitch.IsChecked = opened;
        _qFanqieBody.Visibility = QLook.QLookVisibleRead(opened || !QFanqieFolded);
    }

    private void QFanqieStateRefine()
    {
        IReadOnlyList<QFanqieItem>? items = QFanqieItems;
        bool filled = items is not null && items.Count > 0;
        _qFanqieList.ItemsSource = filled ? items : null;
        _qFanqieLoading.Visibility = QFanqiePending ? Visibility.Visible : Visibility.Collapsed;
        _qFanqieRefresh.Visibility = QLook.QLookVisibleRead(QFanqieRenewable);
        _qFanqieRefresh.SetValue(
            QLook.QLookCueProperty,
            QLook.QLookFirstRead(QFanqiePending, QLookCue.QLookCuePending, QLookCue.QLookCueBase));
        _qFanqieHead.Visibility = QFanqieFolded ? Visibility.Visible : Visibility.Collapsed;
        Visibility = filled || QFanqiePending || QFanqieRenewable
            ? Visibility.Visible
            : Visibility.Collapsed;
    }
}
