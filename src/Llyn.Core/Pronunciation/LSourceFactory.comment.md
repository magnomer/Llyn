# LSourceFactory.cs

## `public interface LSourceFactory`

The port that turns declared source specs into live sources the engine can query.
It is a factory over the network, not storage, so it is not a Vault.
`LSourceFactoryHttp` in Infrastructure is its adapter over the shared `HttpClient`.
The engine caches the sets it builds per language and scheme and never names a client.

## `IReadOnlyList<LSource> LSourceFactoryCreate(IReadOnlyList<LSourceSpec> specs);`

One live source per spec, in the order given.
