# AuditUI.ps1
Hash: `37cb569dd2407655`

The standalone counterpart script of the convention tests `TAuditStrict`, `TAuditTruth` and `TAuditBoundary`.
It never reads, runs or depends on the test project, and the tests never read it.
Both hold the same rules and ceilings, so each still tells the truth when the other breaks.

## Helper

The walkers live in the script as C# text, ported rule for rule from the test walkers.
The script writes them beside `auditbinder.cs` into a helper project and builds it once per text and SDK.
The helper binds the source through the shared binder with Roslyn 4.14.0, the version the tests pin.
The build is cached under the temp folder, so a later run pays for the binding alone.

## Reports

Each run writes `{report.directory}/AuditUI-{version}.md` and a page `AuditUI-{version}.html` beside it.
The helper writes the page data with `System.Text.Json`, so both PowerShell editions write the same bytes.
The script fills the template `AuditUI.html` with that data and opens the page unless `-NoOpen` is given.
`-Open` opens the Markdown report as well.
Each finding row keeps the fields its console text is built from, so the page shows real tables.
The helper also names each gate's meaning, so the console and the page read one text.

## Configuration

`AuditUI.json` holds every rule value of the three test settings, copied by hand.
Its `helper.framework` pins the framework the helper targets.
`AuditUI.ledger.json` holds the ceiling of every kind per file, one block for Strict and one for Truth.
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
A call written inside the contract type's own declaration is not judged, since it only forwards its caller's ID.
The skip goes by the enclosing type declaration's symbol, never by method name or by a forwarded parameter.
So a caller elsewhere that forwards its own parameter into the contract type is still a hit.
A type nested inside the contract type is another type and is judged as before.
Generation 15 added these kinds.
Generation 16 added the Unsealing kind to the structure audit.
Generation 17 renames the kinds to one vocabulary, so every setting carries 17.
Generation 18 stops five false truth findings, so every setting carries 18.
Generation 18 also sees a field passed by `ref` and a delegate stored from a parameter.
It sees an interface event, a static Conduct value and a surface loaded through a built URI.
It traces a delegate handed in from any tracked source, such as the composition root.
Generation 19 adds the comment hash check, so every setting carries 19.
A `break` in a switch skips only the rest of its own section.
An `if` without `else` leaves only when its body always reaches a `return`, `throw`, `break` or `continue`.
Such a jump has only plain blocks between it and the `if`.
A jump under a nested branch, loop, `try`, `using`, lambda or local function may not run.
A `break` or `continue` that counts therefore targets a loop or switch enclosing that `if`.
A `throw` expression such as `x ?? throw` is not a jump statement and never leaves it.
A control read in a switch case guard or switch arm guard that picks a request is misfiring, as in an `if`.
So is a control read as the governing expression when a case or arm label decides on its value.
A label of type tests alone, even nested in property patterns, pairs a row and decides nothing.
An `if`, ternary or guard of such an `is` test, or of a presence test, decides nothing either.
A bare type name in a pattern, and `or`, `and`, `not` or parentheses over type tests, stay type tests.
A local or parameter that carries a control read counts as that read within its member.
A declaration, an assignment or a local function argument carries it, in source order.
A pattern designation never carries, and a call that takes a control is not a read of it.
A type pattern naming a control reads it, even on a value typed `object` such as `sender`.
`truth.focusMembers` lists the members that tell whether the user is at a control.
An `if` without `else` that only drops an event the user did not cause is hearing, not misfiring.
It qualifies as a bare `return` under a negated focus guard, or as a focus guard with no later request.
A focus guard is an `&&` chain whose control reads only test a focus member as true.
Other parts of its type pattern may only be type tests.
A contesting hit lands on the first shell write and lists every other shell write after it.
This changes only the hit's text, never the count.
Contesting follows a written value to its origin through Deportment holders, as `LAuditOriginWalker`.
A holder is a field, a property, a local, a setter's `value` or a constructor parameter in a shell source.
A by-value parameter of an ordinary method is a holder too, written by its call-site arguments.
Its callers are unseen when the method is a method group, generic, overridable, an override or an interface member.
A method with no seen call has unseen callers too.
It is engine only when every writer is visible and engine.
Its writers are assignments, initializers, call arguments, getter bodies, `SetValue` calls and non-neutral defaults.
An unseen writer, a `ref` or compound write, or an unresolved cycle makes it plain.
A non-private member of a type with generated markup code has an unseen writer.
A blank write, null, default, `""` or `string.Empty`, is a clear and contests nothing.
`false` and zero stay plain writes.
A call into a type under `truth.capsuleInclude` resolves plain, and its arguments are not searched.
The script appends `capsuleInclude` to `shellInclude` at load, so each path is written once.
So the script's origin walker reads the shell list alone, where the test joins both lists.
A direct copy of a holder already on the resolve path is neutral and adds no origin.
A holder written only by such copies stays plain.
A `ref` argument into a helper that only assigns another parameter writes that parameter's argument.
When every writer is a call to one shell method, `LAuditLookupWalker` compares the call's inputs instead.
A constant input is a key, so a literal key beside an engine key does not contest.
The report states the generation it reads from `AuditUI.json`.
`-Configuration` swaps the build output the binder reads through a copy of those settings.

## Verdict

A file above the ceiling of a kind fails, and a ceiling above its count is stale and fails.
Every fact the tests gate is one counter, and the counters sum to zero exactly when those tests pass.
The report lists every hit in the line format of the test reports, so the two diff directly.
The `enforced` flag of the `strict` and `truth` settings gates the kinds of each audit.
Both flags are set now.
