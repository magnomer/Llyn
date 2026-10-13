using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QSGalley
{
    private readonly Window _qsGalleySurface;

    private readonly CSPress _csPress;

    private readonly QSGalleyBar _qsGalleyBar;

    private readonly Dictionary<int, byte[]> _qsGalleyCache = [];

    private ObservableCollection<QSGalleyItem> _qsGalleyItem = [];

    private bool _qsGalleyGray;

    internal QSGalley(Window surface, CSPress press)
    {
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(press);

        _qsGalleySurface = surface;
        _csPress = press;
        _qsGalleyBar = new QSGalleyBar(surface, press.CSPressGalley.CSGalleyLens);
        QSGalleyList.ItemsSource = _qsGalleyItem;
        QLookItem.QLookItemAttach(QSGalleyList, QSGalleyItemRefine);
        QContract.QContractFind<QIconImage>(surface, "PSGalleyEarlierIcon").QIconSource =
            QIcon.QIconResolve("earlier", 24);
        QContract.QContractFind<QIconImage>(surface, "PSGalleyLaterIcon").QIconSource =
            QIcon.QIconResolve("later", 24);
        _qsGalleyBar.QSGalleyFactorChanged += QSGalleyFactorRefine;
        QSGalleyCanvas.ScrollChanged += QSGalleyScrollObserve;
        QSGalleyCanvas.ScrollChanged += QSGalleyScrollRefine;
        press.CSPressGalley.CSGalleyPageChanged += QSGalleyPageRefine;
        QSGalleyEarlier.Click += QSGalleyEarlierRefine;
        QSGalleyLater.Click += QSGalleyLaterRefine;
        QSGalleyNumber.KeyDown += QSGalleyNumberObserve;
        surface.PreviewKeyDown += QSGalleyKeyRefine;
        surface.DpiChanged += QSGalleyDpiObserve;
        surface.DpiChanged += QSGalleyDpiRefine;
    }

    private ScrollViewer QSGalleyCanvas =>
        QContract.QContractFind<ScrollViewer>(_qsGalleySurface, "PSGalleyCanvas");

    private ItemsControl QSGalleyList =>
        QContract.QContractFind<ItemsControl>(_qsGalleySurface, "PSGalleyList");

    private Button QSGalleyEarlier => QContract.QContractFind<Button>(_qsGalleySurface, "PSGalleyEarlier");

    private TextBox QSGalleyNumber => QContract.QContractFind<TextBox>(_qsGalleySurface, "PSGalleyNumber");

    private TextBlock QSGalleyTotal => QContract.QContractFind<TextBlock>(_qsGalleySurface, "PSGalleyTotal");

    private Button QSGalleyLater => QContract.QContractFind<Button>(_qsGalleySurface, "PSGalleyLater");

    private TextBlock QSGalleyCount => QContract.QContractFind<TextBlock>(_qsGalleySurface, "PSGalleyCount");

    internal void QSGalleyRefine(CSPressState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        int top = Math.Max(QSGalleySightDraw().QSGalleySightPage, 0);
        _qsGalleyItem = new ObservableCollection<QSGalleyItem>(Enumerable.Range(0, state.CSPressStateCount)
            .Select(index => new QSGalleyItem(index, state.CSPressStateWidth, state.CSPressStateHeight)));
        QSGalleyList.ItemsSource = _qsGalleyItem;
        _qsGalleyBar.QSGalleyLensRefine(state.CSPressStateLens);
        _qsGalleySurface.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, () => QSGalleyTurnRefine(top));
    }

    internal void QSGalleySettingRefine(CSPressSetting setting)
    {
        ArgumentNullException.ThrowIfNull(setting);

        bool gray = setting.CSPressSettingColor == CSPressColor.CSPressColorGray;
        if (gray != _qsGalleyGray)
        {
            _qsGalleyGray = gray;
            QSGalleyPictureRefine();
        }

        IReadOnlyList<bool> included = setting.CSPressSettingIncluded;
        foreach (QSGalleyItem item in _qsGalleyItem)
        {
            item.QSGalleyItemShaded =
                item.QSGalleyItemIndex < included.Count && !included[item.QSGalleyItemIndex];
        }

        QSGalleyCount.Text = setting.CSPressSettingSheets is int sheets
            ? string.Format(
                CultureInfo.CurrentCulture,
                QLocalizationCatalog.QLocalizationTextRead(setting.CSPressSettingTally),
                sheets)
            : string.Empty;
    }

    private QSGalleySight QSGalleySightDraw()
    {
        ScrollViewer canvas = QSGalleyCanvas;
        ItemsControl list = QSGalleyList;
        int top = -1;
        int first = -1;
        int last = -1;
        for (int index = 0; index < _qsGalleyItem.Count; index++)
        {
            if (list.ItemContainerGenerator.ContainerFromIndex(index) is not FrameworkElement container
                || !container.IsVisible)
            {
                continue;
            }

            Rect bounds = container.TransformToAncestor(canvas)
                .TransformBounds(new Rect(container.RenderSize));
            if (bounds.Bottom < 0 || bounds.Top > canvas.ViewportHeight)
            {
                if (first >= 0)
                {
                    break;
                }

                continue;
            }

            first = first < 0 ? index : first;
            last = index;
            top = top < 0 && bounds.Bottom > canvas.ViewportHeight / 3 ? index : top;
        }

        if (last >= 0 && canvas.ScrollableHeight > 0 && canvas.VerticalOffset >= canvas.ScrollableHeight - 1)
        {
            top = last;
        }

        return new QSGalleySight(top < 0 ? first : top, first, last, VisualTreeHelper.GetDpi(canvas).DpiScaleX);
    }

    private int QSGalleyWidthDraw()
    {
        ScrollViewer canvas = QSGalleyCanvas;
        ItemsControl list = QSGalleyList;
        double fit = canvas.ViewportWidth - list.Margin.Left - list.Margin.Right;
        double paper = _qsGalleyItem.FirstOrDefault()?.QSGalleyItemWidth ?? 0.0;
        double scale = VisualTreeHelper.GetDpi(canvas).DpiScaleX;
        return (int)Math.Round(Math.Min(paper * _qsGalleyBar.QSGalleyLens.CSLensStateFactor, fit * 2) * scale);
    }

    private ImageSource QSGalleyPictureDraw(byte[] png, int width)
    {
        BitmapImage picture = new();
        using (MemoryStream stream = new(png))
        {
            picture.BeginInit();
            picture.CacheOption = BitmapCacheOption.OnLoad;
            picture.DecodePixelWidth = Math.Max(width, 0);
            picture.StreamSource = stream;
            picture.EndInit();
        }

        picture.Freeze();
        if (!_qsGalleyGray)
        {
            return picture;
        }

        FormatConvertedBitmap gray = new(picture, PixelFormats.Gray8, null, 0);
        gray.Freeze();
        return gray;
    }

    private void QSGalleyScrollObserve(object sender, ScrollChangedEventArgs e)
    {
        QSGalleySight sight = QSGalleySightDraw();
        _ = _csPress.CSPressGalley.CSGalleySightSet(
            sight.QSGalleySightPage,
            sight.QSGalleySightFirst,
            sight.QSGalleySightLast,
            sight.QSGalleySightScale);
    }

    private void QSGalleyScrollRefine(object sender, ScrollChangedEventArgs e)
    {
        int top = Math.Max(QSGalleySightDraw().QSGalleySightPage, 0);
        if (!QSGalleyNumber.IsKeyboardFocusWithin)
        {
            QSGalleyNumber.Text = top < _qsGalleyItem.Count
                ? _qsGalleyItem[top].QSGalleyItemNumber
                : string.Empty;
        }

        QSGalleyTotal.Text = string.Format(
            CultureInfo.CurrentCulture,
            QLocalizationCatalog.QLocalizationTextRead("Print.PageTotal"),
            _qsGalleyItem.Count);
        QSGalleyEarlier.IsEnabled = top > 0;
        QSGalleyLater.IsEnabled = top < _qsGalleyItem.Count - 1;
    }

    private void QSGalleyPageRefine(CSPressPage page)
    {
        int index = page.CSPressPageIndex;
        if (index >= _qsGalleyItem.Count)
        {
            return;
        }

        if (page.CSPressPagePng is not byte[] png)
        {
            _qsGalleyCache.Remove(index);
            _qsGalleyItem[index].QSGalleyItemPicture = null;
            return;
        }

        _qsGalleyCache[index] = png;
        _qsGalleyItem[index].QSGalleyItemPicture =
            QSGalleyPictureDraw(png, QSGalleyWidthDraw());
    }

    private void QSGalleyPictureRefine()
    {
        int width = QSGalleyWidthDraw();
        foreach ((int page, byte[] png) in _qsGalleyCache)
        {
            if (page < _qsGalleyItem.Count)
            {
                _qsGalleyItem[page].QSGalleyItemPicture = QSGalleyPictureDraw(png, width);
            }
        }
    }

    private void QSGalleyItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QSGalleyItem sheet)
        {
            return;
        }

        if (QLook.QLookPartFind<Border>(container, "PSGalleyPaper") is Border paper)
        {
            paper.Width = sheet.QSGalleyItemWidth * _qsGalleyBar.QSGalleyLens.CSLensStateFactor;
            paper.Height = sheet.QSGalleyItemHeight * _qsGalleyBar.QSGalleyLens.CSLensStateFactor;
        }

        if (QLook.QLookPartFind<Image>(container, "PSGalleyPicture") is Image picture)
        {
            picture.Source = sheet.QSGalleyItemPicture;
        }

        Visibility shade = sheet.QSGalleyItemShaded ? Visibility.Visible : Visibility.Collapsed;
        if (QLook.QLookPartFind<Border>(container, "PSGalleyShade") is Border veil)
        {
            veil.Visibility = shade;
        }

        if (QLook.QLookPartFind<Border>(container, "PSGalleyBadge") is Border badge)
        {
            badge.Visibility = shade;
        }
    }

    private void QSGalleyFactorRefine(double before)
    {
        double factor = _qsGalleyBar.QSGalleyLens.CSLensStateFactor;
        QLookItem.QLookItemApply(QSGalleyList);
        if (Math.Abs(factor - before) > double.Epsilon)
        {
            ScrollViewer canvas = QSGalleyCanvas;
            canvas.UpdateLayout();
            canvas.ScrollToVerticalOffset(canvas.VerticalOffset * factor / before);
        }

        QSGalleyPictureRefine();
    }

    private void QSGalleyDpiObserve(object sender, DpiChangedEventArgs e)
    {
        QSGalleySight sight = QSGalleySightDraw();
        _ = _csPress.CSPressGalley.CSGalleySightSet(
            sight.QSGalleySightPage,
            sight.QSGalleySightFirst,
            sight.QSGalleySightLast,
            sight.QSGalleySightScale);
    }

    private void QSGalleyDpiRefine(object sender, DpiChangedEventArgs e)
    {
        QSGalleyPictureRefine();
    }

    private void QSGalleyEarlierRefine(object sender, RoutedEventArgs e)
    {
        QSGalleyTurnRefine(Math.Max(QSGalleySightDraw().QSGalleySightPage, 0) - 1);
    }

    private void QSGalleyLaterRefine(object sender, RoutedEventArgs e)
    {
        QSGalleyTurnRefine(Math.Max(QSGalleySightDraw().QSGalleySightPage, 0) + 1);
    }

    private void QSGalleyNumberObserve(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            e.Handled = true;
            QSGalleyNumberRefine(_csPress.CSPressPageSelect(QSGalleyNumber.Text));
        }
    }

    private void QSGalleyNumberRefine(CSPressSetting setting)
    {
        int page = setting.CSPressSettingPage;
        QSGalleyNumber.Text = page < _qsGalleyItem.Count
            ? _qsGalleyItem[page].QSGalleyItemNumber
            : string.Empty;
        QSGalleyNumber.SelectAll();
        QSGalleyTurnRefine(page);
    }

    private void QSGalleyTurnRefine(int page)
    {
        if (_qsGalleyItem.Count == 0)
        {
            return;
        }

        int index = Math.Clamp(page, 0, _qsGalleyItem.Count - 1);
        if (QSGalleyList.ItemContainerGenerator.ContainerFromIndex(index) is FrameworkElement container)
        {
            ScrollViewer canvas = QSGalleyCanvas;
            double offset = container.TransformToAncestor(canvas).Transform(default).Y;
            canvas.ScrollToVerticalOffset(canvas.VerticalOffset + offset - QSGalleyList.Margin.Top);
        }
    }

    private void QSGalleyKeyRefine(object sender, KeyEventArgs e)
    {
        if (Keyboard.Modifiers != ModifierKeys.None
            || Keyboard.FocusedElement is TextBoxBase or ComboBox or ComboBoxItem)
        {
            return;
        }

        int top = Math.Max(QSGalleySightDraw().QSGalleySightPage, 0);
        switch (e.Key)
        {
            case Key.Up or Key.Left or Key.PageUp:
                QSGalleyTurnRefine(top - 1);
                break;
            case Key.Down or Key.Right or Key.PageDown:
                QSGalleyTurnRefine(top + 1);
                break;
            case Key.Home:
                QSGalleyTurnRefine(0);
                break;
            case Key.End:
                QSGalleyTurnRefine(_qsGalleyItem.Count - 1);
                break;
            default:
                return;
        }

        e.Handled = true;
    }
}
