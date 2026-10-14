# Strict

This folder holds the surface audit with its rules, conditions, meddling judge, setting, ledger and walker.
It also holds the reach, host and contract walkers the audit runs, and the contract walker's assays.

## `TAssayContract`

Assays of the dangling and hardwiring scans, run through `TAuditContractWalker.TAuditRun`.
The source sits at `TAssayTruth.TAssayDriverPath` and is handed through the binder seam, so no file is written.
Each dangling source holds its own `QContract` stand-in, so a call inside, nested in or outside it can be assayed.
Most assays hand no markup, so no ID is declared and every constant ID outside `QContract` is a hit.
The `UndeclaredId` pair hands one markup text under a virtual `.xaml` path in the Veneer folder.
The walker reads lines from the tree's text, so nothing is written to disk.
A forwarding call or constant ID inside `QContract` is skipped, since the outer call is judged where it is written.
The same call from another type, or from a type nested in `QContract`, is a hit.
A constant ID the handed surface declares as `x:Name` is no hit.
A line holding a pack marker is a hit, read from the handed text rather than from disk.
A `LoadComponent` URI built from a run-time string is a hit, since it can hide a marker.
A URI built from a string literal is left to the line scan.
A check holds path, line, member, kind and reason against the expected hit, or against none.

## `TAuditContractWalker`

Measures how far Deportment stands from the master it must become.
A master never names the Veneer, holds no scaffold, and pulls each part by an ID the Veneer declares.
It counts `Hardwiring`, `Masquerading` and `Dangling` hits for the strict ledger.
It reads the contract IDs from the surface markup, then walks every driver source.
The text comes through `TAuditBinder.TAuditMarkupRead`, so an assay can hand markup like source.
A markup file that does not parse is skipped here and reported by the reach walk.
The hardwiring scan reads the raw lines of each tree's text, and the other three scans read the bound syntax.
Lines come from the parsed text, never from disk, so an assay source is read like a tracked file.
Hardwiring is one hit per line holding a pack marker, named after the first marker found.
String literals are read too, since a pack URI is written as one.
One hit per `Application.LoadComponent` URI argument not built from a string literal is hardwiring too.
A literal is left to the line scan, which sees any pack marker in it.
A URI built at run time can hide its marker from that line scan.
Masquerading is one hit per type declaration whose base types include a scaffold type.
The walk stops at the first scaffold base, so a window counts once.
Dangling finds every call on the contract type and reads each string argument as an ID.
An ID that is not a constant, or that the markup never declares, is one hit.
A call whose nearest enclosing type declaration is the contract type itself is skipped.
Such a call only forwards its caller's ID, and that outer call is judged where it is written.
The skip compares declared symbols, never method names or forwarded parameters.
So a caller elsewhere that forwards its own parameter into the contract type is still a hit.

## `TAuditHostWalker`

Holds Host to construction and wiring, since Host holds no behaviour.
A statement passes as a declaration, an expression statement, a block-bodied local function or a closing `return`.
An expression passes as a name, a literal, a member chain, a call or a construction.
An assignment, an `await` and a lambda pass too.
Anything else is an overworking hit on its line, with every statement nested in it.
One hit is reported per line.
The scan covers the top-level statements and every member body of one tree.
An expression body counts for a method, a property and an indexer alike.
A property accessor with a body is behaviour and counts whole.
A `return` passes only as the block's last statement, so an early return is a hit.
A branch, an operator, a pattern, a cast or a conditional access is a hit.
A `ref`, `in` or `out` argument is a hit.

## `TAuditReachWalker`

