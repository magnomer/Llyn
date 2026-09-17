# LPortraitCard.cs

## `public static class LPortraitCard`

Turns meaning and collocation card drafts into numbered sections.
A meaning card and a collocation card share this shape, exactly as they do on screen.
Which one it is shows only in the kind word that stands in for a missing title.
Every child carries the role it plays on screen, so a writer never guesses a role from a shape.

## `public static IReadOnlyList<LPortraitSection> LPortraitCardCreate(IReadOnlyList<LCardDraft> cards, string kind, LSentenceOrder order, string language, LPortraitLabel label, IReadOnlyDictionary<long, LPortraitLink> targets, IReadOnlyDictionary<long, string> sources)`

One section per card, in stored order, numbered by the card's position.
The heading is the title, else the kind word, and the role says which.
So a writer can mute a heading that is only the kind.
The expression becomes a phrase child standing first, drawn before the meaning as the panel does.
The meaning is the card's one unlabelled line, skipped when empty.
Situations become a scene child, registers a tone child and tags a label child, each of chips.
Translations become a bridge child of links, and an id with no loaded target is dropped.
Each written sentence becomes a child, and the child cards recurse through here.
The children stand in the display's order: phrase, situations, registers, translations, examples, tags, then cards.
An unset field becomes empty text and an unknown one becomes the mark, as the display does.
Media whose location was never written is skipped, because the display draws no empty plate.

## `public static void LPortraitCardAdd(List<LPortraitSection> children, string heading, LPortraitRole role, IReadOnlyList<string> chips, IReadOnlyList<LPortraitLink> links)`

Adds one unnumbered child of the given role carrying the chips or links, skipped when both are empty.

## `private static void LPortraitCardAdd(List<string> chips, LStateValue value, string mark)`

Adds one chip when the value reads as text.
