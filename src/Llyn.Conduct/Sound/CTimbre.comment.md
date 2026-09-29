# CTimbre.cs

## `public sealed class CTimbre`

The sound facts of the entry an editor holds, as the editor shows or offers them.
The pack facts are read for the held draft's language, and the waiting reflexes from the editor's display.
The editor builds it over its desk, its phonology port and its display, so it keeps no copy.

## `public event Action? CTimbreParadigmChanged;`

The held entry's paradigm changed, raised on the driver's thread.
`CTimbreScriptChanged` and `CTimbreReflexChanged` do the same for the script groups and the reflexes.

## `public bool CTimbreFlagged`

Whether the held draft's language has a flag.

## `public IReadOnlyList<string> CTimbreVarietyNames`

The varieties the held draft's language offers, empty while no draft is held.

## `public bool CTimbreReflexShown`

Whether the reflex block shows for the held draft.

## `public bool CTimbreReflexPending`

Whether the held entry's reflex lookup is still running, so the reflex block shows its loading line.

## `public void CTimbreReflexStart()`

Starts the reflex lookup for the held entry, which the editor asks when a stored entry opens without reflexes.
Nothing is started for a fresh draft, since a lookup needs a stored entry.

## `public void CTimbreReflexRebuild()`

The user asked to look the held entry's reflexes up again.

## `internal void LTimbreObserverAttach(Action<Action> marshal)`

Hears the paradigm and reflex subjects of the held entry and the tenure's script subject.

## `public bool CTimbreSpoken`

Whether the held draft's pack is spoken, so the pronunciation and accent rows show.

## `public bool CTimbrePhonemic`

Whether the reading field shows a phonemic respelling, which needs a respelling pack first.

## `private string LTimbreLanguage`

The held draft's language, or empty while no draft is held.

## `private long? LTimbreEntry`

The stored entry the held draft stands on, or null for a fresh draft or an empty desk.
