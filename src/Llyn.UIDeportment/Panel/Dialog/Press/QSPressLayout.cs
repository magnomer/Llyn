using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QSPressLayout
{
    private readonly Window _qsPressSurface;

    private readonly CSPress _csPress;

    internal QSPressLayout(Window surface, CSPress press)
    {
        _qsPressSurface = surface;
        _csPress = press;
        QSPressRange.SelectionChanged += QSPressRangeObserve;
        QSPressPages.TextChanged += QSPressPagesObserve;
        QSPressUpright.Click += QSPressUprightObserve;
        QSPressLandscape.Click += QSPressLandscapeObserve;
        QSPressPaper.SelectionChanged += QSPressPaperObserve;
        QSPressMargin.SelectionChanged += QSPressMarginObserve;
        QSPressMarginTop.TextChanged += QSPressTopObserve;
        QSPressMarginBottom.TextChanged += QSPressBottomObserve;
        QSPressMarginLeft.TextChanged += QSPressLeftObserve;
        QSPressMarginRight.TextChanged += QSPressRightObserve;
        QSPressScale.TextChanged += QSPressScaleObserve;
        QSPressBackground.Click += QSPressBackgroundObserve;
    }

    private ComboBox QSPressRange => QContract.QContractFind<ComboBox>(_qsPressSurface, "PSPressRange");

    private TextBox QSPressPages => QContract.QContractFind<TextBox>(_qsPressSurface, "PSPressPages");

    private TextBlock QSPressWarning => QContract.QContractFind<TextBlock>(_qsPressSurface, "PSPressWarning");

    private RadioButton QSPressUpright =>
        QContract.QContractFind<RadioButton>(_qsPressSurface, "PSPressUpright");

    private RadioButton QSPressLandscape =>
        QContract.QContractFind<RadioButton>(_qsPressSurface, "PSPressLandscape");

    private ComboBox QSPressPaper => QContract.QContractFind<ComboBox>(_qsPressSurface, "PSPressPaper");

    private ComboBox QSPressMargin => QContract.QContractFind<ComboBox>(_qsPressSurface, "PSPressMargin");

    private Grid QSPressInset => QContract.QContractFind<Grid>(_qsPressSurface, "PSPressInset");

    private TextBox QSPressMarginTop => QContract.QContractFind<TextBox>(_qsPressSurface, "PSPressMarginTop");

    private TextBox QSPressMarginBottom =>
        QContract.QContractFind<TextBox>(_qsPressSurface, "PSPressMarginBottom");

    private TextBox QSPressMarginLeft =>
        QContract.QContractFind<TextBox>(_qsPressSurface, "PSPressMarginLeft");

    private TextBox QSPressMarginRight =>
        QContract.QContractFind<TextBox>(_qsPressSurface, "PSPressMarginRight");

    private TextBlock QSPressMeasure => QContract.QContractFind<TextBlock>(_qsPressSurface, "PSPressMeasure");

    private TextBox QSPressScale => QContract.QContractFind<TextBox>(_qsPressSurface, "PSPressScale");

    private ToggleButton QSPressBackground =>
        QContract.QContractFind<ToggleButton>(_qsPressSurface, "PSPressBackground");

    internal void QSPressLayoutRefine(CSPressSetting setting)
    {
        QSPressRange.SelectionChanged -= QSPressRangeObserve;
        QSPressPages.TextChanged -= QSPressPagesObserve;
        QSPressPaper.SelectionChanged -= QSPressPaperObserve;
        QSPressMargin.SelectionChanged -= QSPressMarginObserve;
        QSPressMarginTop.TextChanged -= QSPressTopObserve;
        QSPressMarginBottom.TextChanged -= QSPressBottomObserve;
        QSPressMarginLeft.TextChanged -= QSPressLeftObserve;
        QSPressMarginRight.TextChanged -= QSPressRightObserve;
        QSPressScale.TextChanged -= QSPressScaleObserve;
        try
        {
            QSPress.QSPressItemRefine(QSPressRange, setting.CSPressSettingRange);
            QSPressPages.IsEnabled = setting.CSPressSettingText is not null;
            QSPress.QSPressFieldRefine(QSPressPages, setting.CSPressSettingText);
            QSPressWarning.Visibility =
                setting.CSPressSettingFailure is null ? Visibility.Collapsed : Visibility.Visible;
            QSPressWarning.Text = setting.CSPressSettingFailure is string failure
                ? QLocalizationCatalog.QLocalizationTextRead(failure)
                : string.Empty;

            QSPressUpright.IsChecked =
                setting.CSPressSettingOrientation == CSPressOrientation.CSPressOrientationPortrait;
            QSPressLandscape.IsChecked =
                setting.CSPressSettingOrientation == CSPressOrientation.CSPressOrientationLandscape;
            List<ComboBoxItem> papers = setting.CSPressSettingPaper
                .Select(static paper => new ComboBoxItem
                {
                    Content = string.Format(
                        CultureInfo.CurrentCulture,
                        QLocalizationCatalog.QLocalizationTextRead(paper.CSPressPaperKey),
                        paper.CSPressPaperWidth,
                        paper.CSPressPaperHeight),
                    IsSelected = paper.CSPressPaperChosen,
                })
                .ToList();
            QSPressPaper.ItemsSource = papers;
            QSPressPaper.SelectedItem = papers.Find(static item => item.IsSelected);

            QSPress.QSPressItemRefine(QSPressMargin, setting.CSPressSettingMargin);
            QSPressInset.Visibility =
                setting.CSPressSettingTop is null ? Visibility.Collapsed : Visibility.Visible;
            QSPress.QSPressFieldRefine(
                QSPressMarginTop, setting.CSPressSettingTop?.ToString(CultureInfo.CurrentCulture));
            QSPress.QSPressFieldRefine(
                QSPressMarginBottom, setting.CSPressSettingBottom?.ToString(CultureInfo.CurrentCulture));
            QSPress.QSPressFieldRefine(
                QSPressMarginLeft, setting.CSPressSettingLeft?.ToString(CultureInfo.CurrentCulture));
            QSPress.QSPressFieldRefine(
                QSPressMarginRight, setting.CSPressSettingRight?.ToString(CultureInfo.CurrentCulture));
            QSPressMeasure.Text = QLocalizationCatalog.QLocalizationTextRead(setting.CSPressSettingUnit);
            QSPress.QSPressFieldRefine(
                QSPressScale, setting.CSPressSettingScale.ToString(CultureInfo.CurrentCulture));
            QSPressBackground.IsChecked = setting.CSPressSettingBackground;
        }
        finally
        {
            QSPressRange.SelectionChanged += QSPressRangeObserve;
            QSPressPages.TextChanged += QSPressPagesObserve;
            QSPressPaper.SelectionChanged += QSPressPaperObserve;
            QSPressMargin.SelectionChanged += QSPressMarginObserve;
            QSPressMarginTop.TextChanged += QSPressTopObserve;
            QSPressMarginBottom.TextChanged += QSPressBottomObserve;
            QSPressMarginLeft.TextChanged += QSPressLeftObserve;
            QSPressMarginRight.TextChanged += QSPressRightObserve;
            QSPressScale.TextChanged += QSPressScaleObserve;
        }
    }

    private void QSPressRangeObserve(object sender, SelectionChangedEventArgs e)
    {
        _csPress.CSPressRangeSelect(QSPressRange.SelectedIndex);
    }

    private void QSPressPagesObserve(object sender, TextChangedEventArgs e)
    {
        _csPress.CSPressRangeSet(QSPressPages.Text);
    }

    private void QSPressUprightObserve(object sender, RoutedEventArgs e)
    {
        _csPress.CSPressOrientationSet(CSPressOrientation.CSPressOrientationPortrait);
    }

    private void QSPressLandscapeObserve(object sender, RoutedEventArgs e)
    {
        _csPress.CSPressOrientationSet(CSPressOrientation.CSPressOrientationLandscape);
    }

    private void QSPressPaperObserve(object sender, SelectionChangedEventArgs e)
    {
        _csPress.CSPressPaperSelect(QSPressPaper.SelectedIndex);
    }

    private void QSPressMarginObserve(object sender, SelectionChangedEventArgs e)
    {
        _csPress.CSPressMarginSelect(QSPressMargin.SelectedIndex);
    }

    private void QSPressTopObserve(object sender, TextChangedEventArgs e)
    {
        _csPress.CSPressTopSet(QSPressMarginTop.Text);
    }

    private void QSPressBottomObserve(object sender, TextChangedEventArgs e)
    {
        _csPress.CSPressBottomSet(QSPressMarginBottom.Text);
    }

    private void QSPressLeftObserve(object sender, TextChangedEventArgs e)
    {
        _csPress.CSPressLeftSet(QSPressMarginLeft.Text);
    }

    private void QSPressRightObserve(object sender, TextChangedEventArgs e)
    {
        _csPress.CSPressRightSet(QSPressMarginRight.Text);
    }

    private void QSPressScaleObserve(object sender, TextChangedEventArgs e)
    {
        _csPress.CSPressScaleSet(QSPressScale.Text);
    }

    private void QSPressBackgroundObserve(object sender, RoutedEventArgs e)
    {
        _csPress.CSPressBackgroundSet(QSPressBackground.IsChecked is true);
    }
}
