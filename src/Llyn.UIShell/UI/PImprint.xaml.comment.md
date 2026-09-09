# PImprint.xaml.cs

## `public partial class PImprint : UserControl`

The Source edit area as a control of its own, held by the Source panel.
It keeps the host, the engine, and the panel that owns it, and nothing else.
The stated fields live in `PImprintField.cs`, the credits in `PImprintAuthor.cs`, the held draft in `PImprintHold.cs`.
It reads the shelf's own answers through the panel, because the shelf is what holds them.

## `internal void PImprintAttach(PWindow host, LEngine engine, PReference owner)`

Binds the area to the window, the engine, and the panel it answers to.
It binds its own lists and reads nothing yet.
It subscribes to no announcement, because the panel is announced to and drives the area.

## `internal void PImprintDraftOpen(string? reference)`

Starts a held Source and fills the controls from it, which is never done apart.
Every caller wants both, so the pair is offered as one call.

## `internal void PImprintClear()`

Empties the controls, for when the panel has nothing selected.

## `internal void PImprintClose()`

Closes the popup the area owns, so none outlives the window.
