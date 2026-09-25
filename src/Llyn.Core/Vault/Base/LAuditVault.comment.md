# LAuditVault.cs

## `public interface LAuditVault`

The port for the fault log, where the engine records an exception it could not act on.
`LAuditWriter` in Infrastructure is its adapter over `audit.log` inside the workspace.

## `string? LAuditRecord(Exception exception);`

Appends `exception` with the instant it was recorded.
Returns where the log lives, or `null` when it could not be written.
