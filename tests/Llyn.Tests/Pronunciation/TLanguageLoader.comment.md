# TLanguageLoader.cs

## `public sealed class TLanguageLoader`

Covers the language pack loader reading the variety block, the readings list, and the rewrite blocks.
The English pack declares two flagged varieties and a pack without the block declares none.
A readings list loads in written order with its skip counts.
A flat attempt still loads as one untagged reading.
An audio attempt with a readings list loads one non-phonetic reading per variety.
The English pack loads exactly two respelling groups, both scoped, and an empty cleanup list.
A fixture pack that declares varieties drops every group without a non-empty `varieties` list and keeps the rest.
A fixture pack without varieties keeps such groups, so the key stays optional there.
A pack without either block loads none.
A `cleanup` block loads its rules in order and skips malformed rows.
A group with a broken regex is dropped while its neighbours survive.
The packs are copied beside the test binary, so these tests read the shipped data.
A throwaway pack is written beside them for the fixture cases and removed again.
