using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private readonly ObservableCollection<QTranscriptionItem> _pGlyphItem = [];

    private ItemsControl PGlyph => (ItemsControl)FindName(nameof(PGlyph));

    private void PGlyphIntroduce()
    {
        PGlyph.ItemsSource = _pGlyphItem;
        QLookItem.QLookItemAttach(PGlyph, PGlyphItemRefine);
        CommandBinding notation = new(PGlyphCommand.PGlyphCommandNotation);
        notation.Executed += PGlyphNotationRefine;
        notation.Executed += PGlyphNotationObserve;
        PEditorSound.CommandBindings.Add(notation);
    }

    private void PGlyphNotationRefine(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is QTranscriptionItem)
        {
            PNotationOpenRefine(e.OriginalSource as UIElement ?? PGlyph);
        }
    }

    private void PGlyphNotationObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is QTranscriptionItem row)
        {
            PNotationStartRefine(
                _qEditor.QEditorArea.CEditorDesk.CDeskErrand.CErrandTranscriptionStart(
                    row.QTranscriptionItemId, row.QTranscriptionItemScheme));
        }
    }

    private void PGlyphItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QTranscriptionItem row)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PGlyphLabel") is TextBlock label)
        {
            label.Text = row.QTranscriptionItemLabel;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PGlyphMeasure") is TextBlock measure)
        {
            PGlyphMeasureRefine(measure, row.QTranscriptionItemText);
        }

        if (QLook.QLookPartFind<TextBox>(container, "PGlyphField") is TextBox field)
        {
            field.TextChanged -= PGlyphFieldObserve;
            field.Text = row.QTranscriptionItemText;
            field.SetResourceReference(QField.QFieldHintProperty, "Input.Glyph");
            field.TextChanged += PGlyphFieldObserve;
        }

        if (QLook.QLookPartFind<Button>(container, "PGlyphPhonetician") is Button phonetician)
        {
            phonetician.Visibility = QLook.QLookVisibleRead(PGlyph.Tag is true);
        }
    }

    private static void PGlyphMeasureRefine(TextBlock measure, string text)
    {
        if (text.Length == 0)
        {
            measure.SetResourceReference(TextBlock.TextProperty, "Input.Glyph");
            return;
        }

        measure.Text = text;
    }

    private void PGlyphFieldObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { DataContext: QTranscriptionItem row } field)
        {
            _qEditor.QEditorArea.CEditorTranscription.CTranscriptionSet(row.QTranscriptionItemId, field.Text);
        }
    }

    internal void PGlyphRefine(CEntryDraft _)
    {
        CTimbreGlyph glyph = _qEditor.QEditorArea.CEditorTimbre.CTimbreGlyphRead();
        PGlyph.Visibility = QLook.QLookVisibleRead(glyph.CTimbreGlyphShown);
        PGlyph.Tag = glyph.CTimbreGlyphSourced;

        PCard.PCardRowShow(
            _pGlyphItem,
            glyph.CTimbreGlyphRows,
            static row => row.QTranscriptionItemId,
            static spelled => spelled.CTranscriptionDraftId,
            QTranscriptionItem.QTranscriptionRowRefine,
            PTranscriptionStateRefine);
        QLookItem.QLookItemApply(PGlyph);
    }
}
