# LLiveryCard.cs
Hash: `240587df8d35a9c1`

## `internal static class LLiveryCard`

Writes the meanings, collocations and incoming usages of a Joplin entry body for `LLiverySheet.LLiveryFormat`.
It reads only `LLiveryPage`, the `note` map and the `lookup` it is handed.

## `public static void LLiveryCardAppend(StringBuilder sheet, LLiveryPage page, Func<long, string> note, Func<string, string> lookup)`

`LLiverySheet.LLiveryFormat` calls it right after `LLiveryScript.LLiveryScriptAppend`.
It writes the meanings under `Display.MeaningPlural` through `LLiveryDeckAppend`.
Each meaning receives the entry's collocations, so the ones it is home to nest inside it.
It writes the collocations with no home among the meanings under `Display.CollocationOther` as rows.
Those are picked by `LCollocation.LCollocationOtherRead` and written by `LLiveryCollocation.LLiveryOtherAppend`.
`LLiveryIncomingAppend` writes the incoming usages last.

## `private static void LLiveryDeckAppend(StringBuilder sheet, LLiveryPage page, IReadOnlyList<LCardDraft> cards, string heading, string blank, Func<long, string> note, Func<string, string> lookup, IReadOnlyList<LCardDraft> collocations)`

Writes `heading` as a level two heading, then one card per entry of `cards` through `LLiveryFaceAppend`.
`blank` is the placeholder title for a card without one.
`collocations` is handed on to each card unchanged, and `lookup` words the row headings and stubs.
No cards write nothing, not even the heading.

## `private static void LLiveryFaceAppend(StringBuilder sheet, LLiveryPage page, LCardDraft card, string number, string blank, Func<long, string> note, Func<string, string> lookup, IReadOnlyList<LCardDraft> collocations)`

Writes one card as a `llyn-card` block, followed by a blank line.
A stored card opens through `LLiveryFoldFormat`, open unless `LLiveryPageFold` holds its id.
Its head sits inside a `summary`, so a folded card keeps its number and title.
A blank line follows the head, so Markdown parses the body inside the HTML block.
A card with no stored id stays a plain div, since it has nothing to fold.
Nested child cards follow the same rule.
The head holds `number` as a `llyn-number` chip and the title as a `llyn-title` span.
The title is the stored title, else the expression, else `blank` with `llyn-blank` added.
A card with both a title and an expression writes the expression under the head.
The stored meaning text follows through `LLiveryEtymology.LLiveryMentionFormat`, so its line breaks survive.
The content then follows in view mode's order.
`LLiveryFieldAppend` comes first, then the children, each as a nested card numbered after its parent.
`LLiveryBodyAppend` follows, then `LLiveryCollocation.LLiveryListAppend` with the collocations at home here.
`LLiveryCollocation.LLiveryStubAppend` closes the card with a stub for each further link here.

## `internal static void LLiveryFieldAppend(StringBuilder sheet, LLiveryPage page, LCardDraft card, Func<long, string> note)`

Writes the fields a card shows above its children.
`LLiveryCollocation` reuses it for a row body.
Situations, then registers, come first through `LLiveryBadgeAppend`.
Translations follow through `LLiveryTargetAppend`, above the examples as in view mode.

## `internal static void LLiveryBodyAppend(StringBuilder sheet, LLiveryPage page, LCardDraft card, Func<long, string> note)`

Writes the fields a card shows below its children.
`LLiveryCollocation` reuses it for a row body.
Examples come first through `LLiverySentenceAppend`.
Tags follow through `LLiveryBadgeAppend`.
Images and videos close it through `LLiveryMediaAppend`.

## `internal static string LLiveryFoldFormat(string style, bool opened)`

The opening of a foldable `details` block of class `style`, up to its open `summary` tag.
It carries `open` when `opened` is set.
A `llyn-card` draws its fold chevron at the top right of the summary.
A `llyn-collocation` row of `LLiveryCollocation` draws its chevron before the phrase.
`LLiveryXiesheng` reuses it for each series member with reflex rows.

## `private static string LLiveryNumberFormat(LCardDraft card, int place)`

The card's stored position as text, or `place` plus one when no position is stored.

## `private static void LLiverySentenceAppend(StringBuilder sheet, LLiveryPage page, LSentenceDraft sentence, Func<long, string> note)`

Writes one example as a Markdown list line.
The stored particle and dependence open it as `llyn-particle` and `llyn-dependence` spans.
The example text follows with each linked mention as a note link.
The byline from `LLiveryPageSource` for the example's reference follows as a `llyn-byline` span.
Each stored gloss follows a Markdown backslash break as a `llyn-gloss` span.
The gloss opens with its language's flag from `LLiveryHeader.LLiveryBannerFormat`, when one reads.
An empty sentence writes nothing.

## `private static void LLiveryTargetAppend(StringBuilder sheet, LLiveryPage page, LCardDraft card, Func<long, string> note)`

Writes the card's translation targets from `LLiveryPageTarget` as `llyn-target` chips in one paragraph.
Each chip links its headword through `LLiveryEtymology.LLiveryLinkFormat` and adds its language.
The chip opens with its language's flag from `LLiveryHeader.LLiveryBannerFormat`, when one reads.
A card with no targets writes nothing.

## `private static void LLiveryBadgeAppend(StringBuilder sheet, string style, IReadOnlyList<string> texts)`

Writes every non-blank text as a span of class `style`, all in one paragraph.
No such text writes nothing.

## `private static void LLiveryMediaAppend(StringBuilder sheet, LCardDraft card)`

Writes each stored image, then each stored video, as its own paragraph.
A web address becomes a Markdown link through `LLiveryAddressFormat`, and nothing is fetched.
An image file goes through `LLiveryHeader.LLiveryPictureFormat` with class `llyn-image`.
A relative or unreadable image path writes nothing.
A video file becomes a player opening with `LLiverySheet.LLiveryVideoHead`.
`LLiverySheet` later swaps its path for a resource, or drops the player when the file cannot be read.
A stored video span follows as a `llyn-span` chip.

## `private static void LLiveryIncomingAppend(StringBuilder sheet, LLiveryPage page, Func<long, string> note, Func<string, string> lookup)`

Writes `LLiveryPageIncoming` under the `Display.Translated` heading inside one `llyn-card` div.
Each usage is one paragraph.
It links `LUsageName` to the note of `LUsageEntry` through `LLiveryEtymology.LLiveryLinkFormat`.
The epithet, the owner chip and the language follow.
The language opens with its flag from `LLiveryHeader.LLiveryBannerFormat`, when one reads.
The owner chip reads `Display.MeaningSingle`, `Display.CollocationSingle` or `Portrait.Example`.
No usages write nothing, not even the heading.

## `private static string LLiveryAddressFormat(string location)`

A Markdown link whose text is the escaped `location` and whose target is `location` in angle brackets.
Angle brackets inside the address are percent-encoded, so the target cannot close early.

## `private static bool LLiveryAddressCheck(string location)`

Whether `location` starts with `http://` or `https://`, in any case.
