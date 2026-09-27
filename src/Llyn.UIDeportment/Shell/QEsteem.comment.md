# QEsteem.cs

## `public sealed class QEsteem`

The user's marks on the stored entry an editor holds, its favourite flag and its grasp step.
The frequency chip is read here beside them, since all three stand on the stored id.
The editor builds it over its desk and its Conduct `LDisplay`, which holds every favourite and grasp rule.
It only supplies the stored id, and a refusal reaches `LEditorFailed` through `LDisplayFailed`.

## `private long? QEsteemEntry`

The stored entry the held draft stands on, or null for a fresh draft or an empty desk.

## `public void QEsteemFavoriteSet(bool marked)`

Marks or clears the favourite on the stored entry, then announces a re-read so a refusal redraws.

## `public void QEsteemGraspSet(int step)`

Writes the grasp step on the stored entry, then announces a re-read so a refusal redraws.

## `public int QEsteemGraspStep => _qEsteemDisplay.LDisplayGraspStep;`

The last grasp step, which the star control takes as its limit so it names no Core constant.

## `public string QEsteemGraspFormat(int step)`

The wording of a grasp step, or empty for a fresh draft that has no grasp to word.
The engine words it, so the deportment names no localization and no grasp key.

## `public CFrequency? QEsteemFrequencyRead(string once)`

The frequency chip of the held entry, or null when no source ranks it.
The driver hands the localized word for a one-off figure, so the controller stays out of localization.
