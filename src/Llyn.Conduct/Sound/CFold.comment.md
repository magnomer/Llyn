# CFold.cs
Hash: `3fa924a8083aac80`

## `public sealed class CFold`

The open state of the editor's rime-book and script boxes, which the user keeps across entries.
It belongs to the user, not to the entry, so it reads no desk and holds no entry.
It is its own area since the remembered state and its gates form one concern apart from the sound sheet.

## `private readonly LSettingsPort _cFoldSettingsPort;`

The settings port the open states are read from, saved through, and heard on.

## `private readonly CEnvoy _cFoldEnvoy;`

The editor's envoy, through which a failed save shows its notice.

## `private Action? _cFoldObserver;`

The fold handler the attach built, held so the same delegate can be removed.
Before the driver attaches it is null, so an early attach or detach does nothing.

## `internal CFold(LSettingsPort settings, CEnvoy envoy)`

Takes the settings port and the envoy the editor hands its sound sheet.
Only the editor builds one, so the constructor is internal.

## `public event Action? CFoldChanged;`

A fold gate landed here or in another editor, so every driver repaints the rime-book and script switches.
A closed editor hears only its own gates until it opens again, since its close takes the engine handler off.

## `internal void LFoldObserverAttach(Action<Action> marshal)`

Hears the settings port's `LEngineFoldChanged` and raises `CFoldChanged` on the driver's thread.
A fold toggled in another editor so repaints this editor's switches too.
On a real change the editor that toggled hears it twice, which repaints the same state.
The settings bulletin is not used, since it refills the whole draft and every panel.
The handler is kept in `_cFoldObserver`, so the editor's close can take it off the engine event.

## `internal void LFoldAttach()`

Puts the fold handler on the engine's `LEngineFoldChanged`, once however often it is called.
The editor calls it on every open, so a reopened editor hears the folds again.

## `internal void LFoldDetach()`

Takes the fold handler off the engine's `LEngineFoldChanged`.
The editor calls it on close, so a closed editor hears no other editor's fold.
The engine then lets the closed editor go.

## `public bool CFoldFanqieOpened`

Whether the user keeps the rime-book box open, read from the saved settings.
It holds across entries and restarts.

## `public bool CFoldScriptOpened`

Whether the user keeps the script box open, read from the saved settings like the rime-book state.

## `public void CFoldFanqieToggle(bool opened)`

The user opened or closed the rime-book box.
It saves the state through the settings port and raises `CFoldChanged`.
A failed save is shown, and the remembered state stays as saved.
A saved change also tells every other editor through `LEngineFoldChanged`.

## `public void CFoldScriptToggle(bool opened)`

The user opened or closed the script box.
It saves the state and raises `CFoldChanged`, as the rime-book toggle does.
A failed save is shown, and the remembered state stays as saved.
A saved change also tells every other editor through `LEngineFoldChanged`.
