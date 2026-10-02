# TAssayContesting.cs

## `public sealed class TAssayContesting`

Assays of the contesting hit and the writers it lists.
The driver field is written once by the engine and then by the shell.
They run through `TAuditTruthWalker.TAuditRun` with the helpers of `TAssayTruth`.
With one member line, body line k is driver line 11 + k.
The origin pairs judge one rule each, as a hit beside a no-hit.
Each rule was switched off once, and its no-hit, or its hit for the markup rule, failed.
A failing assay exposes a walker bug, never a source to fix.

## `private const string TAssayMarkupPath = "src/Llyn.UIVeneer/obj/Assay/PAssayView.g.cs";`

Where the markup compiler would write the generated half, so the tracked tree never walks it.

## `private const string TAssayMarkupDriver = """`

A driver whose field copies a property of a surface type with markup.

## `private const string TAssayMarkupText = """`

The generated half of that surface, written as the markup compiler writes it.

## `public void AuditTruth_OnePlainWriter_ReportsContesting()`

One engine writer and one plain writer give a hit at the plain write, with no "also" text.

## `public void AuditTruth_TwoPlainWriters_ListsSecondWriter()`

A second plain writer is listed after the first as ", also at" its file and line.

## `public void AuditTruth_PlainRecord_ReportsContesting()`

A positional record property built from a literal is plain, so the copy contests.
The record reaches its reader through `Array.ForEach`, a method group, so the reader's parameter stays unseen.

## `public void AuditTruth_EngineRecord_AllowsEngineCopy()`

The same property built from an engine read is engine through its `new` argument.

## `public void AuditTruth_PlainProperty_ReportsContesting()`

An auto property assigned a literal is plain.

## `public void AuditTruth_EngineProperty_AllowsEngineCopy()`

The same property assigned an engine read is engine through the assignment.

## `public void AuditTruth_PlainHeld_ReportsContesting()`

A local started from a plain value is plain.

## `public void AuditTruth_EngineHeld_AllowsEngineCopy()`

A local started from an engine read is engine.

## `public void AuditTruth_PlainSetter_ReportsContesting()`

A setter's `value` takes the property's writers, here a plain one.

## `public void AuditTruth_EngineSetter_AllowsEngineCopy()`

With an engine writer of the property, the setter's write is engine.

## `public void AuditTruth_ContentLookup_ReportsContesting()`

One lookup fed an engine read and a plain non-constant input still contests.

## `public void AuditTruth_KeyLookup_AllowsEngineCopy()`

One lookup fed an engine read and a literal key does not contest.

## `public void AuditTruth_PlainConstructor_ReportsContesting()`

A constructor parameter takes its `new` arguments, here a literal.
The object reaches its reader through `Array.ForEach`, so only the constructor rule decides.

## `public void AuditTruth_EngineConstructor_AllowsEngineCopy()`

The same parameter fed an engine read is engine.

## `public void AuditTruth_CopyOnly_ReportsContesting()`

A field written only by a copy of itself stays plain.
It guards against an over-broad copy rule, so it passes with the rule switched off.

## `public void AuditTruth_CopyBeside_AllowsEngineCopy()`

A copy of itself beside an engine writer adds nothing, so the field is engine.

## `public void AuditTruth_PlainHelper_ReportsContesting()`

A `ref` hand-off writes the argument the helper assigns, here a plain one.

## `public void AuditTruth_EngineHelper_AllowsEngineCopy()`

The same hand-off of an engine read is engine, and the helper's own assignment is not counted.

## `public void AuditTruth_MarkupProperty_ReportsContesting()`

A public property of a type with generated markup has an unseen writer and is plain.

## `public void AuditTruth_CodeProperty_AllowsEngineCopy()`

Without the generated half the same property is engine through its C# writer.

## `private static List<TViolation> TAssayMarkupRun(bool markup)`

Walks the markup driver, with or without its generated half, and keeps the contesting hits.
