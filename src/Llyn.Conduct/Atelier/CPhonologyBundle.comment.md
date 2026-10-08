# CPhonologyBundle.cs
Hash: `6f79891d4c5b867d`

## `public sealed class CPhonologyBundle`

The sound and script ports Host hands to Conduct in one piece, one port per narrow slice of the engine.
Each facade implements its own port, so no forwarding outlet stands between Conduct and the engine.
An area builder reads only the ports its constructors call and hands each one down.

## `internal CPhonologyBundle(LFanqiePort fanqies, LDiweiPort diweis, LScriptPort scripts, LLanguagePort languages, LReflexPort reflexes, LParadigmPort paradigms, LSentencePort sentences, LStemPort stems)`

Rejects a missing port and keeps each one.
It is internal, so only Host builds a bundle.

## `public LFanqiePort CPhonologyBundleFanqie { get; }`

The rime-book port, which fetches, reads, rebuilds and reorders an entry's rime-book rows.

## `public LDiweiPort CPhonologyBundleDiwei { get; }`

The diwei port, which finds and resolves rime cells and lists the entries under them.

## `public LScriptPort CPhonologyBundleScript { get; }`

The script port, which fetches, reads and rebuilds an entry's script images.

## `public LLanguagePort CPhonologyBundleLanguage { get; }`

The language port, which starts the sound fetches and reads contours, silence and the accent block.

## `public LReflexPort CPhonologyBundleReflex { get; }`

The reflex port, which fetches and rebuilds reflex readings and tells how each reflex prints.

## `public LParadigmPort CPhonologyBundleParadigm { get; }`

The paradigm port, which fetches and reads an entry's inflection paradigm.

## `public LSentencePort CPhonologyBundleSentence { get; }`

The sentence port, which reads a language's word order, particles and dependencies.

## `public LStemPort CPhonologyBundleStem { get; }`

The stem port, which finds and resolves phonetic series and lists their kindred entries.
