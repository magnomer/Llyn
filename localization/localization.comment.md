# localization

The interface catalogs, one JSON file per language.
They stand apart from the language packs because they translate the interface, not the dictionary data.
A new catalog is listed from the embedded resource names, so no code lists the languages.
Its file still needs an `EmbeddedResource` line in `Llyn.Infrastructure.csproj`.
The file name must be a culture name, since the terms are cased under that culture.

## `en.json`

The English interface catalog, and the language every launch starts from.
It is embedded into `Llyn.Infrastructure` and never read from disk.
Every other catalog must carry exactly these keys, or a `TLocalizationLoader` test fails.

### Shape

The file opens with flat `terms.*` pairs and closes with one `texts` object.
`LLocalizationLoader` refuses any other order, a non-string value or a repeated key.

### `terms.*`

A term is a reusable word or phrase, such as the product name, that texts cite.
Each term is published under its capitalized key, such as `Terms.Product`, so markup or code can read a bare term.
The imperfect, future and conditional terms exist so inflection labels can share them.
The sync term names the push to Joplin, shared by the sync button and the Sync settings page.
The print, cancel, color and custom terms are shared within the print window.
The print term also labels the Scribe print button, and the cancel term the Leave, Coinage and Customs dialogs.
The close term labels the legend's Close key and the help box's Close button.
The deck, flashcard, study and statistics terms name the flashcard feature.
The decks and flashcards terms are the plurals, since each language words its plural its own way.
Korean repeats the singular in both.
The open and save terms label the console's path box admit buttons.
The deck-sync term names the export to the phone app, apart from the sync term that names the Joplin push.
It labels the deck export button and the Depot setting.

### `texts`

