# Module

## TAuditModule.cs

The audit holds authoritative module findings against independent ceilings.
It writes the convention report.
Missing ceilings mean zero.
A written ceiling below the observed count is stale.

## TAuditModuleSetting.cs

Settings are a hand-maintained convention copy.
They do not read the script configuration.
Empty modules let the enforcement machinery land before semantic module ownership is announced.

## TAuditModuleWalker.cs

Configuration resolves completely before any finding is counted.
Explicit file membership defines ownership.
Roslyn symbols decide references and exposed type shapes.

`Bypassing` is one finding per source span and referenced symbol.
`Exposing` is one finding per entrance member and leaked top-level type.
`Cycling` is one finding per canonical strongly connected component.

## TAuditModuleType.cs

Nested declarations belong to their top-level type.
Entrance and contract symbols are the only names an announced module permits from outside.

Type-shape traversal unwraps generic arguments, tuples, arrays, pointers, function pointers, and constraints.
A visited-symbol set stops recursive shapes from multiplying findings.

## TAuditModuleFinding.cs

Finding keys use ordinal project, module, path, span, and symbol text.
Cycle keys use ordinally sorted module names.

## TAuditModuleRow.cs

Each row announces one project-local module, one entrance, explicit files, and optional contracts.

## TAssayModule.cs

Assays pin these cases.
  Clean controls, binding forms, exposed shapes, and cycle ordering.
  Configuration refusal, incremental legacy scope, and known semantic limits.

Runtime authority, retained aliases, transfer relinquishment, call order, completion ownership, and replacement safety are not mechanically proved.
Graft call-cohesion suggestions exist only on the script side.
They never gate convention results.

## Stamp

Commit: `1f3467bbe5c6`
- `TAuditModule.cs` `297e89b02202bab4`
- `TAuditModuleFinding.cs` `7963e83eab935074`
- `TAuditModuleRow.cs` `679e7b535fa1c103`
- `TAuditModuleSetting.cs` `d8ce60c3a5d04d05`
- `TAuditModuleType.cs` `e6a5b73011bf4738`
- `TAuditModuleWalker.cs` `609413d301c56f1f`
