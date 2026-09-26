# auditui.ps1

The standalone counterpart script of the convention tests `TAuditStrict`, `TAuditTruth` and `TAuditBoundary`.
It never reads, runs or depends on the test project, and the tests never read it.
Both hold the same rules and ceilings, so each still tells the truth when the other breaks.

## Helper

The walkers live in the script as C# text, ported rule for rule from the test walkers.
The script writes them beside `auditbinder.cs` into a helper project and builds it once per text and SDK.
The helper binds the source through the shared binder with Roslyn 4.14.0, the version the tests pin.
The build is cached under the temp folder, so a later run pays for the binding alone.

## Configuration

`auditui.json` holds every rule value of the three test settings, copied by hand.
Its `helper.framework` pins the framework the helper targets.
`auditui.ledger.json` holds the ceiling of every kind per file, one block for Strict and one for Truth.
The binder settings and the exclusions come from `auditbinder.json`.
`strict.hookSlots` lists the attributes that hook logic, whether set directly or through `Setter Property=`.
It holds `Command`, `CommandParameter`, `CommandTarget`, `DisplayMemberPath`, `SelectedValuePath` and `RelativeSource`.
`strict.hookLiterals` lists the attributes that hook only when their value is a plain literal.
It holds `Tag`, since a literal tag is a value the code branches on.
A markup extension value is left to the extension rules instead.
Generation 14 added these slots and the literal rule, so every setting carries 14.
`-Configuration` swaps the build output the binder reads through a copy of those settings.

## Verdict

A file above the ceiling of a kind fails, and a ceiling above its count is stale and fails.
Every fact the tests gate is one counter, and the counters sum to zero exactly when those tests pass.
The report lists every hit in the line format of the test reports, so the two diff directly.
The Veneer audit is the Strict surface counters, `Hook` and `Shell` among them.
The driver, host and Truth counters are not gated until stage 3 of the Great Purge.
