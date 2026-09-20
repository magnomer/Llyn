# LVaultSession.cs

## `public interface LVaultSession : IDisposable`

One unit of work over storage, opened by `LVault.LVaultSessionStart`.
Disposing a session that was never committed rolls its work back.
A joined session neither commits nor rolls back, because the outer one owns the transaction.

## `void LVaultSessionCommit();`

Makes every write of the session durable.
Calling it twice is harmless.
