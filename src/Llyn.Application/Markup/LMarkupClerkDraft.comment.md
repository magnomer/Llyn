# LMarkupClerkDraft.cs

## `public sealed class LMarkupClerkDraft`

One markup entry turned into the entry draft the entry clerk saves.
Speeches and morphologies are looked up by name in the workspace, and what is not found is omitted.

## `public LMarkupClerkDraft(LRig rig, LMarkupClerkLink link)`

Reads the speech and morphology ports out of `rig` and keeps the link resolver.

## `public LEntryDraft LMarkupDraftResolve(LMarkupEntry entry, IReadOnlyDictionary<(string, string), long> prepared, List<LMarkupOmission> omissions, List<(LMarkupMention, int)> held)`

The draft of one entry.
A speech the workspace does not know stays as a named draft, an inflection's unknown speech is omitted.
A recording whose location cannot stand loses the recording.

## `private LSpeechValue? LMarkupSpeechResolve(string language, string name, List<LMarkupOmission> omissions)`

The speech value named in the language, omitted when not found.

## `private LInflection LMarkupInflectionResolve(string language, LMarkupInflection inflection, int position, List<LMarkupOmission> omissions)`

One inflection with its speech and morphologies resolved by name, unknown morphologies omitted.

## `private IReadOnlyList<LCardDraft> LMarkupCardResolve(IReadOnlyList<LMarkupCard> cards, LMarkupEntry entry, IReadOnlyDictionary<(string, string), long> prepared, List<LMarkupOmission> omissions, List<(LMarkupMention, int)> held)`

The card drafts, their examples, translations and media resolved through the link, children in turn.
A translation whose target is not found is omitted.
