# Llyn.Core.Windows.csproj
Hash: `52f86fffcc07ada1`

## `<TargetFramework>net10.0-windows10.0.17763.0</TargetFramework>`

The Windows version in the target makes the system PDF reader in `Windows.Data.Pdf` visible.
Host, Deportment and Veneer already target the same version, so no project above needs a new one.

## `<PackageReference Include="Microsoft.Web.WebView2" Version="1.0.4191.47" />`

The browser engine prints, proofs and saves every press page, so the package lives here.
Core.Windows is Core's Windows twin, and no logic ring may reference it.
DPAPI, which protects the Joplin token, comes with the Windows desktop framework, so no package brings it.
