# LRegisterPort.cs
Hash: `9935bb59c37ef592`

## `public interface LRegisterPort`

The slice of the engine the tenor panel sees when it lists or creates registers.
`LCardFacade` implements it, since registers hang on cards.

## `IReadOnlyList<LCatalogRegister> LEngineRegisterFind(LVista vista);`

The registers the tenor panel's vista lists, with its query and order.

## `LRegister LEngineRegisterCreate(string name);`

Answers the register of the typed name, creating it when none matches.
