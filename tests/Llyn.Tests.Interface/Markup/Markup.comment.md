# Markup

This folder holds the markup reader and writer relay.
It also holds the outcome record the engine's import relay answers.

## TInterfaceMarkup

The relay for the markup reader and writer.
A test hands text in and reads entries and omissions back through here alone.
Text becomes the node tree through `LMarkupFile`, the same adapter the engine's rig carries.
An intake for the import is built here too.
A test names an index and a mode, and a target only when one is replaced.
An entry is built here as well, for a writer test that starts from a record rather than text.
It may carry a lexical unit and a freedom, for a test of the unit and freedom elements.
A target is counted here from a stored draft, for a test of what a replacement drops.
The node builders make leaf and branch nodes, for a test that builds a tree by hand.
The parse and format relays serve a test that inspects the tree or round-trips text.

## `TMarkupCeilingRead`

The largest markup file the file adapter reads, so the oversize test builds one byte past it.

## `TMarkupPair`

Two entries that each translate to the other, so both links resolve only through each other.

## `TMarkupLone`

One entry whose translation names an entry the file does not carry.

## `TMarkupSave`

The file lands inside the test workspace, so it is cleaned up with it.

## `TCardCreate`

A card with only a definition, enough for a sense that just needs to exist.
The import, export and link tests all build their entries from it.

## `TMarkupOutcome`

What a test import produced, as the import tests read it.
The engine reports only what the import left behind.
The stored entries are found again by headword, so a test can open what it imported.
The entries are in file order, and so are the omissions the read and import skipped.

## Stamp

Commit: `c2f335c331fe`
- `TInterfaceMarkup.cs` `3ad89a2420a7c58e`
- `TMarkupOutcome.cs` `3ad813ad243474aa`
