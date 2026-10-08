# QCohort.cs
Hash: `65abdd94a738e6b8`

## `internal sealed class QCohort`

The entry list of the tenor panel, with the entry search field over it.
It shows the Entries carrying the chosen Register, and the one the reader stands on.
Choosing a row loads it back from the workspace into the reader, or the editor when that side is open.
It subscribes what it paints itself, so the owner `QTenor` only builds and introduces it.

## `internal QCohort(UserControl surface)`

Takes the tenor page, and finds `PCohort`, `PCohortEmpty` and `PQuest` in it by contract ID.
It sets the search hint and subscribes the search field.

## `internal void QCohortIntroduce(CTenor tenor)`

`QTenorIntroduce` calls it once the Conduct tenor exists.
It subscribes the area's opening event and the cohort panel's rows event.
It binds the list and attaches its row fill.

## `private void QQuestObserve(object sender, TextChangedEventArgs e)`

Each keystroke hands the entry search text to the cohort's query gate.

## `private void QQuestRefine()`

Answers the area's opening event after a chip's arrival or a coinage.
The area has already emptied the entry query, so the field only shows it.
The field's own handler still hears the change, and its gate finds the query already empty.

## `private void QCohortRefine()`

Answers the panel's rows event, which the area raises after a successful Register read.
The Entries are never refilled on their own, because the chosen Register may have just changed or vanished.
The engine is handed the register vista and the entry vista, and it decides what they list.
A failed read has already been shown by the area, which then answers no rows.
Conduct picks the empty line's key from whether the entry search holds text.

## `private void QCohortObserve(object sender, RoutedEventArgs e)`

A clicked row hands its entry's id raw to the row gate, or null when it carries no item.
The gate ignores null, asks the leave question and opens the row.
This panel attaches no station, so the gate records none.

## `private void QCohortItemRefine(FrameworkElement container, object item, string? _)`

Fills one entry row from its item, the work its bindings did before.
The row carries the `Chosen` cue on the chosen item and none otherwise, which the look sheet paints.
The click is subscribed once per row, removed first so a refill never doubles it.
It runs again on every change the item raises, so a chosen row moves without a refill.
The epithet leads with an en space, as its string format did.
