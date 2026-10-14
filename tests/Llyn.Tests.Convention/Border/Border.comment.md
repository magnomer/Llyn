# Border

This folder holds the border audit keeping every ring to the rings it may name.
It also holds drift, linger, the walker, the driver-pair settings and the shared ceiling arithmetic.

## `TAssayBorder`

Assays of the border walker's linger scan, run through `TAuditBorderLinger.TAuditLingerScan`.
The stand-in port sits under ShellEngine so its event lies below Conduct.
The subscribing holder sits under Conduct so the linger scan reads it.
The holder adds the handler in one method and writes the detach line in another.
The `+=` always sits on line 17, so every positive assay expects that line.
A method handler added with no removal anywhere in its type is one hit at the `+=` line.
A detach method removing the same event and handler pairs the subscription, so no hit.
A removal of a different handler leaves the subscription unpaired, so it stays one hit.
A lambda handler is one hit even beside a removal, since no `-=` can name it.
An event declared on another Conduct type is not guarded, so its unpaired subscription gives no hit.

## `TAuditBorder`

Keeps every ring inside its border, meaning which ring a ring may name.
A ring names its one neighbour, ferries the data of any deeper ring and names nothing outward.
Above the cut, a UI ring names only its neighbour, data included.
A pair is counted in names, so a file already over its border cannot add names unseen.
The same border is audited by `AuditStructure.ps1` from its own configuration, and neither reads the other.
No ring names more than one neighbour or itself, and every neighbour named is declared.
A capsule must be declared, have no neighbour and belong to one ring alone.
The cut holds exactly the shell and capsule rings.
Each of the following kinds has a ceiling per pair and fails above it.
  - `Leapfrogging` is behaviour past the neighbour, and a method called on a deeper record counts as behaviour.
  - `Trespassing` is a name from a ring outside the pair's closure.
  - `Undercutting` is a name a UI ring takes from below the cut, and a `using` of such a namespace counts.
  - `Leaking` is an offer signature type from below Conduct.
    Methods, operators, conversions, properties, events, fields and nested public types are all read.
  - `Unsealing` is a sealed member type from below the neighbour, and the ceilings fall as each controller is sealed.
`Poaching` is a neighbour name outside the offer where an offer is written for the pair.
`Poaching`, `Drifting` and `Lingering` are hard, with no ceiling and no ledger.
A drift hit names the pair, the entry or missing type, and the clause it failed.
A linger hit names the subscribing type, the event and the handler.
An exempt row clears a hit as it clears every other border hit.
A Conduct type that leaves an engine event handler attached with no matching removal is `Lingering`.
An engine-lifetime source outlives the Conduct object, so a kept handler keeps the object alive and firing.
A drift specimen proves the comparison fires on a stale, an internal, a wrong-prefix and a missing name.
Each break gives exactly one hit, so no clause reports an entry another clause already caught.
Two cut pairs whose lists equal the neighbour's public types give no hit.
An offer is compared as a set.
No ceiling sits above its count, so a shed name lowers its ceiling.
Every exempt row still matches a hit, so a fixed break deletes its row.

## `TAuditBorderDrift`

Holds each offer list to the public types its neighbour ring really declares.
A list that drifts from the code is a `Drifting` hit.
`TAuditDriftRead` is the pure comparison and binds nothing, so a specimen can feed it a hand-written list.
Every hit sits at the neighbour folder with line zero, as the script places it.
Clause (a) holds every entry to exactly one public type of the neighbour carrying its prefix.
Each entry reports only its first failing check, so one broken entry gives one hit.
Clause (b) holds a cut pair to every public type of its neighbour.
It reports only the unlisted types, since (a) already catches every listed type that is not public there.
Clause (c) holds pairs sharing a neighbour to one list, reporting each entry the other pair adds.
The engine pair sits outside the cut, so it skips (b) and may omit engine types.
A type is public only when every type containing it is public, so it is reachable from outside.
`TAuditPublicRead` is internal because the offer and seal scans of `TAuditBorderWalker` walk members through it.

## `TAuditBorderLinger`

Finds Conduct subscriptions to events of deeper rings that are never removed.
Such a subscription keeps the Conduct type alive as long as the deeper type lives.
A `+=` is guarded when the event's declaring type sits in a ring below Conduct.
The ring comes from the declaring type's source path, never its name prefix, so L-named Conduct events stay unguarded.
A guarded `+=` is a `Lingering` hit unless its type removes the same event with the same handler.
The removal may sit in any partial part and any method, and its receiver need not match.
A lambda or anonymous method handler is always a hit, since no `-=` can name it.
A type subscribing to its own event is skipped.
One hit is kept per line and name.

## `TAuditBorderSetting`

