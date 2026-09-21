# LSounding.cs

The sound sheet of one held entry: its rime-book groups, its script rows and its paradigm slots.
The editor owned all of this before, which left one deportment carrying two subjects at once.
Every call names the entry it works on, so the sheet keeps no entry and caches no answer.
Every read swallows a refusal and answers empty, since a box that cannot fetch still has to draw.
Every write announces the change or the refusal, and the editor passes the refusal on as its own.

## `public LSounding(LPhonologyPort phonology)`

Takes the phonology port every read and write goes through.

## `public event Action? LSoundingChanged;`

A write landed, so the boxes drawn from the sheet are read again.

## `public event Action<string, Exception>? LSoundingFailed;`

A write was refused, announced under its own notice key.

## `public IReadOnlyList<LFanqieGroup> LSoundingFanqieRead(long? entry)`

The entry's rime-book groups, started when missing, or nothing for a fresh draft or a refusal.

## `public string LSoundingReadingRead(long? entry, string headword)`

The headword's representative reading, formed by the engine from the same groups the box draws.

## `public IReadOnlyList<LFanqieRow> LSoundingAnchorRead(long? entry)`

The same rows flattened, for the reflex rows to anchor on.

## `public void LSoundingFanqieRebuild(long? entry)`

Fetches the rime-book rows again and announces the change, or the refusal.

## `public void LSoundingFanqieSet(long? entry, long fanqieId, int rank)`

Ranks one rime-book row among the entry's representative readings and announces the change.
Rank zero unmarks the row, and the last rank appends it after the rows already marked.

## `public IReadOnlyList<LScriptGroup> LSoundingScriptRead(long? entry)`

The entry's script rows, started when missing, or nothing for a fresh draft or a refusal.

## `public IReadOnlyList<LParadigmSlot> LSoundingParadigmRead(long? entry)`

The entry's paradigm slots, or nothing for a fresh draft or a refusal.
