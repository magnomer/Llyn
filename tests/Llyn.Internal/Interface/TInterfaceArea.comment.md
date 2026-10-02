# TInterfaceArea.cs
Hash: `bf5825a6e7d0f03b`

## `internal static class TInterfaceArea`

The relays for each panel area's own vista restore.
An area restores its vistas when it is built, and a workspace change runs the restore again.
A test calls a relay to stand in for that change, without moving the workspace.
Each relay is transparent and carries no test logic of its own.

## `internal static void TCorpusVistaRestore(this CCorpus corpus)`

Relays to the area's internal vista restore, as a workspace change does.

## `internal static void TFavoriteVistaRestore(this CFavorite favorite)`

Relays to the area's internal vista restore, as a workspace change does.

## `internal static void TGuildVistaRestore(this CGuild guild)`

Relays to the area's internal vista restore, as a workspace change does.

## `internal static void TLibraryVistaRestore(this CLibrary library)`

Relays to the area's internal vista restore, as a workspace change does.

## `internal static void TPhonologyVistaRestore(this CPhonology phonology)`

Relays to the area's internal vista restore, as a workspace change does.

## `internal static void TRepertoireVistaRestore(this CRepertoire repertoire)`

Relays to the area's internal vista restore, as a workspace change does.

## `internal static void TShelfVistaRestore(this CShelf shelf)`

Relays to the area's internal vista restore, as a workspace change does.

## `internal static void TTaxonomyVistaRestore(this CTaxonomy taxonomy)`

Relays to the area's internal vista restore, as a workspace change does.

## `internal static void TTenorVistaRestore(this CTenor tenor)`

Relays to the area's internal vista restore, as a workspace change does.

## `internal static void TXieshengVistaRestore(this CXiesheng xiesheng)`

Relays to the area's internal vista restore, as a workspace change does.

## `internal static void TYunjingVistaRestore(this CYunjing yunjing)`

Relays to the area's internal vista restore, as a workspace change does.
