# PDisplay.xaml.cs

## `public partial class PDisplay : UserControl`

The shared read-only entry view as a control.
It is handed a loaded draft and draws it.
It never loads one itself, and it never decides which entry is shown.
That belongs to the browse-style panel it sits in.
Playback, the heart, the star row and the paradigm box each sit in a part of their own.

## `internal void PDisplayAttach(PWindow host, LEngine engine)`

Puts the view on `engine`, the workspace the window opened.
The engine is read for the flag beside the language, the linked headwords and the incoming cards.
The window is held because an incoming row opens the entry it names.

## `private static IReadOnlyList<string> PDisplaySpeechShow(IReadOnlyList<LSpeechDraft> speeches)`

The parts of speech as the names a reader reads.
The draft keeps the stored value beside the name, and the panel shows only the name.

## `internal void PDisplayShow(long id, LEntryDraft draft)`

Draws `draft` as the entry being read.
A field the draft left empty collapses instead of standing as a blank line.
The note is Markdown, drawn as blocks by `PMarkdown` inside the note card.
The primary play button appears only when the entry owns a recording that is still on disk.
The volume tray appears while any pronunciation row has a recording.
The volume is read from the workspace on every show.
A level the editor set is the level this view plays at.
The id is taken as well as the draft, because a draft does not say which entry it is.
That id is what the incoming cards are looked up by.

## `private void PDisplayStampShow(long id)`

Fills the creation and update times from the stored entry, since the draft carries no clock.
The stamps collapse when the entry cannot be read.

## `private void PDisplayFrequencyShow(long id)`

Fills the frequency chip from the stored entry, since the draft does not carry it.
The chip and its tooltip are worded by the shared label, so the editor shows the same.
The section collapses when the entry has no frequency yet or the read fails.
An entry with no value asks the engine to fill it, and the fill announces itself when done.

## `private static string PDisplayStampFormat(string? utc)`

Turns a stored ISO 8601 UTC stamp into local time in the short general format of the current culture.
A missing or unreadable stamp shows as nothing.

## `internal void PDisplayClear()`

Empties the view and leaves the unselected notice in its place.
Playback stops, because what it was playing belonged to the entry that was shown.

## `private void PDisplayBulletinHandle(LBulletin bulletin)`

What the view does when the engine announces that stored data changed.
It answers only for the entry it stands on, and ignores every announcement about another.
A mark set from another tab moves the heart.
A frequency the engine finished fetching fills the chip alone, because nothing else on the page changed.
A headword written elsewhere is read back and redrawn, so two views of one entry never disagree.
An entry that is gone leaves the unselected notice, because there is nothing left to show.
A workspace that moved empties the view, since the id it stood on means nothing in the new database.

## `internal void PDisplayClose()`

Releases this view's own playback.

## Inline notes

### `private void PDisplayCardUpdate()`

The chips a card shows are read through a converter that is filled just before they are drawn.
A converter holding a dictionary says nothing when that dictionary changes.
So the card lists are handed their items again once the words behind the ids are known.
That way the order of the two steps cannot quietly cost the reader every chip.

### `private void PDisplayExampleShow(string language)`

Example lines are drawn inside card templates, where no code can reach one line at a time.
So the pack's example and Gloss typography is put into view resources the templates read.
A pack that declares none has its keys removed, and the card's own fallback typography stands.

### `private void PDisplayCardHandle(object sender, RoutedEventArgs e)`

Every chip on a card names a record kept in some other panel.
Reading it there is one click away.
What was clicked stands for the record it was drawn from.
The record decides the panel.
A situation opens the repertoire, a translation the library, a tag the taxonomy.
One handler serves all three.
The chips are drawn from a shared dictionary that knows no window.
It reaches the cards only through the two lists this sits on.
The chip is read off what was clicked rather than off the click's source.
A click leaving a template is re-sourced to the presenter that drew it.
A chip naming a record the card never saved leads nowhere.

### `private async void PDisplayLanguageShow(string language)`

The flag comes from `PEnsign`, which every tab holding a display reads too.
The first call may await a fetch, so a later entry may be shown before it arrives.
The language shown now is compared before the image is set.

### `PFont.PFontApply(_lEngine, language, PDisplayHeadword);`

The reading view draws a headword exactly as the editor does.
Both ask the same pack, so the two views never differ in family or size.

### `AddHandler(PMention.PMentionClickEvent, new EventHandler<PMentionArgument>(PDisplayMentionHandle));`

Every sentence on every card raises the same bubbling event.
One handler on the control above them all answers it, so a card template stays free of handlers.
The answer itself lives in [PDisplayMention.cs](PDisplayMention.comment.md).

### `PCompassUpdate();`

The contents are rebuilt at the end of showing an entry, after every section has been given its visibility.
Rebuilding earlier would list sections the entry is about to collapse.
