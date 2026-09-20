# LEngineAnatomy.cs

## `public sealed partial class LEngine`

The anatomy side of the engine: the onset, vowel, coda and tone every reflex row carries, in IPA and respelling.
The rules come from the pack of the entry's own language, so only a Classical Chinese entry has any.
The cutting itself is `LLanguageCache`'s, shared with the draft clerk.
What stays here is the tone read the shell asks for and the clearing the reflex sync compares on.

## `public IReadOnlyList<LAnatomyTone> LEngineToneRead(string language)`

The tone correspondence rows the pack named by `language` declares, empty for a blank name or a pack without them.
The anchor dropdown reads them to mark the placements a reflex reading's contour may descend from.

## `private static IReadOnlyList<LReflex> LEngineAnatomyClear(IReadOnlyList<LReflex> rows)`

The rows with their anatomy blanked, so the reflex sync can compare two lists on what the user can see.
