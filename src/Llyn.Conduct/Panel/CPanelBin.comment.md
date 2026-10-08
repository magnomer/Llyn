# CPanelBin.cs
Hash: `0d2b96a54f76e6d4`

## `public sealed class CPanelBin`

The delete request of one browsing panel, composed by `CPanel`.
It reads the chosen row off the panel's `CAperture`, so it holds no vista of its own.
Every question it puts to the user leaves through `CEnvoy`, so no dialog is known here.

## `private readonly LSettingsPort _cPanelBinSettings;`

The port a failed delete reads its ready notice through, before the envoy shows it.

## `private readonly LVistaPort _cPanelBinVistas;`

The port the bin counts and deletes the vista's chosen row through.

## `private readonly string? _cPanelBinScope;`

The localization scope the delete question, its tally and its failure are worded under.
Null says the panel never deletes, as a list that only points at entries.

## `internal CPanelBin(CEnvoy envoy, LSettingsPort settings, LVistaPort vistas, CAperture aperture, string? scope)`

Only the scope may be null, since a list without one simply never deletes.

## `public event Action? CPanelBinDeleted;`

A delete went through, so the owning panel closes what it showed.
`CPanel` wires it to `CPanelEntryClose` when it builds the bin.

## `public void CPanelBinDelete()`

The bin request.
A panel without a delete scope or without a vista does nothing.
The question names how many places the delete reaches, which the vista counts.
A refused delete is told through the envoy, and a done one raises `CPanelBinDeleted`.

## `private bool LPanelBinConfirm(string scope, int usage)`

A record nothing references is a plain question.
One something references is asked with its tally, because the delete drops those references too.
