# Llyn.Tests.Windows.csproj
Hash: `093ab6e30d54e7fc`

Builds the behaviour tests that compile only on Windows.

## `<TargetFramework>net10.0-windows10.0.17763.0</TargetFramework>`

The tests reference Deportment, which builds for Windows.
Deportment hosts WebView2, which needs Windows 10 build 17763, so the tests target the same version.

## `<SupportedOSPlatformVersion>10.0.17763.0</SupportedOSPlatformVersion>`

The lowest Windows version matches Deportment's, so platform analysis agrees across the reference.

## `<UseWPF>true</UseWPF>`

The WPF build packs the linked icons below into the test assembly's own resources.

## `<Resource Include="..\..\assets\icons\*.png;..\..\assets\icons\*.svg" Link="icons\%(Filename)%(Extension)" />`

The rime-book and script boxes build their expand chevron in their constructors.
The look sheet resolves every icon its rows name when it first loads.
The real icons live in Veneer, which the tests do not reference.
So the test assembly carries the whole icon folder, and a test points the icon root at it.

## `<ProjectReference Include="../Llyn.Tests.Interface/Llyn.Tests.Interface.csproj" />`

The tests reuse the relays, fakes and workspace of the relay project.
The reference also brings the language packs beside the test binaries.

## `<ProjectReference Include="../../src/Llyn.UIDeportment/Llyn.UIDeportment.csproj" />`

The tests reference Deportment directly.
This project declares no direct Capsule or Veneer reference.
