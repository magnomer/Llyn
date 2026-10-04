# CEsteem.cs
Hash: `5f3d6e7977baa120`

## `public sealed class CEsteem`

The user's marks on the stored entry an editor holds, its favourite flag and its grasp step.
The frequency chip is read here beside them, since all three stand on the stored id.
The editor builds it over its desk and its display, which holds every favourite and grasp rule.
It only supplies the stored id, and a refusal reaches the window through the display's envoy.

## `private long? LEsteemEntry`

The stored entry the held draft stands on, or null for a fresh draft or an empty desk.

## `public int CEsteemGrasp`

The stored entry's grasp step, zero for a fresh draft or a failed read.
It is never negative and never above `CEsteemGraspStep`, since the stars draw only that range.

## `public int CEsteemGraspStep`

The last grasp step, which the star control takes as its limit so it names no engine constant.
It is never negative, since `LDisplay` raises a negative engine limit to zero.

## `public event Action? CEsteemFavoriteChanged;`

Raised only by `CEsteemFavoriteSet`, refused or not, so the star reads the store again.
No bulletin raises it, unlike `CEsteemGraspChanged`.

## `public event Action? CEsteemFrequencyChanged;`

The held entry's frequency was counted again, raised on the driver's thread.

## `internal void LEsteemObserverAttach(Action<Action> marshal)`

Hears the frequency and grasp subjects of the held entry on every tenure the desk starts.
A grasp heard from elsewhere raises `CEsteemGraspChanged`, as the grasp gate does.

## `public void CEsteemFavoriteSet(bool marked)`

Marks or clears the favourite on the stored entry, then announces a re-read so a refusal redraws.

## `public void CEsteemGraspSet(int step)`

Writes the grasp step on the stored entry, then announces a re-read so a refusal redraws.
The step pressed again while it stands clears the grasp, by the display's one rule.

## `public string CEsteemGraspRead(int step)`

The wording of a grasp step, or empty for a fresh draft that has no grasp to word.
The engine words it, so the driver names no localization key for it.

## `public CFrequency? CEsteemFrequencyRead(string once)`

The frequency chip of the held entry, or null when no source ranks it.
The driver hands the localized word for a one-off figure, so Conduct stays out of localization.
