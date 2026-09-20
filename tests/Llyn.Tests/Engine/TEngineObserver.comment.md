# TEngineObserver.cs

## `public sealed class TEngineObserver`

Covers the engine's subscription as a delegate rather than a contract.
A lambda attached counts one bulletin when a setting changes.
A method group detached with the delegate it was attached with falls silent, since equality is target plus method.
The same method group attached twice is one subscriber, so a re-attached surface keeps one voice.
