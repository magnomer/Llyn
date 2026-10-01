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
`strict.tetheringSlots` lists the attributes that tether logic, whether set directly or through `Setter Property=`.
It holds `Command`, `CommandParameter`, `CommandTarget`, `DisplayMemberPath`, `SelectedValuePath` and `RelativeSource`.
`strict.tetheringLiterals` lists the attributes that tether only when their value is a plain literal.
It holds `Tag`, since a literal tag is a value the code branches on.
A markup extension value is left to the extension rules instead.
Generation 14 added these slots and the literal rule.
`strict.veneerNamespace` names the surface types, so an `x:Class` naming another type is tethering.
`strict.hardwiringMarkers`, `strict.masqueradingTypes` and `strict.contractIds` drive the `Hardwiring`, `Masquerading` and `Dangling` kinds.
`strict.contractType` names Deportment's door to the scaffold, whose string arguments are contract IDs.
Generation 15 added these kinds.
Generation 16 added the Unsealing kind to the structure audit.
Generation 17 renames the kinds to one vocabulary, so every setting carries 17.
`-Configuration` swaps the build output the binder reads through a copy of those settings.

## Verdict

A file above the ceiling of a kind fails, and a ceiling above its count is stale and fails.
Every fact the tests gate is one counter, and the counters sum to zero exactly when those tests pass.
The report lists every hit in the line format of the test reports, so the two diff directly.
The Veneer audit is the Strict surface counters, `Tethering` and `Freelancing` among them.
The driver, host and Truth counters are not gated until stage 3 of the Great Purge.
