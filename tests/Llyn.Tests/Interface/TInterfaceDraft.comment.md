# TInterfaceDraft.cs

## `internal static partial class TInterface`

Sample drafts the draft tests build from, shared so every file reads the same shape.

## `internal static LCardDraft TDraftCardCreate(string title)`

A card carrying nothing but a title, for order that is read by title alone.

## `internal static LDraft TDraftNestedCreate(string origin, string headword)`

A draft with enough nesting to prove the whole shape survives the file.

## `internal static LEntryDraft TDraftPlainCreate(string headword)`

An entry with one meaning and nothing nested, for tests that read links rather than content.

## `internal static LCourt TDraftLinkCreate(long owner, long target, string headword)`

A court link with a fresh id, for tests that seed the register directly.
