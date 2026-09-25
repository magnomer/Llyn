# PCorpusEditor.cs

## `public partial class PCorpus`

Editing behavior of the Corpus panel.
An Example is quoted by any number of cards, so rewriting it here rewrites what every one of them quotes.
The panel offers no way to fork an Example while editing it.
Editing an Example means editing that Example.
A sentence meant for one card alone is a new Example on that card.
What the controls hold is pushed to a draft the engine keeps.
A crash costs the last keystrokes rather than the sentence.
The panel keeps no copy of a stored Example, and asks the engine what counts as a change.

## Inline notes

### `private bool _pTranscriptLoading;`

Set while fields are being filled from the held sentence.
Filling a field raises the same change the user typing raises, and only the second may clear an unknown mark.
It also holds the push back, so a fill never writes what it has just read.

## `private void PSpeakerLoad()`

Reads the workspace languages the picker offers.
No default is chosen, because an Example's language is optional and the store accepts none.
Choosing one here would put a language on every sentence the user never named.

## `internal void PSpeakerHandle(object sender, RoutedEventArgs e)`

The pick goes to the engine, and the chip changes when the draft bulletin returns.

## `private string PTranscriptHintRead(bool unknown)`

The placeholder the sentence field shows while empty.
A never-written sentence asks for a sentence, and an unknown one reads the unknown mark until typing clears it.

## `private void PTranscriptApply(LExample? example)`

Fills every field from the held sentence, or empties them when no draft stands.
The chip line is redrawn from the sentence's Mentions along with the fields.
The language is taken as the sentence carries it, an empty one included.
Keeping the last shown language would write it onto a sentence imported without one.
The tally chip follows the stored id the draft names, not the sentence being written.
A sentence the engine has not stored yet is quoted nowhere.

## `private void PTranscriptShow(LExample example)`

Redraws each field from the held sentence only where the field says something else.
The language and the citation are compared the same way, so a bulletin that changed nothing redraws nothing.

## `private void PTranscriptFieldShow(TextBox field, LStateValue value, ref bool held)`

Writes one field and its mark from a value, unless the field already reads that value.

## `private void PCorpusFreshHandle(object sender, RoutedEventArgs e)`

New makes whatever the emptier panel would list, and the deportment decides which.
With no Example chosen and no Entry shown, it opens the editor on a sentence nothing has stored.
With an Example chosen, or an Entry shown, it starts a new Entry instead.
