# LGraspPort.cs
Hash: `462550ecc6136ead`

## `public interface LGraspPort`

The slice of the engine a deportment sees when it rates how well an entry is known.
`LEntryFacade` implements it, since the grasp is stored on the entry.

## `int LEngineGraspStep { get; }`

The last grasp step, the limit a star control draws to.

## `string LEngineGraspFormat(int step);`

The wording of one grasp step, as a star control's tooltip shows it.

## `int LEngineGraspRead(long entryId);`

The grasp stored on the entry, from zero to the last step.
A missing entry reads zero, the same as one never rated.

## `void LEngineGraspSave(long entryId, int grasp);`

Writes the grasp, then raises a Grasp bulletin for the entry.
