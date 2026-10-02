using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QGlyph
{
    private readonly FrameworkElement _qGlyphSurface;

    private readonly QNotation _qNotation;

    private readonly ObservableCollection<QTranscriptionItem> _qGlyphItem = [];

    private CEditor _cEditor = null!;

    internal QGlyph(FrameworkElement surface, QNotation notation)
    {
        _qGlyphSurface = surface;
        _qNotation = notation;
        QGlyphList.ItemsSource = _qGlyphItem;
        QLookItem.QLookItemAttach(QGlyphList, QGlyphItemRefine);
        CommandBinding command = new(QGlyphCommand.QGlyphCommandNotation);
        command.Executed += QGlyphNotationRefine;
        command.Executed += QGlyphNotationObserve;
        QContract.QContractFind<StackPanel>(_qGlyphSurface, "PEditorSound").CommandBindings.Add(command);
    }

    private ItemsControl QGlyphList => QContract.QContractFind<ItemsControl>(_qGlyphSurface, "PGlyph");

    internal void QGlyphIntroduce(CEditor editor)
    {
        _cEditor = editor;
        editor.CEditorDraftChanged += QGlyphRefine;
    }

    private void QGlyphNotationRefine(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is QTranscriptionItem)
        {
            _qNotation.QNotationOpenRefine(e.OriginalSource as UIElement ?? QGlyphList);
        }
    }

    private void QGlyphNotationObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is QTranscriptionItem row)
        {
            _qNotation.QNotationStartRefine(
                _cEditor.CEditorDesk.CDeskErrand.CErrandTranscriptionStart(
                    row.QTranscriptionItemId, row.QTranscriptionItemScheme));
        }
    }

    private void QGlyphItemRefine(FrameworkElement container, object item, string? _)
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
            QGlyphMeasureRefine(measure, row.QTranscriptionItemText);
        }

        if (QLook.QLookPartFind<TextBox>(container, "PGlyphField") is TextBox field)
        {
            field.TextChanged -= QGlyphFieldObserve;
            field.Text = row.QTranscriptionItemText;
            field.SetResourceReference(QField.QFieldHintProperty, "Input.Glyph");
            field.TextChanged += QGlyphFieldObserve;
        }

        if (QLook.QLookPartFind<Button>(container, "PGlyphPhonetician") is Button phonetician)
        {
            phonetician.Visibility = QLook.QLookVisibleRead(QGlyphList.Tag is true);
        }
    }

    private static void QGlyphMeasureRefine(TextBlock measure, string text)
    {
        if (text.Length == 0)
        {
            measure.SetResourceReference(TextBlock.TextProperty, "Input.Glyph");
            return;
        }

        measure.Text = text;
    }

    private void QGlyphFieldObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { DataContext: QTranscriptionItem row } field)
        {
            _cEditor.CEditorTranscription.CTranscriptionSet(row.QTranscriptionItemId, field.Text);
        }
    }

    private void QGlyphRefine(CEntryDraft _)
    {
        CTimbreGlyph glyph = _cEditor.CEditorTimbre.CTimbreGlyphRead();
        QGlyphList.Visibility = QLook.QLookVisibleRead(glyph.CTimbreGlyphShown);
        QGlyphList.Tag = glyph.CTimbreGlyphSourced;

        PCard.PCardRowShow(
            _qGlyphItem,
            glyph.CTimbreGlyphRows,
            static row => row.QTranscriptionItemId,
            static spelled => spelled.CTranscriptionDraftId,
            QTranscriptionItem.QTranscriptionRowRefine,
            QTranscriptionItem.QTranscriptionStateRefine);
        QLookItem.QLookItemApply(QGlyphList);
    }
}
