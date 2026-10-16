# Scoreboard

This folder holds the statistics gates: the statistics tab, its deck column and its census.

## `CScoreboard`

The statistics tab's gates, one sealed class owning its own chosen deck, null for all decks.
  Its deck choice stands apart from `CDeck`'s and `CStudy`'s, so reading statistics moves no other tab.
It registers the `Scoreboard` tab as `CStudy` registers `Study`: the station is the chosen deck, 0 for all decks.
  A navigation station is a `long`, so 0 stands for all decks as it stands for no deck in `CStudy`.
A workspace open clears the chosen deck back to all decks, because each workspace keeps its own decks.
`CScoreboardChanged` tells the driver to read the column and the census again.
  It also follows every deck bulletin, as `CStudyChanged` does.
`CScoreboardRead` makes the one engine call `LEngineDeckRead` and answers a `CScoreboardRoll`.
  Its first `CScoreboardRow` is the all-decks row, with no id and an empty name the driver words.
  The deck rows follow in shelf order, each with its folder, and its folders reuse `CDeckFolder`.
  A read failure shows `Deck.LoadFailed` and answers an empty roll.
  The roll carries no empty key, since the census names every empty key the view shows.
`CScoreboardSelect` adds a station and chooses the deck, or all decks with null.
`CScoreboardCensusRead` makes the one engine call `LEngineCensusRead` for the chosen deck or all decks.
  It maps the domain census by name into a `CScoreboardCensus` whose every figure is worded for display.
  Every figure is worded in the interface language's culture from `LEngineCultureRead`, never the machine's.
  The five counts are numbers in that culture, so the driver formats nothing.
  The last sync is the local time in the culture's general form, or `Scoreboard.Unsynced` when no sync has run.
  Each `CScoreboardWeek` carries its Monday as month and day, the added count for the bar, and that count as text.
  A read failure shows `Deck.LoadFailed` and answers null, so the driver paints no figure.
The census's empty-state key is chosen here, since it is interaction.
  With cards it names no key.
  Without cards, `Scoreboard.Empty` stands for all decks and `Scoreboard.Vacant` for one chosen deck.
`CScoreboardEmptyKey` names `Scoreboard.Empty` for the driver's first paint, before any workspace is open.

## Stamp

Commit: `121691f60459`
- `CScoreboard.cs` `3d676787d5fcdfbe`
- `CScoreboardCensus.cs` `01d8b1aedfaaaba7`
- `CScoreboardRoll.cs` `392dff988d92e849`
- `CScoreboardRow.cs` `4ccbd7eb16c7be41`
- `CScoreboardWeek.cs` `4bc6551dde201f11`
