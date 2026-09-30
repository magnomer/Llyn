using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private readonly ObservableCollection<QTranscriptionItem> _pTranscriptionItem = [];
    private bool _pTranscriptionFree;

    private ItemsControl PTranscription => (ItemsControl)FindName(nameof(PTranscription));

    private void PTranscriptionAttach()
    {
        PTranscription.ItemsSource = _pTranscriptionItem;
        QLookItem.QLookItemAttach(PTranscription, PTranscriptionRefine);
        PEditorSound.CommandBindings.Add(new CommandBinding(
            PTranscriptionCommand.PTranscriptionCommandAddition, PTranscriptionAddObserve, PTranscriptionAddRefine));
        PEditorSound.CommandBindings.Add(new CommandBinding(
            PTranscriptionCommand.PTranscriptionCommandRemoval, PTranscriptionRemoveObserve));
        CommandBinding notation = new(PTranscriptionCommand.PTranscriptionCommandNotation);
        notation.Executed += PTranscriptionNotationRefine;
        notation.Executed += PTranscriptionNotationObserve;
        PEditorSound.CommandBindings.Add(notation);
    }

    private void PTranscriptionRefine(FrameworkElement container, object item, string? name)
    {
        QTranscriptionItem.QTranscriptionItemRefine(container, item, name);
        if (item is not QTranscriptionItem row)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBox>(container, "PTranscriptionField") is TextBox field)
        {
            field.SetResourceReference(QField.QFieldHintProperty, "Input.Transcription");
            field.TextChanged -= PTranscriptionFieldObserve;
            QField.QFieldTextShow(field, row.QTranscriptionItemText);
            field.TextChanged += PTranscriptionFieldObserve;
        }

        if (QLook.QLookPartFind<ComboBox>(container, "PTranscriptionScheme") is not ComboBox scheme)
        {
            return;
        }

        scheme.SelectionChanged -= PTranscriptionSchemeObserve;
        scheme.DisplayMemberPath = nameof(QTranscriptionChoice.QTranscriptionChoiceLabel);
        scheme.SelectedValuePath = nameof(QTranscriptionChoice.QTranscriptionChoiceScheme);
        scheme.ItemsSource = row.QTranscriptionItemChoice;
        scheme.SelectedValue = row.QTranscriptionItemScheme;
        scheme.SelectionChanged += PTranscriptionSchemeObserve;
        QLookItem.QLookItemAttach(scheme, QTranscriptionChoice.QTranscriptionChoiceRefine);
        if (QLook.QLookPartFind<TextBlock>(scheme, "PSchemeName") is TextBlock label)
        {
            label.Text = row.QTranscriptionItemLabel;
        }
    }

    private void PTranscriptionFieldObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { DataContext: QTranscriptionItem row } field)
        {
            _qEditor.QEditorArea.CEditorTranscription.CTranscriptionSet(row.QTranscriptionItemId, field.Text);
        }
    }

    private void PTranscriptionSchemeObserve(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox { DataContext: QTranscriptionItem row, SelectedValue: string scheme })
        {
            _qEditor.QEditorArea.CEditorTranscription.CTranscriptionSchemeSet(row.QTranscriptionItemId, scheme);
        }
    }

    private void PTranscriptionAddObserve(object sender, ExecutedRoutedEventArgs e)
    {
        _qEditor.QEditorArea.CEditorTranscription.CTranscriptionAdd(
            e.Parameter is QTranscriptionItem row ? row.QTranscriptionItemId : 0);
    }

    private void PTranscriptionAddRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _pTranscriptionFree;
    }

    private void PTranscriptionRemoveObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is QTranscriptionItem row)
        {
            _qEditor.QEditorArea.CEditorTranscription.CTranscriptionRemove(row.QTranscriptionItemId);
        }
    }

    private void PTranscriptionNotationRefine(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is QTranscriptionItem)
        {
            PNotationOpenRefine(e.OriginalSource as UIElement ?? PTranscription);
        }
    }

    private void PTranscriptionNotationObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is QTranscriptionItem row)
        {
            PNotationStartRefine(
                _qEditor.QEditorArea.CEditorDesk.CDeskErrand.CErrandTranscriptionStart(
                    row.QTranscriptionItemId, row.QTranscriptionItemScheme));
        }
    }

    internal void PTranscriptionSheetRefine(CEntryDraft _)
    {
        CTranscriptionSheet sheet = _qEditor.QEditorArea.CEditorTranscription.CTranscriptionRead();
        PTranscription.Visibility = sheet.CTranscriptionSheetShown ? Visibility.Visible : Visibility.Collapsed;
        _pTranscriptionFree = sheet.CTranscriptionSheetFree;
        CommandManager.InvalidateRequerySuggested();

        PCard.PCardRowShow(
            _pTranscriptionItem,
            sheet.CTranscriptionSheetRows,
            static row => row.QTranscriptionItemId,
            static line => line.CTranscriptionRowDraft.CTranscriptionDraftId,
            static line => QTranscriptionItem.QTranscriptionRowRefine(line.CTranscriptionRowDraft),
            (row, line) => PTranscriptionStateRefine(row, line.CTranscriptionRowDraft));

        foreach (CTranscriptionRow line in sheet.CTranscriptionSheetRows)
        {
            foreach (QTranscriptionItem row in _pTranscriptionItem)
            {
                if (row.QTranscriptionItemId == line.CTranscriptionRowDraft.CTranscriptionDraftId)
                {
                    row.QTranscriptionSchemeRefine(line.CTranscriptionRowSchemes);
                }
            }
        }
    }

    private QTranscriptionItem PTranscriptionStateRefine(QTranscriptionItem row, CTranscriptionDraft spelled)
    {
        row.QTranscriptionItemKey = spelled.CTranscriptionDraftKey;
        row.QTranscriptionItemScheme = spelled.CTranscriptionDraftScheme;
        row.QTranscriptionItemText = spelled.CTranscriptionDraftText;
        return row;
    }
}
