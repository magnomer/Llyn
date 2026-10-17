# UnicodeWidth.ps1

Writes the console cell-width table `src/Llyn.UIDemeanor/Kit/BCanvasRange.cs` from the Unicode data.
It is tracked, kept visible by its `!/scripts/UnicodeWidth.ps1` line in `.gitignore`.

## Usage

```
unicodewidth -DataDirectory <path> [-Help]
```

`-DataDirectory` names the folder holding `EastAsianWidth.txt` and `emoji-data.txt`.
`-Help` prints the usage and the download addresses, and exits without reading anything.

## Version

The Unicode version is pinned to the one the .NET runtime carries, 16.0.0 for .NET 10.
The table and the runtime's `StringInfo` must agree, so a cluster and its width read one standard.
A data file whose header names another version is refused with exit 1.
A new runtime with a new Unicode version moves the pin, the addresses and the data together.

## Table

East Asian Width `W` and `F` take 2, `A` is ambiguous, and every other value takes 1.
`Emoji_Presentation` takes 2 over whatever the width file says, since regional indicators read `N` there.
The table partitions every code point into runs, so a lookup is one binary search.
Each run carries its cells under both ambiguous settings, so the table needs no class code.
Zero-width categories stay out of the table, since the runtime's own categories answer them.

## Output

The file is written as UTF-8 without a byte-order mark, with LF line ends.
The last line names the version, the run count and the file written.
Windows PowerShell 5.1 and PowerShell 7 write the same bytes.