Hand-written and tracked settings for the ring structure of the border.
No script writes this file, and the border fact reads no script configuration.
`AuditStructure.json` holds its own copy, kept in step by hand as scripts/principles.md asks.
The neighbours and the cut are tied to the charter and the UI roots by facts, not by hand.
The offers each pair may name live in `TAuditEngineSetting`, `TAuditDeportmentSetting` and `TAuditDemeanorSetting`.
The host project names every project, so it is no ring of the border.
Each ring names its neighbour alone and ferries the data of every ring inside it.
The two adapters name `Llyn.Core` as spokes, since the contracts they implement live there.
The core and the capsule name no neighbour.
A capsule belongs to one ring alone, and a ring names at most one capsule beside its neighbour.
The cut rings name only their neighbour and their capsule.
Offer prefixes say which prefix every offered type must carry, Conduct offering `C` types and the engine `L` types.
A neighbour missing from that map fails every entry offered from it, so a new pair needs its row.
The script holds the same map as `offerPrefixes`.
The unsealing rings have every public top-level type a sealed controller, whatever its prefix.
Their public members may name no type from below the ring's neighbour.
Deportment is sealed whole, the medium-free controllers that later sink into Conduct.
The script holds that list as `unsealingRings`.
A ceiling is the name count a `Kind:Ring>Target` pair may hold.
A count above fails the fact, and a ceiling above the count is stale and fails too.
Lower a ceiling when a ring sheds a name, never raise one to admit a new one.
Every `Undercutting`, `Unsealing` and `Leaking` ceiling is zero, since no driver reads a Core or engine type through Conduct.
An exempt row is `path:name` and breaks the border today, one row per break.
A path ending in `/*` exempts a whole folder for one name.
Every row must still match a hit, so a fixed break deletes its row.

## `TAuditBorderWalker`

Reads, for every type one ring names from another, which ring declares it and whether it is data.
The verdict comes from the binder, not from the position of the name in the line.
Each tree of a ring is scanned on its own thread, and hits come back ordered by file and line.
A name is `Commuting` for the neighbour the table names and for the ring's capsule.
Past the neighbour it is `Trespassing` beyond the closure, `Ferrying` for data and `Leapfrogging` for behaviour.
A method called on a data type counts as behaviour, not data.
In a cut ring, a name from outside the UI rings is `Undercutting` ahead of `Ferrying` and `Leapfrogging`.
A `using` of a deeper ring counts as data.
It is `Undercutting` from a cut ring to an uncut one and `Ferrying` otherwise.
One hit is kept per line, kind, target and name, so a name used twice on a line counts once.
The closure of a ring is its neighbour, that neighbour's neighbour, and so on.
The linger scan reads that closure too.
The offer scan walks every public member of each offer type written for a cut pair, nested public types included.
Each signature type from a ring below the neighbour is a `Leaking` hit at the member line.
The seal scan does the same for each public top-level type of an unsealing ring, whatever its prefix, as `Unsealing`.
The seal scan skips a ring that names more or fewer than one neighbour.
Such a ring has no single neighbour.
A signature reads array elements and type arguments through.
A property accessor is covered by its property.
A namespace maps to the ring whose name is its longest prefix, or to none.

## `TAuditDemeanorSetting`

Hand-written and tracked settings for the Demeanor driver pair of the border.
It holds the offer for `Llyn.UIDemeanor>Llyn.Conduct` alone.
No script writes this file.
`AuditStructure.json` holds its own copy, kept in step by hand as scripts/principles.md asks.
The border facts read it together with the other offer settings.
Demeanor is offered the public `C` types of Conduct, and no other Conduct type.
`TAuditDeportmentOffer` holds the same list in the same order for the other driver.
A change to the public `C` types of Conduct is made in both driver lists.
Each name stands as a literal, so the ratchet can read every entry.

## `TAuditDeportmentSetting`

Hand-written and tracked settings for the Deportment driver pair of the border.
It holds the offer for `Llyn.UIDeportment>Llyn.Conduct` alone.
No script writes this file.
`AuditStructure.json` holds its own copy, kept in step by hand as scripts/principles.md asks.
The border facts read it together with the other offer settings.
Deportment is offered the public `C` types of Conduct, and no other Conduct type.
`TAuditDemeanorOffer` holds the same list in the same order for the other driver.
A change to the public `C` types of Conduct is made in both driver lists.
Each name stands as a literal, so the ratchet can read every entry.

## `TAuditEngineSetting`

Hand-written and tracked settings for the engine pair of the border.
It holds the offer for `Llyn.Conduct>Llyn.ShellEngine` alone.
No script writes this file.
`AuditStructure.json` holds its own copy, kept in step by hand as scripts/principles.md asks.
The border facts read it together with the other offer settings.
Conduct names ShellEngine through 28 `L*Port` interfaces, which ShellEngine declares, and 27 other `L` types.
Every other engine type is a helper Conduct may not name.
Each name stands as a literal, so the ratchet can read every entry.

## `TAuditPair`

The ceiling arithmetic the border, purity and misfiring audits share.
A pair is one kind from one ring to one target, counted in names.
Each pair is held to the ceiling written for it.
An unwritten ceiling is zero, and the hits of an over pair are listed under it.
The message counts the pairs above their ceiling, not the lines listed.
The stale check fails when a ceiling sits above its count, so a shed name lowers its ceiling.
The exempt check fails when an exempt row matches no hit, so a fixed break deletes its row.

## Stamp

Commit: `c6e60141a172`
- `TAssayBorder.cs` `7a21fc1ac676ed8f`
- `TAuditBorder.cs` `0027b680a1d1279d`
- `TAuditBorderDrift.cs` `80233353cf79d45a`
- `TAuditBorderLinger.cs` `e55f695a547e6456`
- `TAuditBorderSetting.cs` `3d8100483444c636`
- `TAuditBorderWalker.cs` `24d841dbc314714b`
- `TAuditDemeanorSetting.cs` `1a315fde18f2ae43`
- `TAuditDeportmentSetting.cs` `644a84d42896a939`
- `TAuditEngineSetting.cs` `e20f3f43dd884b28`
- `TAuditPair.cs` `cdbe97589a6f74fb`
