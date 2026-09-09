# LPortraitCard.cs

## `public sealed record LPortraitCard`

One meaning or collocation card, resolved to what the card shows.
A meaning card and a collocation card share this shape, exactly as they do on screen.
Which one it is shows only in the kind label and in whether an expression is set.

**Parameters**

- `LPortraitCardPosition` - the number in the round badge on the card header.
- `LPortraitCardTitle` - the header title, empty when the card carries none.
- `LPortraitCardKind` - the localized word shown in place of a missing title.
- `LPortraitCardExpression` - the collocation phrase, empty on a meaning card.
- `LPortraitCardMeaning` - the definition line.
- `LPortraitCardSituation` - the situation chips.
- `LPortraitCardTranslation` - the linked entries this card translates to.
- `LPortraitCardExample` - the example lines, each already framed.
- `LPortraitCardTag` - the tag chips.
- `LPortraitCardImage` - the pictures shown under the card body.
- `LPortraitCardVideo` - the video plates shown under the pictures.

## `public static IReadOnlyList<LPortraitCard> LPortraitCardCreate(IReadOnlyList<LCardDraft> cards, string kind, LSentenceOrder order, string mark, IReadOnlyDictionary<string, LPortraitLink> targets)`

Turns stored card drafts into what the cards show.
An unset field becomes empty text and an unreadable one becomes the mark, as the display does.
A translation id with no loaded target is dropped rather than shown as a bare id.
Media whose location was never written is skipped, because the display draws no empty plate.
