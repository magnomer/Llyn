# CGuildUnion.cs
Hash: `4a544da4027fce12`

## `public sealed class CGuildUnion`

The union of the Author being written into a kept Author, beside the guild's autograph.
It asks the union question through `CEnvoy` and shows a failed fold through `CLedger`.
It reads the stored Author off the roll panel's aperture, so it keeps no choice of its own.

## `internal CGuildUnion(LAuthorPort port, CEnvoy envoy, LSettingsPort settings, CDesk autograph, COeuvre oeuvre, CPanel panel, Action<long> seam)`

Holds the author port, the envoy, the settings, the autograph desk, the oeuvre and the roll panel.
`seam` reopens a kept Author on the reading side, which only the guild knows how to do.
A started tenure on the autograph clears the union offer, raised as `CGuildUnionCleared`.
Building it is no user action, so the guild builds it once its panels stand.

## `public event Action? CGuildUnionCleared;`

A tenure started on the autograph, so the union search and its offer are emptied.

## `public bool CGuildUnionShown`

The union section shows only while the autograph holds a stored Author, since a fresh one has nothing to fold.

## `public IReadOnlyList<CCatalogAuthor> CGuildUnionRead(string typed)`

The Authors the one being written may fold into, as the union list shows them.
The engine drops blank text, leaves the Author itself out and caps the list.
The rows are mapped through the oeuvre, as the roll's own rows are.

## `public void CGuildUnionSelect(long? id)`

A pick in the union list asks before folding the written Author into the kept one.
A confirmed fold reopens the kept Author on the reading side through the seam.
A failure is shown as `Guild.MergeFailed` and leaves the written Author open.

## `private bool LGuildUnionConfirm(long kept)`

Asks the union question with the written name first and the kept name second.
The engine reads both names off the held tenure and the kept id, which Conduct hands on unread.
