# PCaret.cs
Hash: `41e4c4447bef05ac`

## `internal sealed class PCaret<PCaretChip> : INotifyPropertyChanged where PCaretChip : class`

One chip field of a card together with the open entry the user types into.
The rows hold only the chips, and this object itself is the entry item the field's `QBerth` seats.
The entry is anchored to the chip it stands before, so moving it never reorders the chips.
The engine holds the chips, the field renders them by id, and every commit or removal is a request.
Where the entry stands is the field's own, and it is the position a new chip is asked for.
Each field differs only in the rules its owner hands the constructor.
It knows only chips, never a Conduct draft, since its field's driver builds the chips.

## `internal PCaret(string hint, Func<PCaretChip, long?> key, Func<PCaretChip, PCaretChip, PCaretChip> update)`

Takes the field's rules: the hint key, the chip's id, and whether a standing chip yields to a fresh one.
The rules are pure functions of their arguments, never a reach back into the owner.
The hint is set here, so an empty card shows its placeholder before any chip arrives.

## `public event PropertyChangedEventHandler? PropertyChanged`

Raised when the entry's text, hint or anchor changes, which the item fill and the berth hear.

## `public ObservableCollection<PCaretChip> PCaretRow { get; }`

The chips the field shows, in the engine's order.
The field's list takes it as its items, and the entry is not among them.

## `public string PCaretText`

What is standing in the entry.
Only a refine or a clear writes it.

## `public string PCaretHint`

The prompt the empty field shows.
The hint belongs to the empty field, not to the entry.
Once a chip stands beside the entry, prompting again would read as a second, unfilled field.

## `public PCaretChip? PCaretAnchor`

The chip the entry stands right before, or null when the entry stands at the end.
Only a step or a redraw writes it, so the chips' order is never touched.
A chip the engine adds at the entry lands before the anchor, so the entry stays after it.
When the anchor chip leaves, the entry moves before the next chip that stands, else to the end.

## `internal int PCaretPosition`

How many chips stand before the anchor, which is the place a new chip is asked for.
An entry with no anchor, or one whose chip is gone, stands at the end.

## `internal void PCaretShow(IReadOnlyList<PCaretChip> chips)`

Makes the row show the fresh chips its driver built, through `QLookItem.QLookItemShow`, matched by id.
A standing chip with the same id stays unless the revise rule answers the fresh one.
The entry stays before the first chip that followed it and still stands.

## `internal PCaretChip? PCaretFind(int step)`

The chip standing one step from the entry, before it or after it, or null.
That is what a backspace at the start, or a delete at the end, reaches for.

## `internal bool PCaretMove(int step)`

Steps the entry one place along the field by anchoring it to another chip.
A chip is passed over as a single character would be.
Reports whether there was anywhere left to go.

## `internal void PCaretClear()`

Empties the entry after a commit or a pick.
The empty text reaches the gate as any edit does, and an empty text adds nothing.

## `internal void PCaretRefine(string rest)`

Puts the text the field's gate keeps in the entry.
The editor's text observer hands every edit to the gate, which reads the comma.
So a pasted comma ends a chip as a typed one does.
