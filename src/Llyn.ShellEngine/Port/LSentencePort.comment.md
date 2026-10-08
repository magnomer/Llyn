# LSentencePort.cs
Hash: `8284e49bd11eb19f`

## `public interface LSentencePort`

The slice of the engine a deportment sees when it builds an example sentence in a language.
`LVocabularyFacade` implements it, since the vocabulary declares the sentence facts.

## `IReadOnlyList<string> LEngineDependenceRead(string language);`

The dependence labels the language's pack declares.

## `IReadOnlyList<string> LEngineParticleRead(string language);`

The particles the language's pack declares.

## `LSentenceOrder LEngineOrderRead(string language);`

The word order the language's pack declares.
