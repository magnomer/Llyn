# auditdraft.ps1

Checks that every draft record property is named on every side that carries a draft.
It is the standalone counterpart of `TAuditDraft` and reports the same hits.
It never reads the convention tests, and the tests never read it.

## Usage

```
auditdraft [-Root <path>] [-ConfigPath <path>] [-Help]
```

`-Root` audits another git working tree with the same configuration.
`-ConfigPath` reads another configuration instead of `auditdraft.json`.

## Scope

Files come from `git ls-files --cached --others --exclude-standard`, as in the test.
Each include is a git pathspec matched without case.
Files sort by full path, ordinal and without case, as the test sorts them.
A path git lists twice is read once.

## Parse

A small helper parses the record sources with the Roslyn of the .NET SDK.
It is built once for `net10.0` and cached under the temp folder by a hash of its text.
It keeps the positional parameters of each listed record, and a later file overwrites an earlier one.
The records keep the order in which each type was first found, as the test's dictionary does.

## Checks

| Counter | Hit |
|---|---|
| Unnamed properties | A property a side neither names as a whole word nor waives. |
| Stale waivers | A side waiver the side names anyway, or one naming no property. |
| Unmatched settings | A listed type that is no record, or a shared waiver naming no property. |

A property in the shared waiver is skipped on every side.
A side is the join of its sources, one line feed between files.
A side matching no file throws, since its pattern is stale.

## Settings

`auditdraft.json` is copied by hand from `TAuditDraftSetting`, and the two never read each other.

## Output

The console follows `report.md`, and every side hit starts with the side name.
The exit code is 1 when any counter is above 0 and 0 otherwise.
