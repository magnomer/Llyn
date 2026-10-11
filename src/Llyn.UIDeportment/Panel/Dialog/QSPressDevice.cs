using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QSPressDevice
{
    private readonly Window _qsPressSurface;

    private readonly CSPress _csPress;

    internal QSPressDevice(Window surface, CSPress press)
    {
        _qsPressSurface = surface;
        _csPress = press;
        QSPressDestination.SelectionChanged += QSPressDestinationObserve;
        QSPressCopies.TextChanged += QSPressCopiesObserve;
        QSPressCollation.Click += QSPressCollationObserve;
        QSPressFull.Click += QSPressFullObserve;
        QSPressGray.Click += QSPressGrayObserve;
        QSPressSide.SelectionChanged += QSPressSideObserve;
    }

    private ComboBox QSPressDestination =>
        QContract.QContractFind<ComboBox>(_qsPressSurface, "PSPressDestination");

    private TextBlock QSPressNotice => QContract.QContractFind<TextBlock>(_qsPressSurface, "PSPressNotice");

    private TextBox QSPressCopies => QContract.QContractFind<TextBox>(_qsPressSurface, "PSPressCopies");

    private ToggleButton QSPressCollation =>
        QContract.QContractFind<ToggleButton>(_qsPressSurface, "PSPressCollation");

    private RadioButton QSPressFull => QContract.QContractFind<RadioButton>(_qsPressSurface, "PSPressFull");

    private RadioButton QSPressGray => QContract.QContractFind<RadioButton>(_qsPressSurface, "PSPressGray");

    private ComboBox QSPressSide => QContract.QContractFind<ComboBox>(_qsPressSurface, "PSPressSide");

    internal void QSPressDeviceRefine(CSPressSetting setting)
    {
        QSPressDestination.SelectionChanged -= QSPressDestinationObserve;
        QSPressCopies.TextChanged -= QSPressCopiesObserve;
        QSPressSide.SelectionChanged -= QSPressSideObserve;
        try
        {
            List<ComboBoxItem> printers = setting.CSPressSettingPrinter
                .Select(static printer => new ComboBoxItem
                {
                    Content = string.Format(
                        CultureInfo.CurrentCulture,
                        QLocalizationCatalog.QLocalizationTextRead(printer.CSPressPrinterText),
                        printer.CSPressPrinterName),
                    IsSelected = printer.CSPressPrinterChosen,
                })
                .ToList();
            QSPressDestination.ItemsSource = printers;
            QSPressDestination.SelectedItem = printers.Find(static item => item.IsSelected);
            QSPressNotice.Visibility =
                setting.CSPressSettingHint is null ? Visibility.Collapsed : Visibility.Visible;
            QSPressNotice.Text = setting.CSPressSettingHint is string hint
                ? QLocalizationCatalog.QLocalizationTextRead(hint)
                : string.Empty;

            QSPressCopies.IsEnabled = setting.CSPressSettingCopies is not null;
            QSPress.QSPressFieldRefine(
                QSPressCopies, setting.CSPressSettingCopies?.ToString(CultureInfo.CurrentCulture));
            QSPressCollation.IsEnabled = setting.CSPressSettingCollation is not null;
            QSPressCollation.IsChecked = setting.CSPressSettingCollation is true;

            QSPressFull.IsEnabled = setting.CSPressSettingColor is not null;
            QSPressGray.IsEnabled = setting.CSPressSettingColor is not null;
            QSPressFull.IsChecked = setting.CSPressSettingColor == CSPressColor.CSPressColorFull;
            QSPressGray.IsChecked = setting.CSPressSettingColor == CSPressColor.CSPressColorGray;
            QSPressSide.IsEnabled = setting.CSPressSettingSide is not null;
            QSPress.QSPressItemRefine(QSPressSide, setting.CSPressSettingSide);
        }
        finally
        {
            QSPressDestination.SelectionChanged += QSPressDestinationObserve;
            QSPressCopies.TextChanged += QSPressCopiesObserve;
            QSPressSide.SelectionChanged += QSPressSideObserve;
        }
    }

    private void QSPressDestinationObserve(object sender, SelectionChangedEventArgs e)
    {
        _csPress.CSPressPrinterSelect(QSPressDestination.SelectedIndex);
    }

    private void QSPressCopiesObserve(object sender, TextChangedEventArgs e)
    {
        _csPress.CSPressCopiesSet(QSPressCopies.Text);
    }

    private void QSPressCollationObserve(object sender, RoutedEventArgs e)
    {
        _csPress.CSPressCollationSet(QSPressCollation.IsChecked is true);
    }

    private void QSPressFullObserve(object sender, RoutedEventArgs e)
    {
        _csPress.CSPressColorSet(CSPressColor.CSPressColorFull);
    }

    private void QSPressGrayObserve(object sender, RoutedEventArgs e)
    {
        _csPress.CSPressColorSet(CSPressColor.CSPressColorGray);
    }

    private void QSPressSideObserve(object sender, SelectionChangedEventArgs e)
    {
        _csPress.CSPressSideSelect(QSPressSide.SelectedIndex);
    }
}
