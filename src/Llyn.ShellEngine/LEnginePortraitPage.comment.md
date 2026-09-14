# LEnginePortraitPage.cs

## `public sealed partial class LEngine`

Assembles the likeness of one example, source or situation from what the workspace holds.
The panels read the same rows through the same engine, so screen and page show one thing.

## `public LPortraitPage LEnginePortraitRead(long id, LOwner owner, LPortraitLegend legend)`

Which realm is portrayed is what `owner` names, and each realm is read under the engine's lock.
A missing row is an error, because a caller asked to portray one that no longer stands.
The usage tally is counted from the rows citing the view, as the catalog counts it.

## `private static InvalidOperationException LEnginePageRaise()`

The one wording for a page whose row is gone.

## `private static LPortraitPage LEnginePageRead(LExample example, LReference? cited, int count, LPortraitLegend legend)`

The sentence heads the page, its language is the chip, and its usage tally follows.
Each gloss is a line labelled with its language, under the translation heading.
The cited source is named as the catalog names it, and an example citing nothing shows no source.

## `private static LPortraitPage LEnginePageRead(LReference reference, IReadOnlyList<LAuthor> credits, int count, LPortraitLegend legend)`

The title heads the page, and the kind and usage tally are the chips.
Credited authors are joined on one line, and an unknown credit shows the mark alone.
Year, address and note each take a section only when written or marked unknown.

## `private static LPortraitPage LEnginePageRead(LSituation situation, int count, LPortraitLegend legend)`

The title heads the page, and the kind and usage tally are the chips.
The description is Markdown, carried as a note so it draws as blocks.
Pictures and videos share one unheaded section, as the vignette shows them without a heading.

## `private static string LEngineTitleRead(LStateValue value, string vacant, string mark)`

The head of the page: the written title, the mark when unknown, or the realm's vacant word.

## `private static void LEngineSectionAdd(List<LPortraitSection> sections, string heading, LStateValue value, string mark)`

Adds a one-line section when the value is written or unknown, and nothing when unspecified.
That is the same rule the read views follow when they collapse an unwritten field.
