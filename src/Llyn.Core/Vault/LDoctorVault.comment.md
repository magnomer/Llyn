# LDoctorVault.cs

## `public interface LDoctorVault`

The port for the last-resort recovery of the workspace store.
`LDoctor` in Infrastructure is its adapter over the workspace database.
The engine asks once per workspace and learns only whether a rescue happened.

## `LDoctorRescue LDoctorDatabaseCreate();`

Opens or creates the workspace store, migrating it to the current shape.
A store this build can no longer read is moved aside and a clean one started in its place.
Returns the healthy rescue, or one naming the file moved aside and why.
