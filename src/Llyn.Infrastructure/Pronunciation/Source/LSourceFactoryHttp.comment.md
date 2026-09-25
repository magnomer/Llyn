# LSourceFactoryHttp.cs

## `public sealed class LSourceFactoryHttp : LSourceFactory`

The adapter behind the source factory port, building live `LSource` sets over one shared `HttpClient`.
It is called once per list, so a transcription set and a recording set never mix.
Every ordinary source becomes a data-driven `LSourceGeneric`.
This is the seam where a future source needing logic a pack cannot express would be wired.
Such a source would go to a hand-written handler instead.
The orchestrators receive the finished set and stay language-agnostic.

## `public LSourceFactoryHttp(HttpClient client)`

Binds the factory to the `client` every source it builds fetches through.

## `public IReadOnlyList<LSource> LSourceFactoryCreate(IReadOnlyList<LSourceSpec> specs)`

One live source per declared spec, in pack order.
