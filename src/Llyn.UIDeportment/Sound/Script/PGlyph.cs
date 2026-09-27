using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;
using Llyn.Core;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private readonly ObservableCollection<LTranscriptionItem> _pGlyphItem = [];
    private CGlyph? _pGlyph;

    private ItemsControl PGlyph => (ItemsControl)FindName(nameof(PGlyph));

    private void PGlyphAttach()
    {
        PGlyph.ItemsSource = _pGlyphItem;
        QLookItem.QLookItemAttach(PGlyph, PGlyphApply);
        PEditorSound.CommandBindings.Add(new CommandBinding(
            PGlyphCommand.PGlyphCommandNotation, PGlyphNotationHandle));
    }

    internal async void PGlyphNotationHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is LTranscriptionItem row && _pGlyph is not null)
        {
            await PNotationOpen(e.OriginalSource as UIElement ?? PGlyph, row.LTranscriptionItemId, _pGlyph.CGlyphName);
        }
    }

    private void PGlyphApply(FrameworkElement container, object item, string? _)
    {
        if (item is not LTranscriptionItem row)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PGlyphLabel") is TextBlock label)
        {
            label.Text = row.LTranscriptionItemLabel;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PGlyphMeasure") is TextBlock measure)
        {
            PGlyphMeasureApply(measure, row.LTranscriptionItemText);
        }

        if (QLook.QLookPartFind<TextBox>(container, "PGlyphField") is TextBox field)
        {
            field.Text = row.LTranscriptionItemText;
            field.SetResourceReference(QField.QFieldHintProperty, "Input.Glyph");
            field.TextChanged -= PGlyphFieldHandle;
            field.TextChanged += PGlyphFieldHandle;
        }

        if (QLook.QLookPartFind<Button>(container, "PGlyphPhonetician") is Button phonetician)
        {
            phonetician.Visibility = QLook.QLookVisibleRead(PGlyph.Tag is true);
        }
    }

    private static void PGlyphMeasureApply(TextBlock measure, string text)
    {
        if (text.Length == 0)
        {
            measure.SetResourceReference(TextBlock.TextProperty, "Input.Glyph");
            return;
        }

        measure.Text = text;
    }

    private static void PGlyphFieldHandle(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { DataContext: LTranscriptionItem row } field)
        {
            row.LTranscriptionItemText = field.Text;
        }
    }

    private bool PGlyphSchemeCheck(string scheme)
    {
        return PGlyphSchemeCheck(_pGlyph, scheme);
    }

    private static bool PGlyphSchemeCheck(CGlyph? glyph, string scheme)
    {
        return glyph is not null && string.Equals(scheme, glyph.CGlyphName, StringComparison.Ordinal);
    }

    private void PGlyphShow(CEntryDraft draft)
    {
        string language = draft.CEntryDraftLanguage;
        _pGlyph = _pEditorHost.PWindowAtelier.CAtelierCatalog.CCatalogGlyphRead(language);
        PGlyph.Visibility = _pGlyph is null ? Visibility.Collapsed : Visibility.Visible;
        PGlyph.Tag = _pGlyph?.CGlyphSourced ?? false;
        QLookItem.QLookItemApply(PGlyph);
        LFontFace.LFontGlyphApply(PGlyph.Resources, _pEditorHost.PWindowAtelier, language);

        List<CTranscriptionDraft> rows = [];
        foreach (CTranscriptionDraft spelled in draft.CEntryDraftTranscriptions)
        {
            if (PGlyphSchemeCheck(spelled.CTranscriptionDraftScheme))
            {
                rows.Add(spelled);
            }
        }

        PCard.PCardRowShow(
            _pGlyphItem,
            rows,
            static row => row.LTranscriptionItemId,
            static spelled => spelled.CTranscriptionDraftId,
            PTranscriptionCreate,
            PTranscriptionUpdate);
    }
}
