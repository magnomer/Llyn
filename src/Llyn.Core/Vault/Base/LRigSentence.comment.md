# LRigSentence.cs
Hash: `0efa207365b2e66c`

## `public sealed record LRigSentence(LExampleVault LRigSentenceExamples, LMentionVault LRigSentenceMentions, LSentenceVault LRigSentenceSentences, LTranslationVault LRigSentenceTranslations)`

The sentence group of the rig, holding the ports that describe a sentence and what hangs on it.
It covers examples, mentions, sentences and translations.
`LRig` holds it as `LRigSentence`, so the rig stays small while each port keeps its own slot.
Every property is named `LRigSentence{Base}`, the base being the port's own, as on the rig.
The composition root builds it through `LRigFactorySentence`, and a test builds it from fakes.
