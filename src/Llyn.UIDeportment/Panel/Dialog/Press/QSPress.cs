using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;
using Llyn.UIDeportment.Capsule;

namespace Llyn.UIDeportment;

internal sealed class QSPress
{
    private readonly Window _qsPressSurface;

    private readonly CSPress _csPress;

    private readonly QPosture _qsPressPosture;

    private readonly QSGalley _qsPressGalley;

    private readonly QSPressDevice _qsPressDevice;

    private readonly QSPressLayout _qsPressLayout;

    private QSPress(Window owner, CSPress press, QPosture posture)
    {
        _qsPressSurface = QContract.QContractSheetFind<Window>("PSPress");
        _qsPressSurface.Owner = owner;
        QLook.QLookStyleAttach(_qsPressSurface);
        _csPress = press;
        _qsPressPosture = posture;
        _qsPressGalley = new QSGalley(_qsPressSurface, press);
        _qsPressDevice = new QSPressDevice(_qsPressSurface, press);
        _qsPressLayout = new QSPressLayout(_qsPressSurface, press);
        if (posture.QPostureRead().LCapsuleContentPress is LCapsulePress stored)
        {
            _qsPressSurface.Width = stored.LCapsulePressWidth;
            _qsPressSurface.Height = stored.LCapsulePressHeight;
        }

        QSPressSettingRefine(press.CSPressSettingRead());
        press.CSPressSettingChanged += QSPressSettingRefine;
        press.CSPressGalley.CSGalleyStateChanged += QSPressStateRefine;
        _qsPressSurface.ContentRendered += QSPressShownObserve;
        QSPressRetry.Click += QSPressRetryObserve;
        QSPressAdmit.Click += QSPressAdmitObserve;
        QSPressSystem.Click += QSPressSystemObserve;
        QSPressQuit.Click += QSPressQuitObserve;
        _qsPressSurface.PreviewKeyDown += QSPressKeyObserve;
        _qsPressSurface.Closing += QSPressClosingObserve;
    }

    private Grid QSPressBusy => QContract.QContractFind<Grid>(_qsPressSurface, "PSPressBusy");

    private Grid QSPressFailure => QContract.QContractFind<Grid>(_qsPressSurface, "PSPressFailure");

    private TextBlock QSPressMessage => QContract.QContractFind<TextBlock>(_qsPressSurface, "PSPressMessage");

    private Button QSPressRetry => QContract.QContractFind<Button>(_qsPressSurface, "PSPressRetry");

    private Button QSPressSystem => QContract.QContractFind<Button>(_qsPressSurface, "PSPressSystem");

    private TextBlock QSPressAlert => QContract.QContractFind<TextBlock>(_qsPressSurface, "PSPressAlert");

    private Button QSPressAdmit => QContract.QContractFind<Button>(_qsPressSurface, "PSPressAdmit");

    private Button QSPressQuit => QContract.QContractFind<Button>(_qsPressSurface, "PSPressQuit");

    internal static bool QSPressConsult(Window owner, CSPress press, QPosture posture)
    {
        ArgumentNullException.ThrowIfNull(owner);
        ArgumentNullException.ThrowIfNull(press);
        ArgumentNullException.ThrowIfNull(posture);

        return new QSPress(owner, press, posture)._qsPressSurface.ShowDialog() == true;
    }

    internal static void QSPressFieldRefine(TextBox field, string? text)
    {
        if (!field.IsKeyboardFocusWithin)
        {
            field.Text = text ?? string.Empty;
        }
    }

    internal static void QSPressItemRefine<QSPressKind>(
        ComboBox choice, IReadOnlyList<CSPressChoice<QSPressKind>>? rows)
    {
        List<ComboBoxItem> items = (rows ?? [])
            .Select(static row => new ComboBoxItem
            {
                Content = QLocalizationCatalog.QLocalizationTextRead(row.CSPressChoiceKey),
                IsSelected = row.CSPressChoiceChosen,
            })
            .ToList();
        choice.ItemsSource = items;
        choice.SelectedItem = items.Find(static item => item.IsSelected);
    }

    private void QSPressSettingRefine(CSPressSetting setting)
    {
        _qsPressDevice.QSPressDeviceRefine(setting);
        _qsPressLayout.QSPressLayoutRefine(setting);
        string action = QLocalizationCatalog.QLocalizationTextRead(setting.CSPressSettingAction);
        QSPressAdmit.Content = action;
        AutomationProperties.SetName(QSPressAdmit, action);
        QSPressAdmit.IsEnabled = setting.CSPressSettingIdle;
        QSPressQuit.IsEnabled = setting.CSPressSettingIdle;
        QSPressAlert.Text = QLocalizationCatalog.QLocalizationTextRead(setting.CSPressSettingBusy);
        _qsPressGalley.QSGalleySettingRefine(setting);
        if (setting.CSPressSettingStale)
        {
            QSPressBusy.Visibility = Visibility.Visible;
            QSPressFailure.Visibility = Visibility.Collapsed;
        }
    }

    private void QSPressStateRefine(CSPressState state)
    {
        _qsPressGalley.QSGalleyRefine(state);
        QSPressBusy.Visibility = Visibility.Collapsed;
        QSPressFailure.Visibility = state.CSPressStateFailure is null ? Visibility.Collapsed : Visibility.Visible;
        if (state.CSPressStateFailure is string failure)
        {
            QSPressMessage.Text = QLocalizationCatalog.QLocalizationTextRead(failure);
        }

        QSPressSettingRefine(state.CSPressStateSetting);
    }

    private void QSPressAlertRefine(string? key)
    {
        QSPressAlert.Text = key is null ? string.Empty : QLocalizationCatalog.QLocalizationTextRead(key);
    }

    private async void QSPressShownObserve(object? sender, EventArgs e)
    {
        await _csPress.CSPressRestore();
    }

    private async void QSPressRetryObserve(object sender, RoutedEventArgs e)
    {
        if (await _csPress.CSPressGalley.CSGalleyDraw() is CSPressState state)
        {
            QSPressStateRefine(state);
        }
    }

    private async void QSPressAdmitObserve(object sender, RoutedEventArgs e)
    {
        QSPressAdmitRefine(await _csPress.CSPressPrint());
    }

    private async void QSPressKeyObserve(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.P || Keyboard.Modifiers != ModifierKeys.Control)
        {
            return;
        }

        e.Handled = true;
        QSPressAdmitRefine(await _csPress.CSPressPrint());
    }

    private void QSPressAdmitRefine(string? key)
    {
        if (key is null)
        {
            _qsPressSurface.DialogResult = true;
            return;
        }

        QSPressAlertRefine(key);
    }

    private async void QSPressSystemObserve(object sender, RoutedEventArgs e)
    {
        QSPressAlertRefine(await _csPress.CSPressSystemShow());
    }

    private void QSPressQuitObserve(object sender, RoutedEventArgs e)
    {
        _qsPressSurface.DialogResult = false;
    }

    private void QSPressClosingObserve(object? sender, CancelEventArgs e)
    {
        e.Cancel = !_csPress.CSPressClose(e.Cancel);
        _qsPressPosture.QPosturePressSave(new LCapsulePress(_qsPressSurface.Width, _qsPressSurface.Height));
    }
}
