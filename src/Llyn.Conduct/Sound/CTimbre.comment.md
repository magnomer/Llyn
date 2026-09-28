# CTimbre.cs

## `public sealed class CTimbre`

The sound facts of the entry an editor holds, as the editor shows or offers them.
The pack facts are read for the held draft's language, and the waiting sections from the editor's display.
The editor builds it over its desk, its phonology port and its display, so it keeps no copy.

## `public bool CTimbreSpoken`

Whether the held draft's pack is spoken, so the pronunciation and accent rows show.

## `public bool CTimbrePhonemic`

Whether the reading field shows a phonemic respelling, which needs a respelling pack first.

## `public bool CTimbreFanqieRebuildable`

Whether the rime-book rows may be fetched again, which needs a stored entry and a pack with a rime book.

## `public bool CTimbreScriptRebuildable`

Whether the script rows may be fetched again, which needs a stored entry and a pack with script styles.

## `private string LTimbreLanguage`

The held draft's language, or empty while no draft is held.

## `private long? LTimbreEntry`

The stored entry the held draft stands on, or null for a fresh draft or an empty desk.
