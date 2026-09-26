# auditbinder.cs

The one compilation every bound audit script shares, the script twin of the convention test's `TAuditBinder`.
auditstructure, auditobject, auditfake and auditplatform copy it into their helper beside their own program.
A helper hashes its text into the cache key, so an edit rebuilds every helper once.

## Configuration

`auditbinder.json` names the source root, the build configuration, the host project and the shared frameworks.
Its exclusions match the name audit settings the convention tests enumerate with.
`TAuditParity` fails when a value drifts from the test settings.

## Binding

The sources are the tracked and untracked `.cs` files under the source root, never ignored ones.
The generated `.g.cs` files of every project join them, `_wpftmp` copies left out.
A project that holds markup but has no generated folder fails, since the solution was not built.
The references are the shared frameworks beside the helper runtime and the host build output, own assemblies left out.
A compile error fails the bind, so an unbound name can never slip past an audit.

## Trees

`Tracked` holds the trees under the source root outside any `obj` folder, the files an audit walks.
The generated trees stay in the compilation, so a partial type binds whole.
