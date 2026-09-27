# LSounding.cs

The sound sheet of one held entry: its rime-book groups, its script rows and its paradigm slots.
The editor owned all of this before, which left one deportment carrying two subjects at once.
Every call names the entry it works on, so the sheet keeps no entry and caches no answer.
Every read swallows a refusal and answers empty, since a box that cannot fetch still has to draw.
Every write announces the change or the refusal, and the editor passes the refusal on as its own.

## `internal LSounding(LPhonologyPort phonology, LDraftPort drafts)`

Takes the phonology port every sound read goes through, and the draft port the anchors ask.
Only the editor builds one, so the constructor is internal.

## `public event Action? LSoundingChanged;`

A write landed, so the boxes drawn from the sheet are read again.

## `public event Action<string, Exception>? LSoundingFailed;`

A write was refused, announced under its own notice key.

## `public IReadOnlyList<CFanqieGroup> LSoundingFanqieRead(long? entry)`

The entry's rime-book groups, started when missing, or nothing for a fresh draft or a refusal.

## `public string LSoundingReadingRead(long? entry, string headword)`

The headword's representative reading, formed by the engine from the same groups the box draws.

## `internal IReadOnlyList<LFanqieRow> LSoundingAnchorRead(long? entry)`

The same rows flattened, for the anchor gates to hand the engine.
It stays internal, since the rows never leave sealed code.

## `internal bool LSoundingAnchorCheck(IReadOnlyList<LFanqieRow> rows, string headword)`

Whether the rows let a reflex anchor at all.

## `internal string LSoundingAnchorFormat(`

The anchors of one reflex written against the rows.

## `internal IReadOnlyList<CAnchorRow> LSoundingAnchorScan(`

The rows one reflex may anchor to, shaped for the anchor menu.

## `public void LSoundingFanqieRebuild(long? entry)`

Fetches the rime-book rows again and announces the change, or the refusal.

## `public void LSoundingFanqieSet(long? entry, long fanqieId, int rank)`

Ranks one rime-book row among the entry's representative readings and announces the change.
Rank zero unmarks the row, and the last rank appends it after the rows already marked.

## `public IReadOnlyList<CScriptGroup> LSoundingScriptRead(long? entry)`

The entry's script rows, started when missing, or nothing for a fresh draft or a refusal.

## `public void LSoundingScriptRebuild(long? entry)`

Drops the entry's script rows and fetches them again, announcing the change or the refusal.

## `public IReadOnlyList<CParadigmSlot> LSoundingParadigmRead(long? entry)`

The entry's paradigm slots, or nothing for a fresh draft or a refusal.

## `internal static IReadOnlyList<CFanqieGroup> LSoundingFanqieRead(IReadOnlyList<LFanqieGroup> groups)`

The one map from the engine's fanqie groups to their shape.
The lectern calls it too, so both fanqie tables draw the same shape.

## `private static CFanqieRow LSoundingRowRead(LFanqieRow row)`

Copies every cell the fanqie line shows, derived cells included.

## `internal static IReadOnlyList<CScriptGroup> LSoundingScriptRead(IReadOnlyList<LScriptGroup> groups)`

The one map from the engine's script groups to their shape, shared with the lectern.

## `internal static IReadOnlyList<CParadigmSlot> LSoundingParadigmRead(IReadOnlyList<LParadigmSlot> slots)`

Joins the slots into paradigm rows in the engine, then shapes each row.
The lectern calls it too, so no driver groups slots.

## `internal static CFrequency? LSoundingFrequencyRead(IReadOnlyList<LFrequency> rows, string once)`

The frequency chip for a set of source rows, or null when there are none.
The display statics rank and format the rows, so the rule stays in Conduct.

## `internal static IReadOnlyList<CPronunciationDraft> LSoundingPronunciationRead(`

The pronunciations of a draft, shaped for the accent rows and the lectern.

## `internal static CPronunciationDraft? LSoundingPrimaryRead(LPronunciationDraft? spoken)`

The main pronunciation, or null when the draft holds none.

## `internal static IReadOnlyList<CTranscriptionDraft> LSoundingTranscriptionRead(`

The transcriptions of a draft, shaped for the transcription rows and the lectern.

## `internal static IReadOnlyList<CReflexDraft> LSoundingReflexRead(IReadOnlyList<LReflexDraft> reflexes)`

The reflexes of a draft, shaped for the reflex rows and the lectern.
The tone travels as text, so no driver reads an anatomy.
