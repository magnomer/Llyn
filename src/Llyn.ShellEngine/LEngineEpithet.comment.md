# LEngineEpithet.cs

## `public sealed partial class LEngine`

The epithet an entry is listed with: the reading a pack's reflex rule names, printed after the headword.
Every list and candidate menu asks for it, so a Han character reads `弄 [희롱할 롱]` wherever it is named.
The epithet is never part of the headword, and the setting turns it off everywhere at once.
It is derived when the reflex rows are stored and kept on the entry row.
So a list reads one stored string per row and runs no rule and no pattern.

## `private static readonly TimeSpan LEngineEpithetPatience`

How long a clip pattern may run over one piece before it is given up.

## `private void LEngineEpithetUpdate(long entryId)`

Derives the epithet of the Entry from its stored reflex rows and its pack's rules, and stores it.
Empty is stored when the Entry keeps no row of an epithet rule or the pack names none.
Every seam that stores reflex rows calls it, so the stored string never lags the rows.

## `private static bool LEngineEpithetCheck(IReadOnlyList<LReflexRule> rules)`

Whether any rule carries an epithet template, so an Entry without one is not read for rows.

## `private static string LEngineEpithetFormat(IReadOnlyList<LReflexRule> rules, IReadOnlyList<LReflex> rows)`

The pieces of every epithet rule, in rule order and then row order, joined by commas.

## `private static string LEngineEpithetFormat(LReflexRule rule, LReflex row)`

One row printed through the rule's template, with the clip pattern cut out and the blanks collapsed.
A clip pattern that fails to compile or times out leaves the piece uncut.
