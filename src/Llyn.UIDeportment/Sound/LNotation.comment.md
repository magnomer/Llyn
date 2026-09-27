# LNotation.cs

## `public sealed class LNotation`

The deportment of the pronunciation menu, reading the editor desk's reading search while readings are searched.
The menu asks it for the search's language, flag, scheme and target, so it words the rows it receives.
The desk holds the search, so this deportment holds no engine handle.
A new start cancels the last search, and a closed menu cancels it too.
The search streams its steps to a delegate the menu hands in at the start, wrapped for the menu's thread.
The menu hands this deportment's own step handler, so the split into source, candidate and end happens here.
Each step becomes one notice, and the menu wires a control write to each notice and decides nothing itself.

## `public event Action<string, int>? LNotationSourceStarted;`

A source began searching, named with its position in the pack's list.
`LNotationCandidateAdded` is a source's one answer, and `LNotationFinished` is the end of the whole lookup.

## `public void LNotationStepHandle(CLookupStep step)`

One step of the lookup, told apart by the verdicts the step answers.
A step carrying a candidate is that source's answer.
A step that is the end closes the lookup.
Any other step is a source starting.
