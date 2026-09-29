# PRegisterTemplate.cs

## `public class PRegisterTemplate : ResourceDictionary`

The templates a card's Register field is drawn with: one chip, one caret, and the field holding both.
It is a dictionary rather than part of the card template because both card kinds show the same field.
A chip is drawn in the helper colour, which separates a Register from a Situation at a glance.
It holds resources only, and the editor's fill subscribes the editor's own methods on each realized part.

## `internal PRegisterTemplate()`

Merges the markup the Veneer holds, since the dictionary carries no class of its own there.
