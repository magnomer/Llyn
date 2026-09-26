using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private readonly ObservableCollection<LTranscriptionItem> _pGlyphItem = [];
    private LGlyph? _pGlyph;

    private ItemsControl PGlyph => (ItemsControl)FindName(nameof(PGlyph));

    private void PGlyphAttach()
    {
        PGlyph.ItemsSource = _pGlyphItem;
        PLookItem.PLookItemAttach(PGlyph, PGlyphApply);
        PEditorSound.CommandBindings.Add(new CommandBinding(
            PGlyphCommand.PGlyphCommandNotation, PGlyphNotationHandle));
    }

    internal async void PGlyphNotationHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is LTranscriptionItem row && _pGlyph is not null)
        {
            await PNotationOpen(e.OriginalSource as UIElement ?? PGlyph, row.LTranscriptionItemId, _pGlyph.LGlyphName);
        }
    }

    private void PGlyphApply(FrameworkElement container, object item, string? _)
    {
        if (item is not LTranscriptionItem row)
        {
            return;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PGlyphLabel") is TextBlock label)
        {
            label.Text = row.LTranscriptionItemLabel;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PGlyphMeasure") is TextBlock measure)
        {
            PGlyphMeasureApply(measure, row.LTranscriptionItemText);
        }

        if (PLook.PLookPartFind<TextBox>(container, "PGlyphField") is TextBox field)
        {
            field.Text = row.LTranscriptionItemText;
            field.TextChanged -= PGlyphFieldHandle;
            field.TextChanged += PGlyphFieldHandle;
        }

        if (PLook.PLookPartFind<Button>(container, "PGlyphPhonetician") is Button phonetician)
        {
            phonetician.Visibility = PLook.PLookVisibleRead(PGlyph.Tag is true);
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

    private static bool PGlyphSchemeCheck(LGlyph? glyph, string scheme)
    {
        return glyph is not null && string.Equals(scheme, glyph.LGlyphName, StringComparison.Ordinal);
    }

    private void PGlyphShow(LEntryDraft draft)
    {
        string language = draft.LEntryDraftLanguage;
        _pGlyph = _pEditorHost.PWindowDeportment.LWindowGlyphRead(language);
        PGlyph.Visibility = _pGlyph is null ? Visibility.Collapsed : Visibility.Visible;
        PGlyph.Tag = _pGlyph?.LGlyphSourced ?? false;
        PLookItem.PLookItemApply(PGlyph);
        LFontFace.LFontGlyphApply(PGlyph.Resources, _pEditorHost.PWindowDeportment, language);

        List<LTranscriptionDraft> rows = [];
        foreach (LTranscriptionDraft spelled in draft.LEntryDraftTranscriptions)
        {
            if (PGlyphSchemeCheck(spelled.LTranscriptionDraftScheme))
            {
                rows.Add(spelled);
            }
        }

        PCard.PCardRowShow(
            _pGlyphItem,
            rows,
            static row => row.LTranscriptionItemId,
            static spelled => spelled.LTranscriptionDraftId,
            PTranscriptionCreate,
            PTranscriptionUpdate);
    }
}
