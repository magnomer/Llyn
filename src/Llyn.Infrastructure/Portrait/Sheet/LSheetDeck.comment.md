# LSheetDeck.cs
Hash: `b6f318017bcde83c`

## `internal static class LSheetDeck`

Writes the meaning band of an exported entry body as folding cards.
The card content comes from the portrait section, and only the frame is written here.

## `internal static void LSheetDeckAppend(StringBuilder sheet, LLiveryPage livery, LPortraitPage portrait, string heading, IReadOnlyList<LCardDraft> cards)`

Writes one headed band of top-level cards, or nothing when the page lacks the heading.
`LPortraitCard` makes one section per draft card in order, so the same index pairs them.
The draft card gives the id the fold state is kept under.
A section past the end of the draft list counts as unstored.

## `private static void LSheetFaceAppend(StringBuilder sheet, LPortraitSection face, long id, IReadOnlySet<long> fold)`

A stored card is a `details` element, open unless `LLiveryPageFold` holds its id.
Its summary holds what `PLeaf` shows in the card header.
The hinge is no element, since the style paints it after the summary.
The rank always shows.
A titled card shows its title.
An untitled one shows its kind as `caption` and its first line as `peek`.
The view swaps those two as the card folds, which a later style does from the open state.
An unstored card cannot fold, so it is an `article` with a header and no peek or hinge.
The body is `LSheetSection.LSheetSenseAppend` in both cases.

## `private static string LSheetPeekRead(LPortraitSection face)`

Reads the peek as `QLecternCard` picks it.
A meaning card peeks its meaning, the first sense line, empty when the card has none.
