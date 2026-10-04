using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QTranscription
{
    private readonly FrameworkElement _qTranscriptionSurface;

    private readonly QNotation _qNotation;

    private readonly ObservableCollection<QTranscriptionItem> _qTranscriptionItem = [];

    private CEditor _cEditor = null!;

    private bool _qTranscriptionFree;

    internal QTranscription(FrameworkElement surface, QNotation notation)
    {
        _qTranscriptionSurface = surface;
        _qNotation = notation;
        QTranscriptionList.ItemsSource = _qTranscriptionItem;
        QLookItem.QLookItemAttach(QTranscriptionList, QTranscriptionRefine);
        StackPanel sound = QContract.QContractFind<StackPanel>(_qTranscriptionSurface, "PEditorSound");
        sound.CommandBindings.Add(new CommandBinding(
            QTranscriptionCommand.QTranscriptionCommandAddition, QTranscriptionAddObserve, QTranscriptionAddRefine));
        sound.CommandBindings.Add(new CommandBinding(
            QTranscriptionCommand.QTranscriptionCommandRemoval, QTranscriptionRemoveObserve));
        CommandBinding command = new(QTranscriptionCommand.QTranscriptionCommandNotation);
        command.Executed += QTranscriptionNotationRefine;
        command.Executed += QTranscriptionNotationObserve;
        sound.CommandBindings.Add(command);
    }

    private ItemsControl QTranscriptionList =>
        QContract.QContractFind<ItemsControl>(_qTranscriptionSurface, "PTranscription");

    internal void QTranscriptionIntroduce(CEditor editor)
    {
        _cEditor = editor;
        editor.CEditorDraftChanged += QTranscriptionSheetRefine;
    }

    private void QTranscriptionRefine(FrameworkElement container, object item, string? name)
    {
        QTranscriptionItem.QTranscriptionItemRefine(container, item, name);
        if (item is not QTranscriptionItem row)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBox>(container, "PTranscriptionField") is TextBox field)
        {
            field.SetResourceReference(QField.QFieldHintProperty, "Input.Transcription");
            field.TextChanged -= QTranscriptionFieldObserve;
            QField.QFieldTextShow(field, row.QTranscriptionItemText);
            field.TextChanged += QTranscriptionFieldObserve;
        }

        if (QLook.QLookPartFind<ComboBox>(container, "PTranscriptionScheme") is not ComboBox scheme)
        {
            return;
        }

        scheme.SelectionChanged -= QTranscriptionSchemeObserve;
        scheme.DisplayMemberPath = nameof(QTranscriptionChoice.QTranscriptionChoiceLabel);
        scheme.SelectedValuePath = nameof(QTranscriptionChoice.QTranscriptionChoiceScheme);
        scheme.ItemsSource = row.QTranscriptionItemChoice;
        scheme.SelectedValue = row.QTranscriptionItemScheme;
        scheme.SelectionChanged += QTranscriptionSchemeObserve;
        QLookItem.QLookItemAttach(scheme, QTranscriptionChoice.QTranscriptionChoiceRefine);
        if (QLook.QLookPartFind<TextBlock>(scheme, "PSchemeName") is TextBlock label)
        {
            label.Text = row.QTranscriptionItemLabel;
        }
    }

    private void QTranscriptionFieldObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { DataContext: QTranscriptionItem row } field)
        {
            _cEditor.CEditorTranscription.CTranscriptionSet(row.QTranscriptionItemId, field.Text);
        }
    }

    private void QTranscriptionSchemeObserve(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox { DataContext: QTranscriptionItem row, SelectedValue: string scheme })
        {
            _cEditor.CEditorTranscription.CTranscriptionSchemeSet(row.QTranscriptionItemId, scheme);
        }
    }

    private void QTranscriptionAddObserve(object sender, ExecutedRoutedEventArgs e)
    {
        _cEditor.CEditorTranscription.CTranscriptionAdd(
            (e.Parameter as QTranscriptionItem)?.QTranscriptionItemId);
    }

    private void QTranscriptionAddRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _qTranscriptionFree;
    }

    private void QTranscriptionRemoveObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is QTranscriptionItem row)
        {
            _cEditor.CEditorTranscription.CTranscriptionRemove(row.QTranscriptionItemId);
        }
    }

    private void QTranscriptionNotationRefine(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is QTranscriptionItem)
        {
            _qNotation.QNotationOpenRefine(e.OriginalSource as UIElement ?? QTranscriptionList);
        }
    }

    private void QTranscriptionNotationObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is QTranscriptionItem row)
        {
            _qNotation.QNotationStartRefine(
                _cEditor.CEditorDesk.CDeskErrand.CErrandTranscriptionStart(
                    row.QTranscriptionItemId, row.QTranscriptionItemScheme));
        }
    }

    private void QTranscriptionSheetRefine(CEntryDraft _)
    {
        CTranscriptionSheet sheet = _cEditor.CEditorTranscription.CTranscriptionRead();
        QTranscriptionList.Visibility = sheet.CTranscriptionSheetShown ? Visibility.Visible : Visibility.Collapsed;
        _qTranscriptionFree = sheet.CTranscriptionSheetFree;
        CommandManager.InvalidateRequerySuggested();

        PCard.PCardRowShow(
            _qTranscriptionItem,
            sheet.CTranscriptionSheetRows,
            static row => row.QTranscriptionItemId,
            static line => line.CTranscriptionRowDraft.CTranscriptionDraftId,
            static line =>
            {
                QTranscriptionItem row = QTranscriptionItem.QTranscriptionRowRefine(line.CTranscriptionRowDraft);
                row.QTranscriptionSchemeRefine(line.CTranscriptionRowSchemes);
                return row;
            },
            static (row, line) =>
            {
                QTranscriptionItem.QTranscriptionStateRefine(row, line.CTranscriptionRowDraft);
                row.QTranscriptionSchemeRefine(line.CTranscriptionRowSchemes);
                return row;
            });
    }
}
