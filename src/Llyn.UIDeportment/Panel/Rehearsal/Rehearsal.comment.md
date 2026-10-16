# Rehearsal

The drivers of the Study and Scoreboard pages.
The deck, cue and stencil drivers live in its subfolders.

## `QStudy`

Drives the veneer's `PStudy` page, which the window places: the deck column and its commands.
The card is `QSitting`'s, built here on the `PSitting` border as `QDeck` builds `QAccession`.
`QStudyIntroduce` builds `CStudy`, handing it `Random.Shared.Next` as the draw for a shuffled sitting.
  The driver hands the draw because the random belongs outside the pure rings.
  It looks up `Study.Empty` at once, and the first roll arrives with the workspace's `CStudyChanged`.
`QStudyRefine` answers `CStudyChanged` from one `CStudyRead`.
  It fills the shelf with `QStudyItem` rows as `QDeck` does: loose decks, then each folder and its decks.
  It looks up the roll's empty-state key, or collapses the line when the roll names none.
  Start is enabled only while the roll names no key.
  It hands the roll's sitting to `QStudyShuffleRefine` and to `QSitting`, so the roll is read once.
`QStudyShuffleRefine` enables Shuffle while a sitting runs, with the `Selected` cue while it is shuffled.
Start calls `CStudyStart`, and Shuffle hands its click to `CSittingShuffleToggle` and paints the answer.
  Both then give the card focus through `QSittingFocusRefine`.

## `QSitting`

Drives the study card inside `PStudy`, found on the `PSitting` border the panel driver hands it.
`QSittingIntroduce` takes the panel's `CStudy`, the owner of the running sitting and of its sound.
`QSittingRefine` paints one `CSittingState` onto the card, or hides the card when there is none.
  Place text, turned state and order come ready from `CSitting`, so the driver only picks which control shows.
  The back face shows only once turned, and the front face and rule leave at the end of a round.
`QSittingFaceRefine` fills one `PFace` from one `CSittingSide`: text, picture line and play button.
  The face template is drawn twice, so each fill finds its parts afresh and rewires the play click.
  The play button shows only for an audible side, and its click plays that side through `CStudySoundPlay`.
Each stage command hands its click to one `CSitting` gate and paints the state that gate answers.
  Again calls `CStudyStart`, End calls `CStudyFinish`, and these repaint through `CStudyChanged`.
Keys reach the driver while the card has focus: Space turns, Left goes back, Right goes on, and Esc ends.
  Again and a press on the card give the card focus.

## `QStudyItem`

One shelf row: a deck or a folder, its name, the active count as text, and the chosen mark.

## `QScoreboard`

Drives the veneer's `PScoreboard` page as `QStudy` drives `PStudy`.
`QScoreboardIntroduce` builds `CScoreboard`, attaches both lists, and looks up `Scoreboard.Empty` at once.
  It subscribes two handlers to `CScoreboardChanged`, each sending one request, so the deck column and the census paint apart.
`QScoreboardRefine` answers from one `CScoreboardRead`.
  It fills the shelf with `QScoreboardItem` rows as `QStudy` does, the All row first among the loose rows.
  It titles the page with the chosen row's name.
  The All row's name comes empty from Conduct, so the driver words it with `Scoreboard.All`.
`QScoreboardCensusRefine` answers from one `CScoreboardCensusRead`.
  It shows the census's empty-state line, or hides the line when the census names none or fails.
  The census surface shows only while the census reads and names no empty key.
  The five tiles and the last-sync line take the census's ready text as it comes.
  The chart takes one `QScoreboardBar` per ready week, in the census's order.
  Every third bar keeps its week's label, the newest included for the twelve weeks, and the rest carry none.
  The peak is the largest week count and only scales the bars.
  A bar stands at its count over the peak.
`QScoreboardBarRefine` sizes a bar by two star rows.
  The gap above takes the peak less the count, and the bar takes the count.
  So the markup sets the chart's height, and the series alone sets the proportions.
  No week edge or label is made here.
  The count's ready text is the week column's tooltip.
  So a hover anywhere over the column shows it, even over an empty bar.
A deck row's click hands its id to `CScoreboardSelect`.
  The All row hands null, and folder rows take no click.

## `QScoreboardItem`

One shelf row: a deck, a folder or the All row, its name, and the chosen mark.
The All row has no id, so the driver words it.

## `QScoreboardBar`

One chart column: the week label or none, the count's ready text, the count, and the peak.

## Stamp

Commit: `121691f60459`
- `QScoreboard.cs` `af9cb48d3e382965`
- `QScoreboardBar.cs` `c946e4c60a6ec019`
- `QScoreboardItem.cs` `ebeaf414f0e99765`
- `QSitting.cs` `0c8cd7e38d922449`
- `QStudy.cs` `2c790ecac58838a4`
- `QStudyItem.cs` `9d45e9fff173528c`
