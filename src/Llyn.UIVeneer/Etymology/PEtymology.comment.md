# PEtymology.cs

## `public sealed class PEtymology : ContentControl`

The etymology field as one control, so the reading view and the editor draw the same thing.
It holds the source chips and the narrative together, since an entry declares its origin in one of the two.
Editable, it adds an entry the user types a source into and writes the narrative in a box.
Read, it prints the narrative with its spans underlined and the chips beside it.
It shows and hides itself from what it was handed, except while editable, where the tab decides.

## `public static readonly DependencyProperty PEtymologyEditableProperty`

Which side of the shell draws it.

## `public static readonly DependencyProperty PEtymologyTextProperty`

The narrative as the draft holds it.

## `public static readonly DependencyProperty PEtymologyLanguageProperty`

The language a clicked span is resolved against.

## `public PEtymology()`

Builds the chips row, both faces of the narrative and the span chips once.
The templates come from the theme, so the two sides cannot drift apart.

## `public bool PEtymologyEditable`

Whether the field takes input.

## `public string PEtymologyText`

The narrative shown or edited.

## `public string PEtymologyLanguage`

The language a span is resolved against.

## `internal TextBox PEtymologyBox`

The narrative box, which the editor reads a selection from.

## `internal PEtymologyCaret PEtymologyEntry`

The entry a typed source word stands in.

## `internal void PEtymologySourceShow(IReadOnlyList<PEtymologyChip> chips)`

Draws the source links already resolved to a headword and a language.

## `internal void PEtymologyMentionShow(IReadOnlyList<LMentionDraft> mentions, string silent)`

Draws the spans, as underlines on the read side and as chips on the edit side.
A window the control was never given leaves the chips empty rather than guessing names.

## `protected override AutomationPeer OnCreateAutomationPeer()`

A surface peer, since the control is a box and not a button.

## Inline notes

### `private static void PEtymologyStateHandle(DependencyObject sender, DependencyPropertyChangedEventArgs e)`

Every property redraws the whole field, which is small enough to build again.

### `private void PEtymologyStateApply()`

Decides which face of the narrative stands and whether the field is worth showing at all.

### `private void PEtymologyItemApply(bool editable)`

The chips first and the typing entry last, so the field reads as one line of words.