Keys are resource names that markup binds to or code reads, grouped by a dotted prefix per area.
A text cites a term as `{Terms.name}` or `{terms.name}`, and the capital sets the casing.
Numbered slots like `{0}` are left for the caller to format.
`Inflection.*` names grammar terms that a pack's inflection layout cites as group, line and header keys.
Most are generic, so other packs can reuse them, and the inflection table shows them in the interface language.
`Paradigm.Fold` names the switch between the short and the full inflection table.
`Paradigm.Short` and `Paradigm.Full` label the buttons that show the short and the full table.
`Card.Fold` and `Card.Unfold` are the hinge tooltips that fold or unfold one card.
`Card.SenseCaption` captions the row of link chips on a collocation card.
`Card.SenseAdd` labels the chip that adds one more Meaning link.
`Display.CollocationOther` heads the collocations with no home Meaning, in the read view and the exports.
`Display.CollocationStub` words the "see N" line under a further link.
Its slot `{0}` is the collocation and `{1}` its home Meaning's number.
`Display.CollocationHeading` captions the collocation rows under a Meaning, and words that Meaning's muted contents line.
It carries no slot, so neither place shows a count.
`Display.CollocationExample` and `Display.CollocationExampleOne` word a closed row's example marker.
The pair holds a plural and a singular form.
`Display.CollocationAlso` words a closed row's "also N" marker.
Its slot `{0}` is the comma-joined numbers of the further Meanings.
`Notebook.*` labels the Sync page field naming the top Joplin notebook.
`Settings.Depot` titles the flashcard folder setting and its folder dialog, and `Settings.DepotHelper` is the note under it.
The setting title is the deck-sync term, as `Settings.Sync` is the sync term.
`Refusal.PathRooted` refuses an absolute folder path, since the folder sits under the workspace root.
`Deck.*` labels the deck list, its folders, the delete dialog and the export to the phone app.
`Deck.DeleteCount` is the label before the flashcard tally that the delete dialog prints under its question.
`Deck.Count` and `Deck.CountOne` word the flashcard count beside a deck, plural and singular.
`Deck.LoadFailed` heads the failure notice when the deck list, a deck, its flashcards or their statistics cannot be read.
`Layout.ReadFailed` is the notice when the stored layout cannot be read at open.
`Input.FindFailed` and `Example.FindFailed` are the notices when a field's suggestions or the anchor choices cannot be looked up.
`Input.SpeechFailed` is the notice when a part of speech cannot be added to the entry.
`Guild.UpdateFailed`, `Source.UpdateFailed`, `Example.UpdateFailed` and `Situation.UpdateFailed` head the notice when a queued change to that desk's record is refused.
`Deck.SaveFailed` heads the failure notice when a deck or folder change is refused or fails.
`Deck.Grain` captions the choice of unit a deck's flashcards are made from, and `Grain.*` names those units.
`Deck.MoveTop` names the top level in a deck's move menu, beside the folders.
`Deck.OrderUp` and `Deck.OrderDown` label the buttons that move a deck one place up or down in its folder.
`Deck.Basic` and `Deck.BasicHelper` switch the export to the Basic text style.
`Deck.ExportReceipt` words the four counts of one export: flashcards written, files copied, files removed and failed.
`Deck.Exporting` is the export line while the deck files are being written, as `Courier.Sending` is the push line.
`Coinage.Deck` and `Coinage.DeckFolder` prompt for the name of a new deck and a new deck folder.
`Navigation.Study` and `Navigation.Scoreboard` label the study and statistics tabs beside the deck tab.
`Study.Empty` and `Scoreboard.Empty` are the empty lines of those two panels.
`Study.Unchosen` is the study line while no deck is chosen, and `Study.Inactive` while the chosen deck has no active flashcard.
`Study.LoadFailed` heads the failure notice when the chosen deck's flashcards cannot be read for a sitting.
`Scoreboard.Vacant` is the statistics line while the chosen deck holds no flashcard.
`Scoreboard.Empty` also stands while all decks are chosen and hold no flashcard.
`Scoreboard.Unsynced` words the last-sync figure while no deck sync has run.
`Scoreboard.All` words the statistics row and title that stand for all decks at once, "All decks".
`Scoreboard.Total`, `Scoreboard.Active`, `Scoreboard.Excluded`, `Scoreboard.Orphaned` and `Scoreboard.Flagged` caption the five figure tiles, each after its count.
`Scoreboard.Synced` captions the last-sync line, before the time or the `Scoreboard.Unsynced` words.
`Scoreboard.Weekly` heads the chart of flashcards added in each recent week.
`CStudy` and `CScoreboard` name each key, and the driver looks it up.
`Study.Launch` and `Study.Shuffle` label the study commands above the deck column.
`Study.Previous`, `Study.Turn`, `Study.Next`, `Study.Again` and `Study.End` label the stage commands.
`Study.Finished` is the stage line at the end of a round.
  The markup names these keys itself, since each labels one fixed control.
