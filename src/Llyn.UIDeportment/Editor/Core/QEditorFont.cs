using System;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QEditorFont
{
    private readonly FrameworkElement _qEditorFontSurface;

    private CEditor _cEditor = null!;

    internal QEditorFont(FrameworkElement surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qEditorFontSurface = surface;
    }

    private TextBox QEditorFontHeadword => QContract.QContractFind<TextBox>(_qEditorFontSurface, "PHeadword");

    private TextBlock QEditorFontGhost => QContract.QContractFind<TextBlock>(_qEditorFontSurface, "PHeadwordGhost");

    private ItemsControl QEditorFontGlyph => QContract.QContractFind<ItemsControl>(_qEditorFontSurface, "PGlyph");

    internal void QEditorFontIntroduce(CEditor editor)
    {
        _cEditor = editor;
        editor.CEditorDraftChanged += QEditorHeadwordRefine;
        editor.CEditorDraftChanged += QEditorExampleRefine;
        editor.CEditorDraftChanged += QEditorGlossRefine;
        editor.CEditorDraftChanged += QEditorGlyphRefine;
    }

    private void QEditorHeadwordRefine(CEntryDraft _)
    {
        QFontFace.QFontRefine(
            _cEditor.CEditorTimbre.CTimbreFontRead(CFontRole.CFontRoleHeadword),
            QEditorFontHeadword,
            QEditorFontGhost);
        QFontFace.QFontBaselineRefine(QEditorFontHeadword, QEditorFontGhost);
    }

    private void QEditorExampleRefine(CEntryDraft _)
    {
        QFontFace.QFontExampleRefine(
            _qEditorFontSurface.Resources, _cEditor.CEditorTimbre.CTimbreFontRead(CFontRole.CFontRoleExample));
    }

    private void QEditorGlossRefine(CEntryDraft _)
    {
        QFontFace.QFontGlossRefine(
            _qEditorFontSurface.Resources, _cEditor.CEditorTimbre.CTimbreFontRead(CFontRole.CFontRoleGloss));
    }

    private void QEditorGlyphRefine(CEntryDraft _)
    {
        QFontFace.QFontGlyphRefine(
            QEditorFontGlyph.Resources, _cEditor.CEditorTimbre.CTimbreFontRead(CFontRole.CFontRoleGlyph));
    }
}
