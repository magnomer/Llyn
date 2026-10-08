using System;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QEditorFont
{
    private readonly FrameworkElement _qEditorFontSurface;

    private CTimbre _cTimbre = null!;

    internal QEditorFont(FrameworkElement surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qEditorFontSurface = surface;
    }

    private TextBox QEditorFontHeadword => QContract.QContractFind<TextBox>(_qEditorFontSurface, "PHeadword");

    private TextBlock QEditorFontGhost => QContract.QContractFind<TextBlock>(_qEditorFontSurface, "PHeadwordGhost");

    private ItemsControl QEditorFontGlyph => QContract.QContractFind<ItemsControl>(_qEditorFontSurface, "PGlyph");

    internal void QEditorFontIntroduce(CEntry entry, CTimbre timbre)
    {
        _cTimbre = timbre;
        entry.CEntryDraftChanged += QEditorHeadwordRefine;
        entry.CEntryDraftChanged += QEditorExampleRefine;
        entry.CEntryDraftChanged += QEditorGlossRefine;
        entry.CEntryDraftChanged += QEditorGlyphRefine;
    }

    private void QEditorHeadwordRefine(CEntryDraft _)
    {
        QFontFace.QFontRefine(
            _cTimbre.CTimbreFontRead(CFontRole.CFontRoleHeadword),
            QEditorFontHeadword,
            QEditorFontGhost);
        QFontFace.QFontBaselineRefine(QEditorFontHeadword, QEditorFontGhost);
    }

    private void QEditorExampleRefine(CEntryDraft _)
    {
        QFontFace.QFontExampleRefine(
            _qEditorFontSurface.Resources, _cTimbre.CTimbreFontRead(CFontRole.CFontRoleExample));
    }

    private void QEditorGlossRefine(CEntryDraft _)
    {
        QFontFace.QFontGlossRefine(
            _qEditorFontSurface.Resources, _cTimbre.CTimbreFontRead(CFontRole.CFontRoleGloss));
    }

    private void QEditorGlyphRefine(CEntryDraft _)
    {
        QFontFace.QFontGlyphRefine(
            QEditorFontGlyph.Resources, _cTimbre.CTimbreFontRead(CFontRole.CFontRoleGlyph));
    }
}