`Study.BackEmpty` is the muted line a turned card shows when its back holds nothing, chosen by `CSitting`.
`Coinage.Stencil` prompts for the name a template is saved under.
`Cue.*` labels one flashcard, its flag, its exclusion from study and its orphaned state.
`Cue.Cell` captions one editable cell of a free flashcard, and `Cue.Pick` the button that picks its picture or sound file.
`Stencil.*` labels the template editor.
It holds the side and column names, one `Slot*` per field a slot can cite, and the two join choices.
`Stencil.Picture` and `Stencil.Sound` also title the file dialogs of the picked cells.
`Stencil.FieldAdd` and `Stencil.FieldRemove` label the field chips of a side.
`Stencil.Custom` marks a side whose text no chip row can show.
`Stencil.AsText` switches the sides to their plain text.
`Stencil.PictureSide` and `Stencil.SoundSide` caption the side each picks, and `Stencil.MediaNone` is the choice of no side.
`Bearing.*` labels the direction, lead side and statistics set of a deck.
`Bearing.Front` and `Bearing.Back` caption the sides each face of a direction shows, and `Bearing.LeadOne` and `Bearing.LeadTwo` name the lead sides.
`Schedule.*` labels the card-order options of the deck editor.
`Schedule.Interval` counts hours, as the stored target does.
`Deck.Code` captions the export file name, and `Deck.Save` the button that stores the deck editor's draft.
`Deck.ViewAdd`, `Deck.ViewCard` and `Deck.ViewDeck` label the three views of the deck management column.
`Accession.*` labels the view that adds library entries to the chosen deck.
It holds the search field, the in-deck mark and both add buttons.
`Accession.Empty`, `Accession.Unmatched` and `Accession.LoadFailed` are its empty, no-match and failure lines.
Each `Refusal.Deck*`, `Refusal.Cue*` and `Refusal.Depot*` text says what the user can do next.
`Xiesheng.ReadingsShow` and `Xiesheng.ReadingsHide` label the series member hinge that shows or hides all its readings.
`Settings.Inflection` names the settings page that holds the `Analysis.*` switch and its hint.
`Input.Ancestor*` labels the etymology editor's boxes, their hints and their insert, add, remove and restore controls.
`Relation.*` and `Tongue.*` serve the relation and language pickers of an etymology box.
The five relation names label the built-in relations, which `LAncestorRelation` maps from their stored names.
Each `DetachConfirm` and `DetachCount` pair replaces `DeleteConfirm` while boxes still use the choice.
A failure text is keyed `<Area>.<Action>Failed`, or `<Area>.Failed` for an area's main action.
It sits beside its area's other texts.
Most reach the user through the ledger, but some show in place, such as `Card.VideoFailed`.
An area without other texts, such as `Sentence`, `Fold` or `Box`, sits beside the nearest texts on the same subject.
`Print.*` labels the print window, one key per control, choice, hint, status line and failure.
`Print.Page` names the page number box.
`Print.PageTotal` is one entry so each language orders "of {0}" itself.
`Print.Sheets` and `Print.SheetsOne` give the plural and singular count of paper sheets.
`Print.Idle` is the footer text while nothing prints.
`Invocation.*` words the console's strict mode.
`Invocation.Usage` is its usage error line.
Most other usage errors fill slot `{0}` with the typed argument or option.
Slot `{1}` takes its value, the second verb, or the option that `Invocation.QualifierMissing` requires.
`Invocation.Conflict` has no slot, since it names `-yes` and `-no` literally.
`Invocation.EntryMissing` says no entry holds the given id.
`Invocation.NewEmpty` says the new entry holds nothing to save, and points to `-headword:value`.
`Invocation.CustomsConfirm` is the question before an import enters the listed entries.
`Invocation.ReplaceConfirm` asks before `-export` replaces a file, and its slot `{0}` is the path.
`Invocation.Help` is the sentence above the option list.
The other `Invocation.Help*` keys give the help text of one option, printed after the literal option name.
`Invocation.HelpFind` names the four columns the option lists.
`Invocation.HelpShow` words the `-show` option, which prints an entry's reading view as plain lines.
`Invocation.HelpAs` lists the five file formats of `-export`, and `Invocation.HelpFormat` the three output formats.
`Pathfinder.*` words the console path box, its path label, its key meanings and its replace question.
`Pathfinder.Open`, `Pathfinder.Save` and `Pathfinder.Choose` label the admit button by what the box does.
`Pathfinder.Missing` and `Pathfinder.Unreadable` are its alerts for a file that is not found and a folder that cannot be read.
`Drive.Title` titles the drive choice, and `Markup.Title` titles the box that picks a markup file to import.
`List.Unmatched` is the index line when a filter matches no entry.
`Index.Position` takes the lit row and the row count in slots `{0}` and `{1}`.
`Index.Position` and `Index.Filtered` word the index border, where the second marks a filtered list.
`Order.Close` and `Sieve.Close` label the Close button of the sort and language filter boxes.
`Display.Missing` marks a picture or video line whose file is gone, and `Display.EtymologyBorrowed` marks a borrowed ancestor.
`Display.Word*` words the reading view's word cursor.
`Display.WordCursor` takes the left and right arrow glyphs in slots `{0}` and `{1}`.
`Display.Retreat` labels the F3 and Escape keys that leave the full view.
`Editor.Title` is the editor frame's title, and its slot `{0}` takes the headword.
`Editor.Unsaved` marks a draft that differs from the stored entry.
`Editor.Open` and `Editor.Close` label the Enter and Escape keys of the headword field.
`Editor.Stack` labels the keys that pick a section of the form, and `Editor.Card` the keys that light another card.
`Editor.More` takes the count of rows below the page in slot `{0}`, for the editor frame's foot.
`Mention.Open` labels the Enter key of the Mention menu, and `Window.Help.Tick` the Space key that ticks a choice.
`Window.*` words the console window's notices and its help.
`Window.Opening` is the line the window shows while it opens.
`Establishment.Courier` begins the console status line, and its slot `{0}` is `Courier.Attach` or `Courier.Connected`.
`Roof.Llyn.Attach` and `Roof.Llyn.Send` are the console menu items that connect Llyn to Joplin and send the entries there.
`Roof.Options.Glyphs.*` name the console's three glyph sets, and `Roof.Options.Settings` opens the Settings tab.
`Roof.Options.Ambiguous` names the setting that draws East Asian ambiguous characters one or two cells wide.
`.Narrow` and `.Wide` are its two choices.
`Roof.Options` titles the console's Options menu.
`.Theme`, `.Glyphs`, `.Ambiguous` and `.Language` are its entries.
`.Theme.Blue` and `.Theme.Dark` name the two themes.
`Roof.Llyn.Send` and `Roof.Options.Settings` cite the sync and settings terms.
Each roof menu and entry that takes a letter has a `.Hotkey` key.
It must hold one ASCII letter, or the entry has no hotkey.
`Window.Help.Page.Tabs` and `Window.Help.Text.Tabs` name and word the help page on the tab menu, the tab box and the history keys.
`Window.Small` is the notice for a console under 80 by 24 cells.
`Window.Small` takes the minimum width and height, then the actual width and height, in slots `{0}` to `{3}`.
`Window.Help.*` words the help box: its title, page names, prose, key names and key meanings.
`Window.Help.Text.Small` takes the minimum width and height in slots `{0}` and `{1}`.
`Window.Help.Position` takes the lit link and the link count in slots `{0}` and `{1}`.
`Window.Help.Key.*` names the keys that help prints, and any other key prints its .NET name.
`Legend.*` labels the keys that the window legend and help list: `Legend.Help`, `Legend.Quit`, `Legend.Close`, `Legend.Import` and `Legend.Menu`.
`Parley.*` labels the Yes, No and OK buttons of the console's boxes.
The console prints each usage error after `llyn: `.
So each is one complete sentence, with option names kept literal.

