# LEngineGrasp.cs

## `public int LEngineGraspStep => LGrasp.LGraspStep;`

The last grasp step, handed out so the shells draw the stars without naming the Core constant.

## `public string LEngineGraspFormat(int step)`

The localized wording of a grasp step.

## `public int LEngineGraspRead(long entryId)`

Reads the half-step grasp stored on the entry.
A missing entry reads zero, the same as an entry never rated.

## `public void LEngineGraspSave(long entryId, int grasp)`

Writes the user's grasp onto the entry and raises a Grasp bulletin for it.
The range is checked before any write, so a bad value reaches neither the store nor a subscriber.
No revision is recorded, because a rating is a reading mark and not an edit of the word.
