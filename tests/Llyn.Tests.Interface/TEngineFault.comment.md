# TEngineFault.cs
Hash: `deab7d5e61cdcbfb`

## `internal static class TEngineFault`

Builds a port or vault that forwards every call to a real instance except one member, which faults.
`TEngineFake` cannot serve here, since it throws on every member its table does not name.
The fault sweep thus runs a gate over the real engine and faults only the member the gate reaches.

## `internal static TEngineKind TEngineFaultCreate<TEngineKind>(TEngineKind real, string member, bool thrown)`

The interface `TEngineKind` over `real`, faulting the member named `member` as `Interface.Member`.
The qualified name keeps a member of the same name on another port forwarding.
With `thrown` the member throws, otherwise it answers a faulted task.
A member that answers no task always throws, since it has no task to fault.

## `public class TEngineFaultProxy : DispatchProxy`

The proxy the runtime generates the interface over, public and constructible as `DispatchProxy` requires.
Static interface members never reach it, so they keep their own bodies and are not faulted.
A forwarded call does not wrap the real member's exception, so a real fault reaches the gate unchanged.
