# AuditUI.ps1
Hash: `3a554b30d2b5963d`

The standalone counterpart script of the convention tests `TAuditStrict`, `TAuditTruth` and `TAuditBoundary`.
It never reads, runs or depends on the test project, and the tests never read it.
Both hold the same rules and ceilings, so each still tells the truth when the other breaks.

## Helper

The walkers live in the script as C# text, ported rule for rule from the test walkers.
The script writes them beside `AuditBinder.cs` into a helper project and builds it once per text and SDK.
The helper binds the source through the shared binder with the Roslyn version the tests pin.
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
The binder settings and the exclusions come from `AuditBinder.json`.
`strict.tetheringSlots` lists the attributes that tether logic, whether set directly or through `Setter Property=`.
`strict.tetheringLiterals` lists the attributes that tether only when their value is a plain literal.
A literal in such an attribute is a value the code branches on.
A markup extension value is left to the extension rules instead.
`strict.veneerNamespace` names the surface types, so an `x:Class` naming another type is tethering.
`strict.hardwiringMarkers`, `strict.masqueradingTypes` and `strict.contractIds` drive the `Hardwiring`, `Masquerading` and `Dangling` kinds.
`strict.contractType` names the driver's door to the scaffold, whose string arguments are contract IDs.
A call written inside the contract type's own declaration is not judged, since it only forwards its caller's ID.
The skip goes by the enclosing type declaration's symbol, never by method name or by a forwarded parameter.
So a caller elsewhere that forwards its own parameter into the contract type is still a hit.
A type nested inside the contract type is another type and is judged as before.
Findings use one vocabulary of -ing kinds and plain measure names.
The truth walk skips a literal bool comparison and a name inside `nameof`.
It sees a field passed by `ref` and a delegate stored from a parameter.
It sees an interface event, a static Conduct value and a surface loaded through a built URI.
It traces a delegate handed in from any tracked source, such as the composition root.
`truth.informative` maps a truth kind to the driver folders it waits on.
The kind gates nothing while a listed folder holds no tracked source.
Its hits still reach the report, under a line naming the kind as informative.
Once every listed folder holds a source, the kind fails like any other.
`truth.controlPrefixes` maps each driver ring folder to its control prefix.
A driver type whose name starts with its ring's prefix is a control type.
Puppeteering names every control member in the relay set, one hit per member.
Its relay set is the fixed point grown before any event joins.
A control raising its own event signals, and the driver handling it requests.
A local function or local stays with its member, and a Conduct record property read is no request.
`TAuditTruthSetting.TAuditControlPrefix` holds the same rows, copied by hand.
Zeroing reports a literal `0`, `0L` or `default` a driver sends toward a gate.
It looks through parentheses, casts, both arms of `?:` and the right side of `??`.
A literal compared with a member read inside a hot argument, or with a logic member, is a hit too.
Zeroing reads `LAuditZeroingNames`, a copy of `LAuditHotNames` grown with constructors and stored parameters.
The copy is built after the relay read and never written back, so Replaying keeps its narrower map.
`TAuditZeroingNames` holds the same rule, written by hand.
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
`truth.inputBase` names the base type of every control that takes user input.
The laundering walk colours a `truth.inputMembers` read only on a receiver deriving from it.
`TAuditTruthSetting.TAuditInputMembers` holds the same list, copied by hand.
An error type, a `dynamic` receiver or a missing type keeps the colour, as `LAuditInputCheck` reads it.
A namespace receiver, as in `System.Text`, reads no value and is not coloured.
`TAuditTruthSetting.TAuditInputBase` holds the same name, copied by hand.
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

A value-typed member set once at construction is identity, so the truth walk does not audit it.
A value type is a bool, char, number, string or enum, or the nullable form of one.
A `readonly` value field is skipped when every write sits in a constructor of its own type.
A write from a lambda, a local function or another member still audits it.
A positional record parameter of a value type is skipped unless the record is copied with `with` or written.
The copy scan covers every tracked source and counts a copied record's base records too.
`TAuditTruthField.TAuditValueCheck` holds the same rule, copied by hand.

A file above the ceiling of a kind fails, and a ceiling above its count is stale and fails.
Every fact the tests gate is one counter, and the counters sum to zero exactly when those tests pass.
The report lists every hit in the line format of the test reports, so the two diff directly.
The `enforced` flag of the `strict` and `truth` settings gates the kinds of each audit.
An informative kind that waits on an empty driver adds no row to its counter.

## Kinds

The script reports three audits, Strict, Truth and Boundary.
Each hit carries a kind, and the ledger holds a ceiling per kind per file.

Strict kinds read the surfaces, the host and the markup.
- `Hoarding` is a field or event field in a surface type.
- `Sharing` is a mutable static field in a driver type.
- `Freelancing` is a surface member other than a constructor.
- `Meddling` is a surface line that is not a plain call, or a field initializer that is not a literal.
- `Prying` is a driver or surface naming a type from below the driver.
- `Homoglyph` is an identifier carrying a non-ASCII glyph.
- `Overworking` is a host line that does more than construction and wiring.
- `Tethering` is a markup line that hooks logic into the markup.
- `Branching` is markup that branches on a condition or computes in a property element or binding.
- `Overreaching` is markup that does not parse, maps a logic namespace, reads a logic constant or names logic.
- `Hardwiring` is a driver loading the surface through a built URI or naming a surface marker.
- `Masquerading` is a driver type deriving from a type in `strict.masqueradingTypes`.
- `Dangling` is a contract ID that is not a constant or has no element or resource in the surface.

Truth kinds read the drivers.
- `Laundering` is computing over the user input.
- `Misfiring` is input deciding a request, or a second request for one action.
- `Gatekeeping` is a held value or an engine answer deciding a request.
- `Zeroing` is a literal `0`, `0L` or `default` sent toward a gate.
- `Moonlighting` is comparing or computing over a value from below Conduct.
- `Duplicating` is a field naming a state, holding an untyped value, caching a gate answer or holding a type from below Conduct.
- `Contesting` is a field written both from a gate answer and from the driver own logic.
- `Replaying` is a held value or a gate answer passed into a request.
- `Tampering` is assigning a member of a logic object, or reordering a collection Conduct gave.
- `Puppeteering` is a control member calling logic itself.
- `Spoonfeeding` is handing the surface a type from below the cut, Conduct included.
- `Mismatching` is a Conduct member one medium reaches and another never does.
- `Homoglyph` is also reported here for the driver sources.

The Boundary counters guard the walkers and carry no kind.
They count forbidden state builds, state compares, hidden lines, reflection calls, skipped sources, logic panels and hold timers.
They also count stale boundary rows.

## Ceilings

A ceiling is the hit count of one kind that one file may hold, written in `AuditUI.ledger.json`.
A file above its ceiling fails, and a ceiling above its count is stale and fails.
So a ceiling only walks down.
A kind not written in the ledger holds a ceiling of zero.
Driver disk lines and surface catalog lines keep no ceiling and must stay at zero.
A kind in `truth.informative` gates nothing until its driver folders hold a source.

## Exemptions

Only two lists exempt anything, `strict.diskExempt` and `strict.catalogExempt`.
Each names a file by its file name, and the file is spared the disk or catalog line check.
A name that matches no line is stale and fails.
The Truth and Strict kinds take no exemption.
The Boundary settings keep their own rows, and a row that matches nothing fails.
## In Llyn

The ledger holds only Mismatching rows, which wait on the Demeanor folder.