## `ko.json`

The Korean interface catalog, with the same shape and rules as `en.json`.
The keys mirror `en.json` exactly, and numbered placeholders stay for the caller.
Each failure text sits beside its area's other texts at the same place as in `en.json`.
A failure area with no other texts, such as `Sentence`, `Fold` or `Box`, sits where `en.json` puts it.
The person headers spell out person and number instead of the English abbreviations.
`Display.CollocationStub` reads `→ {0} ({1} 참조)`, keeping the same two slots as `en.json`.
`Display.CollocationExample` and `Display.CollocationExampleOne` both read `예문 {0}개`, since Korean counts without a plural.
`Display.CollocationAlso` reads `{0}번 뜻에도 쓰임`, "used in sense N too", rather than a literal "also".
`Print.*` reads the print window in polite Korean, with short noun labels for controls and 합니다 sentences for failures.
`Print.PageTotal` reads `/ {0}`, a slash in place of the English word "of".
`Deck.*`, `Cue.*` and `Refusal.Deck*` pick the Korean particle that fits the fixed term, such as 덱을 and 플래시카드를.
`Yunjing.Division` cites slot `{0}`, the raw division, where `en.json` cites the formatted `{1}`.

## Stamp

Commit: `c05429890cec`
- `en.json` `b002fe8b14b16db7`
- `ko.json` `c28945e94ff3f038`
