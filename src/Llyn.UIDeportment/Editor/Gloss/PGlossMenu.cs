using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Shapes;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private const string PWindowLanguage = "English";

    private readonly PLanguageTemplate _pLanguageTemplate;

    private readonly ObservableCollection<PLanguageItem> _pLanguageItem = [];

    private ToggleButton PSpeaker => (ToggleButton)FindName(nameof(PSpeaker));

    private Image PSpeakerFlag => (Image)FindName(nameof(PSpeakerFlag));

    private Ellipse PSpeakerGlobe => (Ellipse)FindName(nameof(PSpeakerGlobe));

    private TextBlock PSpeakerName => (TextBlock)FindName(nameof(PSpeakerName));

    private Popup PLanguage => (Popup)FindName(nameof(PLanguage));

    private ItemsControl PLanguageList => (ItemsControl)FindName(nameof(PLanguageList));

    private void PSpeakerAttach()
    {
        PLanguageList.ItemsSource = _pLanguageItem;
        PLookItem.PLookItemAttach(PLanguageList, PSpeakerApply);
        PChoice.PChoiceDropperAttach(PSpeaker, PLanguage, PSpeaker);
    }

    internal async void PSpeakerLoad()
    {
        await LEnsignImage.LEnsignLoad(_pEditorHost.PWindowDeportment);
        PLanguageItem.PLanguageItemReset(_pLanguageItem, _pEditorHost.PWindowDeportment.LWindowLanguageRead());
        PEditorLanguageUpdate();
        PLinkFlagUpdate();
    }

    internal void PSpeakerHandle(object sender, RoutedEventArgs e)
    {
        _lEditor.LEditorLanguageSet(PLanguageItem.PLanguageNameRead(sender));
        PSpeaker.IsChecked = false;
    }

    private async void PSpeakerFlagUpdate()
    {
        await LEnsignImage.LEnsignLoad(_pEditorHost.PWindowDeportment);
        LEnsignImage.LEnsignFlagShow(PSpeakerFlag, PSpeakerGlobe, _lEditor.LEditorLanguage);
    }

    private void PSpeakerApply(FrameworkElement container, object item, string? change)
    {
        PLanguageItem.PLanguageItemApply(container, item, change);

        if (item is PLanguageItem language
            && PLook.PLookPartFind<Ellipse>(container, "PSpeakerGlobe") is Ellipse globe)
        {
            globe.Visibility = PLook.PLookVisibleRead(language.PLanguageItemFlag is null);
        }

        if (PLook.PLookPartFind<Button>(container, "PSpeakerChoice") is Button choice)
        {
            choice.Click -= _pLanguageTemplate.PSpeakerHandle;
            choice.Click += _pLanguageTemplate.PSpeakerHandle;
        }
    }

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
