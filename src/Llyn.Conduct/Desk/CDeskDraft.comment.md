# CDeskDraft.cs
Hash: `be8bb5c89177a70e`

## `public sealed class CDeskDraft`

The held tenure of one desk, with the quills and engines built over it.
Every quill getter answers null while no tenure is held, so no gate reaches a dropped tenure.
It also raises the draft announcements, so the fill flag and the echo it guards live together.

## `private bool _cDeskDraftFilling;`

Raised while the draft is being written into the controls, so the echo of that writing defers nothing.
It is read through its verdict and never branched on by name, so the walker sees a verdict.

## `private LErrand? _cDeskDraftErrand;`

The reading writes of the held tenure, taken from it when a tenure starts and dropped when it ends.

## `private LEasel? _cDeskDraftEasel;`

The media edits of the held tenure, built when a tenure starts and dropped when it ends.

## `private LQuillChip? _cDeskDraftChip;`

The chip row edits of the held tenure, built when a tenure starts and dropped when it ends.

## `private LQuillSpeech? _cDeskDraftSpeech;`

The part of speech edits of the held tenure, built when a tenure starts and dropped when it ends.
The quill keeps the pending part typed into the field, so the desk holds one per tenure.

## `private readonly LSettingsPort _cDeskDraftSettings;`

The settings port the ledger reads a failed reread's notice through.

## `internal CDeskDraft(LDraftPort drafts, LSettingsPort settings, string scope, CEnvoy envoy)`

Only the desk builds one, handing on the port the chip quill finds its offers through.
It also takes the desk's settings, scope and envoy, so a failed reread shows under the desk's own key.

## `internal event Action<LDraft>? CDeskDraftPrepared;`

The held draft was read again, handed whole to the controllers that own the desk.
It fires before `CDeskDraftChanged`, so a controller's own announcement reaches its driver first.

## `public event Action<CDraft>? CDeskDraftChanged;`

The held draft was read again, so the edit area writes its controls from it.
A driver receives only the Conduct copy.

## `internal LErrand? CDeskDraftErrand`

The held tenure's reading writes, which the errand's lookup ends in.
It is null while no tenure is held or while the desk fills its controls.
A fill echoes values the draft already holds, so nothing is written back.

## `internal LQuillReference? CDeskDraftReference`

A fresh quill of the held Source's typed edits, so no gate builds a request.
It is null while no tenure is held or while the desk fills its controls.

## `internal LQuillAuthor? CDeskDraftAuthor`

A fresh quill of the held Author's typed name.
It is null while no tenure is held or while the desk fills its controls.

## `internal LQuillExample? CDeskDraftExample`

A fresh quill of the held Example's own fields.
It is null while no tenure is held or while the desk fills its controls.

## `internal LQuillSentence? CDeskDraftSentence`

A fresh quill of the sentence rows and glosses, the held Example's glosses among them.
It is null while no tenure is held or while the desk fills its controls.

## `internal LQuillMention? CDeskDraftMention`

A fresh quill that drops one Mention by id.
It is null while no tenure is held or while the desk fills its controls.

## `internal LQuillTranscription? CDeskDraftTranscription`

A fresh quill of the held entry's transcription rows.
It is null while no tenure is held or while the desk fills its controls.

## `internal LQuillEtymology? CDeskDraftEtymology`

A fresh quill of the held entry's etymology narrative, links and spans.
It is null while no tenure is held or while the desk fills its controls.

## `internal LEasel? CDeskDraftEasel`

The held tenure's media edits, so no driver builds a media request.
It is null while no tenure is held or while the desk fills its controls.

## `internal LQuillChip? CDeskDraftChip`

The held tenure's chip row edits: tags, situations, registers and translations of a card.
It is null while no tenure is held or while the desk fills its controls.

## `internal LQuillSpeech? CDeskDraftSpeech`

The held tenure's part of speech edits, null while no tenure is held.
It stays open while the desk fills its controls, since the field read runs then.
Each edit gate checks the fill itself.

## `public bool CDeskDraftFilling`

Whether the desk is writing the draft into the controls right now.
The areas that reach the tenure directly withhold it while this holds, so a fill's echo writes nothing back.

## `internal LTenure? CDeskDraftTenure`

The held tenure, or null while nothing is held.
The desk reads every state and step through it, so the tenure has one keeper.

## `public bool CDeskDraftAltered`

Whether the held draft differs from what is stored, as the tenure last reported it, applying nothing first.

## `public bool CDeskDraftStorable`

Whether the held draft has changed and nothing refuses its store.
The tenure answers the verdict, so the holder combines no state fields itself.

## `internal void LDeskDraftSet(LTenure started)`

Holds the started tenure and its errand, and builds the easel, chip and speech quills over it.
The desk calls it once per successful start, before the vigil applies its observers.

## `internal void LDeskDraftClear()`

Drops the held tenure and every quill built over it, announcing nothing.
The desk decides what follows, a cancel of the tenure or a change of state.

## `internal LDraft? CDeskDraftRead()`

The held draft after the deferred requests have been applied, or null while nothing is held.

## `public void CDeskDraftResonate()`

Reads the draft again and announces it, unless the announcement itself is what is running.
A failing read is shown under the scope's `LoadFailed` key and announces nothing.

## `private LDraft? CDeskDraftPrepare()`

The held draft after the deferred requests and the tenure's completion have run, or null while nothing is held.
The completion runs inside the tenure's prepare turn, so the rows it adds raise no draft bulletin.

## `public void CDeskDraftPersist()`

Applies the deferred requests now, as a field is left, unless the controls are being filled.

## `private void CDeskDraftShow(LDraft? draft)`

Announces the draft with the filling flag raised, so the controls' echo is ignored.
A null draft announces nothing.
