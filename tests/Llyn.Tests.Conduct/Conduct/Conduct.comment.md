# Conduct

These tests cover dropdown movement, Markdown mapping, phonology, playback, state wording, usage keys and duplex wings.
Direct value tests sit beside workspace tests that exercise queries, restoration and file-backed media.

## `TLantern`

Boundary cases distinguish wrapping a lit offer from choosing an end when no row is lit.

## `TMarkdown`

Mapped blocks check that list formatting and link addresses survive parsing.

## `TPhonology`

Workspace tests exercise panel reads, editing, export and observer notifications together.
Repeated restoration checks query retention and duplicate notifications.
A real export file distinguishes choosing an entry from requesting export without a selection.

## `TPlayback`

Playback tests create placeholder files to check address resolution without playing media.
Accent tests distinguish missing files from missing rows and unopened desks.

## `TStateValue`

Direct values distinguish written text from the empty value's absent display text.

## `TUsage`

Direct usage values check owner keys independently of loading citing records.

## `TWing`

Separate left and right tests check workspace slot ownership.
Restoration checks event ordering before the saved entry loads.
Keyboard selection tests distinguish marking rows from loading entries and saving workspace slots.

## Stamp

Commit: `c37ca1837ded`
- `TLantern.cs` `a0a4f67dca4a3f12`
- `TMarkdown.cs` `299d86e17a73ffc2`
- `TPhonology.cs` `aa517d06433c9366`
- `TPlayback.cs` `1c4ee29bf84cdede`
- `TStateValue.cs` `f7358dccbc2fdd45`
- `TUsage.cs` `670e2af1d20f3d1a`
- `TWing.cs` `b7a0c1a0b236eb6b`
