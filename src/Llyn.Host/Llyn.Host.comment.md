# Llyn.Host.csproj

The composition root that will build the engine and the Conduct root and hand them to a UI.
It holds no source yet and references nothing yet.

## Project ring

It is the only project that may name every project and every Windows twin.
It holds no behaviour, and each UI's entry point adds only what depends on its medium.
It targets the Windows framework because it will wire the Windows twins.
Until it takes over, `App.xaml.cs` in the veneer stays the composition root.
