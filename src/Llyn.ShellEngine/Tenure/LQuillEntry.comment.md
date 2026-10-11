# LQuillEntry.cs
Hash: `923cb6b552ca3f64`

## `public sealed class LQuillEntry`

The edits of the held entry's own fields: headword, note, language, lexical unit and freedom.
Each edit builds exactly one request for the tenure.

## `private readonly LTenure _lQuillEntryTenure;`

The tenure every request is built for and handed to.

## `public LQuillEntry(LTenure tenure)`

Builds the edits over one tenure, which they never swap.

## `public void LQuillHeadwordSet(string text)`

Defers the typed headword.

## `public void LQuillNoteSet(string text)`

Defers the note without the trailing line breaks a text box carries.
`LDraftClerkEntry.LNoteResolve` owns that trim.

## `public static bool LQuillNoteCheck(string text, string note)`

Answers whether a typed text already holds the note, by the same trim the set applies.
A driver asks it before painting, so a line break just typed is not overwritten.

## `public void LQuillLanguageSet(string language)`

Sends the chosen language at once.
An empty choice changes nothing.

## `public LUnit LQuillUnitRead()`

The lexical unit of the displayed draft, unchosen when no draft is held.

## `public IReadOnlyList<LUnit> LQuillUnitScan()`

The units every draft is offered, in dropdown order.
The language plays no part, since the user alone decides which unit fits.

## `public void LQuillUnitSet(LUnit unit)`

Applies the chosen unit to the draft at once.
Choosing the unit already held clears it, so the dropdown both sets and unsets.

## `public LFreedom LQuillFreedomRead()`

The freedom of the displayed draft, unchosen when no draft is held.

## `public IReadOnlyList<LFreedom> LQuillFreedomScan()`

The freedoms every draft is offered, in dropdown order, whatever its unit or language.

## `public void LQuillFreedomSet(LFreedom freedom)`

Applies the chosen freedom to the draft at once.
Choosing the freedom already held clears it, so the dropdown both sets and unsets.
