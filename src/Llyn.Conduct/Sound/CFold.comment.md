# CFold.cs
Hash: `1d4f1209ad356aac`

## `public sealed class CFold`

Rime-book and script box states belong to one entry, not to settings or this area's memory.
Each read asks the engine for that entry's stored state.
These states and their failure gates form a concern separate from the sound sheet.
The editor builds one over its held entry, and the reading view builds one over its shown entry.
Both modes share one rule and one owner, so the area takes an entry source, never a mode flag.

## `private readonly Func<long?> _cFoldEntry;`

The entry source names the stored entry owning both box states.
It is asked on every read and write, so the area never holds a stale id.

## `private readonly LReflexPort _cFoldReflexPort;`

The reflex port keeps box persistence behind the engine boundary.

## `private readonly LSettingsPort _cFoldSettingsPort;`

Failure notices obtain their wording through the settings port.

## `private readonly CEnvoy _cFoldEnvoy;`

The envoy presents read and write failures within its owner's notice scope.

## `private readonly CLedgerNoticed _cFoldNoticed;`

Repaint memory suppresses repeated read-failure notices until the user acts.

## `internal CFold(Func<long?> entry, LReflexPort reflexes, LSettingsPort settings, CEnvoy envoy, CLedgerNoticed noticed)`

The owner supplies entry context, persistence and notice dependencies together.
The editor passes its desk's stored entry, and the reading view passes its shown entry.
Each owner passes the repaint memory its other reads already use.
The internal constructor keeps composition within Conduct.

## `public bool CFoldFanqieOpened`

The entry's rime-book box defaults to closed without a stored entry or after a refused storage read.

## `public bool CFoldScriptOpened`

The script box follows the same entry-scoped read and closed fallback as the rime-book box.

## `public bool CFoldFanqieSpread(bool opened)`

True means the port returned normally, not that a row changed.
The engine's fold bulletin lets the editor and the reading view repaint from storage.
So a fold in either mode folds both.
Without a stored entry, false is returned without a write.
A write failure shows `Box.SpreadFailed` and returns false, allowing the driver to restore the switch.

## `public bool CFoldScriptSpread(bool opened)`

The script gate has the same normal-return verdict, absent-entry refusal and failure notice as the rime-book gate.

## `private static bool LFoldCheck(LReflexPort reflexes, CLedgerNoticed noticed, CEnvoy envoy, LSettingsPort settings, long? entry, LFoldBox box)`

Both box reads share one closed fallback and repaint-failure policy.
Only this area calls it, since the reading view reads through its own instance.
No entry means no engine read.
Storage failures use `Box.SpreadReadFailed` through repaint memory and answer closed.

## `private long? LFoldEntry`

A fresh draft, an empty desk or an empty lectern has no stored entry to own box state.
