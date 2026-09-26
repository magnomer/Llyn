# Llyn.Tests.csproj

## `<TargetFramework>net10.0-windows10.0.17763.0</TargetFramework>`

The tests reference Deportment, which builds for Windows.
Deportment hosts WebView2, which needs Windows 10 build 17763, so the tests target the same version.

## `<SupportedOSPlatformVersion>10.0.17763.0</SupportedOSPlatformVersion>`

The lowest Windows version matches Deportment's, so platform analysis agrees across the reference.

## `<ItemGroup>`

The engine seeds the controlled vocabularies from the language packs when it binds to a workspace.
So the packs have to sit beside the test binaries as they sit beside the application's.
