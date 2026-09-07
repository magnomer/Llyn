# LSource.cs

## `public interface LSource`

A single pronunciation or audio source.
Which of the two it is comes from the list the language pack declared it in, not from the source itself.
The provider is language-agnostic.
What it searches and how it extracts a value are supplied by a language pack (see `LSourceSpec`).
Nothing is hardcoded here.
It returns the bare extracted value as a string.
That is a phonetic form for a pronunciation source, or an audio URL for an audio source.
It returns `null` when the source has none.

## `string LSourceName { get; }`

The source's declared name, for example `"Cambridge"`.

## `Task<string?> LSourceFind(string word, CancellationToken cancellation);`

Finds a value for `word`, returning `null` when the source has none.
Implementations must not throw for an ordinary "not found" or network failure.
They return `null`, so one failing source never fails the whole search.
