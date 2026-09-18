using System;
using System.Windows;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    private const string PWindowLanguage = "English";

    internal void PGlossAddHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PSentence row } || PCardSentenceFind(row) is not PCard card)
        {
            return;
        }

        PEditorRequestSend(new LRequestGlossAddition(
            PEditorDraft, card.PCardId, row.PSentenceRow, PGlossLanguageRead(), row.PSentenceGloss.Count));
    }

    internal void PGlossRemoveHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is not PGloss gloss
            || e.Source is not FrameworkElement { DataContext: PSentence row }
            || PCardSentenceFind(row) is not PCard card)
        {
            return;
        }

        PEditorRequestSend(new LRequestGlossRemoval(PEditorDraft, card.PCardId, row.PSentenceRow, gloss.PGlossId));
    }

    private void PGlossChangeHandle(PCard card, PSentence row, PGloss gloss, string field)
    {
        if (field == nameof(PGloss.PGlossLanguage))
        {
            PEditorRequestSend(new LRequestGlossLanguage(
                PEditorDraft, card.PCardId, row.PSentenceRow, gloss.PGlossId, gloss.PGlossLanguage));
        }
    }

    private void PGlossChangeHandle(PGloss gloss, LStateWritten written)
    {
        if (PSentenceGlossFind(gloss) is (PCard card, PSentence row))
        {
            PEditorRequestDefer(
                new LRequestGlossText(PEditorDraft, card.PCardId, row.PSentenceRow, gloss.PGlossId, written));
        }
    }

    private (PCard, PSentence)? PSentenceGlossFind(PGloss gloss)
    {
        foreach (PCard card in _pMeaningList)
        {
            foreach (PSentence row in card.PCardSentence)
            {
                if (row.PSentenceGloss.Contains(gloss))
                {
                    return (card, row);
                }
            }
        }

        foreach (PCard card in _pCollocationList)
        {
            foreach (PSentence row in card.PCardSentence)
            {
                if (row.PSentenceGloss.Contains(gloss))
                {
                    return (card, row);
                }
            }
        }

        return null;
    }

    private string PGlossLanguageRead()
    {
        foreach (PLanguageItem item in _pLanguageItem)
        {
            if (string.Equals(item.PLanguageItemName, PWindowLanguage, StringComparison.Ordinal))
            {
                return item.PLanguageItemName;
            }
        }

        return _pLanguageItem.Count > 0 ? _pLanguageItem[0].PLanguageItemName : string.Empty;
    }
}
