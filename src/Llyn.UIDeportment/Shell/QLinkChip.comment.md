# QLinkChip.cs
Hash: `961fb2f6e76128ff`

## `public sealed class QLinkChip : INotifyPropertyChanged`

One committed link inside a card's Translation field, copied from a ready Conduct target.
It holds plain values only, so no Conduct record rides a caret or a leaf.
The id names another Entry and is the only saved part.
Its headword and language are display state, fixed when the chip is built.
A link whose target should change is closed and written again rather than edited in place.

## `public QLinkChip(long id, string headword, string language)`

Holds the target's id, headword and language, and finds the language's flag through `QEnsignImage`.

## `public long QLinkChipId { get; }`

The linked Entry's id, which a chip's pick or removal hands to its gate.

## `public string QLinkChipHeadword { get; }`

The linked Entry's headword the chip paints.

## `public string QLinkChipLanguage { get; }`

The linked Entry's language, which picks the chip's flag.

## `public ImageSource? QLinkChipFlag`

The flag of the target's language, or null while that flag has not loaded.

## `public event PropertyChangedEventHandler? PropertyChanged`

Raised with `QLinkChipFlag` once a late flag is found.

## `internal void QLinkChipRefine()`

Reads the flag again when the chip was built before the flags loaded.
A found flag is announced, so the chip's view reloads its image in place.
The chip keeps its slot in the field, since only its image changes.
A chip that already has its flag is left alone.
