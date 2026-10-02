# QContract.cs

## `public static class QContract`

The deportment's only door to the scaffold the veneer holds.
Each part is pulled by its contract ID alone, so the deportment never names the veneer.
A missing part throws at once, since a broken contract must not fail silently later.

## `public static QContractPart QContractFind<QContractPart>(DependencyObject scope, string id)`

The element under `scope` that carries `id` as its `x:Name`.
The name scope of `scope` is asked first.
The logical tree is walked next, since a nested page keeps its names in its own scope.

## `public static QContractSheet QContractSheetFind<QContractSheet>(string id)`

The application resource keyed `id`.
A page marked `x:Shared="False"` comes back as a fresh instance on every call.

## `public static QContractSheet QContractSheetFind<QContractSheet>(FrameworkElement scope, string id)`

The resource keyed `id` that `scope` sees, walking up through its parents.
A page-local dictionary merged by its markup is found this way, since it is not an application resource.

## `private static QContractPart QContractResolve<QContractPart>(object? found, string id, string kind)`

The one rule every find shares: the part found must exist and carry the asked type.
Otherwise it throws, naming the contract `kind`, the ID and the asked type.
`kind` is "element" for a named element and "resource" for a keyed resource.
