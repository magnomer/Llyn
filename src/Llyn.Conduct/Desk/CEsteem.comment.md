# CEsteem.cs

## `public sealed class CEsteem`

The user's marks on the stored entry an editor holds, its favourite flag and its grasp step.
The frequency chip is read here beside them, since all three stand on the stored id.
The editor builds it over its desk and its display, which holds every favourite and grasp rule.
It only supplies the stored id, and a refusal reaches the window through `LDisplayFailed`.

## `private long? LEsteemEntry`

The stored entry the held draft stands on, or null for a fresh draft or an empty desk.

## `public int CEsteemGraspStep`

The last grasp step, which the star control takes as its limit so it names no engine constant.

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
