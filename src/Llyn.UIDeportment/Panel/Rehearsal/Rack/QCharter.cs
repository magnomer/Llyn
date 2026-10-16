using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QCharter
{
    private readonly UserControl _qCharterSurface;

    private CCharter _cCharter = null!;

    internal QCharter(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qCharterSurface = surface;
        QCharterStore.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("save", 24));
        QCharterStore.Click += QCharterStoreObserve;
        QCharterBasic.Click += QCharterBasicObserve;
        QLookItem.QLookItemAttach(QBearing, QBearingItemRefine);
    }

    private Button QCharterStore => QContract.QContractFind<Button>(_qCharterSurface, "PCharterStore");

    private ItemsControl QBearing => QContract.QContractFind<ItemsControl>(_qCharterSurface, "PBearing");

    private ComboBox QCharterLead => QContract.QContractFind<ComboBox>(_qCharterSurface, "PCharterLead");

    private ComboBox QCharterSequence => QContract.QContractFind<ComboBox>(_qCharterSurface, "PCharterSequence");

    private TextBox QCharterGoal => QContract.QContractFind<TextBox>(_qCharterSurface, "PCharterGoal");

    private TextBox QCharterSpaced => QContract.QContractFind<TextBox>(_qCharterSurface, "PCharterSpaced");

    private TextBox QCharterPool => QContract.QContractFind<TextBox>(_qCharterSurface, "PCharterPool");

    private TextBox QCharterStreak => QContract.QContractFind<TextBox>(_qCharterSurface, "PCharterStreak");

    private TextBox QCharterInterval => QContract.QContractFind<TextBox>(_qCharterSurface, "PCharterInterval");

    private TextBox QCharterQuorum => QContract.QContractFind<TextBox>(_qCharterSurface, "PCharterQuorum");

    private TextBox QCharterCode => QContract.QContractFind<TextBox>(_qCharterSurface, "PCharterCode");

    private CheckBox QCharterBasic => QContract.QContractFind<CheckBox>(_qCharterSurface, "PCharterBasic");

    internal void QCharterIntroduce(CCharter charter)
    {
        _cCharter = charter;
    }

    internal void QCharterRefine()
    {
        CCharterState? state = _cCharter.CCharterRead();
        _qCharterSurface.Visibility = QLook.QLookVisibleRead(state is not null);
        if (state is null)
        {
            QBearing.ItemsSource = null;
            return;
        }

        QCharterObserveDetach();
        QBearing.ItemsSource = state.CCharterStateBearings
            .Select(static bearing => new QBearingItem(
                bearing.CCharterBearingDirection,
                bearing.CCharterBearingFront,
                bearing.CCharterBearingBack,
                bearing.CCharterBearingStatistics,
                bearing.CCharterBearingCaption))
            .ToList();
        QCharterLead.SelectedIndex = state.CCharterStateLead;
        QCharterSequence.SelectedIndex = state.CCharterStateOrder;
        QCharterGoal.Text = state.CCharterStateGoal.ToString(CultureInfo.CurrentCulture);
        QCharterSpaced.Text = state.CCharterStateSpaced.ToString(CultureInfo.CurrentCulture);
        QCharterPool.Text = state.CCharterStatePool.ToString(CultureInfo.CurrentCulture);
        QCharterStreak.Text = state.CCharterStateStreak.ToString(CultureInfo.CurrentCulture);
        QCharterInterval.Text = state.CCharterStateTarget.ToString(CultureInfo.CurrentCulture);
        QCharterQuorum.Text = state.CCharterStateQuorum is int quorum
            ? quorum.ToString(CultureInfo.CurrentCulture)
            : string.Empty;
        QCharterCode.Text = state.CCharterStateCode;
        QCharterBasic.IsChecked = state.CCharterStateBasic;
        QCharterObserveAttach();
    }

    private void QCharterObserveDetach()
    {
        QCharterLead.SelectionChanged -= QCharterLeadObserve;
        QCharterSequence.SelectionChanged -= QCharterOrderObserve;
        QCharterGoal.TextChanged -= QCharterGoalObserve;
        QCharterSpaced.TextChanged -= QCharterSpacedObserve;
        QCharterPool.TextChanged -= QCharterPoolObserve;
        QCharterStreak.TextChanged -= QCharterStreakObserve;
        QCharterInterval.TextChanged -= QCharterIntervalObserve;
        QCharterQuorum.TextChanged -= QCharterQuorumObserve;
        QCharterCode.TextChanged -= QCharterCodeObserve;
    }

    private void QCharterObserveAttach()
    {
        QCharterLead.SelectionChanged += QCharterLeadObserve;
        QCharterSequence.SelectionChanged += QCharterOrderObserve;
        QCharterGoal.TextChanged += QCharterGoalObserve;
        QCharterSpaced.TextChanged += QCharterSpacedObserve;
        QCharterPool.TextChanged += QCharterPoolObserve;
        QCharterStreak.TextChanged += QCharterStreakObserve;
        QCharterInterval.TextChanged += QCharterIntervalObserve;
        QCharterQuorum.TextChanged += QCharterQuorumObserve;
        QCharterCode.TextChanged += QCharterCodeObserve;
    }

    private void QBearingItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QBearingItem bearing)
        {
            return;
        }

        QLook.QLookPartFind<TextBlock>(container, "PBearingCaption")?
            .SetResourceReference(TextBlock.TextProperty, bearing.QBearingItemCaption);
        QBearingSideRefine(container, "PBearingFront", bearing.QBearingItemFront, QBearingFrontObserve);
        QBearingSideRefine(container, "PBearingBack", bearing.QBearingItemBack, QBearingBackObserve);
        if (QLook.QLookPartFind<TextBox>(container, "PBearingStatistics") is TextBox set)
        {
            set.TextChanged -= QBearingStatisticsObserve;
            set.Text = bearing.QBearingItemStatistics.ToString(CultureInfo.CurrentCulture);
            set.TextChanged += QBearingStatisticsObserve;
        }
    }

    private static void QBearingSideRefine(
        FrameworkElement container, string part, int sides, RoutedEventHandler observe)
    {
        if (QLook.QLookPartFind<Panel>(container, part) is not Panel panel)
        {
            return;
        }

        int bit = 1;
        foreach (CheckBox side in panel.Children.OfType<CheckBox>())
        {
            side.IsChecked = (sides & bit) != 0;
            side.Click -= observe;
            side.Click += observe;
            bit <<= 1;
        }
    }

    private void QBearingFrontObserve(object sender, RoutedEventArgs e)
    {
        if (sender is CheckBox { Parent: Panel panel, DataContext: QBearingItem row } side)
        {
            _cCharter.CCharterFrontToggle(row.QBearingItemDirection, panel.Children.IndexOf(side));
        }
    }

    private void QBearingBackObserve(object sender, RoutedEventArgs e)
    {
        if (sender is CheckBox { Parent: Panel panel, DataContext: QBearingItem row } side)
        {
            _cCharter.CCharterBackToggle(row.QBearingItemDirection, panel.Children.IndexOf(side));
        }
    }

    private void QBearingStatisticsObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { DataContext: QBearingItem row } set)
        {
            _cCharter.CCharterStatisticsSet(row.QBearingItemDirection, set.Text);
        }
    }

    private void QCharterLeadObserve(object sender, SelectionChangedEventArgs e)
    {
        _cCharter.CCharterLeadSet(QCharterLead.SelectedIndex);
    }

    private void QCharterOrderObserve(object sender, SelectionChangedEventArgs e)
    {
        _cCharter.CCharterOrderSet(QCharterSequence.SelectedIndex);
    }

    private void QCharterGoalObserve(object sender, TextChangedEventArgs e)
    {
        _cCharter.CCharterGoalSet(QCharterGoal.Text);
    }

    private void QCharterSpacedObserve(object sender, TextChangedEventArgs e)
    {
        _cCharter.CCharterSpacedSet(QCharterSpaced.Text);
    }

    private void QCharterPoolObserve(object sender, TextChangedEventArgs e)
    {
        _cCharter.CCharterPoolSet(QCharterPool.Text);
    }

    private void QCharterStreakObserve(object sender, TextChangedEventArgs e)
    {
        _cCharter.CCharterStreakSet(QCharterStreak.Text);
    }

    private void QCharterIntervalObserve(object sender, TextChangedEventArgs e)
    {
        _cCharter.CCharterTargetSet(QCharterInterval.Text);
    }

    private void QCharterQuorumObserve(object sender, TextChangedEventArgs e)
    {
        _cCharter.CCharterQuorumSet(QCharterQuorum.Text);
    }

    private void QCharterCodeObserve(object sender, TextChangedEventArgs e)
    {
        _cCharter.CCharterCodeSet(QCharterCode.Text);
    }

    private void QCharterBasicObserve(object sender, RoutedEventArgs e)
    {
        _cCharter.CCharterBasicSet(QCharterBasic.IsChecked == true);
    }

    private void QCharterStoreObserve(object sender, RoutedEventArgs e)
    {
        _cCharter.CCharterSave();
    }
}
