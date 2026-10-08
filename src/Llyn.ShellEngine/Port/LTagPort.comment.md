# LTagPort.cs
Hash: `da3c61686b209828`

## `public interface LTagPort`

The slice of the engine the taxonomy panel sees when it lists or creates Tags.
`LCardFacade` implements it, since Tags hang on cards.

## `IReadOnlyList<LCatalogTag> LEngineTagFind(LVista vista);`

The Tags the taxonomy panel's vista lists, with its query and order.
The vista's language filter is not applied, since it hides entries, not Tags.
A vista whose chosen Tag no longer answers is deselected.

## `LTag LEngineTagCreate(string text);`

Answers the Tag of the typed text, creating it when none stands.
