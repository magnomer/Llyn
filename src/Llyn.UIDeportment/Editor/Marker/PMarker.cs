using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private readonly PMarkerTemplate _pMarkerTemplate;

    private WrapPanel PMarker => (WrapPanel)FindName(nameof(PMarker));

    private ItemsControl PMarkerList => (ItemsControl)FindName(nameof(PMarkerList));

    private Border PMarkerSurface => (Border)FindName(nameof(PMarkerSurface));

    private TextBox PMarkerField => (TextBox)FindName(nameof(PMarkerField));

    private ToggleButton PMarkerSwitch => (ToggleButton)FindName(nameof(PMarkerSwitch));

    private QIconImage PMarkerIcon => (QIconImage)FindName(nameof(PMarkerIcon));

    private void PMarkerAttach()
    {
        PMarkerList.ItemsSource = _pMarkerChip;
        QLookItem.QLookItemAttach(PMarkerList, PMarkerApply);
        PMarker.SizeChanged += (_, e) => PMarkerList.MaxWidth = e.NewSize.Width;
        PMarkerField.SetResourceReference(QField.QFieldHintProperty, "Speech.Title");
        PMarkerField.KeyDown += PMarkerCloseRefine;
        PMarkerField.KeyDown += PMarkerCommitObserve;
        PMarkerField.TextChanged += PMarkerTextObserve;
        PMarkerIcon.QIconSource = QIcon.QIconResolve("expand", 12);
    }

    private void PMarkerApply(FrameworkElement container, object item, string? _)
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
            eraser.Click -= PMarkerEraseObserve;
            eraser.Click += PMarkerEraseObserve;
        }
    }

    private readonly ObservableCollection<PMarkerChip> _pMarkerChip = [];

    private void PMarkerEraseObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PMarkerChip chip })
        {
            _qEditor.QEditorArea.CEditorSpeech.CCardSpeechRemove(chip.PMarkerChipName);
        }
    }

    private void PMarkerCloseRefine(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            PMarkerSwitch.IsChecked = false;
            e.Handled = true;
        }
    }

    private void PMarkerCommitObserve(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        _qEditor.QEditorArea.CEditorSpeech.CCardSpeechAdd(PMarkerField.Text);
        e.Handled = true;
        PMarkerFieldRefine();
    }

    private void PMarkerTextObserve(object sender, TextChangedEventArgs e)
    {
        PMarkerDropperRefine(_qEditor.QEditorArea.CEditorSpeech.CCardSpeechSet(PMarkerField.Text));
    }

    private void PMarkerDropperRefine(CCategory category)
    {
        PCategoryRefine(category);
        PMarkerSwitch.IsChecked = category.CCategoryShown;
    }

    private void PMarkerFieldRefine()
    {
        PMarkerField.Text = string.Empty;
    }

    internal void PMarkerRefine(CEntryDraft _)
    {
        CMarker marker = _qEditor.QEditorArea.CEditorSpeech.CCardSpeechRead();
        _pMarkerChip.Clear();
        foreach (string speech in marker.CMarkerSpeeches)
        {
            _pMarkerChip.Add(new PMarkerChip(speech));
        }

        QField.QFieldTextShow(PMarkerField, marker.CMarkerTyped);

        PCategoryRefine(marker.CMarkerCategory);
    }
}
