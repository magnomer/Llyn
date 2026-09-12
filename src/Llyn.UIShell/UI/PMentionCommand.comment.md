# PMentionCommand.cs

## `public static class PMentionCommand`

The four commands the linking gesture is asked through, one per menu item.
They are routed commands rather than click handlers because the menu is one shared resource.
A resource dictionary carries no code, so the menu cannot name a handler of its own.
The host the menu opened over binds each command where it lives.
The card row binds them on the row, the corpus scribe on the transcript.
The same commands serve the chip line, so a chip's remove button asks the same way the menu does.