Parses the surface markup as XML and looks at every element, attribute and text node.
A name below the driver is an overreaching hit, and a trigger or a computing binding is a branching hit.
A line through which logic reaches the markup is one tethering hit, whatever the layer it reaches.
A name that starts with `L` and a capital is a logic name, wherever it stands in a value.
A name only the Deportment namespace declares is the driver's own and is not overreaching.
A binding without a logic name, such as `{Binding}`, raises no overreaching hit.
The value scan reports only names the logic patterns match.
A file that does not parse is itself a hit, since a walk that cannot read it sees nothing.
The tethering markers of a file are gathered by line and reported one hit per line, markers joined.
An element line is marked for a binding element, a tethering slot property element, or a converter or selector.
An attribute line is marked for a tethering slot or a listed extension.
It is also marked for an extension whose prefix maps a code namespace.
A `Setter` whose `Property` names a tethering slot marks its line with that slot.
A literal value of a tethering-literal attribute, or of a `Setter` naming one, marks its line too.
An `x:Class` naming a type outside the surface namespace marks its line.
An `x:Class` naming a surface type is the page's shell and is allowed.
An element resolves to a type through its `clr-namespace` or the namespaces its XML namespace defines.
It is tethering when that type or a base holds a listed type among itself and its interfaces.
A property element is never a type and is skipped.
The XML namespaces come from the `XmlnsDefinitionAttribute` of every reference and of the source itself.
The control read lists every `x:Name` or `Name` value the markup declares.
A file that does not parse is skipped there and reported by the reach walk.
The branching scan runs first, then every attribute but `x:Class` and every text node of one element.
A namespace declaration goes to the namespace scan, the rest to the value scan.
A trigger element is one hit.
Each computing slot in an attribute, a value or a property element is one more.
An `xmlns` mapping a logic namespace is one hit.
A logic constant read is one hit.
So is each other logic name in the value, with the slot named.
A name found in the Deportment set is skipped.

## `TAuditStrict`

These are the surface rules.
A surface is what the user sees, and a surface member only calls a function.
A surface holds no state, never branches or computes, and names only its driver.
The rules hold the Veneer and UITerminal alike, and Host is held to construction and wiring.
A surface type holds no member but a constructor, and its markup holds no hook into logic.
Each fact holds one hit kind against the ledger.
While `TAuditStrictEnforced` is false every fact passes and reports as a warning.
Enforced, a kind fails in any file whose count stands above its ledger ceiling.
The ledger keys each ceiling by file, so each medium's lag is its own.
A new file starts at zero.
The walk runs once and every fact reads the same hits, surface types and walked sources.
The report is written once, on the first hit-kind fact that runs.
A surface field, event field, auto-property or primary constructor parameter is hoarding.
A constant is no state and is left out.
A `DependencyProperty` or `RoutedEvent` static counts, since the surface allows no exception.
A mutable static field in a driver type is state shared past any one gate.
A surface line that is not a plain call is meddling.
That includes a branch, a loop, an operator, an assignment or a declaration.
A LINQ query is a breach too, since a filter decides what is drawn.
Every line inside a breaching statement counts, so logic added inside one moves the count.
A surface line naming a type from below the driver, Conduct's included, is prying.
It overlaps the border audit's `Undercutting` count, which counts the same names per ring pair.
A markup line is overreaching when it maps a namespace below the driver.
It is also overreaching when it reads a constant from there or names a member there.
A markup trigger or visual state, or a binding slot that converts, formats, selects or validates, is branching.
Each is a branch or a computation standing in the surface.
A markup line through which logic reaches the markup is tethering.
That covers a binding, a command, an input binding or a code reach.
A code reach is `x:Static` or a markup extension whose prefix maps a code namespace.
A command parameter or target, a display member path or a selected value path is tethering too.
So is a literal `Tag`, since code reads it as a value to branch on.
Each counts as an attribute or as the property a `Setter` names.
A converter or a template or style selector declared in the markup is tethering too.
Each line counts once, however many markers it holds.
`{StaticResource}`, `{DynamicResource}` and `{x:Type}` stay allowed.
A surface member that is not a constructor is freelancing, whether a method, property, event, indexer or operator.
The shell only constructs, so every handler or helper belongs in Deportment.
An event attribute in markup therefore shows as the missing handler it needs.
Generated code is left out, as for every surface fact.
A surface identifier holding a character outside ASCII is a homoglyph.
A look-alike glyph would let a name pass every prefix rule while reading as another.
A Host line that does more than construct and wire is overworking.
That means a branch, a choice, an operator or an early return.
Host holds no behaviour, so each such line belongs in the layer that owns the decision.
A driver line holding a pack URI or naming the Veneer assembly is hardwiring.
Deportment pulls every part by contract ID and never names the Veneer.
The line counts once, under the first marker it holds.
A driver type deriving from a scaffold type, such as a window, a page or a resource dictionary, is masquerading.
The Veneer holds the scaffold, so such a type is scaffold standing in the wrong layer.
Each partial part counts in its own file.
A contract ID passed to `QContract` that no `x:Name` or `x:Key` in the surface markup declares is dangling.
An ID that is not a constant counts too, since no markup can be checked against it.
Every string argument of a `QContract` call is an ID.
A driver line that names a file system type, outside the exempt files, is disk work.
A driver holds only what its medium needs, and disk work stays behind the engine's ports.
The console's reader and writer live in `System.IO` too, so the namespace alone is no hit.
A surface line is catalog work when it reads a file, parses JSON or runs a regex.
Starting a process or a task is catalog work too.
Each of those is work the engine or `LUsher` does, and the surface only asks for the answer.
A file whose stream use is the framework's own may be exempt by name.
The disk and catalog facts assert zero hits and keep no ledger.
Their source scan blanks string literals and line comments first, so a word inside a message is not a hit.
The core read of `-flag` returns within five seconds instead of looping on the operator.
A branch in an expression-bodied indexer or local function is a host breach.
Every exempt file still holds a line its exemption spares, so a stale exemption is removed.
Every ledger ceiling equals its count, so a ceiling left above the count is stale.
The lowered ledger is written under `temp/audit` to copy over the tracked one.
Every tracked surface and host source must reach the walkers, so a file the binder dropped does not pass unseen.
The UI sources, host sources, surface markup and driver sources are enumerated with Git.
It fails when the UI or host set is empty, since the audit would then pass vacuously.
An empty markup set passes, since the Veneer gets its markup back one batch at a time.
The four walkers run once, with paths made repo-relative.
The contract walker reads the driver sources and the surface markup together.
The report opens with the counts, then a per-type table, then every hit by kind.

