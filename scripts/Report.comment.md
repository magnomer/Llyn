# Report.ps1
Hash: `266f57f6c769539b`

Writes every analysis document afresh in one call, from the steps `Report.json` lists.
The documents are the audits, the method map, the statistics and the scans.
The pages land in the folder `report.directory` names in `Report.json`.

## Usage

```
report [-ConfigPath <path>] [-Keep] [-NoOpen] [-Help]
```

`-Keep` skips the deletion of older pages.
`-NoOpen` writes every page without opening any.

## Run

First the older pages of every step are deleted.
A step owns the files named after its script and a dash.
Its `clear` list adds prefixes.
Any other file stays, since the folder holds every script family's reports.
Subfolders are never touched.

Then each script runs in order, with `-NoOpen` and `-NoPause` where it declares them.
Its console is captured, not shown, so the terminal holds only progress.
A script's own progress lines, those starting with `[n/m]`, show indented below its step.
That is how a script that runs many parts shows each one as it finishes.
A missing or throwing script is recorded as failed, and the run continues.

## Steps

`Report.json` lists the steps in order.
A `script` step runs the one named script.
A `family` step runs every script whose header names that family, in ordinal order.

A family step finds its scripts by the header line `# <NAME> - <FAMILY> GENERATION <n>.`.
A new script of a listed family therefore joins the report without a configuration change.
A family step that finds no script stops the run with an error.

The last step is `{ "family": "SCAN" }`, which runs `ScanTest.ps1`.
The script is found by its header line `# SCANTEST - SCAN GENERATION 1.`.
It owns the pages whose names start with `ScanTest-`, which are the `.md`, `.html` and `.json` pages.
The scan prints its own `[n/12]` progress lines, which show indented below its step.
A scan has no failing status, so its step fails only when the script itself breaks.

## Result

Each script ends its progress line with its status, the pages it left and its duration.
A failing script also shows the last lines of its captured console.
The verdict line follows, and the run exits 1 when any script fails.

## Open

`open` in `Report.json` names the script whose newest page opens at the end.
The newest `.html` page whose name starts with that script name and a dash is the one that opens, unless `-NoOpen` is given.

## Generation

Generation 2 adds the SCAN step, since the report now writes the scan pages too.
Generation 1 had the audit run, the method map and the statistics.
The generation rises only when the written documents change.
`Report.json` is raised to generation 2 to match.
