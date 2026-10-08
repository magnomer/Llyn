# LSituationPort.cs
Hash: `240c79bb9942082b`

## `public interface LSituationPort`

The slice of the engine the atlas panel sees when it lists situations.
`LSituationFacade` implements it.

## `IReadOnlyList<LCatalogSituation> LEngineSituationFind(LVista vista, string unknown = "", string untitled = "");`

The situations the vista lists, named distinctly, with `unknown` and `untitled` wording blank fields.
