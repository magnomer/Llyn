# Llyn.Infrastructure.csproj
Hash: `73765e6a646c35fc`

## `<PackageReference Include="SQLitePCLRaw.bundle_e_sqlite3" Version="2.1.13" />`

Microsoft.Data.Sqlite 9.0.11 pins SQLitePCLRaw 2.1.10, which carries a high-severity SQLite advisory (GHSA-2m69-gcr7-jv3q).
This reference overrides the transitive bundle with a patched version.

## `<WarningsAsErrors>CA1416</WarningsAsErrors>`

A Windows-only call turns the build red, so the platform stays out of Infrastructure.

## `<EmbeddedResource Include="..\..\assets\icons\favorite.svg" LogicalName="Llyn.Infrastructure.Icons.favorite.svg" />`

The favorite, marked favorite, star and expand icons ride inside the assembly, as the theme does.
`LPortraitIcon.LPortraitIconRead` reads them, so an exported page embeds them without a file beside the program.
Each logical name is `LPortraitIcon.LPortraitIconPrefix` plus the file name.
