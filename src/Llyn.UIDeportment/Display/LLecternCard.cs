using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Llyn.Conduct;
using Llyn.Core;

namespace Llyn.UIDeportment;

public sealed class LLecternCard
{
    private readonly LDisplay _lLecternCardDisplay;

    private readonly ObservableCollection<LUsageItem> _lLecternCardUsage = [];

    private LWindow _lLecternCardWindow = null!;

    private ResourceDictionary _lLecternCardResources = null!;

    private ItemsControl _lLecternCardMeaning = null!;

    private UIElement _lLecternCardDefinition = null!;

    private ItemsControl _lLecternCardCollocation = null!;

    private UIElement _lLecternCardPhrase = null!;

    private ScrollViewer _lLecternCardContents = null!;

    private LCompass _lLecternCardCompass = null!;

    private Action<IReadOnlyList<LTranslationTarget>> _lLecternCardTranslation = null!;

    private Action<IReadOnlyDictionary<long, string>> _lLecternCardCitation = null!;

    private Action<LSentenceOrder> _lLecternCardOrder = null!;

    private UIElement _lLecternCardReferral = null!;

    private UIElement _lLecternCardEtymology = null!;

    private UIElement _lLecternCardOrigin = null!;

    private Action<string, string, IReadOnlyList<LTranslationTarget>> _lLecternCardLineage = null!;

    private Func<long, bool> _lLecternCardEntry = null!;

    private Func<long, bool> _lLecternCardSituation = null!;

    private Func<long, bool> _lLecternCardRegister = null!;

    private Func<long, bool> _lLecternCardTag = null!;

    public LLecternCard(LDisplay display)
    {
        ArgumentNullException.ThrowIfNull(display);

        _lLecternCardDisplay = display;
    }

    public void LLecternCardAttach(
        LWindow window,
        ResourceDictionary resources,
        ItemsControl meaning,
        UIElement meaningSection,
        ItemsControl collocation,
        UIElement collocationSection,
        ScrollViewer contents,
        LCompass compass)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(resources);
        ArgumentNullException.ThrowIfNull(meaning);
        ArgumentNullException.ThrowIfNull(meaningSection);
        ArgumentNullException.ThrowIfNull(collocation);
        ArgumentNullException.ThrowIfNull(collocationSection);
        ArgumentNullException.ThrowIfNull(contents);
        ArgumentNullException.ThrowIfNull(compass);

