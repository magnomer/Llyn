# CCatalogOrder.cs
Hash: `28b1ad168467f1c9`

## `public enum CCatalogOrder`

One ordering a catalog can list its rows in, as a driver offers it in a menu.
It mirrors the engine's ordering member for member, and the controller maps between the two.
Its members keep the engine's order, because the map casts.
A driver never learns the engine's ordering, so its menus survive the move below the cut.
