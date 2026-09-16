# TLanguageLoader.cs

## `public sealed class TLanguageLoader`

Covers the language pack loader reading the pack-level blocks: varieties, flags, rewrites, schemes and the spelling list.
The English pack declares two flagged varieties and a pack without the block declares none.
The Mandarin and Cantonese packs declare tone and the English pack does not.
The Classical Chinese pack is silent, and the Wu pack is phonemic but stays off the language list.
The English pack loads exactly two respelling groups, both scoped, and an empty cleanup list.
A fixture pack that declares varieties drops every group without a non-empty `varieties` list and keeps the rest.
A fixture pack without varieties keeps such groups, so the key stays optional there.
A pack without either block loads none.
A `cleanup` block loads its rules in order and skips malformed rows.
A group with a broken regex is dropped while its neighbours survive.
A `transcription` entry loads as a scheme whether it is a bare name or an object with sources.
Blank and doubled scheme names are dropped.
The Mandarin pack declares its two schemes with sources, so its rows can be looked up.
A good `spelling` list is stamped on every source kind, and one with a broken regex loads as no rules.
The Classical Latin pack carries its twelve macron rules on its lookup, harvest and frequency sources alike.
A pack without the list stamps none.
A `flag` ending in `.svg` loads as the rooted path of that file in the pack folder.
A country code loads as itself.
The source rows a pack declares are covered in `TLanguageLoaderSource.cs`.
The packs are copied beside the test binary, so these tests read the shipped data.
A throwaway pack is written beside them for the fixture cases and removed again.
