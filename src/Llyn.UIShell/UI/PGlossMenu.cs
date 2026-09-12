using System;
using System.Windows;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    internal void PGlossAddHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PSentence row } || PCardSentenceFind(row) is not PCard card)
        {
            return;
        }

        PEditorRequestSend(new LRequestGlossAddition(
            _pEditorDraft, card.PCardId, row.PSentenceRow, PGlossLanguageRead(), row.PSentenceGloss.Count));
    }

    internal void PGlossRemoveHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is not PGloss gloss
            || e.Source is not FrameworkElement { DataContext: PSentence row }
            || PCardSentenceFind(row) is not PCard card)
        {
            return;
        }

        PEditorRequestSend(new LRequestGlossRemoval(_pEditorDraft, card.PCardId, row.PSentenceRow, gloss.PGlossId));
    }

    private void PGlossChangeHandle(PCard card, PSentence row, PGloss gloss, string field)
    {
        switch (field)
        {
            case nameof(PGloss.PGlossText):
                PEditorRequestDefer(
                    PEditorRequestFormat(card, row.PSentenceRow, PSentence.PSentenceGlossFormat(gloss, field)),
                    new LRequestGlossText(
                        _pEditorDraft, card.PCardId, row.PSentenceRow, gloss.PGlossId, gloss.PGlossTextRead()));
                break;
            case nameof(PGloss.PGlossLanguage):
                PEditorRequestSend(new LRequestGlossLanguage(
                    _pEditorDraft, card.PCardId, row.PSentenceRow, gloss.PGlossId, gloss.PGlossLanguage));
                break;
        }
    }

    private string PGlossLanguageRead()
    {
        foreach (PLanguageItem item in _pLanguageItem)
        {
            if (!string.Equals(item.PLanguageItemName, _pSpeakerChoice, StringComparison.Ordinal))
            {
                return item.PLanguageItemName;
            }
        }

        return string.Empty;
    }
}
