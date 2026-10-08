# QLecternCard.cs
Hash: `be101fd4c673b303`

## `public sealed class QLecternCard`

The reading view's card section, standing between the veneer and the areas [CDisplay](../../Llyn.Conduct/Display/CDisplay.comment.md) hands out.
It reads the card, route and sound areas only.
It draws the Meaning and Collocation cards, their links and bylines.
It hears every chip, link and word clicked on them and hands each to one gate.
[QLectern](QLectern.comment.md) builds it once in its constructor over the view's page.
It pulls its own parts by contract ID and subscribes its clicks.
The lectern subscribes its redraws to the display's open and close.
It knows no window, so a word's offer leaves through `QLecternMentionNotice`.

## `public QLecternCard(FrameworkElement surface, CDisplayCard area, CDisplayRoute route, CDisplaySound sound)`

Pulls the two card lists with their sections from `surface`, and holds its resources the example templates read.
The section names no `L` type, since it takes only the three areas it reads.
The card area answers the card read.
The route area answers every chip, link and word gate.
The sound area answers the pack's typography, reading the shown language itself.
The card lists take the ready cards the card area answers, so no engine record reaches the veneer.

## `internal event Action<PMention, CMentionOffer?>? QLecternMentionNotice;`

Carries a clicked word's offer and the example control it sits under.
The view that owns the lectern wires it to the mention menu's `QMentionOfferRefine`, which paints the menu.

## `public void QLecternExampleRefine()`

Answers an entry opening by putting the shown language's example typography into the view's resources.
Example lines are drawn inside templates, so the typography reaches them through resources.
The lectern subscribes the typography before the cards, so the example lines draw in the pack's faces.

## `public void QLecternGlossRefine()`

Answers an entry opening the same way for the gloss typography under the example lines.

## `public void QLecternCardRefine()`

Sets the two card lists and the two sections' visibility from the cards `CDisplayCardRead` answers.
The lectern subscribes it to both open and close, since a closed display answers no cards.

## `private void QLecternLeafRefine(CLecternCard card)`

Hands the card lists the ready cards of `card`.
The lists are emptied first, so every template redraws even when a list keeps its length.

## `private void QLecternChipObserve(object sender, RoutedEventArgs e)`

Hears a click on a card chip and hands `CDisplayChipOpen` the ready chip it carries.
The chip is read off what was clicked, since a click leaving a template is re-sourced to its presenter.
A link chip also hands the linked Entry's id it carries.
The click is marked handled only when a tab was asked to open.

## `private void QLecternMentionObserve(object? sender, PMentionArgument e)`

Hears a word clicked in an example line and reads the sentence row and the raw click values.
It hands them unchanged to its one gate, `CDisplayMentionFind`.
The gate converts the click, reads the sentence itself, finds, opens and reports.
Its offer leaves through `QLecternMentionNotice` under the clicked control.

## Inline notes

### `_qLecternCardMeaning.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QLecternChipObserve));`

The chips are drawn from a shared dictionary that knows no window.
They reach the cards only through the two lists this sits on, as bubbling clicks.

### `_qLecternCardMeaning.AddHandler(PMention.PMentionClickEvent, new EventHandler<PMentionArgument>(QLecternMentionObserve));`

Every sentence on every card raises the same bubbling event.
One handler on each card list answers it, so a card template stays free of handlers.
