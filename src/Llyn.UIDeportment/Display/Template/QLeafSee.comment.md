# QLeafSee.cs
Hash: `19c10c485fdc4d46`

## `internal sealed class QLeafSee`

Presentation item for one "see" line under a meaning, naming a collocation homed under another meaning.
It carries the collocation's id for the click that opens its row, and the ready line the link prints.

## `internal QLeafSee(long id, string text)`

Builds the item from the collocation's id and the line Conduct worded.

## `internal long QLeafSeeId { get; }`

The collocation's id, handed back unread to the fold gate and the contents spotlight.

## `internal string QLeafSeeText { get; }`

The ready line, "→ {phrase} see {N}".
