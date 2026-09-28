using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QLecternCard
{
    private readonly LDisplay _qLecternCardDisplay;

    private readonly CDisplay _qLecternCardArea;

    private readonly ObservableCollection<QUsageItem> _qLecternCardUsage = [];

    private PWindow _qLecternCardHost = null!;

    private CAtelier _qLecternCardAtelier = null!;

    private ResourceDictionary _qLecternCardResources = null!;

    private ItemsControl _qLecternCardMeaning = null!;

    private UIElement _qLecternCardDefinition = null!;

    private ItemsControl _qLecternCardCollocation = null!;

    private UIElement _qLecternCardPhrase = null!;

    private ScrollViewer _qLecternCardContents = null!;

    private QCompass _qLecternCardCompass = null!;

    private Action<IReadOnlyList<CTranslationTarget>> _qLecternCardTranslation = null!;

    private Action<IReadOnlyDictionary<long, string>> _qLecternCardCitation = null!;

    private Action<CSentenceOrder> _qLecternCardOrder = null!;

    private UIElement _qLecternCardReferral = null!;

    private UIElement _qLecternCardEtymology = null!;

    private UIElement _qLecternCardOrigin = null!;

    private Action<string, string, IReadOnlyList<CTranslationTarget>> _qLecternCardLineage = null!;

    public QLecternCard(LDisplay display)
    {
        ArgumentNullException.ThrowIfNull(display);

        _qLecternCardDisplay = display;
        _qLecternCardArea = display.CDisplayArea;
    }

    public void QLecternCardIntroduce(
        CAtelier atelier,
        ResourceDictionary resources,
        ItemsControl meaning,
        UIElement meaningSection,
        ItemsControl collocation,
        UIElement collocationSection,
        ScrollViewer contents,
        QCompass compass)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(resources);
        ArgumentNullException.ThrowIfNull(meaning);
        ArgumentNullException.ThrowIfNull(meaningSection);
        ArgumentNullException.ThrowIfNull(collocation);
        ArgumentNullException.ThrowIfNull(collocationSection);
        ArgumentNullException.ThrowIfNull(contents);
        ArgumentNullException.ThrowIfNull(compass);

        _qLecternCardAtelier = atelier;
        _qLecternCardResources = resources;
        _qLecternCardMeaning = meaning;
        _qLecternCardDefinition = meaningSection;
        _qLecternCardCollocation = collocation;
        _qLecternCardPhrase = collocationSection;
        _qLecternCardContents = contents;
        _qLecternCardCompass = compass;
    }

    public void QLecternLinkIntroduce(
        Action<IReadOnlyList<CTranslationTarget>> translationSeam,
        Action<IReadOnlyDictionary<long, string>> citationSeam,
        Action<CSentenceOrder> orderSeam)
    {
        ArgumentNullException.ThrowIfNull(translationSeam);
        ArgumentNullException.ThrowIfNull(citationSeam);
        ArgumentNullException.ThrowIfNull(orderSeam);

        _qLecternCardTranslation = translationSeam;
        _qLecternCardCitation = citationSeam;
        _qLecternCardOrder = orderSeam;
    }

    public void QLecternIncomingIntroduce(ItemsControl incoming, UIElement section)
    {
        ArgumentNullException.ThrowIfNull(incoming);
        ArgumentNullException.ThrowIfNull(section);

        incoming.ItemsSource = _qLecternCardUsage;
        _qLecternCardReferral = section;
    }

    public void QLecternEtymologyIntroduce(
        UIElement etymology,
        UIElement section,
        Action<string, string, IReadOnlyList<CTranslationTarget>> etymologySeam)
    {
        ArgumentNullException.ThrowIfNull(etymology);
        ArgumentNullException.ThrowIfNull(section);
        ArgumentNullException.ThrowIfNull(etymologySeam);

        _qLecternCardEtymology = etymology;
        _qLecternCardOrigin = section;
        _qLecternCardLineage = etymologySeam;
    }

    internal void QLecternRouteIntroduce(PWindow host)
    {
        ArgumentNullException.ThrowIfNull(host);

        _qLecternCardHost = host;
    }

    public void QLecternCardRefine()
    {
        QLecternCardRefine(_qLecternCardArea.CDisplayCardRead());
    }

    public void QLecternLeafRefine()
    {
        _qLecternCardMeaning.ItemsSource = null;
        _qLecternCardCollocation.ItemsSource = null;
        _qLecternCardMeaning.ItemsSource = _qLecternCardDisplay.LDisplaySound.LDisplayShown!.LEntryDraftMeanings;
        _qLecternCardCollocation.ItemsSource =
            _qLecternCardDisplay.LDisplaySound.LDisplayShown!.LEntryDraftCollocations;
    }

    public void QLecternIncomingRefine()
    {
        QLecternIncomingRefine(_qLecternCardArea.CDisplayIncomingRead());
    }

    public void QLecternEtymologyRefine()
    {
        QLecternEtymologyRefine(_qLecternCardArea.CDisplayEtymologyRead());
    }

    public void QLecternBlankRefine()
    {
        _qLecternCardTranslation([]);
        _qLecternCardCitation(new Dictionary<long, string>());
        _qLecternCardMeaning.ItemsSource = null;
        _qLecternCardCollocation.ItemsSource = null;
        _qLecternCardDefinition.Visibility = Visibility.Collapsed;
        _qLecternCardPhrase.Visibility = Visibility.Collapsed;
        _qLecternCardUsage.Clear();
        _qLecternCardReferral.Visibility = Visibility.Collapsed;
    }

    private void QLecternCardRefine(CLecternCard card)
    {
        _qLecternCardOrder(card.CLecternCardOrder);
        LFontFace.LFontExampleRefine(
            _qLecternCardResources, _qLecternCardAtelier, _qLecternCardArea.CDisplayShown.CLecternLanguage);
        _qLecternCardCitation(card.CLecternCardCitations);
        _qLecternCardTranslation(card.CLecternCardTargets);
        _qLecternCardDefinition.Visibility = QLook.QLookVisibleRead(card.CLecternCardDefined);
        _qLecternCardPhrase.Visibility = QLook.QLookVisibleRead(card.CLecternCardCollocated);
    }

    private void QLecternIncomingRefine(IReadOnlyList<CUsage> usages)
    {
        _qLecternCardUsage.Clear();
        foreach (CUsage usage in usages)
        {
            _qLecternCardUsage.Add(new QUsageItem(usage));
        }

        _qLecternCardReferral.Visibility = QLook.QLookVisibleRead(_qLecternCardUsage.Count > 0);
    }

    private void QLecternEtymologyRefine(CLecternEtymology etymology)
    {
        _qLecternCardLineage(
            etymology.CLecternEtymologyLanguage,
            etymology.CLecternEtymologyText,
            etymology.CLecternEtymologyTargets);
        _qLecternCardEtymology.Visibility = QLook.QLookVisibleRead(etymology.CLecternEtymologyShown);
        _qLecternCardOrigin.Visibility = QLook.QLookVisibleRead(etymology.CLecternEtymologyDerived);
    }

    public void QLecternChipObserve(RoutedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        e.Handled = _qLecternCardArea.CDisplayChipOpen(
            QSender.QSenderSourceRead<object>(e),
            QSender.QSenderSourceRead<LLinkChip>(e)?.LLinkChipId,
            _qLecternCardHost.PWindowEntryShow,
            _qLecternCardHost.PWindowSituationShow,
            _qLecternCardHost.PWindowRegisterShow,
            _qLecternCardHost.PWindowTagShow);
    }

    public void QLecternIncomingObserve(RoutedEventArgs e)
    {
        QSender.QSenderSourceRead<QUsageItem>(e)?.QUsageItemUsage.CUsageOpen(
            _qLecternCardHost.PWindowExampleShow,
            _qLecternCardHost.PWindowEntryShow);
    }

    public void QLecternEtymonObserve(object parameter)
    {
        _qLecternCardArea.CDisplayChipOpen(
            null,
            parameter as long?,
            _qLecternCardHost.PWindowEntryShow,
            _qLecternCardHost.PWindowSituationShow,
            _qLecternCardHost.PWindowRegisterShow,
            _qLecternCardHost.PWindowTagShow);
    }

    public void QLecternMentionObserve<QLecternAnchor>(
        QLecternAnchor anchor,
        string text,
        string language,
        int offset,
        IReadOnlyList<CMentionMark>? mentions,
        Action<QLecternAnchor, CMentionResult> show)
    {
        QLecternMentionRefine(anchor, _qLecternCardArea.CDisplayMentionFind(text, language, offset, mentions), show);
    }

    private static void QLecternMentionRefine<QLecternAnchor>(
        QLecternAnchor anchor, CMentionResult? result, Action<QLecternAnchor, CMentionResult> show)
    {
        ArgumentNullException.ThrowIfNull(show);

        if (result is not null)
        {
            show(anchor, result);
        }
    }

    public void QLecternSpotlightRefine(long id)
    {
        _qLecternCardContents.Dispatcher.BeginInvoke(
            DispatcherPriority.Loaded, () => QLecternSpotlightRefine(_qLecternCardArea.CDisplayCardFind(id)));
    }

    private void QLecternSpotlightRefine((CCompassPart, int)? place)
    {
        if (place is not (CCompassPart part, int index))
        {
            return;
        }

        ItemsControl cards = part == CCompassPart.CCompassPartCollocation
            ? _qLecternCardCollocation
            : _qLecternCardMeaning;
        if (cards.ItemContainerGenerator.ContainerFromIndex(index) is FrameworkElement card)
        {
            _qLecternCardCompass.QCompassTargetRefine(card);
            ((Storyboard)_qLecternCardContents.FindResource("Theme.Card.Spotlight")).Begin(card);
        }
    }
}
