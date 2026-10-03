# Report.ps1
Hash: `f4303d55ff955d9c`

Writes every analysis document afresh in one call: the audits, the method map and the statistics.
The pages land in `docs-analysis/`, the folder uploaded to GitHub with each version.
It is tracked, kept visible by its `!/scripts/Report.ps1` line in `.gitignore`.

## Usage

```
report [-ConfigPath <path>] [-Keep] [-Help]
```

`-Keep` skips the deletion of older pages.

## Run

First the older pages of every step are deleted.
A step owns the files named after its script and a dash.
Its `clear` list adds prefixes, so the audit step owns every `Audit` page.
Any other file stays, since the folder holds every script family's reports.
Subfolders such as `records` are never touched.

Then each script runs live, in order, with `-NoOpen` and `-NoPause` where it declares them.
A missing or throwing script is recorded as failed, and the run continues.

## Steps

`Report.json` lists the steps in order.

| Step | Scripts |
|---|---|
| `script: Audit` | `Audit.ps1`, which runs every audit itself. |
| `script: MapMethod` | `MapMethod.ps1`. |
| `family: STATS` | Every script whose header names the Stats family, in ordinal order. |

A family step finds its scripts by the header line `# <NAME> - <FAMILY> GENERATION <n>.`.
A new Stats script therefore joins the report without a configuration change.

## Result

The console ends with a `Result` table in the audit console grammar of `report.md`.
Each script is one row, and its exit code is the count.
The meaning names the pages it left and its duration.
The run exits 1 when any script fails.
