# CSentenceOrder.cs
Hash: `54e18d6ed1fbafa9`

## `public sealed record CSentenceOrder(int CSentenceOrderParticle, int CSentenceOrderDependence)`

Where a language places the particle and the dependence around an example.

**Parameters**

- `CSentenceOrderParticle`: the particle's slot.
- `CSentenceOrderDependence`: the dependence's slot.

## `public static CSentenceOrder CSentenceOrderPlain { get; }`

The particle first and the dependence second.
It stands where the pack states no valid order, and before any order arrives.
