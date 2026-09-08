# LSource.cs

## `public interface LSource`

A single pronunciation or audio source.
Which of the two it is comes from the list the language pack declared it in.
It does not come from the source itself.
The provider is language-agnostic.
What it searches and how it extracts a value are supplied by a language pack (see `LSourceSpec`).
Nothing is hardcoded here.
It returns an `LAnswer`, which carries the extracted value and whether the source was reached.

## `string LSourceName { get; }`

The source's declared name, for example `"Cambridge"`.

## `Task<LAnswer> LSourceFind(string word, CancellationToken cancellation);`

Finds a value for `word`.
Implementations must not throw for an ordinary "not found" or network failure.
They report it in the answer, so one failing source never fails the whole search.
`LAnswerBlank` means the source answered and had nothing, `LAnswerLost` that it was never reached.
