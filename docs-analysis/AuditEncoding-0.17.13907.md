# Encoding report - 0.17.13907

- Project: Llyn
- Generated: 2026-10-09 06:46:54 +09:00
- Generation: 21
- Project root: `D:\Programming\Llyn`
- Scanned: 4,062 text files, 14,959,812 bytes
- Includes: `*.cs`, `*.xaml`, `*.md`, `*.json`, `*.csproj`, `*.props`, `*.slnx`, `*.ps1`
- Excluded segments: `.git`, `bin`, `obj`, `out`, `publish`, `snapshots`, `TestResults`
- Skipped names: `version.json`
- Tabless suffixes: `.cs`, `.xaml`
- ASCII suffixes: `.ps1`
- Control pattern: `[\u0000-\u0008\u000B\u000C\u000E-\u001F\u007F]`
- Trail pattern: `[\u00A0-\u00BF\u0152\u0153\u0160\u0161\u0178\u017D\u017E\u0192\u02C6\u02DC\u2013\u2014\u2018-\u201E\u2020-\u2022\u2026\u2030\u2039\u203A\u20AC\u2122]`
- Mojibake pattern: `\u00EF\u00BB\u00BF|(?:[\u00E0-\u00EF]{trail}{2}){2}|(?:[\u00C2-\u00DF]{trail}){2}`, where `{trail}` stands for the trail pattern

## Result

| Status | Count | Gate | Meaning |
|--------|------:|------|---------|
| OK | 0 | Invalid UTF-8 | files with a byte sequence that is not UTF-8 |
| OK | 0 | Byte order marks | files with a byte order mark |
| OK | 0 | Carriage returns | files with a carriage return |
| OK | 0 | Missing final newlines | non-empty files not ending in a line feed |
| OK | 0 | Control characters | files with a raw control character |
| OK | 0 | Double-encoded text | files with UTF-8 read back through a single-byte page |
| OK | 0 | Tabs in sources | tabless files containing a tab |
| OK | 0 | Non-ASCII scripts | scripts with a byte outside ASCII |

PASS: all 8 gates at 0.

## Invalid UTF-8 (0)

None.

## Byte order marks (0)

None.

## Carriage returns (0)

None.

## Missing final newlines (0)

None.

## Control characters (0)

None.

## Double-encoded text (0)

None.

## Tabs in sources (0)

None.

## Non-ASCII scripts (0)

None.
