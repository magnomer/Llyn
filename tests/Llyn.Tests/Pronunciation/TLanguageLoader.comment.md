# TLanguageLoader.cs

## `public sealed class TLanguageLoader`

Covers the language pack loader reading the variety block and the readings list.
The English pack declares two flagged varieties and a pack without the block declares none.
A readings list loads in written order with its skip counts.
A flat attempt still loads as one untagged reading.
The packs are copied beside the test binary, so these tests read the shipped data.
