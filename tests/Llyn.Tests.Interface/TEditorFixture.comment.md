# TEditorFixture.cs
Hash: `4559c9a91f4e504c`

## `internal sealed class TEditorFixture`

The relay a test holds instead of the editor composer itself.
It hands each test the one facet the test reads, plus the few editor actions tests drive.
Tests can use individual facets without repeatedly reaching through the composer.

## `internal TEditorFixture(CEditor editor)`

Wraps an editor some relay or the atelier already built.
The fixture adds no state of its own, so two fixtures over one editor read alike.

## `internal static TEditorFixture TEditorFixtureCreate(CAtelier atelier, CEnvoy envoy)`

Builds the editor the way production does, over the atelier's ports and wiring, and wraps it.

## `internal CEditor TEditorFixtureEditor { get; }`

The wrapped editor, available when a test or sibling relay needs the whole composer.
A vista restore, a draft read and a finish reach the editor itself, so no facet serves them.

## `internal CDesk TEditorFixtureDesk`

The editor's desk, which holds the draft every other facet works on.

## `internal CEntry TEditorFixtureEntry`

The entry facet, for the headword, language, note and pronunciation fields.

## `internal CDisplay TEditorFixtureDisplay`

The display facet the editor shares with its display rule.

## `internal CCard TEditorFixtureCard`

The card facet, for translations and etymons on a card.

## `internal CSentence TEditorFixtureSentence`

The sentence facet, for example sentences and their glosses.

## `internal CSounding TEditorFixtureSounding`

The sounding facet of the editor.

## `internal CFold TEditorFixtureFold`

The box fold facet, whose gates store each box state for the held entry.

## `internal CEsteem TEditorFixtureEsteem`

The esteem facet of the editor.

## `internal CTimbre TEditorFixtureTimbre`

The timbre facet, for accents, fonts and flags.

## `internal CKindred TEditorFixtureKindred`

The kindred facet, for reflex rows.

## `internal CPlayback TEditorFixturePlayback`

The playback facet, for stored audio.

## `internal CTranscription TEditorFixtureTranscription`

A transcription facet over the desk, fresh on each read since it holds no state.

## `internal CCardSpeech TEditorFixtureSpeech`

The speech facet, for parts of speech and units.

## `internal CCardField TEditorFixtureField`

A card field facet over the desk, fresh on each read since it holds no state.

## `internal CImage TEditorFixtureImage`

An image facet over the desk, fresh on each read since it holds no state.

## `internal CVideo TEditorFixtureVideo`

A video facet over the desk, fresh on each read since it holds no state.

## `internal bool TEditorFixtureOwned`

Whether the restored vista is the input one, which decides where a stored fresh entry goes.

## `internal void TEditorFixtureOpen(long? id)`

Opens the entry `id` in the editor, or a fresh one for null.

## `internal void TEditorFixtureClose()`

Closes the editor, dropping its draft and any running errand.

## `internal void TEditorFixtureSave()`

Saves the held entry when it changed.

## `internal void TEditorFixtureUndo()`

Reopens the stored entry, dropping every unsaved change.
