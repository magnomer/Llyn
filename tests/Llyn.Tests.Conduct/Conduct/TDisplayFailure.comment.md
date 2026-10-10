# TDisplayFailure.cs
Hash: `b20d14569923d6de`

## `public sealed class TDisplayFailure`

Covers the failure routes of the reading view's reads, on a real workspace with fake ports.
Every port answers only the entry load.
So each read below meets a refusal and shows its key through the fake envoy.
The tests call reads rather than drawing WPF controls.

## `public void DisplayEntryOpen_StampPortFails_ShowsTheStampFailure()`

Opening an entry whose stamp read fails shows `Display.StampFailed`.

## `public void DisplayCardRead_OrderPortFails_ShowsTheOrderFailure()`

A card read whose sentence order read fails shows `Display.OrderFailed`.

## `public void DisplayCardRead_CitationPortFails_ShowsTheCitationFailure()`

A card read whose Source line read fails shows `Display.CitationFailed`.
The test verifies the citation failure notice after reading the cards.

## `public void DisplayReflexRead_ReflexPortFails_ShowsTheReflexFailure()`

A failed reflex scan shows `Display.ReflexFailed` and answers no rows.
This case asserts the reflex notice and empty rows.

## `public void DisplayFanqieRead_ReadingPortFails_ShowsTheReadingFailure()`

A failed rime book reading shows `Display.ReadingFailed`.

## `public void DisplayParadigmRead_LanguagePortFails_ShowsTheLanguageFailure()`

A failed paradigm language read shows `Display.LanguageFailed`.
This case asserts the language notice.

## `public void DisplayEntryOpen_SoundStartFails_ShowsTheStartFailure()`

Opening an entry whose fetch start fails shows `Sound.StartFailed` through the voice's failure event.

## `public void DisplayParadigmRead_ScanPortFails_ShowsTheParadigmFailure()`

A failed paradigm row read shows `Display.ParadigmReadFailed` through the voice's failure event and answers no slots.

## `public void DisplayParadigmRead_CheckPortFails_ShowsThePendingFailure()`

A failed waiting check shows `Sound.PendingFailed` through the voice's failure event.

## `public void DisplayFanqieRead_AnchorPortFails_ShowsTheAnchorFailure()`

The fanqie read answers no groups.
A subsequent direct reflex read shows `Display.AnchorFailed`.

## `public void DisplayFavoriteRead_PortFails_ShowsTheReadFailure()`

A failed favorite read shows `Favorite.ReadFailed` and answers false.

## `public void DisplayFavoriteRead_PortFailsTwice_ShowsOneNotice()`

A favorite read that fails on two repaints shows `Favorite.ReadFailed` once.

## `public void DisplayCardRead_OrderPortFailsTwice_ShowsOneNotice()`

Two card reads over a failing order port show `Display.OrderFailed` once.

## `public void DisplayCardRead_FoldPortFails_ShowsTheFoldReadFailureOnce()`

Two card reads over a fold port that answers nothing show `Fold.ReadFailed` once.
The assertion covers notice deduplication, not the returned cards' fold values.

## `public void DisplayFavoriteToggle_PortFailsTwice_ShowsTwoNotices()`

A favorite mark the user retries shows `Favorite.MarkFailed` each time, since it is a user act.

## `public void DisplayGraspRead_PortFails_ShowsTheReadFailure()`

A failed grasp read shows `Grasp.ReadFailed` and answers zero.

## `public void DisplayFrequencyRead_PortFails_ShowsTheReadFailure()`

A failed frequency read shows `Frequency.ReadFailed` and answers no chip.

## `private static CAtelier TDisplayAtelierCreate(LEngine engine)`

An atelier on fake ports that answer only the entry load, from the real engine.

## `private static CWing TDisplayWingOpen(LEngine engine, CAtelier atelier, List<string> asked)`

Saves one English entry and opens it in a wing whose envoy records every shown key in `asked`.
