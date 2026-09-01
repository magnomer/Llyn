# LSource.cs

## `public interface LSource`

A single pronunciation or audio source. The provider is language-agnostic: what it searches and how it extracts a value are supplied by a language pack (see `LSourceSpec`), never hardcoded here. It returns the bare extracted value — a phonetic form for a pronunciation source, an audio URL for an audio source — as a string, or `null` when the source has none.

## `string LSourceName { get; }`

The source's declared name, for example `"Cambridge"`.

## `string LSourceKind { get; }`

The kind of value this source yields: `"pronunciation"` or `"audio"`.

## `Task<string?> LSourceFind(string word, CancellationToken cancellation);`

Finds a value for `word`, returning `null` when the source has none. Implementations must not throw for an ordinary "not found" or network failure; they return `null` so one failing source never fails the whole search.
