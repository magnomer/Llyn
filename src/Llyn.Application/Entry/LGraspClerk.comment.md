# LGraspClerk.cs
Hash: `e5f79f75737ea851`

## `public sealed class LGraspClerk`

The user's grasp of an entry, a half-step rating stored on the entry row.
It runs over the entry port of one rig and records no revision.
A rating is a reading mark, so it stays apart from the clerk that edits the word.

## `public LGraspClerk(LRig rig)`

Reads the entry port out of `rig`.

## `public static int LGraspClerkStep`

The number of grasp steps, for the shell to draw.

## `public static string LGraspClerkFormat(int step)`

The localized label of one grasp step.

## `public void LGraspClerkSet(long entryId, int grasp)`

Writes the user's grasp onto the entry.
The range is checked before any write, so a bad value reaches neither the store nor a subscriber.
No revision is recorded, because a rating is a reading mark and not an edit of the word.