## `TAuditStrictCondition`

Shared syntax predicates on conditions, patterns and their operands.
The strict audit, the moonlighting and laundering walkers and the truth sink read them.
So no single walker owns them.
The core of a condition is unwrapped from parentheses and negation.
Any other prefix operator stops the unwrapping and is returned as the core.
A pattern passes for a null constant, its negation, an empty property pattern or a type pattern.
A declaration or `var` pattern that only names the value also passes.
A null check is the `null` or `default` literal.
A node is data when any name inside it resolves below Conduct, by its symbol or by its type.
A Conduct value is the driver's to reshape, so it is not data here.

## `TAuditStrictLedger`

The surface audit's ceilings, by kind and then by repo-relative file.
Every kind the surface audit counts reads its ceilings from this one ledger.
Every kind is empty now, so every file stands at zero.
A kind the ledger omits, such as `Sharing`, also has ceilings of zero.
Each ceiling is the count of that kind in that file, and may only fall.
A file missing from a kind has a ceiling of zero, so a new UITerminal file starts clean.
`TAuditRatchet` holds every ceiling against the committed copy, and a new generation baselines it afresh.
When a count falls, the ledger fact writes the lowered ledger under `temp/audit` to copy here.

## `TAuditStrictMeddling`

Judges each member of a surface type against the one rule that only a call may stand.
`TAuditStrictWalker` hands it every member of a surface part.
It walks bodies down to their operands and collects every breach.
One hit is kept per line of a surface member that is not a plain call.
Every statement nested in a breach is a breach of its own line, so a long branch counts in full.
The hit names the syntax kind that broke the rule.
The bodies of a member are its block, arrow, accessor bodies and a constructor initializer's arguments.
A field or property initializer is a body too, so a lambda there cannot hide its logic.
A block must hold only call statements, and an arrow, lambda body or initializer must be one call.
A statement passes as a call statement or as a `return` of a call.
Any other statement is a breach, whether a branch, a loop, a declaration or a `try`.
The expression must be a call on a name or a plain member chain, with plain arguments.
A call to a method of a query type is a breach.
A `ref`, `in` or `out` argument is a breach.
A plain operand is a name, `this`, `base`, a predefined type, a literal or a member chain.
A call or a lambda is plain too, and a lambda is judged by its own body.
An operator, an assignment, a `new`, a cast, an `await` or a `?.` is a breach.

## `TAuditStrictSetting`

