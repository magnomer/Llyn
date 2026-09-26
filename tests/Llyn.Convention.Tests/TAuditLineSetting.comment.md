# TAuditLineSetting.cs

## `internal static class TAuditLineSetting`

Hand-written and tracked: the line-audit thresholds, ceilings and scope live here, not in a generated sidecar.
`TAuditLineLimit` is the last line count that passes and `TAuditLineWarning` the last that passes silently.
`TAuditWidthLimit` maps an extension to the widest line it allows, and `TAuditWidthBand` sets the warning band under it.
`TAuditLineCeiling` counts how many hits of each kind may stand, and the ratchet holds it from rising.
The ratchet holds every other field here from changing without a commit.
auditlines.ps1 reads its own tracked auditlines.json and never writes this file.
The audits read no script configuration, and both copies change together as scripts/principles.md asks.
