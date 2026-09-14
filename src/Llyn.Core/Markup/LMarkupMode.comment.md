# LMarkupMode.cs

## `public enum LMarkupMode`

The three ways a parsed entry may enter a workspace.

## `LMarkupModeNew`

A new entry is stored under the file's headword and gets a fresh id.

## `LMarkupModeMerge`

The parsed rows are appended to a chosen stored entry, whose headword and existing rows are kept.

## `LMarkupModeReplace`

A chosen stored entry keeps its id and its body is swapped for the parsed one.
