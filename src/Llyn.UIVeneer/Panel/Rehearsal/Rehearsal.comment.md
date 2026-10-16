# Rehearsal

The study and statistics pages.
Its markup carries no hook.
Its subfolders hold the deck list and editor, the cue adder and cell editor, and the template editor.

## `PStudy`

The study page, shelled as `PDeck` is.
The left column is the deck shelf in the catalog styles, with each deck's active count in a pellet.
  Start and Shuffle sit above it in one command group, and Shuffle stays disabled until a sitting runs.
The right column holds the empty line in the catalog's empty style, and below it the `PSitting` card.
  The empty line's text is unset in markup, since the driver looks up the key Conduct names.
  The card shows the place, the front face, a rule, the back face, and the stage commands below it.
  Both faces draw one `Study.Face` template: the `PFace` text, picture line and play button.
  Each host holds its own `Study.Face.Text` style, which the `PFace` text takes.
  The front style is based on `Theme.Text.Headword`, and the back style sets a smaller size.
  A muted `PSittingBlank` line stands in for a turned back with nothing on it.
  At the end of a round, the `Study.Finished` line and Again replace the front, rule and stepping commands.
  The card takes focus so the driver's keys reach it, and its commands refuse focus to keep it there.

## `PScoreboard`

The statistics page, shelled as `PStudy` is, without its command row.
The left column is `PStudy`'s deck shelf without the count pellet, with the All row first.
It keeps `PStudy`'s header and seam rows, so the deck, study and statistics shelves start on one line.
The tab has no command, so its header holds a hidden command segment.
So the header row is as tall as the other tabs' command strips.
The right column holds the empty line in the catalog's empty style, and below it the `PCensus` surface.
  The empty line's text is unset in markup, since the driver looks up the key Conduct names.
`PCensus` stands on the panel surface, no wider than the study card, and scrolls in a short window.
  `PCensusName` is the page title in `Theme.Page.Title`, the same shape as the Author page's title.
  Five tiles in `Theme.Page.Tally` follow, laid as the Author page lays its tallies.
  They are total, active, excluded, orphaned and flagged.
  Each tile holds its figure, filled by the driver, and its caption, looked up in markup.
  The last-sync line pairs the `Scoreboard.Synced` caption with the driver's time or no-sync words.
  `PCensusChart` lays one `PWeek` column per week in a one-row uniform grid, 180 high.
  A `PWeek` is a transparent grid, so its whole height takes the hover.
  It stacks a gap row, the `PWeekBar` row, a baseline and the `PWeekMonday` label.
  The label reaches past its column by negative margins, so a full date shows under a narrow bar.

## Stamp

Commit: `bd299c638b2c`
- `PScoreboard.xaml` `1b1d8009957466f0`
- `PScoreboard.xaml.cs` `34a5060453e0812a`
- `PStudy.xaml` `97e568d64dbe1711`
- `PStudy.xaml.cs` `c6964d18e1764eed`
