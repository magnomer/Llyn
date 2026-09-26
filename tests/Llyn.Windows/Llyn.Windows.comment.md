# Llyn.Windows.csproj

Builds the behaviour tests that compile only on Windows.

## `<TargetFramework>net10.0-windows10.0.17763.0</TargetFramework>`

The tests reference Deportment, which builds for Windows.
Deportment hosts WebView2, which needs Windows 10 build 17763, so the tests target the same version.

## `<SupportedOSPlatformVersion>10.0.17763.0</SupportedOSPlatformVersion>`

The lowest Windows version matches Deportment's, so platform analysis agrees across the reference.

## `<ProjectReference Include="../Llyn.Internal/Llyn.Internal.csproj" />`

The tests reuse the relays, fakes and workspace of the portable project.
The reference also brings the language packs beside the test binaries.
