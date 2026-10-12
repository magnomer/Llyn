# TSheetEntry.cs
Hash: `1b22334d2e3275b0`

## `public sealed class TSheetEntry`

Covers the entry sheet that HTML, PDF and print share, one fact per rule.
Each page is read from a stored entry and changed with `with` only where a fact needs it.
So every other member stays real.
The lookup answers each key as itself, so a label reads as its key.
Each fact asserts the fragment that carries the rule, never the whole document.

## `private const string TSheetEntryFlag = "data:image/png;base64,iVBORw0KGgo=";`

A tiny inline image that stands for a flag file.
The sheet embeds it once however many places show it.

## `public void SheetEntryFormat_StoredEntry_PaintsTheSurfaceWithThePaneInsets()`

The body takes the surface colour and the entry takes the insets of the view pane.

## `public void SheetEntryFormat_VarietyWithFlag_ShowsFlagInsteadOfLabel()`

A variety with a flag shows the flag with the variety as its title and no label.

## `public void SheetEntryFormat_VarietyWithoutFlag_ShowsLabel()`

A variety with no flag shows its name.

## `public void SheetEntryFormat_LanguageWithoutFlag_ShowsGlobe()`

A language with no flag shows the globe beside its name.

## `public void SheetEntryFormat_GraspThree_FillsOneFullAndOneHalfStar()`

A grasp of three fills one star and half of the next, and leaves three empty.
The rating label beside the stars is the key of level three.

## `public void SheetEntryFormat_GraspAboveStep_ClampsToTopLabel()`

A grasp of twelve clamps to ten and shows the key of the top level.

## `public void SheetEntryFormat_Unrated_ShowsNotRatedLabel()`

An entry never rated fills no star and shows the key of level zero.

## `public void SheetEntryFormat_RankedFrequency_WritesOnePillWithSourceTitle()`

A ranked frequency writes one pill whose title lists the source and the figure.
The Everyday tier lights three of the four pips and leaves the last one dim.

## `public void SheetEntryFormat_BreakRows_WritesPartNameTextGrid()`

A break row writes its part, its name and its shown text as three cells.
The part and the name are read from the row, since the row owns them.

## `public void SheetEntryFormat_HaberView_ChecksShortAndMarksIrregularRun()`

A view whose short and full tables differ writes the switch with the short side checked.
An irregular run of a form is wrapped in the marked span.

## `public void SheetEntryFormat_FoldedCard_WritesClosedDetails()`

A folded stored card writes closed details and an unfolded one writes open details.

## `public void SheetEntryFormat_UnlinkedCollocation_WritesOtherRow()`

A collocation linked to no meaning writes a row under the Other collocations heading.
The row sits in the collocations box and opens, since nothing folded it.
Its summary shows the phrase and then the sense.
The Other band is a plain band, never a deck of numbered cards.

## `public void SheetEntryFormat_OpenedFanqie_WritesOpenDetails()`

A fanqie box the entry holds open writes open details under its heading.
The heading is read from the label the relay hands the writer, never typed.

## `public void SheetEntryFormat_Stamps_WritesCreatedAndUpdatedFooter()`

The footer writes the created and updated stamps as a label and time grid.

## `public void SheetEntryFormat_FlagRepeated_EmbedsItOnce()`

The language and a variety share one flag.
The sheet writes one image rule and both places point at it.

## `public void SheetEntryFormat_PrintStyle_KeepsTheScreenSurface()`

The print rules ask for exact colours and drop only the frame padding.
The page margin is set once in its own `@page` rule.
They never force a white background.

## `private static LEntry TSheetEntrySave(LEngine engine)`

Stores one English entry with a plain note and one card, and answers it.

## `private static LLiveryPage TSheetEntryRead(LEngine engine, LEntry entry)`

Reads the stored entry's Livery page, so each fact starts from what the engine answers.

## `private static LLiveryPage TLiveryVarietyCreate(LEngine engine, LEntry entry, IReadOnlyDictionary<string, string> ensign)`

The stored page with a Brazil primary accent row and no other row.
The language banner is emptied, so only the variety can carry a flag.
`ensign` maps the variety to its flag, or stays empty for a variety without one.

## `private static string TSheetTextFormat(LEngine engine, LEntry entry)`

Writes the sheet of the stored entry's own page.

## `private static string TSheetTextFormat(LEngine engine, LEntry entry, LLiveryPage page)`

Writes the sheet of `page` beside the entry's portrait tree, with a lookup that answers each key as itself.

## `private static LParadigmForm TParadigmFormBuild(string text, int marked)`

A paradigm cell whose first `marked` letters are marked, or with no mark when `marked` is zero.
