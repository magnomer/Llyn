# Script parameter grammar

This file is the single lexicon of parameter names across every script in `scripts/`.

## Rule

A parameter name carries exactly one meaning.
A script may lack a parameter, but a script that has it uses that meaning.
One meaning carries exactly one name, so no two names share a meaning.
An alias is a name too and follows the same rule.
PowerShell common parameters already own their meanings, so no script parameter repeats one.

## Adding a parameter

Look the meaning up in the lexicon below first.
When a row matches the meaning, reuse its name unchanged.
When no row matches, pick a name no row holds and add the row in the same change.
A name whose meaning must change is renamed everywhere in the same change.

## Common parameters

| Name | Meaning | Scripts |
|---|---|---|
| `-Verbose` | Print the detail the script otherwise leaves out. | check |
| `-WhatIf` | Show every write without performing it. | clean, version |
| `-Confirm` | Ask before every write, and `-Confirm:$false` skips every prompt. | clean, version |

## Lexicon

| Name | Meaning | Scripts |
|---|---|---|
| `-Help`, `-?` | Print the help and exit without doing anything else. | all |
| `-Root` | Project root to work on, defaulting to the parent of `scripts/`. | auditfake, auditnames, auditnewnames, auditobject, auditplatform, auditstructure, syncnames |
| `-ConfigPath` | JSON configuration file, defaulting to the script's own file beside it. | auditcomments, auditlines, auditui |
| `-Configuration` | Build configuration, `Debug` or `Release`. | auditui, test |
| `-SourceRoots` | Source roots to scan, overriding `sources.roots`. | auditcomments, auditlines |
| `-Extensions` | File extensions to scan, overriding `sources.extensions`. | auditlines |
| `-Exclude` | Git pathspecs left out of every measurement. | vestimate |
| `-Segments` | Path segments under a root that form one folder row. | auditcomments, auditlines |
| `-MaxWords` | Word limit of one comment line, overriding `rules.maxWords`. | auditcomments |
| `-LimitThreshold` | Last line count that passes, overriding `thresholds.limit`. | auditlines |
| `-WarningThreshold` | Last line count below the warning band, overriding `thresholds.warning`. | auditlines |
| `-Commits` | How many recent commits feed the statistics. | auditlines, vestimate |
| `-Top` | How many rows a console table shows. | auditfake, auditobject, vestimate |
| `-OutputPath` | File the Markdown output is written to. | auditcomments, auditlines, auditui, tracer |
| `-ReportDirectory` | Folder the Markdown report is written to under its versioned name. | auditfake, auditobject, auditplatform |
| `-Open` | Open the report once the run finishes. | auditcomments, auditfake, auditlines, auditobject, auditplatform, auditstructure, auditui |
| `-NoPause` | Never stop at a console page. | auditcomments, auditfake, auditlines, auditnames, auditobject, auditplatform, auditstructure, vestimate |
| `-Name` | Symbol names the walk starts from. | detector, test |
| `-Depth` | Maximum hops a walk takes, where 0 walks until nothing new turns up. | detector, tracer |
| `-Entry` | Method that takes the injected value. | tracer |
| `-Value` | Value injected into the entry. | tracer |
| `-Parameter` | Parameter of the entry that takes the value. | tracer |
| `-Json` | Write the result as JSON instead of the report. | detector |
| `-Version` | The `major.minor.revision` version the script acts on, or `stable` where accepted. | build, run, stop, timemachine, version |
| `-Command` | Version operation to perform. | version |
| `-Rebuild` | Discard the existing result and produce it again from scratch. | build, gitdownload, run, synccode, timemachine, vestimate |
| `-All`, `-a` | Take every item instead of the default subset. | stop, test |
| `-Force` | Kill a process at once instead of asking its window to close. | stop |
| `-TimeoutSeconds` | Seconds to wait for a window to close before the kill. | stop |
| `-NoBuild` | Skip the build and use the existing build output. | check, test |
| `-NoRestore` | Skip the package restore. | test |
| `-NoTest` | Skip the test run. | check |
| `-NoAudit` | Skip the audits. | check |
| `-NoLaunch` | Build and install without starting the application. | timemachine |
| `-Grep` | Patterns whose every hit in the sources counts as a failure. | check |
| `-Filter` | VSTest filter expression that selects the tests. | test |
| `-Project` | Single test project to run. | test |
| `-Convention`, `-c` | Run the convention test project only. | test |
| `-Main`, `-m` | Run every test project except the convention project. | test |
| `-List` | List the matching tests without running them. | test |
| `-Repeat` | Number of runs, stopping at the first failure. | test |
| `-Verbosity` | Logging level passed to `dotnet test`. | test |
| `-AdditionalArguments` | Arguments passed unchanged to `dotnet test`. | test |
| `-Runtime` | Runtime identifier for the .NET publish. | debug |
| `-TempOnly` | Remove only stray WPF temporary-project files. | clean |
| `-IdeCache` | Also reset the VS Code C# Dev Kit workspace cache. | clean |
| `-FingerprintOnly` | Print the source fingerprint without writing an archive. | zip |
| `-Repository` | GitHub repository in `owner/name` form. | gitdownload |
| `-SnapshotDirectory` | Folder that receives the version folders and their archives. | gitdownload |
| `-Threshold` | Exclusive version threshold saved to `snapshot.json`, where 0 takes every version. | gitdownload |
| `-Destination` | Folder the dispatchers are written to. | install |
| `-File` | Text file of name tokens to check. | auditnewnames |
| `-Token` | Name tokens to check. | auditnewnames |
| `-Round` | Granularity of the suggested version step. | vestimate |
| `-MovedWeight` | Weight of a moved line. | vestimate |
| `-ReflowWeight` | Weight of a reflowed line. | vestimate |
| `-DeletedWeight` | Weight of a novel deleted line. | vestimate |
| `-PathWeights` | Weight of each path class. | vestimate |
| `-Calibrate` | Search the line weights for the least spread and print the best pair. | vestimate |

## Internal parameters

These names exist for one script calling another and are never typed by hand.

| Name | Meaning | Scripts |
|---|---|---|
| `-CurrentDestination` | Layout key that receives a current-source build. | timemachine |
| `-WorkRoot` | Layout key whose temp subfolder holds the intermediate files. | timemachine |
| `-MirrorCurrentSnapshot` | Also copy a current-source publish into its snapshot folder. | timemachine |
| `-Invoker` | Name of the delegating script, written into the record. | timemachine |
| `-SnapshotStagingDirectory` | Staging folder used while installing a current snapshot. | zip |
