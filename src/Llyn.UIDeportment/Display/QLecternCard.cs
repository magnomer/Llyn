using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QLecternCard
{
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

    private UIElement _qLecternCardReferral = null!;

    private PEtymology _qLecternCardEtymology = null!;

    private UIElement _qLecternCardOrigin = null!;

    public QLecternCard(LDisplay display)
    {
        ArgumentNullException.ThrowIfNull(display);

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

    public void QLecternIncomingIntroduce(ItemsControl incoming, UIElement section)
    {
        ArgumentNullException.ThrowIfNull(incoming);
        ArgumentNullException.ThrowIfNull(section);

        incoming.ItemsSource = _qLecternCardUsage;
        _qLecternCardReferral = section;
    }

    internal void QLecternEtymologyIntroduce(PEtymology etymology, UIElement section)
    {
        ArgumentNullException.ThrowIfNull(etymology);
        ArgumentNullException.ThrowIfNull(section);

        _qLecternCardEtymology = etymology;
        _qLecternCardOrigin = section;
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
        _qLecternCardMeaning.ItemsSource = null;
        _qLecternCardCollocation.ItemsSource = null;
        _qLecternCardDefinition.Visibility = Visibility.Collapsed;
        _qLecternCardPhrase.Visibility = Visibility.Collapsed;
        _qLecternCardUsage.Clear();
        _qLecternCardReferral.Visibility = Visibility.Collapsed;
    }

    private void QLecternCardRefine(CLecternCard card)
    {
        LFontFace.LFontExampleRefine(
            _qLecternCardResources, _qLecternCardAtelier, _qLecternCardArea.CDisplayShown.CLecternLanguage);
        QLecternLeafRefine(card);
        _qLecternCardDefinition.Visibility = QLook.QLookVisibleRead(card.CLecternCardDefined);
        _qLecternCardPhrase.Visibility = QLook.QLookVisibleRead(card.CLecternCardCollocated);
    }

    private void QLecternLeafRefine(CLecternCard card)
    {
        _qLecternCardMeaning.ItemsSource = null;
        _qLecternCardCollocation.ItemsSource = null;
        _qLecternCardMeaning.ItemsSource =
            card.CLecternCardMeanings.Select(static leaf => new QLeafItem(leaf)).ToList();
        _qLecternCardCollocation.ItemsSource =
            card.CLecternCardCollocations.Select(static leaf => new QLeafItem(leaf)).ToList();
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
        _qLecternCardEtymology.PEtymologyLanguage = etymology.CLecternEtymologyLanguage;
        _qLecternCardEtymology.PEtymologyText = etymology.CLecternEtymologyText;
        _qLecternCardEtymology.PEtymologySourceShow(etymology.CLecternEtymologyTargets
            .Select(static target => new PEtymon(
                target.CTranslationTargetId, target.CTranslationTargetHeadword, target.CTranslationTargetLanguage))
            .ToList());
        _qLecternCardEtymology.Visibility = QLook.QLookVisibleRead(etymology.CLecternEtymologyShown);
        _qLecternCardOrigin.Visibility = QLook.QLookVisibleRead(etymology.CLecternEtymologyDerived);
    }

    public void QLecternChipObserve(RoutedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        e.Handled = _qLecternCardArea.CDisplayChipOpen(
            QSender.QSenderSourceRead<QLeafChip>(e)?.QLeafChipOrigin,
            QSender.QSenderSourceRead<LLinkChip>(e)?.LLinkChipId);
    }

    public void QLecternIncomingObserve(RoutedEventArgs e)
    {
        _qLecternCardHost.PWindowAtelier.CAtelierNavigation.CNavigationUsageOpen(
            QSender.QSenderSourceRead<QUsageItem>(e)?.QUsageItemUsage);
    }

    public void QLecternEtymonObserve(object parameter)
    {
        _qLecternCardArea.CDisplayChipOpen(null, parameter as long?);
    }

    public void QLecternMentionObserve(PMentionArgument e)
    {
        ArgumentNullException.ThrowIfNull(e);

        _qLecternCardHost.PWindowMentionRefine(
            e.PMentionArgumentOrigin,
            _qLecternCardArea.CDisplayMentionFind(
                e.PMentionArgumentText,
                e.PMentionArgumentLanguage,
                e.PMentionArgumentOffset,
                e.PMentionArgumentMention));
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