        _lLecternCardWindow = window;
        _lLecternCardResources = resources;
        _lLecternCardMeaning = meaning;
        _lLecternCardDefinition = meaningSection;
        _lLecternCardCollocation = collocation;
        _lLecternCardPhrase = collocationSection;
        _lLecternCardContents = contents;
        _lLecternCardCompass = compass;
    }

    public void LLecternLinkAttach(
        Action<IReadOnlyList<LTranslationTarget>> translationSeam,
        Action<IReadOnlyDictionary<long, string>> citationSeam,
        Action<LSentenceOrder> orderSeam)
    {
        ArgumentNullException.ThrowIfNull(translationSeam);
        ArgumentNullException.ThrowIfNull(citationSeam);
        ArgumentNullException.ThrowIfNull(orderSeam);

        _lLecternCardTranslation = translationSeam;
        _lLecternCardCitation = citationSeam;
        _lLecternCardOrder = orderSeam;
    }

    public void LLecternIncomingAttach(ItemsControl incoming, UIElement section)
    {
        ArgumentNullException.ThrowIfNull(incoming);
        ArgumentNullException.ThrowIfNull(section);

        incoming.ItemsSource = _lLecternCardUsage;
        _lLecternCardReferral = section;
    }

    public void LLecternEtymologyAttach(
        UIElement etymology,
        UIElement section,
        Action<string, string, IReadOnlyList<LTranslationTarget>> etymologySeam)
    {
        ArgumentNullException.ThrowIfNull(etymology);
        ArgumentNullException.ThrowIfNull(section);
        ArgumentNullException.ThrowIfNull(etymologySeam);

        _lLecternCardEtymology = etymology;
        _lLecternCardOrigin = section;
        _lLecternCardLineage = etymologySeam;
    }

    public void LLecternRouteAttach(
        Func<long, bool> entrySeam,
        Func<long, bool> situationSeam,
        Func<long, bool> registerSeam,
        Func<long, bool> tagSeam,
        Action<string, Exception> failSeam)
    {
        ArgumentNullException.ThrowIfNull(entrySeam);
        ArgumentNullException.ThrowIfNull(situationSeam);
        ArgumentNullException.ThrowIfNull(registerSeam);
        ArgumentNullException.ThrowIfNull(tagSeam);
        ArgumentNullException.ThrowIfNull(failSeam);

        _lLecternCardEntry = entrySeam;
        _lLecternCardSituation = situationSeam;
        _lLecternCardRegister = registerSeam;
        _lLecternCardTag = tagSeam;
        _lLecternCardDisplay.LDisplayFailed += failSeam;
    }

    public void LLecternCardShow()
    {
        if (_lLecternCardDisplay.LDisplaySound.LDisplayShown is null)
        {
            LLecternCardClear();
            return;
        }

        LLecternCardDraw(_lLecternCardDisplay.LDisplaySound.LDisplayShown);
        LLecternIncomingShow(_lLecternCardDisplay.LDisplayIncomingRead());
        LLecternEtymologyShow(
            _lLecternCardDisplay.LDisplaySound.LDisplayShown.LEntryDraftLanguage,
            _lLecternCardDisplay.LDisplaySound.LDisplayShown.LEntryDraftEtymology.LEtymologyDraftText,
            _lLecternCardDisplay.LDisplayEtymonRead(),
            _lLecternCardDisplay.LDisplaySound.LDisplayShown.LEntryDraftDerived);
    }

    private void LLecternCardDraw(LEntryDraft draft)
    {
        _lLecternCardOrder(_lLecternCardDisplay.LDisplayOrderRead());
        LFontFace.LFontExampleApply(_lLecternCardResources, _lLecternCardWindow, draft.LEntryDraftLanguage);
        _lLecternCardCitation(_lLecternCardDisplay.LDisplayCitationRead());
        _lLecternCardTranslation(_lLecternCardDisplay.LDisplayTargetRead());

        _lLecternCardMeaning.ItemsSource = null;
        _lLecternCardCollocation.ItemsSource = null;
        _lLecternCardMeaning.ItemsSource = draft.LEntryDraftMeanings;
        _lLecternCardCollocation.ItemsSource = draft.LEntryDraftCollocations;
        _lLecternCardDefinition.Visibility = draft.LEntryDraftDefined ? Visibility.Visible : Visibility.Collapsed;
        _lLecternCardPhrase.Visibility = draft.LEntryDraftCollocated ? Visibility.Visible : Visibility.Collapsed;
    }

    private void LLecternIncomingShow(IReadOnlyList<LUsage> usages)
    {
        _lLecternCardUsage.Clear();
        foreach (LUsage usage in usages)
        {
            _lLecternCardUsage.Add(new LUsageItem(
                usage,
                LLocalizationCatalog.LLocalizationTextRead(LDisplay.LDisplayOwnerRead(usage)),
                LLocalizationCatalog.LLocalizationTextRead("Display.Unknown"),
                string.Empty,
                usage.LUsageEpithet));
        }

        _lLecternCardReferral.Visibility = _lLecternCardUsage.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
    }

    private void LLecternEtymologyShow(
        string language, string text, IReadOnlyList<LTranslationTarget> etymons, bool derived)
    {
        _lLecternCardLineage(language, text, etymons);
        _lLecternCardEtymology.Visibility = LDisplay.LDisplayEtymologyCheck(text, etymons.Count)
            ? Visibility.Visible
            : Visibility.Collapsed;
        _lLecternCardOrigin.Visibility = derived ? Visibility.Visible : Visibility.Collapsed;
    }

    public void LLecternCardClear()
    {
        _lLecternCardTranslation([]);
        _lLecternCardCitation(new Dictionary<long, string>());
        _lLecternCardMeaning.ItemsSource = null;
        _lLecternCardCollocation.ItemsSource = null;
        _lLecternCardDefinition.Visibility = Visibility.Collapsed;
        _lLecternCardPhrase.Visibility = Visibility.Collapsed;
        _lLecternCardUsage.Clear();
        _lLecternCardReferral.Visibility = Visibility.Collapsed;
    }

    public void LLecternCardHandle(RoutedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        if (e.OriginalSource is not FrameworkElement chip)
        {
            return;
        }

        switch (chip.DataContext)
        {
            case LSituationDraft { LSituationDraftStored: true } situation:
                e.Handled = true;
                _lLecternCardSituation(situation.LSituationDraftId);
                break;
            case LRegisterDraft { LRegisterDraftStored: true } register:
                e.Handled = true;
                _lLecternCardRegister(register.LRegisterDraftId);
                break;
            case LLinkChip link when link.LLinkChipId != 0:
                e.Handled = true;
                _lLecternCardEntry(link.LLinkChipId);
                break;
            case LTagDraft { LTagDraftStored: true } tag:
                e.Handled = true;
                _lLecternCardTag(tag.LTagDraftId);
                break;
        }
    }

    public void LLecternIncomingHandle(object sender)
    {
        if (sender is FrameworkElement { DataContext: LUsageItem item })
        {
            _lLecternCardEntry(item.LUsageItemEntry);
        }
    }

    public void LLecternEtymonHandle(object parameter)
    {
        if (parameter is long id)
        {
            _lLecternCardEntry(id);
        }
    }

    public LMentionResult LLecternMentionFind(
        string text, string language, int offset, IReadOnlyList<LMention>? mentions) =>
        _lLecternCardDisplay.LDisplayMentionFind(text, language, offset, mentions);

    public void LLecternCardScroll(long id)
    {
        _lLecternCardContents.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, () =>
        {
            if ((LLecternCardFind(_lLecternCardMeaning, id) ?? LLecternCardFind(_lLecternCardCollocation, id))
                is FrameworkElement card)
            {
                _lLecternCardCompass.LCompassScroll(card);
                ((Storyboard)_lLecternCardContents.FindResource("Theme.Card.Spotlight")).Begin(card);
            }
        });
    }

    private static FrameworkElement? LLecternCardFind(ItemsControl cards, long id)
    {
        for (int index = 0; index < cards.Items.Count; index++)
        {
            if (cards.Items[index] is not LCardDraft card)
            {
                continue;
            }

            if (LDisplay.LDisplayCardCheck(card, id))
            {
                return cards.ItemContainerGenerator.ContainerFromIndex(index) as FrameworkElement;
            }
        }

        return null;
    }
}
