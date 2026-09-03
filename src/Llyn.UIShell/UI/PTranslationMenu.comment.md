# PTranslationMenu.cs

## `public partial class PEditor`

The editor's half of the Translation field: chip removal, caret keys, focus routing and resolution.
The templates that draw the field cannot reach the card the links belong to.
An item's data context is the link or the entry, not the card.
So each handler asks which card's collection holds the item, as the Tag field does.

Resolution lives here rather than on the card because it is the editor that holds the engine.
A card holds ids and knows nothing about how a word becomes one.

## `internal void PTranslationAttach(PCard card)`

Gives a card the way back to the editor its typed words are resolved through.
Every card the editor builds is attached, whether it was loaded or added by hand.

## `internal void PTranslationChipHandle(object sender, RoutedEventArgs e)`

Closes the link whose button was pressed.

## `internal void PTranslationEntryHandle(object sender, KeyEventArgs e)`

An open dropdown is offered every key first, so it answers the arrows and enter before the field does.
Enter resolves what is standing in the entry once no dropdown is open.
Escape closes an open dropdown and leaves the word alone.
Backspace at the start of the entry drops the link before it.
Delete at the end of the entry drops the link after it.
The arrow keys walk the entry past a link once the text runs out.
Every other key is left to the text box.

## `internal void PTranslationCloseHandle(object sender, RoutedEventArgs e)`

Resolves what is standing in the entry when focus leaves the field.
A word one Entry answers becomes a link, as it would on enter.
Anything else is left in the entry rather than opening a dropdown the user has already walked away from.

## `internal void PTranslationFocusHandle(object sender, MouseButtonEventArgs e)`

Puts the caret in the entry when the field's empty space is clicked.
So the whole box behaves as the one input it looks like.

## `internal void PTranslationMenuHandle(object sender, MouseButtonEventArgs e)`

Takes the row the pointer chose out of the dropdown and links it.
The row is a plain surface rather than a button, so the list beneath it keeps its own selection.

## `internal void PTranslationFlagUpdate()`

Redraws every card's chips once the workspace's flags have finished loading.

## `internal void PTranslationFreshClear()`

Forgets the stubs made while editing, because a saved entry keeps every stub it linked.

## `internal void PTranslationFreshDelete()`

Drops the stubs made while editing when the edit is thrown away.
A stub something else already links to is left standing.
A stub that will not delete is passed over rather than stopping the discard.

## Inline notes

### `private bool PTranslationMenuHandle(Key key)`

The dropdown never takes focus, so the caret's key handler drives it.
The arrows walk the selection and wrap at either end, and enter takes what is selected.
Reports whether the dropdown used the key, so an unused one falls through to the field.

### `private void PTranslationMenuSelect(PTranslationItem item)`

Links the chosen row and closes the dropdown, making the stub Entry first when the row is a create row.
A stub that will not be made raises a notice rather than leaving the field looking unanswered.

### `private bool PTranslationResolve(PCard card, string text, bool offered)`

One Entry whose whole headword is the word is not a choice, so it is linked without asking.
Anything else is a question, and a question is only asked when the user is still in the field.
That is what `offered` says.
An unasked question leaves the word standing in the entry, where the user can still see it.
A search the workspace refuses raises a notice, because a silent nothing reads as no match.

### `private void PTranslationMenuShow(PCard card, string word, IReadOnlyList<LEntry> found)`

The create row stands after the matches rather than among them.
It carries the typed word untouched, because that word is what the stub Entry will be called.

### `private IReadOnlyList<string> PTranslationLangcodeRead()`

The stub is offered once per language the workspace holds, so the user picks rather than accepts.
The language being edited comes last, because a link usually crosses into another one.
The workspace's own list is asked, because a language it does not hold has no flag to draw.
A list that has not loaded yet still names the language being edited, so a stub is always reachable.

### `private TextBox? PTranslationBoxFind(PCard card)`

The dropdown hangs off the caret it was opened from.
The caret has no name to bind to, so the focused box is asked whether it is that card's.

### `private static void PTranslationEntryApply(TextBox box, PTranslationEntry row, int caret)`

Moving the entry rebuilds its item, so the focused box is gone by the time the move lands.
The caret is put back on the newly drawn entry once the field is laid out again.

### `private static TextBox? PTranslationEntryFind(DependencyObject root)`

The entry is found by walking the drawn field, because it is one item of a templated collection.
Its position moves as links are added.
