using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Application;
using Llyn.Conduct;


namespace Llyn.UIDeportment;

public partial class PEditor
{
    private readonly ObservableCollection<QTranscriptionItem> _pTranscriptionItem = [];
    private IReadOnlyList<string> _pTranscriptionSchemes = [];

    private ItemsControl PTranscription => (ItemsControl)FindName(nameof(PTranscription));

    private void PTranscriptionAttach()
    {
        PTranscription.ItemsSource = _pTranscriptionItem;
        QLookItem.QLookItemAttach(PTranscription, PTranscriptionApply);
        PEditorSound.CommandBindings.Add(new CommandBinding(
            PTranscriptionCommand.PTranscriptionCommandAddition, PTranscriptionAddHandle, PTranscriptionAddCheck));
        PEditorSound.CommandBindings.Add(new CommandBinding(
            PTranscriptionCommand.PTranscriptionCommandRemoval, PTranscriptionRemoveHandle));
        PEditorSound.CommandBindings.Add(new CommandBinding(
            PTranscriptionCommand.PTranscriptionCommandNotation, PTranscriptionNotationHandle));
    }

    private void PTranscriptionApply(FrameworkElement container, object item, string? name)
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

        scheme.SelectionChanged -= PTranscriptionSchemeHandle;
        scheme.DisplayMemberPath = nameof(QTranscriptionChoice.QTranscriptionChoiceLabel);
        scheme.SelectedValuePath = nameof(QTranscriptionChoice.QTranscriptionChoiceScheme);
        scheme.ItemsSource = row.QTranscriptionItemChoice;
        scheme.SelectedValue = row.QTranscriptionItemScheme;
        scheme.SelectionChanged += PTranscriptionSchemeHandle;
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
            _qEditor.QEditorArea.CEditorTimbre.CTimbreTranscriptionSet(row.QTranscriptionItemId, field.Text);
        }
    }

    private void PTranscriptionSchemeHandle(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox { DataContext: QTranscriptionItem row, SelectedValue: string scheme })
        {
            PEditorRequestSend(new LRequestTranscriptionScheme(PEditorDraft, row.QTranscriptionItemId, scheme));
        }
    }

    internal void PTranscriptionAddHandle(object sender, ExecutedRoutedEventArgs e)
    {
        string? scheme = PTranscriptionSchemeFind();
        if (scheme is null)
        {
            return;
        }

        int position = e.Parameter is QTranscriptionItem row
            ? _pTranscriptionItem.IndexOf(row) + 1
            : _pTranscriptionItem.Count;
        PEditorRequestSend(new LRequestTranscriptionAddition(PEditorDraft, scheme, position));
    }

    internal void PTranscriptionAddCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = PTranscriptionSchemeFind() is not null;
    }

    internal void PTranscriptionRemoveHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is QTranscriptionItem row)
        {
            PEditorRequestSend(new LRequestTranscriptionRemoval(PEditorDraft, row.QTranscriptionItemId));
        }
    }

    internal async void PTranscriptionNotationHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is QTranscriptionItem row)
        {
            await PNotationOpen(
                e.OriginalSource as UIElement ?? PTranscription,
                row.QTranscriptionItemId,
                row.QTranscriptionItemScheme);
        }
    }

    private string? PTranscriptionSchemeFind()
    {
        foreach (string scheme in _pTranscriptionSchemes)
        {
            if (_pTranscriptionItem.All(
                row => !string.Equals(row.QTranscriptionItemScheme, scheme, StringComparison.Ordinal)))
            {
                return scheme;
            }
        }

        return null;
    }

    private QTranscriptionItem? PTranscriptionFind(long id)
    {
        foreach (QTranscriptionItem row in _pTranscriptionItem)
        {
            if (row.QTranscriptionItemId == id)
            {
                return row;
            }
        }

        foreach (QTranscriptionItem row in _pGlyphItem)
        {
            if (row.QTranscriptionItemId == id)
            {
                return row;
            }
        }

        return null;
    }

    internal void PTranscriptionShow(CEntryDraft draft)
    {
        _pTranscriptionSchemes =
            _pEditorHost.PWindowAtelier.CAtelierCatalog.CCatalogSchemeRead(draft.CEntryDraftLanguage);
        PTranscription.Visibility = _pTranscriptionSchemes.Count == 0 ? Visibility.Collapsed : Visibility.Visible;

        PCard.PCardRowShow(
            _pTranscriptionItem,
            _qEditor.QEditorArea.CEditorTimbre.CTimbreGlyphRead().CTimbreGlyphOther,
            static row => row.QTranscriptionItemId,
            static spelled => spelled.CTranscriptionDraftId,
            QTranscriptionItem.QTranscriptionRowRefine,
            PTranscriptionUpdate);

        foreach (QTranscriptionItem row in _pTranscriptionItem)
        {
            row.QTranscriptionSchemeRefine(
                _qEditor.QEditorArea.CEditorSounding.CSoundingSchemeRead(row.QTranscriptionItemId));
        }
    }

    private QTranscriptionItem PTranscriptionUpdate(QTranscriptionItem row, CTranscriptionDraft spelled)
    {
        row.QTranscriptionItemKey = spelled.CTranscriptionDraftKey;
        row.QTranscriptionItemScheme = spelled.CTranscriptionDraftScheme;
        row.QTranscriptionItemText = spelled.CTranscriptionDraftText;
        return row;
    }
}
