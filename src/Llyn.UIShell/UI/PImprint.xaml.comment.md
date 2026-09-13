# PImprint.xaml.cs

## `public partial class PImprint : UserControl`

The Source edit area as a control of its own, held by the Source panel.
It keeps the host, the engine, and the panel that owns it.
It keeps too the notice it raises when the held draft changes.
The notice is the panel's, as the entry editor's is, so the rail's save stays the panel's to settle.
The stated fields live in `PImprintField.cs`, the credits in `PImprintAuthor.cs`, the held draft in `PImprintHold.cs`.
It reads the shelf's own answers through the panel, because the shelf is what holds them.

## `public PImprint()`

Merges the dropdown row template, which is given the area so its click can reach it.
Binds the credit rows and the dropdown to their lists, and gives the dropdown its placement.

## `internal void PImprintAttach(PWindow host, LEngine engine, PReference owner)`

Binds the area to the window, the engine, and the panel it answers to.
It binds its own lists and reads nothing yet.
It subscribes to no announcement, because the panel is announced to and drives the area.

## `internal void PImprintDraftOpen(long? reference)`

Starts a held Source and fills the controls from it, which is never done apart.
Every caller wants both, so the pair is offered as one call.

## `internal void PImprintBulletinHandle(LBulletin bulletin)`

Takes the bulletins the owning panel receives, since this area holds no observer of its own.
A draft bulletin redraws the held draft when it is the one named.
Any other bulletin re-reads the author catalog, and an author bulletin redraws the credits too.
So a rename made elsewhere shows its new name in the credit rows.

## `internal void PImprintClear()`

Empties the controls, for when the panel has nothing selected.

## `internal void PImprintClose()`

Closes the popups the area owns, so none outlives the window.
