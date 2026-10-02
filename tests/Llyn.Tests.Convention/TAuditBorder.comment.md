# TAuditBorder.cs
Hash: `08e3733dc78abcaf`

## `public sealed class TAuditBorder`

Keeps every ring inside its border: which ring a ring may name.
A ring names its one neighbour, ferries the data of any deeper ring and names nothing outward.
Above the cut, a UI ring names only its neighbour, data included.
A pair is counted in names, so a file already over its border cannot add names unseen.
The same border is audited by `auditstructure.ps1` from its own configuration, and neither reads the other.

## `private const string TAuditBorderAudit = "AUDITBORDER";`

The audit name every report line opens with.

## `private static readonly string[] TAuditBorderHeld`

The kinds a ceiling is written for.

## `private static readonly Lazy<IReadOnlyList<TAuditHit>> TAuditBorderHits`

The tree hits, the offer hits and the seal hits, bound once and shared by every fact.

## `public void AuditBorder_Rings_NameOneNeighbour()`

No ring names more than one neighbour or itself, and every neighbour named is declared.
A capsule must be declared, have no neighbour and belong to one ring alone.

## `public void AuditBorder_Cut_MatchesShellRoots()`

The cut holds exactly the shell and capsule rings.

## `public void AuditBorder_Sources_HoldNoLeapfrogging()`

No pair holds more `Leapfrogging` names, behaviour past the neighbour, than its ceiling.
A method called on a deeper record is behaviour, even though the record is data.

## `public void AuditBorder_Sources_HoldNoTrespassing()`

No pair holds more `Trespassing` names, from a ring outside its closure, than its ceiling.

## `public void AuditBorder_Sources_HoldNoUndercutting()`

No pair holds more `Undercutting` names a UI ring takes from below the cut than its ceiling.
A `using` of a namespace below the cut counts as a name.

## `public void AuditBorder_OfferSignatures_HoldNoLeaking()`

No pair holds more `Leaking` offer signature types from below Conduct than its ceiling.
Methods, operators, conversions, properties, events, fields and nested public types are all read.

## `public void AuditBorder_SealedTypes_HoldNoUnsealing()`

No pair holds more `Unsealing` sealed member types from below the neighbour than its ceiling.
The ceilings fall as each job seals a controller, until none is left to count.

## `public void AuditBorder_Sources_HoldNoPoaching()`

Where an offer is written for a pair, no `Commuting` name outside it is named.
A neighbour name outside the offer is `Poaching`.

## `public void AuditBorder_Ceiling_MatchesHits()`

No ceiling sits above its count, so a shed name lowers its ceiling.

## `public void AuditBorder_Exempt_MatchesSource()`

Every exempt row still matches a hit, so a fixed break deletes its row.
