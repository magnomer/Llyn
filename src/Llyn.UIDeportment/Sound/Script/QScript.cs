using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Llyn.UIDeportment;

public sealed class QScript : Decorator
{
    public static readonly DependencyProperty QScriptItemsProperty = DependencyProperty.Register(
        nameof(QScriptItems),
        typeof(IReadOnlyList<QScriptItem>),
        typeof(QScript),
        new FrameworkPropertyMetadata(null, static (sender, _) => ((QScript)sender).QScriptStateRefine()));

    public static readonly DependencyProperty QScriptPendingProperty = DependencyProperty.Register(
        nameof(QScriptPending),
        typeof(bool),
        typeof(QScript),
        new FrameworkPropertyMetadata(false, static (sender, _) => ((QScript)sender).QScriptStateRefine()));

    public static readonly DependencyProperty QScriptFoldedProperty = DependencyProperty.Register(
        nameof(QScriptFolded),
        typeof(bool),
        typeof(QScript),
        new FrameworkPropertyMetadata(false, static (sender, _) =>
        {
            QScript box = (QScript)sender;
            box._qScriptBody.Visibility = QLook.QLookVisibleRead(!box.QScriptFolded);
            box.QScriptStateRefine();
        }));

    public static readonly DependencyProperty QScriptRenewableProperty = DependencyProperty.Register(
        nameof(QScriptRenewable),
        typeof(bool),
        typeof(QScript),
        new FrameworkPropertyMetadata(false, static (sender, _) => ((QScript)sender).QScriptStateRefine()));

    private readonly Grid _qScriptHead = new();
    private readonly ToggleButton _qScriptSwitch = new();
    private readonly StackPanel _qScriptBody = new();
    private readonly ItemsControl _qScriptList = new();
    private readonly TextBlock _qScriptLoading = new();
    private readonly Button _qScriptRefresh = new();

    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new QVeilPeer(this);
    }

    public QScript()
    {
        Visibility = Visibility.Collapsed;

        TextBlock label = new();
        label.SetResourceReference(StyleProperty, "Theme.Script.Head");
        label.SetResourceReference(TextBlock.TextProperty, "Display.Script");
        QIconImage chevron = new() { Width = 12, Height = 12, QIconSource = QIcon.QIconResolve("expand", 12) };
        _qScriptSwitch.Content = chevron;
        _qScriptSwitch.SetResourceReference(StyleProperty, "Theme.Marker.Switch");
        _qScriptRefresh.SetResourceReference(StyleProperty, "Theme.Sound.Rebuild");
        _qScriptRefresh.Click += QScriptRefreshObserve;
        _qScriptHead.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        _qScriptHead.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        _qScriptHead.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        Grid.SetColumn(_qScriptRefresh, 1);
        Grid.SetColumn(_qScriptSwitch, 2);
        _qScriptHead.Children.Add(label);
        _qScriptHead.Children.Add(_qScriptRefresh);
        _qScriptHead.Children.Add(_qScriptSwitch);

        Grid.SetIsSharedSizeScope(_qScriptList, true);
        _qScriptList.SetResourceReference(ItemsControl.ItemTemplateProperty, "Theme.Script.Row");
        QLookItem.QLookItemAttach(_qScriptList, QScriptItem.QScriptItemRefine);
        _qScriptLoading.SetResourceReference(StyleProperty, "Theme.Script.Loading");
        _qScriptLoading.SetResourceReference(TextBlock.TextProperty, "Display.ScriptLoading");
        _qScriptBody.Children.Add(_qScriptList);
        _qScriptBody.Children.Add(_qScriptLoading);

        StackPanel stack = new();
        stack.Children.Add(_qScriptHead);
        stack.Children.Add(_qScriptBody);
        Border box = new() { Child = stack };
        box.SetResourceReference(StyleProperty, "Theme.Script.Box");
        Child = box;
        QLocalizationCatalog.QLocalizationCatalogCurrent.PropertyChanged += QScriptLanguageRefine;
        QScriptStateRefine();
    }

    internal IReadOnlyList<QScriptItem>? QScriptItems
    {
        get => (IReadOnlyList<QScriptItem>?)GetValue(QScriptItemsProperty);
        set => SetValue(QScriptItemsProperty, value);
    }

    public bool QScriptPending
    {
        get => (bool)GetValue(QScriptPendingProperty);
        set => SetValue(QScriptPendingProperty, value);
    }

    public bool QScriptFolded
    {
        get => (bool)GetValue(QScriptFoldedProperty);
        set => SetValue(QScriptFoldedProperty, value);
    }

    internal bool QScriptRenewable
    {
        get => (bool)GetValue(QScriptRenewableProperty);
        set => SetValue(QScriptRenewableProperty, value);
    }

    internal event Action? QScriptRenewalNotice;

    internal event Action<Exception>? QScriptFailureNotice;

    internal void QScriptFailureRefine(Exception exception)
    {
        Dispatcher.BeginInvoke(() => QScriptFailureNotice?.Invoke(exception));
    }

    private void QScriptLanguageRefine(object? sender, PropertyChangedEventArgs e)
    {
        if (QScriptItems is not IReadOnlyList<QScriptItem> items)
        {
            return;
        }

        foreach (QScriptItem item in items)
        {
            foreach (QScriptImage picture in item.QScriptItemImages)
            {
                picture.QScriptImageRaise();
            }
        }
    }

    private void QScriptRefreshObserve(object sender, RoutedEventArgs e)
    {
        QScriptRenewalNotice?.Invoke();
    }

    internal ToggleButton QScriptSwitch => _qScriptSwitch;

    internal void QScriptFoldRefine(bool opened)
    {
        _qScriptSwitch.IsChecked = opened;
        _qScriptBody.Visibility = QLook.QLookVisibleRead(opened || !QScriptFolded);
    }

    private void QScriptStateRefine()
    {
        IReadOnlyList<QScriptItem>? items = QScriptItems;
        bool filled = items is not null && items.Count > 0;
        _qScriptList.ItemsSource = filled ? items : null;
        _qScriptLoading.Visibility = QScriptPending ? Visibility.Visible : Visibility.Collapsed;
        _qScriptRefresh.Visibility = QLook.QLookVisibleRead(QScriptRenewable);
        _qScriptRefresh.SetValue(
            QLook.QLookCueProperty,
            QLook.QLookFirstRead(QScriptPending, QLookCue.QLookCuePending, QLookCue.QLookCueBase));
        _qScriptHead.Visibility = QScriptFolded ? Visibility.Visible : Visibility.Collapsed;
        Visibility = filled || QScriptPending || QScriptRenewable
            ? Visibility.Visible
            : Visibility.Collapsed;
    }
}
