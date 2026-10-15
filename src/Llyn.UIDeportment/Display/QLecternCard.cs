using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QLecternCard
{
    private readonly CDisplayCard _qLecternCardArea;

    private readonly CDisplayRoute _qLecternCardRoute;

    private readonly CDisplaySound _qLecternCardSound;

    private readonly ResourceDictionary _qLecternCardResources;

    private readonly ItemsControl _qLecternCardMeaning;

    private readonly UIElement _qLecternCardDefinition;

    private readonly ItemsControl _qLecternCardCollocation;

    private readonly UIElement _qLecternCardPhrase;

    public QLecternCard(FrameworkElement surface, CDisplayCard area, CDisplayRoute route, CDisplaySound sound)
    {
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(area);
        ArgumentNullException.ThrowIfNull(route);
        ArgumentNullException.ThrowIfNull(sound);

        _qLecternCardArea = area;
        _qLecternCardRoute = route;
        _qLecternCardSound = sound;
        _qLecternCardResources = surface.Resources;
        _qLecternCardMeaning = QContract.QContractFind<ItemsControl>(surface, "PDisplayMeaning");
        _qLecternCardDefinition = QContract.QContractFind<StackPanel>(surface, "PDisplayMeaningSection");
        _qLecternCardCollocation = QContract.QContractFind<ItemsControl>(surface, "PDisplayCollocation");
        _qLecternCardPhrase = QContract.QContractFind<StackPanel>(surface, "PDisplayCollocationSection");

        _qLecternCardMeaning.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QLecternChipObserve));
        _qLecternCardCollocation.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QLecternChipObserve));
        _qLecternCardMeaning.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QLecternHingeObserve));
        _qLecternCardCollocation.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QLecternHingeObserve));
        _qLecternCardMeaning.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QLecternSeeObserve));
        _qLecternCardMeaning.AddHandler(
            QMention.QMentionClickEvent, new EventHandler<QMentionArgument>(QLecternMentionObserve));
        _qLecternCardCollocation.AddHandler(
            QMention.QMentionClickEvent, new EventHandler<QMentionArgument>(QLecternMentionObserve));
    }

    internal event Action<QMention, CMentionOffer?>? QLecternMentionNotice;

    internal event Action<long>? QLecternSeeNotice;

    public void QLecternExampleRefine()
    {
        QFontFace.QFontExampleRefine(
            _qLecternCardResources, _qLecternCardSound.CDisplayFontRead(CFontRole.CFontRoleExample));
    }

    public void QLecternGlossRefine()
    {
        QFontFace.QFontGlossRefine(
            _qLecternCardResources, _qLecternCardSound.CDisplayFontRead(CFontRole.CFontRoleGloss));
    }

    public void QLecternCardRefine()
    {
        CLecternCard card = _qLecternCardArea.CDisplayCardRead();
        QLecternLeafRefine(card);
        _qLecternCardDefinition.Visibility = QLook.QLookVisibleRead(card.CLecternCardDefined);
        _qLecternCardPhrase.Visibility = QLook.QLookVisibleRead(card.CLecternCardCollocated);
    }

    private void QLecternLeafRefine(CLecternCard card)
    {
        _qLecternCardMeaning.ItemsSource = null;
        _qLecternCardCollocation.ItemsSource = null;
        _qLecternCardMeaning.ItemsSource =
            card.CLecternCardMeanings.Select(static leaf => new QLeafItem(leaf, leaf.CLeafMeaning)).ToList();
        _qLecternCardCollocation.ItemsSource =
            card.CLecternCardOther.Select(static leaf => new QLeafItem(leaf, leaf.CLeafPhrase)).ToList();
    }

    private void QLecternChipObserve(object sender, RoutedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        e.Handled = _qLecternCardRoute.CDisplayChipOpen(
            QSender.QSenderSourceRead<QLeafChip>(e)?.QLeafChipOrigin,
            QSender.QSenderSourceRead<QLinkChip>(e)?.QLinkChipId);
    }

    private void QLecternHingeObserve(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is ToggleButton { DataContext: QLeafItem card } hinge)
        {
            QLook.QLookCheckedRefine(
                hinge,
                _qLecternCardArea.CDisplayFoldToggle(card.QLeafItemId, QLook.QLookCheckedRead(hinge.IsChecked)));
        }
    }

    private void QLecternSeeObserve(object sender, RoutedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        if (QSender.QSenderSourceRead<QLeafSee>(e) is QLeafSee see)
        {
            e.Handled = _qLecternCardArea.CDisplayFoldToggle(see.QLeafSeeId, false);
            QLecternSeeNotice?.Invoke(see.QLeafSeeId);
        }
    }

    private void QLecternMentionObserve(object? sender, QMentionArgument e)
    {
        ArgumentNullException.ThrowIfNull(e);

        QLecternMentionNotice?.Invoke(
            e.QMentionArgumentOrigin,
            _qLecternCardRoute.CDisplayMentionFind(
                e.QMentionArgumentSentence, e.QMentionArgumentText, e.QMentionArgumentUnit));
    }
}
