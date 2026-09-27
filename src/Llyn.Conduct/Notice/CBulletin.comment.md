# CBulletin.cs

## `public sealed record CBulletin(long CBulletinId, bool CBulletinStored);`

One notice as a driver's observer receives it, copied from the engine's notice by the controller.
It carries no subject, because the observer attached for one subject already knows it.

**Parameters**

- `CBulletinId`: the row the notice speaks about.
- `CBulletinStored`: whether the row is stored, copied from the engine so the rule stays there.