Hand-written and tracked.
The surface-audit switch, the report path and the classifiers live here.
The per-file ceilings live in the ledger named by `TAuditLedgerFile`.
No script writes this file.
The enforced switch false makes every strict fact a warning that passes.
True fails a fact on any file whose count of a kind stands above its ledger ceiling.
The report path carries the version, and `temp` is ignored by Git, so a run never dirties the tree.
The Veneer include patterns cover the Veneer and UITerminal both.
A type declared under one of them is a surface type, whatever its base.
The Deportment include patterns are scanned for a reach into the file system.
The host include patterns are held to construction and wiring.
The Deportment namespace holds the types and members a markup binding may name.
The driver is the surface's path.
A name a type below the cut also declares is left out.
So a deeper binding cannot pass by coincidence.
An `x:Class` in the surface markup naming a type outside the Veneer namespace is tethering.
`QContract` is Deportment's only door to the scaffold.
Every string argument of a call on it is a contract ID.
The hardwiring markers match as plain text, string literals included, since a pack URI is a literal.
A driver type with one of the masquerading types among its base types is masquerading.
The contract ID attributes are read in any XML namespace.
`Name` covers `x:Name` and the framework's own `Name`.
The query types are those whose extension methods are queries, each a decision a surface may not make.
A catalog pattern marks file, JSON, regex or process work, or a started task, in the surface.
That work belongs below the UI, in the engine or in an `LUsher` adapter.
The catalog exempt list is empty, since the exempt files moved to Deportment in the Great Purge.
The disk patterns name the types, so the console's `TextReader` and `TextWriter` pass.
The disk exempt list is empty, since no driver file names the file system.
The overreaching namespaces are those below the driver that markup may not map.
A mapping lets a binding reach them.
The branching slots convert, format, fall back, select a template or style, or validate.
Each is a computation in the markup, whether set as an attribute, inside an extension or as a property element.
The tethering slots tether logic as an attribute, a property element or through a `Setter`.
The command slots, `DisplayMemberPath` and `SelectedValuePath` all name what code reads.
The tethering literals tether only when set to a plain literal, directly or through a `Setter`.
`Tag` is one, since code branches on a literal tag.
A value opening a markup extension is left to the extension rules.
The script reads the same list from `tetheringLiterals` in `AuditUI.json`.
The tethering extensions bind to logic or read a code member.
A markup extension whose prefix maps a code namespace is tethering without being listed.
The tethering types are converter and selector types a markup element may not be.
An element is one when its type or a base type is listed or implements a listed interface.

## `TAuditStrictWalker`

Applies the surface rules to the UI sources by symbol.
A type is a surface by its folder alone, since a driver uses its medium freely.
Partial types are joined by symbol, and one surface part makes the whole type a surface.
Every type of the UI sources is grouped, then scanned for hoarding, meddling, prying and freelancing per surface type.
The meddling scan of each member lives in `TAuditStrictMeddling`.
Every surface file is also scanned for homoglyphs, enums and delegates.
Outside the surfaces only a mutable static field is a hit.
The surface type names come back for the report.
Enum and delegate declarations of a surface file are read separately, since no type scan reaches them.
A name they carry from below the driver is a prying hit.
An enum value that is not a literal is a computation.
Every field, event field, auto-property and primary constructor parameter of a surface type is hoarding.
A `readonly` or a wired `null!` field counts too, and a constant does not.
In a driver type, every mutable static field is a sharing hit.
One freelancing hit is kept per method, operator, property, event or indexer the surface part declares.
A constructor is the one member the shell keeps.
A property whose accessors have no body stores a value.
One prying hit is kept per line of a surface declaration naming a type from below the driver.
Nested types are left to their own scan.
One homoglyph hit is kept per identifier holding a character outside ASCII.
The surface walk and the driver walk each count it under their own audit.

## Stamp

Commit: `c6e60141a172`
- `TAssayContract.cs` `6d42d21454d111ed`
- `TAuditContractWalker.cs` `0da0456f202264dd`
- `TAuditHostWalker.cs` `2a48c5d2dab0f8e1`
- `TAuditReachWalker.cs` `e54546f894fc3f86`
- `TAuditStrict.cs` `bec8aee286d94f66`
- `TAuditStrictCondition.cs` `920bd6f4febc550a`
- `TAuditStrictLedger.json` `b964a1e8c46e04b7`
- `TAuditStrictMeddling.cs` `dd697aec78b53c98`
- `TAuditStrictSetting.cs` `d1b6fdf69ec8079b`
- `TAuditStrictWalker.cs` `723eb9e5bd2465ff`
