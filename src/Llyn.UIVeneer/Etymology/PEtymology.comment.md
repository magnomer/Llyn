# PEtymology.cs

## `public sealed class PEtymology : ContentControl`

The etymology field as one control, so the reading view and the editor draw the same thing.
It holds the source chips and the narrative together, since an entry declares its origin in one of the two.
Editable, it adds an entry the user types a source into and writes the narrative in a box.
Read, it prints the narrative with its spans underlined and the chips beside it.
The host decides whether the whole field shows, since the editor's tab and the reading view differ.
Every visibility verdict comes from the display deportment, so the control only draws.

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

## `internal void PEtymologyShow(string language, string text, IReadOnlyList<LTranslationTarget> etymons)`

Sets the language and the narrative, then draws the source links.
The reading view's card deportment calls it as its seam.

## `internal void PEtymologySourceShow(IReadOnlyList<LTranslationTarget> etymons)`

Draws the source links the engine already resolved to a headword and a language.
The typing entry always closes the row, collapsed on the read side, so the row has one shape.

## `internal void PEtymologyMentionShow(LWindow window, string text, IReadOnlyList<LMentionDraft> mentions, string silent)`

Draws the spans as chips under the narrative box.
Only the editor calls it, and it hands the window and the text in, so nothing is guessed.

## `protected override AutomationPeer OnCreateAutomationPeer()`

A surface peer, since the control is a box and not a button.

## Inline notes

### `private static void PEtymologyStateHandle(DependencyObject sender, DependencyPropertyChangedEventArgs e)`

Every property redraws the whole field, which is small enough to build again.

### `private void PEtymologyStateApply()`

Decides which face of the narrative stands, from the verdicts the display deportment gives.
