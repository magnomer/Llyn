# PSentenceCitation.cs

## `public partial class PEditor`

The citation field of an Example row: what a typed line becomes, and how rows show the Source they cite.
A citation is either a stored Source or nothing, so the field never keeps a line that matches no Source.
One list of Sources serves every row on the form, Example and Situation alike.

## `private void PCitationCommitObserve(object sender, KeyEventArgs e)`

Hears enter in the citation field and hands the typed line to the card gate, which shows its own failure.
The dropdown's own key handlers hear each key first, so a key the open dropdown takes never reaches here.
It then shuts the dropdown and puts the cited byline back, since a failed line never stays.

## `private void PCitationEscapeRefine(object sender, KeyEventArgs e)`

Puts the cited byline back in the field when escape is pressed and the dropdown is already shut.
The open dropdown takes escape itself, so this only discards the typed line.

## `private void PCitationLeaveRefine(object sender, KeyboardFocusChangedEventArgs e)`

Leaving the field discards whatever was typed and not taken, and shuts the dropdown.
Only the focused field can have opened the dropdown, so it is shut without asking which field did.
A citation is either a stored Source or nothing, so a half-typed line is never kept.

## `private static void PCitationTextRefine(TextBox box)`

Puts the cited byline back in the field by looking it up from the row again.
The field keeps no line of its own, so discarding the typed one is only a re-read.

## `private static void PSentenceRevealAttach(ItemsControl list)`

Shows each row's writing controls while the sentence list is hovered or holds the focus.
The list is unsubscribed before it is subscribed, so its rows may be filled any number of times.
Data triggers reaching up to the list did this before.

## `private static void PSentenceRevealRefine(object sender, MouseEventArgs e)`

Hears the pointer entering or leaving the sentence list and has the list show or fade its controls.
WPF needs one handler per event signature, so this and the focus overload are the same look role.

## `private static void PSentenceRevealRefine(object sender, DependencyPropertyChangedEventArgs e)`

Hears the focus entering or leaving the sentence list and has the list show or fade its controls.

## `internal static void PSentenceRevealRefine(ItemsControl list)`

Shows or fades the controls of every row, and hides an empty citation while the list is idle.

## `internal void PSentenceCitationRefine()`

Reads every Source the workspace holds, with its byline, into the list the form offers.
It then has every row read the byline of the Source it cites again.
It answers each opened workspace and every reference change the sentence area raises.
So a byline edited elsewhere is never stale here.
The read shows its own failure and answers no Sources.
So the list is left empty rather than failing the form.
