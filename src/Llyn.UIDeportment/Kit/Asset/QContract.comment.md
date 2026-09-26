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
