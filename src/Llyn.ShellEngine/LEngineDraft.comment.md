# LEngineDraft.cs

## `public sealed partial class LEngine`

What is left beside the draft path once the save moved to `LEngineUpdate.cs` and the clerks.
The note parse the veneer draws with, and the transcription reset the save runs before its own sync.

## `public IReadOnlyList<LMarkdownBlock> LEngineMarkdownParse(string? text)`

The blocks of a note, parsed for the veneer that draws them.

## `private static IReadOnlyList<LTranscriptionDraft> LEngineTranscriptionReset(IReadOnlyList<LTranscriptionDraft> drafts)`

Every transcription row of the draft as a row still to be created.
A positive id the draft still holds names a row of an entry since deleted.
It is reset to zero first, so the commit writes the row anew instead of refusing it.
A negative id is kept, because the identity map records what it became.
