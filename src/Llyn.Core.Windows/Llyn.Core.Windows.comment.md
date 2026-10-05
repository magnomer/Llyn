# Llyn.Core.Windows.csproj
Hash: `f355e82e31d13b41`

## `<PackageReference Include="Microsoft.Web.WebView2" Version="1.0.4191.47" />`

Core.Windows is Core's Windows twin, and no logic ring may reference it.

## `<PackageReference Include="System.Security.Cryptography.ProtectedData" Version="10.0.12" />`

Core.Windows needs DPAPI to protect the Joplin token for the current Windows user.
Only this twin may hold it, since Core stays free of platform code.
