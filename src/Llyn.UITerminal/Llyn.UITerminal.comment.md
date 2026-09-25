# Llyn.UITerminal.csproj

The console surface, the second UI beside the WPF veneer.
It holds no source yet.

## `<ProjectReference Include="..\Llyn.UIDemeanor\Llyn.UIDemeanor.csproj" />`

Demeanor is the only reference, as Deportment is the veneer's only reference.
A UITerminal member only calls a function, and Demeanor drives it.
It targets the portable framework, and a Windows API fails its build through CA1416.
