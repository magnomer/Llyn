# Llyn.Host.csproj

The composition root that builds the engine and hands it to a UI.
It builds the executable `Llyn.exe`, so the installed program keeps its name.

## Project ring

It is the only project that may name every project and every Windows twin.
It holds no behaviour, and each UI's entry point adds only what depends on its medium.
It takes the veneer's framework, runtime identifiers and icon, since it is now the executable.
It uses WPF because it starts the veneer's application on its own thread.
