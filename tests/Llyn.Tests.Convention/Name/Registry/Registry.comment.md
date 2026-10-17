# Registry

This folder holds every generated registry the name audit compiles against, and nothing hand-written.
Each file is generated from the registered names and must never be edited by hand.
No generated file outgrows the line limit, so the bases, verbs and exemptions are split over several files.
Every file carries the generated-file header, so the comment audit exempts all of them.

## `TAuditExemptRegistry`

This exemption registry holds the exemptions apart from the bases and the verbs.
An entry too wide for one line keeps its key once and wraps its file list below it.

## `TAuditLogicRegistry`

This base registry holds the bases whose kind starts with `Logic`.
It carries the generation the convention compares with the setting generations.

## `TAuditTestRegistry`

This base registry holds the bases whose kind starts with `Test`.
It carries the generation the convention compares with the setting generations.

## `TAuditUIRegistry`

This base registry holds the bases whose kind starts with `UI`, the UI and logic kinds included.
It carries the generation the convention compares with the setting generations.

## `TAuditVerbRegistry`

This verb registry holds the verbs apart from the bases and the exemptions.
