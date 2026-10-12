# LPortraitCard.cs
Hash: `75f8fa2f83350454`

## `public static class LPortraitCard`

Turns meaning card drafts into numbered sections, and collocations into rows.
A homed collocation and an unlinked one share the row shape, exactly as they do on screen.
Every child carries the role it plays on screen, so a writer never guesses a role from a shape.

## `public static IReadOnlyList<LPortraitSection> LPortraitCardCreate(IReadOnlyList<LCardDraft> cards, string kind, LSentenceOrder order, string language, LPortraitLabel label, IReadOnlyDictionary<long, LPortraitLink> targets, IReadOnlyDictionary<long, string> sources, IReadOnlyList<LCardDraft> meanings, IReadOnlyList<LCardDraft> collocations, IReadOnlySet<long> folds)`

One section per card, in stored order, numbered by the card's position.
The heading is the title, else the kind word, and the role says which.
So a writer can mute a heading that is only the kind.
The expression becomes a phrase child standing first, drawn before the meaning as the panel does.
The card's fields follow through `LPortraitBodyAdd`, then the child cards recurse through here.
`meanings` is the whole meaning tree and `collocations` the collocations that may link into it.
Under a meaning, the collocations whose home it is follow its child cards as one list child.
The list is headed by the label's heading word filled with the row count, and holds the rows `LPortraitRowsCreate` gives.
After the list come the see-lines of the collocations that link it without it being their home.
`folds` holds the cards whose fold is stored, handed on to the rows.
An unset field becomes empty text and an unknown one becomes the mark, as the display does.
Media whose location was never written is skipped, because the display draws no empty plate.

## `public static IReadOnlyList<LPortraitSection> LPortraitRowsCreate(IReadOnlyList<LCardDraft> collocations, LSentenceOrder order, string language, LPortraitLabel label, IReadOnlyDictionary<long, LPortraitLink> targets, IReadOnlyDictionary<long, string> sources, IReadOnlySet<long> folds)`

One row per collocation, in the given order.
A homed list and the unlinked band share this one row shape.
A row whose collocation is in `folds` carries the fold, the rest stand open.

## `private static LPortraitSection LPortraitRowCreate(LCardDraft collocation, LSentenceOrder order, string language, LPortraitLabel label, IReadOnlyDictionary<long, LPortraitLink> targets, IReadOnlyDictionary<long, string> sources, bool folded)`

One unnumbered row for a collocation.
The heading is the phrase `LCollocation.LCollocationPhraseRead` gives, and the line is the meaning.
Every filled field follows as a child through `LPortraitBodyAdd`, and the row carries its images and video.
`folded` is kept on the row as its fold, for a writer that folds.
The row's content is the same either way.

## `private static List<LPortraitLine> LPortraitMeaningRead(LCardDraft card, string mark)`

The card's meaning as its one unlabelled line, or no line when it reads empty.

## `private static void LPortraitBodyAdd(List<LPortraitSection> children, LCardDraft card, LSentenceOrder order, string language, LPortraitLabel label, IReadOnlyDictionary<long, LPortraitLink> targets, IReadOnlyDictionary<long, string> sources)`

Adds the fields a card and a row share, in the display's order.
Situations become a scene child, registers a tone child, translations a bridge child of links, each skipped when empty.
A translation id with no loaded target is dropped.
Each written sentence becomes a child, then the tags a label child of chips.

## `public static void LPortraitCardAdd(List<LPortraitSection> children, string heading, LPortraitRole role, IReadOnlyList<string> chips, IReadOnlyList<LPortraitLink> links)`

Adds one unnumbered child of the given role carrying the chips or links, skipped when both are empty.

## `private static void LPortraitStubAdd(List<LPortraitSection> children, LCardDraft card, IReadOnlyList<LCardDraft> meanings, IReadOnlyList<LCardDraft> collocations, LPortraitLabel label)`

Adds one see-line for each collocation that links the meaning but has its home elsewhere.
The line names the phrase `LCollocation.LCollocationPhraseRead` gives, and the number of the home meaning.
A collocation whose home has no number is skipped.
The line is an unnumbered section with one unlabelled line and no heading.

## `private static void LPortraitCardAdd(List<string> chips, LStateValue value, string mark)`

Adds one chip when the value reads as text.
