# LMarkupClerkDraft.cs
Hash: `729e0446a5ce83a9`

## `public sealed class LMarkupClerkDraft`

One markup entry turned into the entry draft the entry clerk saves.
Speeches and morphologies are looked up by name in the workspace.

## `public LMarkupClerkDraft(LRig rig, LMarkupClerkLink link)`

Reads the speech and morphology ports out of `rig` and keeps the link resolver.

## `public LEntryDraft LMarkupDraftResolve(LMarkupEntry entry, IReadOnlyDictionary<(string, string), long> prepared, List<LMarkupOmission> omissions, List<(LMarkupMention, int, long)> held, List<(LMarkupReference, long)> cited)`

The draft of one entry, its held mentions and cited references gathered for the intake to settle.
A speech the workspace does not know stays as a named draft.
An inflection's unknown speech is omitted.
A recording whose location cannot stand loses the recording.
The etymology is resolved beside the rest, both shapes at once.
The lexical unit and freedom pass through as the file names them.

## `private LSpeechValue? LMarkupSpeechResolve(string language, string name, List<LMarkupOmission> omissions)`

The speech value named in the language, omitted when not found.
The name is trimmed for the lookup, as the reader keeps text untrimmed and the archive trims only spaces.
The omission shows the name as the file wrote it.

## `private LInflection LMarkupInflectionResolve(string language, LMarkupInflection inflection, int position, List<LMarkupOmission> omissions)`

One inflection with its speech and morphologies resolved by name, unknown morphologies omitted.
Each morphology name is trimmed for the lookup, and an omission shows it as written.

## `private IReadOnlyList<LCardDraft> LMarkupCardResolve(IReadOnlyList<LMarkupCard> cards, LMarkupEntry entry, IReadOnlyDictionary<(string, string), long> prepared, List<LMarkupOmission> omissions, List<(LMarkupMention, int, long)> held, List<(LMarkupReference, long)> cited)`

The card drafts, their examples, translations and media resolved through the link, children in turn.
A translation whose target is not found is omitted.
