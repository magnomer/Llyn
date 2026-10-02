# QLinkChip.cs

## `public sealed class QLinkChip : INotifyPropertyChanged`

One committed link inside a card's Translation field, painted from its ready Conduct target.
The target carries the id of another Entry and nothing else that is saved.
Its headword and language are display state, refreshed whenever the card is shown.
A link whose target should change is closed and written again rather than edited in place.

## `public QLinkChip(CTranslationTarget target)`

Holds the ready target and finds its flag for the target's language through `QEnsignImage`.

## `public CTranslationTarget QLinkChipTarget { get; }`

The ready target the chip paints, whose id a chip's pick or removal hands to its gate.

## `public ImageSource? QLinkChipFlag`

The flag of the target's language, or null while that flag has not loaded.

## `internal void QLinkChipRefine()`

Reads the flag again when the chip was built before the flags loaded.
A found flag is announced, so the chip's view reloads its image in place.
The chip keeps its slot in the field, since only its image changes.
A chip that already has its flag is left alone.
