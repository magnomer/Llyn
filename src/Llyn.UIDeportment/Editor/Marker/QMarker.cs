using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QMarker
{
    private readonly FrameworkElement _qMarkerSurface;

    private readonly ObservableCollection<PMarkerChip> _qMarkerChip = [];

    private CEditor _cEditor = null!;

    private QCategory _qMarkerCategory = null!;

    internal QMarker(FrameworkElement surface)
    {
        _qMarkerSurface = surface;
        QMarkerList.ItemsSource = _qMarkerChip;
        QLookItem.QLookItemAttach(QMarkerList, QMarkerApply);
        QMarkerPanel.SizeChanged += (_, e) => QMarkerList.MaxWidth = e.NewSize.Width;
        QMarkerField.SetResourceReference(QField.QFieldHintProperty, "Speech.Title");
        QMarkerField.KeyDown += QMarkerCloseRefine;
        QMarkerField.KeyDown += QMarkerCommitObserve;
        QMarkerField.TextChanged += QMarkerTextObserve;
        QMarkerIcon.QIconSource = QIcon.QIconResolve("expand", 12);
    }

    private WrapPanel QMarkerPanel => QContract.QContractFind<WrapPanel>(_qMarkerSurface, "PMarker");

    private ItemsControl QMarkerList => QContract.QContractFind<ItemsControl>(_qMarkerSurface, "PMarkerList");

    private TextBox QMarkerField => QContract.QContractFind<TextBox>(_qMarkerSurface, "PMarkerField");

    private ToggleButton QMarkerSwitch => QContract.QContractFind<ToggleButton>(_qMarkerSurface, "PMarkerSwitch");

    private QIconImage QMarkerIcon => QContract.QContractFind<QIconImage>(_qMarkerSurface, "PMarkerIcon");

    internal void QMarkerIntroduce(CEditor editor, QCategory category)
    {
        _cEditor = editor;
        _qMarkerCategory = category;
        editor.CEditorDraftChanged += QMarkerRefine;
    }

    private void QMarkerApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PMarkerChip chip)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PMarkerName") is TextBlock name)
        {
            name.Text = chip.PMarkerChipName;
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PMarkerIcon") is QIconImage icon)
        {
            icon.QIconSource = QIcon.QIconResolve("close", 12);
        }

        if (QLook.QLookPartFind<Button>(container, "PMarkerEraser") is Button eraser)
        {
            eraser.Click -= QMarkerEraseObserve;
            eraser.Click += QMarkerEraseObserve;
        }
    }

    private void QMarkerEraseObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PMarkerChip chip })
        {
            _cEditor.CEditorSpeech.CCardSpeechRemove(chip.PMarkerChipName);
        }
    }

    private void QMarkerCloseRefine(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            QMarkerSwitch.IsChecked = false;
            e.Handled = true;
        }
    }

    private void QMarkerCommitObserve(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        _cEditor.CEditorSpeech.CCardSpeechAdd(QMarkerField.Text);
        e.Handled = true;
        QMarkerFieldRefine();
    }

    private void QMarkerTextObserve(object sender, TextChangedEventArgs e)
    {
        QMarkerDropperRefine(_cEditor.CEditorSpeech.CCardSpeechSet(QMarkerField.Text));
    }

    private void QMarkerDropperRefine(CCategory category)
    {
        _qMarkerCategory.QCategoryRefine(category);
        QMarkerSwitch.IsChecked = category.CCategoryShown;
    }

    internal void QMarkerFieldRefine()
    {
        QMarkerField.Text = string.Empty;
    }

    private void QMarkerRefine(CEntryDraft _)
    {
        CMarker marker = _cEditor.CEditorSpeech.CCardSpeechRead();
        _qMarkerChip.Clear();
        foreach (string speech in marker.CMarkerSpeeches)
        {
            _qMarkerChip.Add(new PMarkerChip(speech));
        }

        QField.QFieldTextShow(QMarkerField, marker.CMarkerTyped);

        _qMarkerCategory.QCategoryRefine(marker.CMarkerCategory);
    }
}
