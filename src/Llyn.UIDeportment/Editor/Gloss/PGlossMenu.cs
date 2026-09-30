using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Llyn.UIDeportment;

public partial class PEditor
{
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
        QLookItem.QLookItemAttach(PLanguageList, PSpeakerApply);
        QChoice.QChoiceDropperAttach(PSpeaker, PLanguage, PSpeaker);
    }

    internal async void PLanguageRefine()
    {
        PLanguageItem.PLanguageItemReset(
            _pLanguageItem,
            await _pEditorHost.PWindowAtelier.CAtelierCatalog.CCatalogEnsignLoad(QEnsignImage.QEnsignDraw));
        PSpeakerEnsignRefine();
        PLinkFlagRefine();
    }

    private void PSpeakerObserve(object sender, RoutedEventArgs e)
    {
        _qEditor.QEditorArea.CEditorLanguageSet(PLanguageItem.PLanguageNameRead(sender));
        PSpeakerChoiceRefine();
    }

    private void PSpeakerChoiceRefine()
    {
        PSpeaker.IsChecked = false;
    }

    private async void PSpeakerFlagRefine()
    {
        await _pEditorHost.PWindowAtelier.CAtelierCatalog.CCatalogEnsignLoad(QEnsignImage.QEnsignDraw);
        PSpeakerEnsignRefine();
    }

    private void PSpeakerEnsignRefine()
    {
        QEnsignImage.QEnsignFlagRefine(PSpeakerFlag, PSpeakerGlobe, _qEditor.QEditorArea.CEditorLanguage);
    }

    private void PSpeakerApply(FrameworkElement container, object item, string? change)
    {
        PLanguageItem.PLanguageItemApply(container, item, change);

        if (item is PLanguageItem language
            && QLook.QLookPartFind<Ellipse>(container, "PSpeakerGlobe") is Ellipse globe)
        {
            globe.Visibility = QLook.QLookVisibleRead(language.PLanguageItemFlag is null);
        }

        if (QLook.QLookPartFind<Button>(container, "PSpeakerChoice") is Button choice)
        {
            choice.Click -= PSpeakerObserve;
            choice.Click += PSpeakerObserve;
        }
    }

    private void PGlossAddObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PSentence row } && PCardSentenceFind(row) is PCard card)
        {
            _qEditor.QEditorArea.CEditorSentence.CSentenceGlossAdd(card.PCardId, row.PSentenceRow);
        }
    }

    private void PGlossRemoveObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is PGloss gloss
            && e.Source is FrameworkElement { DataContext: PSentence row }
            && PCardSentenceFind(row) is PCard card)
        {
            _qEditor.QEditorArea.CEditorSentence.CSentenceGlossRemove(card.PCardId, row.PSentenceRow, gloss.PGlossId);
        }
    }

    private void PGlossTextObserve(PGloss gloss, string text)
    {
        if (PSentenceGlossFind(gloss) is (PCard card, PSentence row))
        {
            _qEditor.QEditorArea.CEditorSentence.CSentenceGlossSet(
                card.PCardId, row.PSentenceRow, gloss.PGlossId, text);
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
}
