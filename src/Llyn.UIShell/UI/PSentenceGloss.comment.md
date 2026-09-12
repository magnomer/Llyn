# PSentenceGloss.cs

## `internal sealed partial class PSentence`

The Gloss rows a card's sentence row shows under its sentence.

## `public ObservableCollection<PGloss> PSentenceGloss { get; }`

The rows in the order the Example keeps them.

## `internal static string PSentenceGlossFormat(PGloss gloss, string field)`

The field name of one Gloss edit, the property joined to the Gloss id.
The sentence row raises it as its own property change, so the card forwards it like any other field.
The editor keys its pending request by it, and the redraw asks by it.

## `internal PGloss? PSentenceGlossFind(string field, out string name)`

Splits a field name of that shape back into the Gloss it names and the property.
Any other field gives null.

## `private void PSentenceGlossShow(IReadOnlyList<LGlossDraft> drafts, Func<PGloss, string, bool> pending)`

Redraws the rows from the drafts, keeping the rows whose ids survive and skipping a field still pending.

## `private PGloss PSentenceGlossCreate(LGlossDraft draft)`

One row, subscribed so its edits reach the card.

## `private void PSentenceGlossChange(object? sender, PropertyChangedEventArgs arguments)`

Forwards a text or language edit of one row as a property change of the sentence row.
