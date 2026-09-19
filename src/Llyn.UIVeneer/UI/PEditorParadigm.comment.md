# PEditorParadigm.cs

## `public partial class PEditor`

The paradigm box of the editor: the inflected forms of the entry the draft stands on.
The box draws the rows the engine already filtered, so the editor never judges regularity.
The two modes read alike, so the box sits where the view puts it, under the frequency chip.

## `internal void PEditorParadigmShow()`

Reads the held entry's rows, the morphology switch, and whether a fetch is running.
A failed read empties the box.
A form standing on nothing has no entry to ask, so the box is emptied rather than asked.
The editor never asks the engine to fetch, since the engine declines while a draft holds the entry.
An empty row therefore says the form is looked up once the entry is saved.
A fetch started elsewhere that still runs is the exception.
The fetch is asked for once the store commits, before the next draft holds the entry again.
The box takes the entry's headword font, read by the language the rows carry.
The fetch announces itself as an inflection bulletin, and the editor re-reads the box alone.
