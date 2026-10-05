# TLocalizationChoice.cs
Hash: `3d142b2f15fb96b3`

## `public sealed class TLocalizationChoice`

Covers the settings language choice as the driver paints it from the ledger.
The choice runs over a real ledger from the atelier relay and a real workspace's settings.
The combo box lives on its own STA thread through the shared window runner.

## `public void LocalizationRefine_RepeatedRefine_SavesNoLanguage()`

The choice is painted twice in code with a language other than the stored one.
The box shows the painted language, so its selection really moved.
No save runs and the stored language stays, so the code paint is never heard as the user's pick.
A heard paint would save the painted language and change the settings on every repaint.
