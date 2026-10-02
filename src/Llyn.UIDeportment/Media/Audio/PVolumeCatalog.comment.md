# PVolumeCatalog.cs
Hash: `14f683ff44188bce`

## `public sealed class PVolumeCatalog : INotifyPropertyChanged`

The one loudness the program plays at, held where anything that plays can reach it.

The slider that sets it stands in the playback tray, which belongs to the pronunciation.
A video is played from a card and knows nothing of that tray.
The two cannot be wired to each other directly.
They are wired to this instead, which is a value and not a control.

The level is a mirror, not the truth.
The truth is the workspace's audio level in the posture, which the volume owner sets as it moves.
The volume owner paints this on every tray, so a move in one tray moves every other.

## Inline notes

### `public double PVolumeCatalogLevel { get; set; }`

Held between silence and full, because that is the range every player here takes.
A value outside it is pulled to the nearest end rather than refused.
Only the volume owner sets it, from a moved grip or from the level a workspace opens with.
A set to the level it already holds raises no notice.
