# TPress.cs
Hash: `655181875f05072d`

## `internal sealed class TPress : LPress`

A press that neither prints nor writes a file, and remembers what it was handed.
Tests read back the page, the path and whether a board was opened.
So a print or a PDF export is checked without a browser or a printer.

## `internal string? TPressHtml`

The last page handed to a save or a board, or null before either.

## `internal string? TPressPath`

The path the last save was handed, or null when nothing was saved.

## `internal bool TPressOpened`

Whether a print board was opened at least once.

## `internal LPressBoard TPressBoard`

The board every open answers, a fresh fake unless a test hands its own.
A test sets it before the engine opens a print session.
It is typed as the Core port, so a fault sweep can hand a fault double over a fake board.
It then reads back what the session drew, printed or disposed on that board.

## `public Task LPressSave(string html, string path)`

Records the page and the path, and writes nothing.

## `public Task<LPressBoard> LPressBoardOpen(string html)`

Records the page, marks the press as opened and answers `TPressBoard`.
So a test can check which sheet a print session opened.
Every open answers the same board, so one press serves one print session in a test.
