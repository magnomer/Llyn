# TEditorOpen.cs

## `public sealed class TEditorOpen`

Covers the editor spine over its desk.
An open by id fills the desk and reads the stored entry.
An open with null resets to a fresh draft.
A fresh draft is completed with one card of each kind and a sentence under each.
A store on the input tab reopens a blank draft, while a store elsewhere reopens what was stored.
A finish stores and reopens, a finish without store lets the tenure go, and a reset drops the typing.
