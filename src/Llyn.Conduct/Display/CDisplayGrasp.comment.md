# CDisplayGrasp.cs
Hash: `73c703c3cad06bed`

## `public sealed class CDisplayGrasp`

The reading view's grasp area, holding the stars' reads, their gate and their change event.
It is split from [CDisplay](CDisplay.comment.md) by concern, since the grasp touches no other part of the header.
The header area builds one over its rules.
It holds no state of its own, and every member works on the entry the rules say is chosen.

## `internal CDisplayGrasp(LDisplay rule)`

Only the header area builds its grasp area, over the rules it built.
The grasp rule and the chosen entry both stay in `LDisplay`, so this area only calls them.

## `public event Action<CBulletin>? CDisplayGraspChanged;`

Raised on the engine's thread when the chosen entry's grasp step changed.
A driver marshals it onto its own thread before it reads.

## `public int CDisplayGraspStep`

The last grasp step, which the star control takes as its limit so it names no engine constant.
It is never negative, since `LDisplay` raises a negative engine limit to zero.

## `internal void LDisplayGraspAttach()`

Subscribes the grasp subject of the current vista to `CDisplayGraspChanged`.
The header area calls it from its vista restore, since each vista is new and is subscribed once.

## `public CGrasp CDisplayGraspRead()`

The chosen entry's grasp step with its wording.
The step is never negative and never above `CDisplayGraspStep`, since the stars draw only that range.

## `public string CDisplayGraspRead(int step)`

The wording of any step, for the step under the pointer, or empty while nothing is chosen.

## `public CGrasp CDisplayGraspSet(int step)`

The gate for a star the reader pressed on the chosen entry, answering the stored stars.
A press on the standing step comes back cleared, since the display's one rule owns that.
